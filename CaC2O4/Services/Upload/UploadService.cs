using CaC2O4.Repositories;
using CSharpVitamins;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace CaC2O4.Services.Upload;

public class UploadService : IUploadService.IUploadServiceBase {
    readonly ILogger<UploadService> _logger;
    readonly DbCtx _dbCtx;
    readonly Providers.IAliOss _oss;

    public UploadService(ILogger<UploadService> logger, DbCtx dbCtx, Providers.IAliOss oss) {
        _logger = logger;
        _dbCtx = dbCtx;
        _oss = oss;
    }

    [Authorize("Staff")]
    public override async Task<UploadInfo> AllocateNewUpload(Types.Empty _, ServerCallContext ctx) {
        var now = Utils.TxnTime.Get(ctx);

        var id = ShortGuid.NewGuid();
        var upload = new UploadingRecord {
            Key = $"{now.Year}/{id}",
            Creator = Utils.Auth.GetUserId(ctx),
            CreatedAt = now,
        };

        await _dbCtx.AddAsync(upload);
        await _dbCtx.SaveChangesAsync();

        return new UploadInfo {
            Key = id,
            Url = _oss.NewDocUpload(now, upload.Key).AbsoluteUri,
            CallbackUrl = "/general_upload_callback/",
            CallbackBody = "key=${object}",
        };
    }
}
