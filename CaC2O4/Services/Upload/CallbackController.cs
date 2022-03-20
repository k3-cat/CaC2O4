using CaC2O4.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CaC2O4.Services;

[AllowAnonymous]
[ApiController]
public partial class CallbackController : ControllerBase {
    readonly ILogger<CallbackController> _logger;
    readonly DbCtx _dbCtx;
    readonly Providers.IAliOss _oss;

    public CallbackController(ILogger<CallbackController> logger, DbCtx dbCtx, Providers.IAliOss oss) {
        _logger = logger;
        _dbCtx = dbCtx;
        _oss = oss;
    }

    [HttpPost("/general_upload_callback/")]
    public async Task GeneralCallbackAsync(String key) {
        var i = key.LastIndexOf('/');
        if (i == -1) {
            throw new ArgumentException("invalid key", nameof(key));
        }

        if (!_oss.VerifySignature(HttpContext)) {
            throw new Exception();
        }

        var record = await _dbCtx.UploadingRecords.FindAsync(key[(i + 1)..]);
        if (record is null) {
            throw new ArgumentException("key not exist", nameof(key));
        }

        record.IsSuccess = true;
        record.SuccessedAt = Utils.TxnTime.Get(HttpContext);

        await _dbCtx.SaveChangesAsync();
    }
}
