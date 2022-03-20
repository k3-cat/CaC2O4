using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ProtobufWebToken.AspNetCore;
using ProtobufWebToken.Key;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();
Log.Information("Starting up!");


// -------------------------------------------
// - - - - [Configure Services] - - - - - - -
// -------------------------------------------

var builder = WebApplication.CreateBuilder();

// Unix Socket
if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
    builder.WebHost.ConfigureKestrel(options => {
        options.ListenUnixSocket(builder.Configuration["UnixSocket"]);
    });
}

// Database
builder.Services.AddDbContext<CaC2O4.Repositories.DbCtx>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("Database"),
        x => x.MigrationsAssembly("CaC2O4.Repositories")
    ));

// Controller
builder.Services.AddGrpc();

builder.Services
    .AddControllers()
    .AddJsonOptions(option => {
        option.JsonSerializerOptions.Converters.Clear();
        option.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        option.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

// Log
builder.Host.UseSerilog((ctx, config) =>
    CaC2O4.Providers.SerilogService.GetConfiguration(builder.Configuration, config)
);

// Health Check
builder.Services.AddGrpcHealthChecks()
    .AddCheck("Sample", () => HealthCheckResult.Healthy());

builder.Services.AddHealthChecks()
    .AddDbContextCheck<CaC2O4.Repositories.DbCtx>();

// Auth
builder.Services
    .AddAuthentication(options => {
        options.DefaultScheme = PwtBearerDefaults.AuthenticationScheme;
        options.DefaultAuthenticateScheme = PwtBearerDefaults.AuthenticationScheme;
    })
    .AddPwtBearer(options => {
        var key = new EcPrivateKey(
            builder.Configuration["PwtKeyInfo:CruveOid"],
            builder.Configuration["PwtKeyInfo:HashAlgOid"],
            builder.Configuration["PwtKeyInfo:PrivateKey"]
        );
        options.SigningKey = key;
        options.SigningKeys.Add(key.KeyId, key);
    });

builder.Services.AddAuthorization(options => {
    options.FallbackPolicy = options.DefaultPolicy;

    options.AddPolicy("Admin", policy => {
        var roles = new CaC2O4.Repositories.UserAcl[] {
            CaC2O4.Repositories.UserAcl.Admin,
            CaC2O4.Repositories.UserAcl.Super
        };
        policy.RequireAuthenticatedUser();
        policy.RequireRole(roles.Select(x => x.ToString()).ToArray());
    });

    options.AddPolicy("Archivist", policy => {
        var roles = new CaC2O4.Repositories.UserAcl[] {
            CaC2O4.Repositories.UserAcl.Archivist,
            CaC2O4.Repositories.UserAcl.Admin,
            CaC2O4.Repositories.UserAcl.Super
        };
        policy.RequireAuthenticatedUser();
        policy.RequireRole(roles.Select(x => x.ToString()).ToArray());
    });

    options.AddPolicy("Staff", policy => {
        var roles = new CaC2O4.Repositories.UserAcl[] {
            CaC2O4.Repositories.UserAcl.Staff,
            CaC2O4.Repositories.UserAcl.Archivist,
            CaC2O4.Repositories.UserAcl.Admin,
            CaC2O4.Repositories.UserAcl.Super
        };
        policy.RequireAuthenticatedUser();
        policy.RequireRole(roles.Select(x => x.ToString()).ToArray());
    });
});

// AliOss
builder.Services.AddScoped<CaC2O4.Providers.IAliOss, CaC2O4.Providers.AliOss>();

// Debug
builder.Services.AddDatabaseDeveloperPageExceptionFilter();


// -------------------------------------------
// - - - - [Configure] - - - - - - - - - - - -
// -------------------------------------------

var app = builder.Build();

if (builder.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<CaC2O4.Features.TxnTimeMiddleware>();
app.UseSerilogRequestLogging(options =>
    options.EnrichDiagnosticContext = (diagnosticCtx, httpCtx) => {
        diagnosticCtx.Set("IP", httpCtx.Connection.RemoteIpAddress);
        diagnosticCtx.Set("TxnTime", CaC2O4.Utils.TxnTime.Get(httpCtx));
        diagnosticCtx.Set("UserId", CaC2O4.Utils.Auth.GetUserId(httpCtx));
        diagnosticCtx.Set("UserName", CaC2O4.Utils.Auth.GetUserName(httpCtx));
    }
);

app.MapControllers();
CaC2O4.Services.ServiceHelper.MapGrpcService(app);

app.MapGrpcHealthChecksService();
app.MapHealthChecks("/healthz")
    .AllowAnonymous();


// -------------------------------------------
// - - - - [Run] - - - - - - - - - - - - - - -
// -------------------------------------------

try {
    app.Run();

    Log.Information("Stopped cleanly");
}
catch (Exception ex) {
    Log.Fatal(ex, "An unhandled exception occurred during bootstrapping");
}
finally {
    Log.CloseAndFlush();
}
