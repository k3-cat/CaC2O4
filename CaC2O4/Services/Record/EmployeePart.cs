using CaC2O4.Repositories;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace CaC2O4.Services.Record;

public partial class RecordService {
    public override async Task<EmployeeDetail> GetEmployee(Types.Uuid id, ServerCallContext ctx) {
        var employee = await _dbCtx.Employees.FindAsync(new Guid(id.Id.ToArray()));
        if (employee is null) {
            throw new Exception();
        }

        return new EmployeeDetail {
            Id = Google.Protobuf.ByteString.CopyFrom(employee.Id.ToByteArray()),
            LogicId = employee.LogicId,

            Tin = employee.Tin,
            Name = employee.Name,
            Gender = employee.Gender,
            Ethnicity = employee.Ethnicity,
            Dob = Types.DateSerilzer.Serilize(employee.Dob),
            Address = employee.Address,
            Household = employee.Household,
            CertificateNo = employee.CertificateNo,
            SCardExpireAt = Types.DateSerilzer.Serilize(employee.SCardExpireAt),
            RecordNo = employee.RecordNo,

            Phone = employee.Phone,
            EducationLevel = employee.EducationLevel,

            Log = employee.Log,
            Timestamp = Timestamp.FromDateTimeOffset(employee.Timestamp),
        };
    }

    public override async Task SearchEmployees(SearchEmployeeReq req, IServerStreamWriter<EmployeeBrief> resStream, ServerCallContext ctx) {
        var q = _dbCtx.Employees.AsQueryable();
        if (req.SearchBy is SearchEmployeeReq.Types.By.Index) {
            q = q.Where(e => e.NameIndex == req.Query);
        }
        else if (req.SearchBy is SearchEmployeeReq.Types.By.Name) {
            q = q.Where(e => EF.Functions.Like(e.Name, $"%{req.Query}%"));
        }
        else if (req.SearchBy is SearchEmployeeReq.Types.By.Phone) {
            q = q.Where(e => EF.Functions.Like(e.Phone, $"%{req.Query}%"));
        }

        var p = q.Select(e => new EmployeeBrief {
            Id = Google.Protobuf.ByteString.CopyFrom(e.Id.ToByteArray()),
            LogicId = e.LogicId,
            Name = e.Name,
        });

        await foreach (var employee in p.AsAsyncEnumerable()) {
            await resStream.WriteAsync(employee);
        }
    }

    [Authorize("Archivist")]
    public override async Task<Types.Empty> AddEmployee(AddEmployeeReq req, ServerCallContext ctx) {
        var now = Utils.TxnTime.Get(ctx);

        var id = new Guid();
        var employee = new Employee {
            Id = id,
            LogicId = req.EmployeeId,
            Log = $"ADD by {Utils.Auth.GetUserName(ctx)} - {now.ToUnixTimeSeconds()}",
            Timestamp = now,
        };

        await _BatchAllocateDocsAsync(DocSubject.Employee, id, DocHelper.EmployeeDef.TitleList, now);

        await _dbCtx.AddAsync(employee);
        await _dbCtx.SaveChangesAsync();

        return new Types.Empty();
    }

    [Authorize("Archivist")]
    public override async Task<Types.Empty> RemoveEmployee(Types.Uuid id, ServerCallContext ctx) {
        var employee = await _dbCtx.Employees.FindAsync(new Guid(id.Id.ToArray()));
        if (employee is null) {
            throw new Exception();
        }

        var q = from d in _dbCtx.Documents
                where d.SubjectId == employee.Id
                select d;

        var now = Utils.TxnTime.Get(ctx);
        await _BatchRemoveDocsAsync(q, now);

        employee.IsRemoved = true;
        employee.Timestamp = now;
        employee.Log = $"REMOVE by {Utils.Auth.GetUserName(ctx)} - {now.ToUnixTimeSeconds()}";

        await _dbCtx.SaveChangesAsync();

        return new Types.Empty();
    }
}
