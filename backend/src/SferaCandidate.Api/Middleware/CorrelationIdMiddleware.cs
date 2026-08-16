using Microsoft.Extensions.Primitives;

namespace SferaCandidate.Api.Middleware;

public sealed class CorrelationIdMiddleware(
    RequestDelegate next,
    ILogger<CorrelationIdMiddleware> logger)
{
    public const string HeaderName = "X-Correlation-ID";
    private const int MaximumIncomingLength = 128;

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = ResolveCorrelationId(context.Request.Headers);

        context.TraceIdentifier = correlationId;

        // ExceptionHandlerMiddleware can clear an in-progress response before it
        // writes a handled error. Registering OnStarting guarantees the correlation
        // header is attached to the final response, including 4xx/5xx error models.
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[HeaderName] = correlationId;
            return Task.CompletedTask;
        });

        using (logger.BeginScope(new Dictionary<string, object>
               {
                   ["CorrelationId"] = correlationId
               }))
        {
            await next(context);
        }
    }

    private static string ResolveCorrelationId(IHeaderDictionary headers)
    {
        if (headers.TryGetValue(HeaderName, out StringValues incoming))
        {
            var value = incoming.ToString().Trim();

            if (value.Length is > 0 and <= MaximumIncomingLength &&
                value.All(character => !char.IsControl(character)))
            {
                return value;
            }
        }

        return Guid.NewGuid().ToString("N");
    }
}
