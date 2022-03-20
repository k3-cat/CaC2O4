using CaC2O4.Repositories;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace CaC2O4.Services.Admin;

public partial class AdminService : IAdminService.IAdminServiceBase {
    public override async Task<UserDetail> GetUser(Types.Uuid id, ServerCallContext ctx) {
        var user = await _dbCtx.Users.FindAsync(id.Id);
        if (user is null) {
            throw new Exception();
        }

        return new UserDetail {
            Id = Google.Protobuf.ByteString.CopyFrom(user.Id.ToByteArray()),
            Name = user.Name,
            Phone = user.Phone,
            Acl = user.Acl,
        };
    }

    public override async Task ListAllUsers(Types.Empty _, IServerStreamWriter<UserDetail> resStream, ServerCallContext ctx) {
        var q = from u in _dbCtx.Users
                where u.Acl != UserAcl.Forbidden
                select new UserDetail {
                    Id = Google.Protobuf.ByteString.CopyFrom(u.Id.ToByteArray()),
                    Name = u.Name,
                    Phone = u.Phone,
                    Acl = u.Acl,
                };

        await foreach (var user in q.AsAsyncEnumerable()) {
            await resStream.WriteAsync(user);
        }
    }

    public override async Task<Types.Empty> AddUser(AddUserReq req, ServerCallContext ctx) {
        var user = new User {
            Id = Guid.NewGuid(),
            Name = req.Name,
            Phone = req.Phone,
            Acl = req.Acl,
        };
        user.Password = _pwdHasher.HashPassword(req.Phone);
        await _dbCtx.AddAsync(user);

        return new Types.Empty();
    }

    public override async Task<Types.Empty> AssignAcl(AssignAclReq req, ServerCallContext ctx) {
        var user = await _dbCtx.Users.FindAsync(req.Id);
        if (user == null) {
            throw new InvalidOperationException();
        }
        user.Acl = req.Acl;
        await _dbCtx.SaveChangesAsync();

        return new Types.Empty();
    }

    public override async Task<Types.Empty> DeleteUser(Types.Uuid id, ServerCallContext ctx) {
        var user = await _dbCtx.Users.FindAsync(id.Id);
        if (user == null) {
            throw new InvalidOperationException();
        }
        _dbCtx.Remove(user);

        return new Types.Empty();
    }
}
