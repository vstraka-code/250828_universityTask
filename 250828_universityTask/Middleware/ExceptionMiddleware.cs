using _250828_universityTask.Exceptions;
using _250828_universityTask.Logger;
using _250828_universityTask.Models.Dtos;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

// 400 Bad Request  => ValidationException
// 403 Forbid => if claims don’t match / wrong role
// 404 Not Found => if student doesn’t exist
// 200 OK with DTO => when successful
// 500 Internal Server Error => everything else

namespace _250828_universityTask.Middleware
{
    public class ExceptionMiddleware
    {
        // Represents next middleware in ASP.NET Core pipeline => _next(context) passes req down the chain => stores next middleware in pipeline
        private readonly RequestDelegate _next;
        // Used to log exceptions or errors that occur during req processing, stores logger for logging errors
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly FileLoggerProvider _fileLoggerProvider;

        // constructor injection => You don’t have to manually create _next or _logger => framework does it for you
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, FileLoggerProvider fileLoggerProvider)
        {
            _next = next;
            _logger = logger;
            _fileLoggerProvider = fileLoggerProvider;
        }

        // Tries to execute next middleware in Pipeline => req will continue to other middleware
        // If any exception occurs Downstream => will be caught by this middleware
        // Clients always get proper status codes + error messages
        // framework looks for method called Invoke with HttpContext parameter
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exceptions.ValidationException vex)
            {
                await WriteJsonResponse(context, StatusCodes.Status400BadRequest, "Validation failed", _fileLoggerProvider, vex.Errors);
                _logger.LogWarning("Validation failed: {Message}", vex.Message);
            }
            catch (UnauthorizedAccessException uaex)
            {
                await WriteJsonResponse(context, StatusCodes.Status403Forbidden, uaex.Message ?? "Forbidden", _fileLoggerProvider);
                _logger.LogWarning("Unauthorized access: {Message}", uaex.Message);
            }
            catch (KeyNotFoundException knfex)
            {
                await WriteJsonResponse(context, StatusCodes.Status404NotFound, knfex.Message ?? "Resource not found", _fileLoggerProvider);
                _logger.LogWarning("Resource not found: {Message}", knfex.Message);
            }
            catch (ArgumentNullException anex)
            {
                await WriteJsonResponse(context, StatusCodes.Status400BadRequest, anex.Message ?? "Value cannot be null.", _fileLoggerProvider);
                _logger.LogWarning("Argument mull exception: {Message}", anex.Message);
            }
            catch (Exception ex)
            {
                await WriteJsonResponse(context, StatusCodes.Status500InternalServerError, "An unexpected error occurred.", _fileLoggerProvider); // new { ex.Message, ex.StackTrace }
                _logger.LogError("Unhandled exception occurred: {Message}", ex.Message);
            }
        }

        private static async Task WriteJsonResponse(HttpContext context, int statusCode, string message, FileLoggerProvider fileLoggerProvider, object? errors = null)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var payload = new ErrorResponse(
                                statusCode,
                                message,
                                JsonSerializer.Serialize(errors));

            string mess = "Status Code: " + statusCode.ToString() + ", Message: " + message + ", Errors: " + JsonSerializer.Serialize(errors);
            fileLoggerProvider.SaveExceptionLogging(mess);

            await context.Response.WriteAsJsonAsync(payload);
        }
    }
}
