using _250828_universityTask.Exceptions;
using System;
using System.ComponentModel.DataAnnotations;

// 400 Bad Request  => ValidationException
// 403 Forbid => if claims don’t match / wrong role
// 404 Not Found => if student doesn’t exist
// 200 OK with DTO => when successful
// 500 Internal Server Error => everything else

namespace _250828_universityTask.Middleware
{
    public class ExceptionMiddleware
    {
        // private only class can access and readonly: once assigned in constructor, they can't change

        // Represents next middleware in ASP.NET Core pipeline => _next(context) passes req down the chain => stores next middleware in pipeline
        private readonly RequestDelegate _next;
        // Used to log exceptions or errors that occur during req processing - stores logger for logging errors
        private readonly ILogger<ExceptionMiddleware> _logger;

        // constructor injection => You don’t have to manually create _next or _logger => framework does it for you
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        // Tries to execute next middleware in Pipeline => req will continue to other middleware
        // If any exception occurs Downstream => will be caught by this middleware
        // Clients always get proper status codes + error messages
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exceptions.ValidationException vex)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new { vex.Errors });
            }
            catch (UnauthorizedAccessException)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
            }
            catch (KeyNotFoundException)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred.");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new { error = "An unexpected error occurred." });
            }
        }
    }
}
