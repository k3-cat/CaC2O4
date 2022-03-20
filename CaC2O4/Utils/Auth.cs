using System.Security.Claims;
using Grpc.Core;

namespace CaC2O4.Utils;

static class Auth {
    static readonly String _guidNone = Guid.Empty.ToString();

    internal static Guid GetUserId(HttpContext ctx) {
        var guid = ctx.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? _guidNone;
        return Guid.Parse(guid);
    }

    internal static Guid GetUserId(ServerCallContext ctx) {
        return GetUserId(ctx.GetHttpContext());
    }

    internal static String GetUserName(HttpContext ctx) {
        return ctx.User.Identity?.Name ?? "";
    }

    internal static String GetUserName(ServerCallContext ctx) {
        return GetUserName(ctx.GetHttpContext());
    }
}
