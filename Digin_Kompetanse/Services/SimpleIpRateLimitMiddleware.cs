namespace Digin_Kompetanse.Services;

public class SimpleIpRateLimitMiddleware
{
    private readonly RequestDelegate _next;
    private static readonly Dictionary<string, List<DateTime>> _requests = new();
    private static readonly object _lock = new();

    public SimpleIpRateLimitMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";
        if (!path.StartsWith("/auth/request-otp") && !path.StartsWith("/auth/verify-otp"))
        {
            await _next(context);
            return;
        }

        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var now = DateTime.UtcNow;

        lock (_lock)
        {
            if (!_requests.ContainsKey(ip))
                _requests[ip] = new List<DateTime>();

            _requests[ip] = _requests[ip].Where(t => (now - t).TotalMinutes < 1).ToList();

            if (_requests[ip].Count >= 30) // f.eks. 30 requests per minutt per IP
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                return;
            }

            _requests[ip].Add(now);
        }

        await _next(context);
    }
}
