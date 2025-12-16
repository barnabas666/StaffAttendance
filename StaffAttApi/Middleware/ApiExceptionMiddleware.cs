using System.Net;
using System.Text.Json;

namespace StaffAttApi.Middleware;

/// <summary>
/// Middleware that handles unhandled exceptions occurring during HTTP request processing and generates standardized
/// error responses.
/// </summary>
/// <remarks><para> <see cref="ApiExceptionMiddleware"/> intercepts exceptions thrown by downstream middleware or
/// request handlers, logs the error, and returns a problem details response with HTTP status code 500 (Internal Server
/// Error) in the <c>application/problem+json</c> format. </para> <para> Register this middleware early in the ASP.NET
/// Core middleware pipeline to ensure that unhandled exceptions are consistently caught and formatted for API clients.
/// </para></remarks>
public class ApiExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiExceptionMiddleware> _logger;

    public ApiExceptionMiddleware(RequestDelegate next, ILogger<ApiExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Invokes the middleware to process the HTTP request. Catches unhandled exceptions,
    /// logs the error, and returns a standardized error response.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unhandled exception while processing {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            await HandleExceptionAsync(context);
        }
    }

    /// <summary>
    /// Generates a standardized error response for unhandled exceptions.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    private static async Task HandleExceptionAsync(HttpContext context)
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/problem+json";

        var problem = new
        {
            type = "https://httpstatuses.com/500",
            title = "Internal Server Error",
            status = 500,
            detail = "An unexpected error occurred.",
            traceId = context.TraceIdentifier
        };

        var json = JsonSerializer.Serialize(problem);
        await context.Response.WriteAsync(json);
    }
}
