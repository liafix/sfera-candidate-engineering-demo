using System.Text.Json;
using SferaCandidate.Application.Abstractions.Persistence;
using SferaCandidate.Application.Common;

namespace SferaCandidate.Application.Assessments;

public sealed class GetAssessmentResultHandler(
    IAssessmentRepository assessments,
    IRecommendationRepository recommendations,
    ISyntheticLeadRepository leads)
{
    public async Task<RecommendationResultDto> HandleAsync(
        Guid assessmentId,
        CancellationToken cancellationToken = default)
    {
        _ = await assessments.GetByIdAsync(assessmentId, cancellationToken)
            ?? throw new NotFoundException($"Assessment '{assessmentId}' was not found.");

        var recommendation = await recommendations.GetByAssessmentIdAsync(
            assessmentId,
            cancellationToken)
            ?? throw new NotFoundException(
                $"Assessment '{assessmentId}' has not been evaluated yet.");

        var lead = await leads.GetByAssessmentIdAsync(assessmentId, cancellationToken)
            ?? throw new ConflictException("Evaluation exists but its synthetic lead is missing.");

        var reasons = JsonSerializer.Deserialize<string[]>(recommendation.ReasonsJson) ?? [];

        return new RecommendationResultDto(
            recommendation.Id,
            recommendation.AssessmentId,
            recommendation.ProductCode,
            recommendation.DisplayName,
            recommendation.FitScore,
            recommendation.Status,
            recommendation.RequiresExpertReview,
            reasons,
            recommendation.RuleSetVersion,
            recommendation.CreatedAt,
            lead.Id);
    }
}
