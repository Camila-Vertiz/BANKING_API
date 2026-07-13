using FluentValidation;
using System.Net;
using System.Text.Json;

namespace Banking.Api.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                await HandleValidationExceptionAsync(context, ex);
            }
            catch (KeyNotFoundException ex)
            {
                await HandleNotFoundExceptionAsync(context, ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                await HandleUnauthorizedExceptionAsync(context, ex);
            }
            catch (InvalidOperationException ex)
            {
                await HandleConflictExceptionAsync(context, ex);
            }
            catch (Exception ex)
            {
                await HandleGenericExceptionAsync(context, ex);
            }
        }

        private static async Task HandleValidationExceptionAsync(
            HttpContext context,
            ValidationException exception)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            context.Response.ContentType = "application/json";

            var errors = exception.Errors.Select(exception => exception.ErrorMessage);

            var response = new
            {
                statusCode = 400,
                message = "Validation failed",
                errors
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }

        private static async Task HandleNotFoundExceptionAsync(
            HttpContext context,
            KeyNotFoundException exception)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;

            context.Response.ContentType = "application/json";

            var response = new
            {
                statusCode = 404,
                message = exception.Message
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }

        private async Task HandleGenericExceptionAsync(
            HttpContext context,
            Exception exception)
        {
            _logger.LogError(exception, exception.Message);

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            context.Response.ContentType = "application/json";

            var response = new
            {
                statusCode = 500,
                message = "An unexpected error occurred."
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }

        private static async Task HandleUnauthorizedExceptionAsync(
            HttpContext context,
            UnauthorizedAccessException exception)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;

            context.Response.ContentType = "application/json";

            var response = new
            {
                statusCode = 403,
                message = exception.Message
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response));
        }

        private static async Task HandleConflictExceptionAsync(
            HttpContext context,
            InvalidOperationException exception)
        {
            context.Response.StatusCode = StatusCodes.Status409Conflict;

            context.Response.ContentType = "application/json";

            var response = new
            {
                statusCode = 409,
                message = exception.Message
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response));
        }
    }
}
