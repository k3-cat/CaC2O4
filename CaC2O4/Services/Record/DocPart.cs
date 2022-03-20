using CaC2O4.Repositories;
using CSharpVitamins;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace CaC2O4.Services.Record;

public partial class RecordService {
    async Task _BatchAllocateDocsAsync(DocSubject subject, Guid subjectId, IEnumerable<String> titles, DateTimeOffset now) {
        var log = $"ALLOCATE - {now.ToUnixTimeSeconds()}";
        foreach (var title in titles) {
            var doc = new Document {
                Id = new Guid(),
                Subject = subject,
                SubjectId = subjectId,
                Title = title,
                State = FileState.Unavailable,
                Log = log,
                Timestamp = now,
            };

            await _dbCtx.AddAsync(doc);
        }
    }

    async Task _BatchRemoveDocsAsync(IQueryable<Document> q, DateTimeOffset now) {
        var log = $"CASCADE REMOVE - {now.ToUnixTimeSeconds()}\n";
        await foreach (var doc in q.AsAsyncEnumerable()) {
            if (doc.State is FileState.Archived) {
                continue;
            }
            if (doc.State is not FileState.Current) {
                foreach (var key in doc.Versions) {
                    _oss.DeleteFile(key);
                }
                _dbCtx.Remove(doc);
                continue;
            }
            doc.State = FileState.Archived;
            doc.Log = log + doc.Log;
        }
    }

    public override async Task<UrlRsn> AccessDoc(Types.Uuid id, ServerCallContext ctx) {
        var doc = await _dbCtx.Documents.FindAsync(new Guid(id.Id.ToArray()));
        if (doc == null) {
            throw new Exception();
        }
        return new UrlRsn {
            Url = $"{doc.Timestamp.Year}/{ShortGuid.Encode(doc.Id)}"
        };
    }

    public override async Task ListDocs(ListDocsReq req, IServerStreamWriter<DocBrief> resStream, ServerCallContext ctx) {
        var q = from d in _dbCtx.Documents
                where d.SubjectId == new Guid(req.SubjectId.ToArray()) && d.State != FileState.Archived
                select new DocBrief {
                    Id = Google.Protobuf.ByteString.CopyFrom(d.Id.ToByteArray()),
                    Title = d.Title,
                    State = d.State,

                    Log = d.Log,
                    Timestamp = Timestamp.FromDateTimeOffset(d.Timestamp),
                };

        await foreach (var doc in q.AsAsyncEnumerable()) {
            await resStream.WriteAsync(doc);
        }
    }

    static Task<(String, FileState, String)> _GenerateDoc(String generateFor, String title) {
        return Task.FromResult(new ValueTuple<String, FileState, String>(title, FileState.Unavailable, null!));
    }

    [Authorize("Archivist")]
    public override async Task<Types.Empty> AllocateDoc(AllocateDocReq req, ServerCallContext ctx) {
        var (title, state, fileId) = await _GenerateDoc(req.For, req.Title);

        var now = Utils.TxnTime.Get(ctx);
        var log = $"INIT by {Utils.Auth.GetUserName(ctx)} - {now.ToUnixTimeSeconds}";
        var doc = new Document {
            Id = new Guid(),
            Subject = req.Subject,
            SubjectId = new Guid(req.SubjectId.ToArray()),
            Title = title,
            State = state,
            Log = log,
            Timestamp = now,
        };

        if (fileId is not null) {
            doc.Versions.Add(fileId);
        }

        await _dbCtx.AddAsync(doc);
        await _dbCtx.SaveChangesAsync();

        return new Types.Empty();
    }

    [Authorize("Archivist")]
    public override async Task<Types.Empty> MarkDocForEdit(Types.Uuid id, ServerCallContext ctx) {
        var doc = await _dbCtx.Documents.FindAsync(new Guid(id.Id.ToArray()));
        if (doc is null) {
            throw new Exception();
        }

        var now = Utils.TxnTime.Get(ctx);
        doc.State = FileState.Editing;
        doc.Log = $"MARK EDIT by {Utils.Auth.GetUserName(ctx)} - {now.ToUnixTimeSeconds()}\n" + doc.Log;
        doc.Timestamp = now;

        await _dbCtx.SaveChangesAsync();

        return new Types.Empty();
    }

    [Authorize("Archivist")]
    public override async Task<Types.Empty> RemoveDoc(Types.Uuid id, ServerCallContext ctx) {
        var doc = await _dbCtx.Documents.FindAsync(new Guid(id.Id.ToArray()));
        if (doc is null) {
            throw new Exception();
        }

        var now = Utils.TxnTime.Get(ctx);
        doc.State = FileState.Archived;
        doc.Log = $"REMOVE by {Utils.Auth.GetUserName(ctx)} - {now.ToUnixTimeSeconds()}\n" + doc.Log;
        doc.Timestamp = now;

        await _dbCtx.SaveChangesAsync();

        return new Types.Empty();
    }

    [Authorize("Staff")]
    public override async Task<Types.Empty> FinalizeEdit(FinalizeEditReq req, ServerCallContext ctx) {
        var doc = await _dbCtx.Documents.FindAsync(new Guid(req.Id.ToArray()));
        if (doc is null) {
            throw new Exception();
        }

        var userId = Utils.Auth.GetUserId(ctx);
        var record = await _dbCtx.UploadingRecords.FindAsync(req.UploadKey);
        if (record is null || !record.IsSuccess || record.Creator != userId) {
            throw new Exception();
        }

        _dbCtx.Remove(record);

        if (doc.State is FileState.Blank) {
            _oss.DeleteFile(doc.Versions[0]);
            doc.Versions.Clear();
        }

        DocHelper.IDoc obj;
        DocHelper.IDocHelper helper;
        if (doc.Subject is DocSubject.Vehicle) {
            obj = (DocHelper.IDoc)(await _dbCtx.Vehicles.FindAsync(doc.SubjectId))!;
            helper = new DocHelper.VehicleHelper(req.NewRecord);
        }
        else if (doc.Subject is DocSubject.Employee) {
            obj = (DocHelper.IDoc)(await _dbCtx.Employees.FindAsync(doc.SubjectId))!;
            helper = new DocHelper.EmployeeHelper(req.NewRecord);
        }
        else {
            throw new Exception();
        }

        var now = Utils.TxnTime.Get(ctx);
        var value = helper.ProcessDoc(obj, doc.Title);
        obj.Log = $"EDIT {doc.Title} with {value} by {Utils.Auth.GetUserName(ctx)} - {now.ToUnixTimeSeconds()}\n" + obj.Log;
        obj.Timestamp = now;

        doc.State = FileState.Current;
        doc.Versions.Insert(0, req.UploadKey);
        doc.Log = $"UPLOAD by {Utils.Auth.GetUserName(ctx)} - {now.ToUnixTimeSeconds()}\n" + doc.Log;
        doc.Timestamp = now;

        await _dbCtx.SaveChangesAsync();

        return new Types.Empty();
    }
}
