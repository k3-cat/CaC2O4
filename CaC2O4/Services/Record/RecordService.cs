using CaC2O4.Repositories;

namespace CaC2O4.Services.Record;

public partial class RecordService : IRecordService.IRecordServiceBase {
    readonly ILogger<RecordService> _logger;
    readonly DbCtx _dbCtx;
    readonly Providers.IAliOss _oss;

    public RecordService(ILogger<RecordService> logger, DbCtx dbCtx, Providers.IAliOss oss) {
        _logger = logger;
        _dbCtx = dbCtx;
        _oss = oss;
    }
}
