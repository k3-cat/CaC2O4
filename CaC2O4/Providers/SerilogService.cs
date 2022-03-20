using Serilog;

namespace CaC2O4.Providers;

static class SerilogService {
    internal static LoggerConfiguration GetConfiguration(IConfiguration cfg, LoggerConfiguration config) {
        return config
            .ReadFrom.Configuration(cfg)
            .WriteTo.Sentry(
                dsn: cfg["Sentry:Dsn"]
            );
    }
}
