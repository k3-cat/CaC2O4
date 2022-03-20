using CaC2O4.Services.Admin;
using CaC2O4.Services.Auth;
using CaC2O4.Services.Office;
using CaC2O4.Services.Record;
using CaC2O4.Services.Upload;

namespace CaC2O4.Services;

public class ServiceHelper {
    public static void MapGrpcService(WebApplication app) {
        app.MapGrpcService<AdminService>();
        app.MapGrpcService<AuthenticationService>();
        app.MapGrpcService<OfficeService>();
        app.MapGrpcService<RecordService>();
        app.MapGrpcService<UploadService>();
    }
}
