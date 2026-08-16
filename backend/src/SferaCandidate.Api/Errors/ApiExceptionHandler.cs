using Microsoft.AspNetCore.Diagnostics;
using SferaCandidate.Application.Common;
using SferaCandidate.Domain.Common;

namespace SferaCandidate.Api.Errors;

public sealed class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, code, message) = exception switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, "NOT_FOUND", exception.Message),
            ConflictException => (StatusCodes.Status409Conflict, "CONFLICT", exception.Message),
            DomainValidationException => (StatusCodes.Status422UnprocessableEntity, "VALIDATION_FAILED", exception.Message),
            _ => (StatusCodes.Status500InternalServerError, "INTERNAL_ERROR", "An unexpected server error occurred.")
        };

        if (statusCode >= 500)
        {
            logger.LogError(exception, "Unhandled API exception. CorrelationId: {CorrelationId}", httpContext.TraceIdentifier);
        }
        else
        {
            logger.LogInformation(
                exception,
                "Handled API exception {Code}. CorrelationId: {CorrelationId}",
                code,
                httpContext.TraceIdentifier);
        }

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(
            new ApiErrorResponse(
                new ApiError(
                    code,
                    message,
                    httpContext.TraceIdentifier,
                    [])),
            cancellationToken);

        return true;
    }
}

public sealed record ApiErrorResponse(ApiError Error);

public sealed record ApiError(
    string Code,
    string Message,
    string CorrelationId,
    IReadOnlyList<ApiFieldError> Fields);

public sealed record ApiFieldError(string Field, string Code);
