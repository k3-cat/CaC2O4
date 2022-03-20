using Grpc.Core;

namespace CaC2O4.Utils;

static class TxnTime {
    internal static DateTimeOffset Get(HttpContext ctx) {
        return ctx.Features.Get<DateTimeOffset>()!;
    }

    internal static DateTimeOffset Get(ServerCallContext ctx) {
        return Get(ctx.GetHttpContext());
    }
}
