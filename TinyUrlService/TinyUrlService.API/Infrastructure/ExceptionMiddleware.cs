using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System;

namespace TinyUrlService.API.Infrastructure;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _ = context ?? throw new ArgumentNullException(nameof(context));

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new { message = "Internal Server Error" };
            var json = JsonSerializer.Serialize(response);

            await context.Response.WriteAsync(json);
        }
    }
}