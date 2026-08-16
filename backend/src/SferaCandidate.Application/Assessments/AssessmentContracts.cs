using SferaCandidate.Domain.Assessments;
using SferaCandidate.Domain.Recommendations;

namespace SferaCandidate.Application.Assessments;

public sealed record AssessmentDto(
    Guid Id,
    AssessmentStatus Status,
    ParticipantType? ParticipantType,
    string? RuleSetVersion,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    IReadOnlyDictionary<string, string> Answers);

public sealed record SaveAnswerResult(
    Guid AssessmentId,
    string QuestionKey,
    string Value,
    AssessmentStatus AssessmentStatus,
    DateTimeOffset UpdatedAt);

public sealed record RecommendationResultDto(
    Guid RecommendationId,
    Guid AssessmentId,
    string ProductCode,
    string DisplayName,
    int FitScore,
    RecommendationStatus Status,
    bool RequiresExpertReview,
    IReadOnlyList<string> Reasons,
    string RuleSetVersion,
    DateTimeOffset CreatedAt,
    Guid SyntheticLeadId);
