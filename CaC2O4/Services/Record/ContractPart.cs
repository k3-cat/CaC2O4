using CaC2O4.Repositories;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace CaC2O4.Services.Record;

public partial class RecordService {
    public override async Task<ContractDetail> GetContract(Types.Uuid id, ServerCallContext ctx) {
        var contract = await _dbCtx.Contracts.FindAsync(new Guid(id.Id.ToArray()));
        if (contract is null) {
            throw new Exception();
        }

        return new ContractDetail {
            Id = Google.Protobuf.ByteString.CopyFrom(contract.Id.ToByteArray()),
            VehicleId = Google.Protobuf.ByteString.CopyFrom(contract.VehicleId.ToByteArray()),
            EmployeeId = Google.Protobuf.ByteString.CopyFrom(contract.EmployeeId.ToByteArray()),

            Log = contract.Log,
            Timestamp = Timestamp.FromDateTimeOffset(contract.Timestamp),
        };
    }

    IQueryable<ContractBrief> _ListByVehicle(Guid id) {
        return from c in _dbCtx.Contracts
               where c.VehicleId == id
               join e in _dbCtx.Employees on c.EmployeeId equals e.Id
               select new ContractBrief {
                   Id = Google.Protobuf.ByteString.CopyFrom(c.Id.ToByteArray()),
                   SubjectId = Google.Protobuf.ByteString.CopyFrom(e.Id.ToByteArray()),
                   Hint = e.Name,
               };
    }

    IQueryable<ContractBrief> _ListByEmployee(Guid id) {
        return from c in _dbCtx.Contracts
               where c.EmployeeId == id
               join v in _dbCtx.Vehicles on c.VehicleId equals v.Id
               select new ContractBrief {
                   Id = Google.Protobuf.ByteString.CopyFrom(c.Id.ToByteArray()),
                   SubjectId = Google.Protobuf.ByteString.CopyFrom(v.Id.ToByteArray()),
                   Hint = v.NumberPlate,
               };
    }

    public override async Task ListContract(ListContractsReq req, IServerStreamWriter<ContractBrief> resStream, ServerCallContext ctx) {
        IQueryable<ContractBrief> q;
        if (req.Subject is ContractSubject.Vehicle) {
            q = _ListByVehicle(new Guid(req.SubjectId.ToArray()));
        }
        else if (req.Subject is ContractSubject.Employee) {
            q = _ListByEmployee(new Guid(req.SubjectId.ToArray()));
        }
        else {
            throw new Exception();
        }

        await foreach (var contract in q.AsAsyncEnumerable()) {
            await resStream.WriteAsync(contract);
        }
    }

    static void _SetOwner(Vehicle? vehicle, Guid ownerId, String op, DateTimeOffset now) {
        if (vehicle is null || vehicle.OwnerId != Guid.Empty) {
            throw new InvalidOperationException();
        }
        vehicle.OwnerId = ownerId;
        vehicle.Log = $"SETOWNER {ownerId} by {op} - {now.ToUnixTimeSeconds()}\n" + vehicle.Log;
    }

    [Authorize("Archivist")]
    public override async Task<Types.Empty> SignContract(SignContractReq req, ServerCallContext ctx) {
        var now = Utils.TxnTime.Get(ctx);
        var op = Utils.Auth.GetUserName(ctx);

        Guid vehicleId;
        Guid employeeId;
        if (req.Subject is ContractSubject.Vehicle) {
            vehicleId = new Guid(req.SubjectId.ToArray());
            var q = from e in _dbCtx.Employees
                    where e.LogicId == req.Target
                    select e.Id;

            employeeId = await q.SingleAsync();

            if (req.IsOwner) {
                var vehicle = await _dbCtx.Vehicles.FindAsync(vehicleId);
                _SetOwner(vehicle, vehicleId, op, now);
            }
        }
        else if (req.Subject is ContractSubject.Employee) {
            employeeId = new Guid(req.SubjectId.ToArray());
            var q = from v in _dbCtx.Vehicles
                    where v.LogicId == req.Target
                    select v;

            var vehicle = await q.SingleAsync();
            vehicleId = vehicle.Id;

            if (req.IsOwner) {
                _SetOwner(vehicle, vehicleId, op, now);
            }
        }
        else {
            throw new Exception();
        }

        var contract = new Contract() {
            Id = new Guid(),
            VehicleId = vehicleId,
            EmployeeId = employeeId,
            Log = $"ADD by {op} - {now.ToUnixTimeSeconds()}",
            Timestamp = now,
        };

        await _dbCtx.AddAsync(contract);
        await _dbCtx.SaveChangesAsync();

        // TODO generate

        return new Types.Empty();
    }

    [Authorize("Archivist")]
    public override async Task<Types.Empty> RescindContract(Types.Uuid id, ServerCallContext ctx) {
        var contract = await _dbCtx.Contracts.FindAsync(new Guid(id.Id.ToByteArray()));
        if (contract is null) {
            throw new Exception();
        }

        var vehicle = (await _dbCtx.Vehicles.FindAsync(contract.VehicleId))!;
        if (vehicle.OwnerId == contract.EmployeeId) {
            vehicle.OwnerId = Guid.Empty;
        }

        var q = from d in _dbCtx.Documents
                where d.SubjectId == contract.Id
                select d;

        var now = Utils.TxnTime.Get(ctx);
        await _BatchRemoveDocsAsync(q, now);

        contract.IsRemoved = true;
        contract.Timestamp = now;
        contract.Log = $"REMOVE by {Utils.Auth.GetUserName(ctx)} - {now.ToUnixTimeSeconds()}";

        await _dbCtx.SaveChangesAsync();

        return new Types.Empty();
    }
}
