using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using MyTemplate.Api.Common.Exceptions;
using System.Net;
using System.Text.Json;

namespace MyTemplate.Api.Common.Behaviors;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

        var response = httpContext.Response;
        response.ContentType = "application/json";

        var responseModel = new ErrorResponse
        {
            Title = "An error occurred",
            Status = (int)HttpStatusCode.InternalServerError,
            Detail = exception.Message
        };

        switch (exception)
        {
            case ValidationException validationException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                responseModel.Status = (int)HttpStatusCode.BadRequest;
                responseModel.Title = "Validation Error";
                responseModel.Detail = "One or more validation errors occurred";
                responseModel.Errors = validationException.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray());
                break;

            case NotFoundException:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                responseModel.Status = (int)HttpStatusCode.NotFound;
                responseModel.Title = "Not Found";
                break;

            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Detail = "An internal server error occurred";
                break;
        }

        var jsonResponse = JsonSerializer.Serialize(responseModel);
        await response.WriteAsync(jsonResponse, cancellationToken);

        return true;
    }

    private class ErrorResponse
    {
        public string Title { get; set; } = string.Empty;
        public int Status { get; set; }
        public string Detail { get; set; } = string.Empty;
        public Dictionary<string, string[]>? Errors { get; set; }
    }
}