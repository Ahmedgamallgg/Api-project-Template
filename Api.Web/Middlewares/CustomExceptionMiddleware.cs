using Domain.Exceptions;
using Shared.ErrorModels;

namespace Api.Web.Middlewares;

public class CustomExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CustomExceptionMiddleware> _logger;

    public CustomExceptionMiddleware(RequestDelegate next, ILogger<CustomExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next.Invoke(context);
            await HandleNotFoundEndPointAsync(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleNotFoundEndPointAsync(HttpContext context)
    {
        // Logic 
        if (context.Response.StatusCode == StatusCodes.Status404NotFound)
        {
            context.Response.ContentType = "application/json";

            var response = new ErrorDetails
            {
                ErrorMessage = $"Path {context.Request.Path} Not Found",
                StatusCode = StatusCodes.Status404NotFound
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        _logger.LogError(ex, "Something Went Wrong");


        context.Response.ContentType = "application/json";
        //context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var response = new ErrorDetails
        {
            ErrorMessage = ex.Message,
        };
        response.StatusCode = ex switch
        {
            NotFoundException => StatusCodes.Status404NotFound,
            UnauthorizedException => StatusCodes.Status401Unauthorized,
            BadRequestException exception => GetErrors(exception, response),
            _ => StatusCodes.Status500InternalServerError
        };
        context.Response.StatusCode = response.StatusCode;
        await context.Response.WriteAsJsonAsync(response);
    }

    private static int GetErrors(BadRequestException exception, ErrorDetails response)
    {
        response.Errors = exception.Errors;
        return StatusCodes.Status400BadRequest;
    }
}

public static class CustomExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<CustomExceptionMiddleware>();
    }
}
