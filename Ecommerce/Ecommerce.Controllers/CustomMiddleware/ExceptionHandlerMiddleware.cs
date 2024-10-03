using System.Text.Json;
using System.Text.Json.Serialization;
using Ecommerce.Controllers.CustomExceptions;
using Ecommerce.Domain.Common.Exceptions;
using Ecommerce.Services.AuthService.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Ecommerce.Controllers.CustomMiddleware
{
    public class ExceptionHandlerMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;
        private readonly IConfiguration _configuration;
        private readonly string allowedOrigin;

        public ExceptionHandlerMiddleware(ILogger<ExceptionHandlerMiddleware> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            allowedOrigin = _configuration["AllowedOrigin"] ?? throw new ArgumentNullException(nameof(allowedOrigin));
        }

        private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
        {
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (PostgresException ex)
            {
                await HandlePostgresExceptionAsync(context, ex);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            const string contentType = "application/json";
            context.Response.Clear();
            context.Response.ContentType = contentType;

            context.Response.Headers.Append("Access-Control-Allow-Origin", $"{allowedOrigin}");
            context.Response.Headers.Append("Access-Control-Allow-Methods", "GET, POST, PUT, PATCH, DELETE, OPTIONS");
            context.Response.Headers.Append("Access-Control-Allow-Headers", "Content-Type, Authorization");
            context.Response.Headers.Append("Access-Control-Allow-Credentials", "true");


            ProblemDetails problemDetails;
            string json;

            switch (ex)
            {
                case InsufficientStockException insufficientStockException:
                    context.Response.StatusCode = 400;
                    problemDetails = CreateProblemDetails(context, 400, insufficientStockException.Message);
                    json = ToJson(problemDetails);
                    await context.Response.WriteAsync(json);
                    break;
                case WrongCredentialsException wrongCredentialsException:
                    context.Response.StatusCode = 400;
                    problemDetails = CreateProblemDetails(context, 400, wrongCredentialsException.Message);
                    json = ToJson(problemDetails);
                    await context.Response.WriteAsync(json);
                    break;
                case NotUniqueEmailException notUniqueEmailException:
                    context.Response.StatusCode = 409;
                    problemDetails = CreateProblemDetails(context, 409, notUniqueEmailException.Message);
                    json = ToJson(problemDetails);
                    await context.Response.WriteAsync(json);
                    break;
                case UnauthorizedAccessException unauthorizedAccessException:
                    context.Response.StatusCode = 403;
                    problemDetails = CreateProblemDetails(context, 403, unauthorizedAccessException.Message);
                    json = ToJson(problemDetails);
                    await context.Response.WriteAsync(json);
                    break;
                case ContstraintViolationException contstraintViolationException:
                    context.Response.StatusCode = 409;
                    problemDetails = CreateProblemDetails(context, 409, contstraintViolationException.Message);
                    json = ToJson(problemDetails);
                    await context.Response.WriteAsync(json);
                    break;
                case DbUpdateException dbUpdateException:
                    context.Response.StatusCode = 418;
                    problemDetails = CreateProblemDetails(context, 418, dbUpdateException.Message);
                    json = ToJson(problemDetails);
                    await context.Response.WriteAsync(json);
                    break;
                case InvalidTokenException invalidTokenException:
                    context.Response.StatusCode = 400;
                    problemDetails = CreateProblemDetails(context, 400, invalidTokenException.Message);
                    json = ToJson(problemDetails);
                    await context.Response.WriteAsync(json);
                    break;
                case MethodNotAllowedException methodNotAllowedException:
                    context.Response.StatusCode = 405;
                    problemDetails = CreateProblemDetails(context, 405, methodNotAllowedException.Message);
                    json = ToJson(problemDetails);
                    await context.Response.WriteAsync(json);
                    break;
                default:
                    if (IsEntityNotFoundException(ex, out string entityName))
                    {
                        context.Response.StatusCode = 404;
                        problemDetails = CreateProblemDetails(context, 404, $"{entityName} not found");
                        json = ToJson(problemDetails);
                        await context.Response.WriteAsync(json);
                    }
                    else
                    {
                        context.Response.StatusCode = 500;
                        problemDetails = CreateProblemDetails(context, 500, "An unexpected error occurred.");
                        json = ToJson(problemDetails);
                        await context.Response.WriteAsync(json);
                    }
                    break;
            }
            _logger.LogError(ex, "An exception has occurred: {Message}", ex.Message);
        }

        private static bool IsEntityNotFoundException(Exception exception, out string entityName)
        {
            entityName = string.Empty;
            var type = exception.GetType();
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(EntityNotFoundException<>))
            {
                entityName = type.GetGenericArguments()[0].Name;
                return true;
            }
            return false;
        }

        private async Task HandlePostgresExceptionAsync(HttpContext context, PostgresException ex)
        {

            const string contentType = "application/json;";

            context.Response.Clear();
            context.Response.ContentType = contentType;

            context.Response.Headers.Append("Access-Control-Allow-Origin", $"{allowedOrigin}");
            context.Response.Headers.Append("Access-Control-Allow-Methods", "GET, POST, PUT, PATCH, DELETE, OPTIONS");
            context.Response.Headers.Append("Access-Control-Allow-Headers", "Content-Type, Authorization");
            context.Response.Headers.Append("Access-Control-Allow-Credentials", "true");


            ProblemDetails problemDetails;
            string json;

            switch (ex.SqlState)
            {
                case "23505":
                    problemDetails = CreateProblemDetails(context, 409, $"Duplicate key error: {ex.MessageText}");
                    json = ToJson(problemDetails);
                    await context.Response.WriteAsync(json);
                    break;
                case "P0001":
                    problemDetails = CreateProblemDetails(context, 400, $"Something went wrong: {ex.MessageText}");
                    json = ToJson(problemDetails);
                    await context.Response.WriteAsync(json);
                    break;
                default:
                    problemDetails = CreateProblemDetails(context, 500, ex.Message);
                    json = ToJson(problemDetails);
                    await context.Response.WriteAsync(json);
                    break;
            }

            _logger.LogError(ex, "PostgresException occurred: {Message}", ex.Message);
        }

        private static ProblemDetails CreateProblemDetails(HttpContext context, int statusCode, string detail)
        {
            context.Response.StatusCode = statusCode;
            return new ProblemDetails
            {
                Status = statusCode,
                Detail = detail
            };
        }

        private string ToJson(ProblemDetails problemDetails)
        {
            try
            {
                return JsonSerializer.Serialize(problemDetails, SerializerOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception has occurred while serializing error to JSON.");
            }
            return string.Empty;
        }
    }
}