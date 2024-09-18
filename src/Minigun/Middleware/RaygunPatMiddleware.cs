namespace Minigun.Middleware;

public class RaygunPatMiddleware
{
    private readonly RequestDelegate _next;
    private const string RequiredCookieName = "RaygunPAT";
    private const string RedirectPath = "/";
    private static readonly string[] ExcludedPaths = ["/", "/favicon.ico"];
    private static readonly string[] ExcludedExtensions = [".css", ".js", ".png", ".jpg", ".jpeg", ".gif", ".svg"];

    public RaygunPatMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLower();

        // Allow access to excluded paths and static resources
        if (path != null && ShouldAllowRequest(path))
        {
            await _next(context);
            return;
        }

        // Check for PAT cookie
        if (!context.Request.Cookies.ContainsKey(RequiredCookieName))
        {
            // Redirect to home page if PAT cookie is missing
            context.Response.Redirect(RedirectPath);
            return;
        }

        // PAT cookie exists, continue to the next middleware
        await _next(context);
    }

    private static bool ShouldAllowRequest(string path)
    {
        if (string.IsNullOrEmpty(path)) return false;

        return ExcludedPaths.Any(p => path.Equals(p, StringComparison.OrdinalIgnoreCase)) ||
               ExcludedExtensions.Any(ext => path.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
    }
}

public static class RaygunPatMiddlewareExtensions
{
    public static IApplicationBuilder UseRaygunPatMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RaygunPatMiddleware>();
    }
}