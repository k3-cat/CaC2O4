using CaC2O4.Repositories;
using CSharpVitamins;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ProtobufWebToken;
using ProtobufWebToken.AspNetCore;
using PwdHasher;

namespace CaC2O4.Services.Auth;

[AllowAnonymous]
public class AuthenticationService : IAuthenticationService.IAuthenticationServiceBase {
    readonly ILogger<AuthenticationService> _logger;
    readonly DbCtx _dbCtx;
    readonly IPwdHasher _pwdHasher;
    readonly PTokenHelper _tokenHelper;

    public AuthenticationService(ILogger<AuthenticationService> logger, DbCtx dbCtx, IPwdHasher pwdHasher, PTokenHelper tokenHelper) {
        _logger = logger;
        _dbCtx = dbCtx;
        _pwdHasher = pwdHasher;
        _tokenHelper = tokenHelper;
    }

    async Task<TokenRes> _GenerateTokenResponseAsync(User user, ServerCallContext ctx) {
        var now = Utils.TxnTime.Get(ctx);
        var key = ShortGuid.NewGuid();

        user.LastAccess = now;
        var loginEntry = new Login() {
            Id = key,
            UserId = user.Id,
            IssueAt = now,
        };

        await _dbCtx.AddAsync(loginEntry);
        await _dbCtx.SaveChangesAsync();

        return new TokenRes {
            AccessToken = _tokenHelper.CreateToken(new PTokenDescriptor {
                SubjectId = user.Id,
                DisplayName = user.Name,
                ExpireAt = now + TimeSpan.FromMinutes(15),
                ValidFrom = now,
                Roles = new String[] { user.Acl.ToString() },
            }),
            RefreshToken = key,
            ExpireAt = Timestamp.FromDateTimeOffset(now),
            Acl = user.Acl,
        };
    }

    Boolean _VerifyPassword(User user, String password) {
        var result = _pwdHasher.VerifyHashedPassword(user.Password, password);
        if (result == VerificationResult.Failed) {
            return false;
        }

        if (result == VerificationResult.SuccessRehashNeeded) {
            user.Password = _pwdHasher.HashPassword(password);
        }
        return true;
    }

    public override async Task<TokenRes> Login(LoginReq req, ServerCallContext ctx) {
        var q = from u in _dbCtx.Users
                where u.Name == req.Username
                select u;

        var user = await q.SingleOrDefaultAsync();
        if (user is null || !_VerifyPassword(user, req.Password)) {
            _logger.LogInformation("login failed for user {@Name}", req.Username);
            throw new Exception();
        }
        if (user.Acl is UserAcl.Forbidden) {
            _logger.LogInformation("login denied for user {@Name}", req.Username);
            throw new Exception();
        }

        return await _GenerateTokenResponseAsync(user, ctx);
    }

    public override async Task<TokenRes> RefreshToken(RefreshTokenReq req, ServerCallContext ctx) {
        var token = await _dbCtx.Logins.FindAsync(req.RefreshToken);
        if (token is null) {
            throw new Exception();
        }
        var user = await _dbCtx.Users.FindAsync(token.UserId);
        if (user!.Acl is UserAcl.Forbidden) {
            throw new Exception();
        }

        return await _GenerateTokenResponseAsync(user, ctx);
    }
}
