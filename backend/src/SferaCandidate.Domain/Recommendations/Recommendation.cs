using System.Text.Json;
using SferaCandidate.Domain.Common;

namespace SferaCandidate.Domain.Recommendations;

public sealed class Recommendation
{
    private Recommendation()
    {
        ProductCode = string.Empty;
        DisplayName = string.Empty;
        ReasonsJson = "[]";
        RuleSetVersion = string.Empty;
    }

    private Recommendation(
        Guid id,
        Guid assessmentId,
        RecommendationDecision decision,
        DateTimeOffset createdAt)
    {
        Id = id;
        AssessmentId = assessmentId;
        ProductCode = decision.ProductCode;
        DisplayName = decision.DisplayName;
        FitScore = decision.FitScore;
        RequiresExpertReview = decision.RequiresExpertReview;
        Status = decision.Status;
        ReasonsJson = JsonSerializer.Serialize(decision.Reasons);
        RuleSetVersion = decision.RuleSetVersion;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }

    public Guid AssessmentId { get; private set; }

    public string ProductCode { get; private set; }

    public string DisplayName { get; private set; }

    public int FitScore { get; private set; }

    public RecommendationStatus Status { get; private set; }

    public bool RequiresExpertReview { get; private set; }

    public string ReasonsJson { get; private set; }

    public string RuleSetVersion { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public static Recommendation Create(
        Guid id,
        Guid assessmentId,
        RecommendationDecision decision,
        DateTimeOffset createdAt)
    {
        if (id == Guid.Empty)
        {
            throw new DomainValidationException("Recommendation id must not be empty.");
        }

        if (assessmentId == Guid.Empty)
        {
            throw new DomainValidationException("Assessment id must not be empty.");
        }

        ArgumentNullException.ThrowIfNull(decision);

        if (decision.FitScore < 0 || decision.FitScore > 100)
        {
            throw new DomainValidationException("Fit score must be between 0 and 100.");
        }

        return new Recommendation(id, assessmentId, decision, createdAt);
    }
}
