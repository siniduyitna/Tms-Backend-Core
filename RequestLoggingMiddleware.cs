using System.Diagnostics;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 1. ልዩ አጭር መታወቂያ (Correlation ID) መፍጠር
        var correlationId = Guid.NewGuid().ToString("N")[..8];

        // 2. ጥያቄው ገና ሲገባ ሎግ ማድረግ
        _logger.LogInformation("➡️ Entry: {Method} {Path} [CorrelationId: {CorrelationId}]", 
            context.Request.Method, context.Request.Path, correlationId);

        // 3. መልሱ ላይ ሄደሩን ቀድሞ መቅረጽ (ከሚቀጥለው ሚድልዌር በፊት መሆን አለበት)
        context.Response.Headers["X-Correlation-Id"] = correlationId;

        var stopwatch = Stopwatch.StartNew();

        // ወደ ሚቀጥለው ሚድልዌር ማሳለፍ
        await _next(context);

        stopwatch.Stop();

        // 4. ጥያቄው ተጠናቆ ሲወጣ መልሱን ሎግ ማድረግ
        _logger.LogInformation("⬅️ Exit: {Path} responded {StatusCode} in {ElapsedMs}ms [CorrelationId: {CorrelationId}]", 
            context.Request.Path, context.Response.StatusCode, stopwatch.ElapsedMilliseconds, correlationId);
    }
}