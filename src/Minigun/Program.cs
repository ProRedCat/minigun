using Microsoft.AspNetCore.Mvc.Razor;
using Mindscape.Raygun4Net.AspNetCore;
using Minigun.Middleware;
using Minigun.Services;
using System.Threading.RateLimiting;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient();

builder.Services.AddRaygun(builder.Configuration).AddRaygunUserProvider();

builder.Services.AddHttpContextAccessor();

// Configure rate limiting for Raygun API (10 requests per second)
builder.Services.AddTransient<RateLimitingHandler>();
builder.Services.AddHttpClient<IRaygunApiService, RaygunApiService>()
    .AddHttpMessageHandler<RateLimitingHandler>();

builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
    options.AppendTrailingSlash = false;
});

builder.Services.Configure<RazorViewEngineOptions>(options =>
{
    options.AreaViewLocationFormats.Clear();
    options.AreaViewLocationFormats.Add("/Areas/{2}/Views/{1}/{0}.cshtml");
    options.AreaViewLocationFormats.Add("/Areas/{2}/Views/Shared/{0}.cshtml");
    options.AreaViewLocationFormats.Add("/Views/Shared/{0}.cshtml");
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseRaygunPatMiddleware();

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.UseStaticFiles();


// This feels gross and wrong
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "root",
    pattern: "/",
    defaults: new { area = "Home", controller = "Home", action = "Index" });

app.Run();
