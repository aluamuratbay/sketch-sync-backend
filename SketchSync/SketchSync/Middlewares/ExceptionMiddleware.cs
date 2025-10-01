using Microsoft.AspNetCore.Mvc;
using SketchSync.Exceptions;

namespace SketchSync.Middlewares;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(context, ex, logger);
        }   
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception, ILogger logger)
    {
        var exceptionDetails = GetExceptionDetails(exception);
        logger.LogError("Exception Type: {ExceptionType}, Title: {Title}, Detail: {Detail}", exceptionDetails.Type, exceptionDetails.Title, exceptionDetails.Detail);

        var problemDetails = new ProblemDetails
        {
            Status = exceptionDetails.Status,
            Type = exceptionDetails.Type,
            Title = exceptionDetails.Title,
            Detail = exceptionDetails.Detail
        };

        if (exceptionDetails.Errors is not null)
        {
            problemDetails.Extensions["errors"] = exceptionDetails.Errors;
        }

        context.Response.StatusCode = exceptionDetails.Status;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(problemDetails);
    }
 
    private static ExceptionDetails GetExceptionDetails(Exception exception)
    {
        return exception switch
        {
            BadHttpRequestException badHttpRequestException => new ExceptionDetails(
                StatusCodes.Status400BadRequest,
                "BadRequest",
                "Bad Request",
                badHttpRequestException.Message,
                null),
            NotFoundException notFoundException => new ExceptionDetails(
                StatusCodes.Status404NotFound,
                "NotFound",
                "Not Found",
                notFoundException.Message,
                null),
            ForbiddenException forbiddenException => new ExceptionDetails(
                StatusCodes.Status403Forbidden,
                "Forbidden",
                "Forbidden",
                forbiddenException.Message,
                null),
            UnauthorizedException unauthorizedException => new ExceptionDetails(
                StatusCodes.Status401Unauthorized,
                "Unauthorized",
                "Unauthorized",
                unauthorizedException.Message,
                null),
            ValidationException validationException => new ExceptionDetails(
                StatusCodes.Status400BadRequest,
                "ValidationFailure",
                "Validation error",
                "One or more validation errors has occurred",
                validationException.Errors),
            _ => new ExceptionDetails(
                StatusCodes.Status500InternalServerError,
                "ServerError",
                "Server Error",
                "An unexpected error has occurred",
                null)
        };
    }
    
    private record ExceptionDetails(
        int Status,
        string Type,
        string Title,
        string Detail,
        IEnumerable<object>? Errors);
}