using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TinyUrlService.API.Infrastructure;

public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
{
    private readonly ILogger<ApiExceptionFilterAttribute> _logger;

    public ApiExceptionFilterAttribute(ILogger<ApiExceptionFilterAttribute> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override void OnException(ExceptionContext context)
    {
        _ = context ?? throw new ArgumentNullException(nameof(context));

        _logger.LogError(context.Exception, "An unhandled exception occurred.");

        var response = new
        {
            Error = "An unexpected error occurred.",
            Details = context.Exception.Message
        };

        context.Result = new JsonResult(response)
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };

        base.OnException(context);
    }
}


