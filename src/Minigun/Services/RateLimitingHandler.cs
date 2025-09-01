using System.Threading.RateLimiting;

namespace Minigun.Services;

public class RateLimitingHandler : DelegatingHandler
{
    private readonly RateLimiter _rateLimiter = new SlidingWindowRateLimiter(new SlidingWindowRateLimiterOptions
    {
        PermitLimit = 10,
        Window = TimeSpan.FromSeconds(1),
        SegmentsPerWindow = 1
    });

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        using var lease = await _rateLimiter.AcquireAsync(1, cancellationToken);
        
        if (!lease.IsAcquired)
        {
            throw new HttpRequestException("Rate limit exceeded");
        }

        return await base.SendAsync(request, cancellationToken);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _rateLimiter.Dispose();
        }
        base.Dispose(disposing);
    }
}
