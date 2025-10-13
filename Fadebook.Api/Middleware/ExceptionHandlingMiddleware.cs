using System.Net;
using System.Text.Json;
using Fadebook.Exceptions;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Fadebook.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _env;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IWebHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = HttpStatusCode.InternalServerError;
        var message = "An unexpected error occurred. Please try again later.";
        var isDevelopment = _env.IsDevelopment();

        switch (exception)
        {
            case UnauthorizedException unauthorizedEx:
                statusCode = HttpStatusCode.Unauthorized;
                message = unauthorizedEx.Message;
                break;

            case NotFoundException notFoundEx:
                statusCode = HttpStatusCode.NotFound;
                message = notFoundEx.Message;
                break;

            case ConflictException conflictEx:
                statusCode = HttpStatusCode.Conflict;
                message = conflictEx.Message;
                break;

            case BadRequestException badRequestEx:
                statusCode = HttpStatusCode.BadRequest;
                message = badRequestEx.Message;
                break;

            case NoUsernameException noUsernameEx:
                statusCode = HttpStatusCode.BadRequest;
                message = noUsernameEx.Message;
                break;

            case KeyNotFoundException:
                statusCode = HttpStatusCode.NotFound;
                message = "The requested resource was not found.";
                break;

            case ValidationException validationEx:
                statusCode = HttpStatusCode.BadRequest;
                message = validationEx.Message;
                break;

            case ArgumentNullException:
                statusCode = HttpStatusCode.BadRequest;
                message = "Required parameter is missing.";
                break;

            case ArgumentException:
                statusCode = HttpStatusCode.BadRequest;
                message = "Invalid parameter value provided.";
                break;

            case InvalidOperationException:
                statusCode = HttpStatusCode.Conflict;
                message = "The requested operation could not be completed.";
                break;

            case UnauthorizedAccessException:
                statusCode = HttpStatusCode.Unauthorized;
                message = "You are not authorized to perform this action.";
                break;

            case DbUpdateException dbEx:
                statusCode = HttpStatusCode.Conflict;
                // Don't expose SQL-specific details in production
                if (isDevelopment)
                {
                    message = $"Database update error: {dbEx.InnerException?.Message ?? dbEx.Message}";
                }
                else
                {
                    message = "A database conflict occurred. Please verify your data and try again.";
                }
                break;

            default:
                // For unhandled exceptions, never expose details in production
                if (isDevelopment)
                {
                    message = $"Internal error: {exception.Message}";
                }
                break;
        }

        // Create response object based on environment
        object response;
        if (isDevelopment)
        {
            response = new
            {
                status = (int)statusCode,
                message = message,
                timestamp = DateTime.UtcNow,
                path = context.Request.Path.Value,
                traceId = context.TraceIdentifier,
                exceptionType = exception.GetType().Name
            };
        }
        else
        {
            response = new
            {
                status = (int)statusCode,
                message = message,
                timestamp = DateTime.UtcNow
            };
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
    }
}