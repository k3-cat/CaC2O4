namespace CaC2O4.Features;

public class TxnTimeMiddleware {
    readonly RequestDelegate _next;

    public TxnTimeMiddleware(RequestDelegate next) {
        _next = next;
    }

    public Task InvokeAsync(HttpContext context) {
        context.Features.Set(DateTimeOffset.Now);

        return _next(context);
    }
}
