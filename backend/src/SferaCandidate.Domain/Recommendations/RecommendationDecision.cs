namespace SferaCandidate.Domain.Recommendations;

public sealed record RecommendationDecision(
    string ProductCode,
    string DisplayName,
    int FitScore,
    RecommendationStatus Status,
    bool RequiresExpertReview,
    IReadOnlyList<string> Reasons,
    string RuleSetVersion);
