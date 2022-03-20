using CaC2O4.Repositories;
using CSharpVitamins;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace CaC2O4.Services.Office;

public partial class OfficeService : IOfficeService.IOfficeServiceBase {
    static String _GrantAccess(Paper? paper) {
        if (paper == null) {
            throw new Exception();
        }
        return $"{paper.Timestamp.Year}/{ShortGuid.Encode(paper.Id)}";
    }

    public override async Task<UrlRsn> AccessPaper(Types.Uuid id, ServerCallContext ctx) {
        var paper = await _dbCtx.Papers.FindAsync(id);
        return new UrlRsn {
            Url = _GrantAccess(paper),
        };
    }

    public override async Task ListPapersByVehicle(Types.Uuid id, IServerStreamWriter<PaperBrief> resStream, ServerCallContext ctx) {
        var q = from p in _dbCtx.Papers
                where p.VehicleId == new Guid(id.Id.ToArray()) && p.State != FileState.Archived
                select new PaperBrief {
                    Id = Google.Protobuf.ByteString.CopyFrom(p.Id.ToByteArray()),
                    SerialNo = p.SerialNo,
                    Title = p.Title,
                    State = p.State,
                    Timestamp = Timestamp.FromDateTimeOffset(p.Timestamp),
                };

        await foreach (var paper in q.AsAsyncEnumerable()) {
            await resStream.WriteAsync(paper);
        }
    }

    async Task<UInt32> _GenSerialNoAsync(Int32 year) {
        var q = from l in _dbCtx.Papers
                where l.Timestamp.Year == year
                select l;

        return (UInt32)await q.CountAsync();
    }

    [Authorize("Archivist")]
    public override async Task<Types.Empty> GeneratePaper(GeneratePaperReq req, ServerCallContext ctx) {
        var now = Utils.TxnTime.Get(ctx);
        var paper = new Paper {
            Id = new Guid(),
            VehicleId = new Guid(req.VehicleId.ToArray()),
            SerialNo = (await _GenSerialNoAsync(now.Year)) + 1,
            Title = req.Title,
            State = FileState.Unavailable,
            Log = $"INIT by {Utils.Auth.GetUserName(ctx)} - {now.ToUnixTimeSeconds()}",
            Timestamp = now,
        };

        await _dbCtx.AddAsync(paper);
        await _dbCtx.SaveChangesAsync();

        return new Types.Empty();
    }

    [Authorize("Staff")]
    public override async Task<Types.Empty> FinalizePaper(FinalizePaperReq req, ServerCallContext ctx) {
        var now = Utils.TxnTime.Get(ctx);
        var paper = await _dbCtx.Papers.FindAsync(new Guid(req.Id.ToArray()));
        if (paper is null) {
            throw new Exception();
        }

        paper.State = FileState.Current;
        paper.Log = $"UPLOAD by {Utils.Auth.GetUserName(ctx)} - {now.ToUnixTimeSeconds()}" + paper.Log;
        paper.Timestamp = now;

        await _dbCtx.SaveChangesAsync();

        return new Types.Empty();
    }
}
