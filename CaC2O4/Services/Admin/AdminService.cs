using CaC2O4.Repositories;
using Microsoft.AspNetCore.Authorization;
using PwdHasher;

namespace CaC2O4.Services.Admin;

[Authorize("Admin")]
public partial class AdminService : IAdminService.IAdminServiceBase {
    readonly ILogger<AdminService> _logger;
    readonly DbCtx _dbCtx;
    readonly IPwdHasher _pwdHasher;

    public AdminService(ILogger<AdminService> logger, DbCtx dbCtx, IPwdHasher pwdHasher) {
        _logger = logger;
        _dbCtx = dbCtx;
        _pwdHasher = pwdHasher;
    }
}
