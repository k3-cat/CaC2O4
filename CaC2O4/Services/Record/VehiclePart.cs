using CaC2O4.Repositories;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace CaC2O4.Services.Record;

public partial class RecordService {

    public override async Task<VehicleDetail> GetVehicle(Types.Uuid id, ServerCallContext ctx) {
        var vehicle = await _dbCtx.Vehicles.FindAsync(new Guid(id.Id.ToArray()));
        if (vehicle is null) {
            throw new ArgumentException();
        }

        return new VehicleDetail {
            Id = Google.Protobuf.ByteString.CopyFrom(vehicle.Id.ToByteArray()),
            LogicId = vehicle.LogicId,

            NumberPlate = vehicle.NumberPlate,
            Model = vehicle.Model,
            Vin = vehicle.Vin,
            EngineNo = vehicle.EngineNo,
            FuelType = vehicle.FuelType,
            RegisterDate = Types.DateSerilzer.Serilize(vehicle.RegisterDate),
            Usci = vehicle.Usci,
            LicenseNo = vehicle.LicenseNo,
            BusinessName = vehicle.BusinessName,
            TransportPermitExpireAt = Types.DateSerilzer.Serilize(vehicle.TransportPermitExpireAt),
            BusinessLicenseExpireAt = Types.DateSerilzer.Serilize(vehicle.BusinessLicenseExpireAt),
            RecordNo = vehicle.RecordNo,
            OwnerId = Google.Protobuf.ByteString.CopyFrom(vehicle.OwnerId.ToByteArray()),

            Log = vehicle.Log,
            Timestamp = Timestamp.FromDateTimeOffset(vehicle.Timestamp),
        };
    }

    public override async Task SearchVehicles(SearchVehicleReq req, IServerStreamWriter<VehicleBrief> resStream, ServerCallContext ctx) {
        var q = from v in _dbCtx.Vehicles
                where EF.Functions.Like(v.NumberPlate, $"%{req.Query}%")
                select new VehicleBrief {
                    Id = Google.Protobuf.ByteString.CopyFrom(v.Id.ToByteArray()),
                    LogicId = v.LogicId,
                    NumberPlate = v.NumberPlate,
                };

        await foreach (var vehicle in q.AsAsyncEnumerable()) {
            await resStream.WriteAsync(vehicle);
        }
    }

    [Authorize("Archivist")]
    public override async Task<Types.Empty> AddVehicle(AddVehicleReq req, ServerCallContext ctx) {
        var now = Utils.TxnTime.Get(ctx);

        var id = new Guid();
        var vehicle = new Vehicle {
            Id = id,
            LogicId = req.VehicleId,
            Log = $"ADD by {Utils.Auth.GetUserName(ctx)} - {now.ToUnixTimeSeconds()}",
            Timestamp = now,
        };

        await _BatchAllocateDocsAsync(DocSubject.Vehicle, id, DocHelper.VehicleDef.TitleList, now);

        await _dbCtx.AddAsync(vehicle);
        await _dbCtx.SaveChangesAsync();

        return new Types.Empty();
    }

    [Authorize("Archivist")]
    public override async Task<Types.Empty> RemoveVehicle(Types.Uuid id, ServerCallContext ctx) {
        var vehicle = await _dbCtx.Vehicles.FindAsync(new Guid(id.Id.ToArray()));
        if (vehicle is null) {
            throw new Exception();
        }

        var q = from d in _dbCtx.Documents
                where d.SubjectId == vehicle.Id
                select d;

        var now = Utils.TxnTime.Get(ctx);
        await _BatchRemoveDocsAsync(q, now);

        vehicle.IsRemoved = true;
        vehicle.Timestamp = now;
        vehicle.Log = $"REMOVE by {Utils.Auth.GetUserName(ctx)} - {now.ToUnixTimeSeconds()}";

        await _dbCtx.SaveChangesAsync();

        return new Types.Empty();
    }
}
