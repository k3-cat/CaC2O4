using CaC2O4.Repositories;

namespace CaC2O4.Services.Office;

public partial class OfficeService : IOfficeService.IOfficeServiceBase {
    readonly ILogger<OfficeService> _logger;
    readonly DbCtx _dbCtx;

    public OfficeService(ILogger<OfficeService> logger, DbCtx dbCtx) {
        _logger = logger;
        _dbCtx = dbCtx;
    }
}
