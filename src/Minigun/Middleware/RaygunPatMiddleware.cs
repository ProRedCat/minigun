namespace Minigun.Middleware;

public class RaygunPatMiddleware
{
    private readonly RequestDelegate _next;
    private const string RequiredCookieName = "RaygunPAT";
    private const string RedirectPath = "/";
    private static readonly string[] ProtectedPaths = ["/crashreporting"];

    public RaygunPatMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLower();

        if (path != null && ShouldProtectPath(path))
        {
            if (!context.Request.Cookies.ContainsKey(RequiredCookieName))
            {
                context.Response.Redirect(RedirectPath);
                return;
            }
        }

        await _next(context);
    }

    private static bool ShouldProtectPath(string path)
    {
        return ProtectedPaths.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase));
    }
}

public static class RaygunPatMiddlewareExtensions
{
    public static IApplicationBuilder UseRaygunPatMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RaygunPatMiddleware>();
    }
}