using Microsoft.AspNetCore.Http.HttpResults;
using SferaCandidate.Application.Assessments;
using SferaCandidate.Application.Roi;
using SferaCandidate.Domain.Roi;

namespace SferaCandidate.Api.Endpoints;

public static class AssessmentEndpoints
{
    public static IEndpointRouteBuilder MapCandidateAssessmentEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/assessments")
            .WithTags("Assessments");

        group.MapPost(string.Empty, CreateAssessmentAsync)
            .WithName("CreateAssessment")
            .Produces<AssessmentDto>(StatusCodes.Status201Created);

        group.MapGet("/{assessmentId:guid}", GetAssessmentAsync)
            .WithName("GetAssessment")
            .Produces<AssessmentDto>();

        group.MapPut("/{assessmentId:guid}/answers/{questionKey}", SaveAnswerAsync)
            .WithName("SaveAssessmentAnswer")
            .Produces<SaveAnswerResult>();

        group.MapPost("/{assessmentId:guid}/evaluate", EvaluateAsync)
            .WithName("EvaluateAssessment")
            .Produces<RecommendationResultDto>();

        group.MapGet("/{assessmentId:guid}/result", GetResultAsync)
            .WithName("GetAssessmentResult")
            .Produces<RecommendationResultDto>();

        group.MapPost("/{assessmentId:guid}/roi", CalculateRoiAsync)
            .WithName("CalculateRoi")
            .Produces<RoiScenarioDto>();

        return endpoints;
    }

    private static async Task<Created<AssessmentDto>> CreateAssessmentAsync(
        CreateAssessmentHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(cancellationToken);
        return TypedResults.Created($"/api/v1/assessments/{result.Id}", result);
    }

    private static Task<AssessmentDto> GetAssessmentAsync(
        Guid assessmentId,
        GetAssessmentHandler handler,
        CancellationToken cancellationToken) =>
        handler.HandleAsync(assessmentId, cancellationToken);

    private static Task<SaveAnswerResult> SaveAnswerAsync(
        Guid assessmentId,
        string questionKey,
        SaveAnswerRequest request,
        SaveAnswerHandler handler,
        CancellationToken cancellationToken) =>
        handler.HandleAsync(assessmentId, questionKey, request.Value, cancellationToken);

    private static Task<RecommendationResultDto> EvaluateAsync(
        Guid assessmentId,
        EvaluateAssessmentHandler handler,
        CancellationToken cancellationToken) =>
        handler.HandleAsync(assessmentId, cancellationToken);

    private static Task<RecommendationResultDto> GetResultAsync(
        Guid assessmentId,
        GetAssessmentResultHandler handler,
        CancellationToken cancellationToken) =>
        handler.HandleAsync(assessmentId, cancellationToken);

    private static Task<RoiScenarioDto> CalculateRoiAsync(
        Guid assessmentId,
        CalculateRoiRequest request,
        CalculateRoiHandler handler,
        CancellationToken cancellationToken) =>
        handler.HandleAsync(
            assessmentId,
            new CalculateRoiCommand(
                request.ScenarioName,
                request.CasesPerMonth,
                request.MinutesSavedPerCase,
                request.LoadedHourlyCost,
                request.AnnualOperatingCost,
                request.ImplementationCost),
            cancellationToken);
}

public sealed record SaveAnswerRequest(string Value);

public sealed record CalculateRoiRequest(
    RoiScenarioName ScenarioName,
    decimal CasesPerMonth,
    decimal MinutesSavedPerCase,
    decimal LoadedHourlyCost,
    decimal AnnualOperatingCost,
    decimal ImplementationCost);
