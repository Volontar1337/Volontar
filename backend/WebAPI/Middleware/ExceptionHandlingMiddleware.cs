using System.Net;
using System.Text.Json;
using Application.Exceptions;

namespace WebAPI.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context); // Gå vidare till nästa middleware eller controller
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception caught.");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = ex switch
            {
                DuplicateEmailException => (int)HttpStatusCode.Conflict,
                _ => (int)HttpStatusCode.InternalServerError
            };

            var response = new
            {
                error = ex.Message
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
