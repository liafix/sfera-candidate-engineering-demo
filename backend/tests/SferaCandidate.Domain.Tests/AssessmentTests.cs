using SferaCandidate.Domain.Assessments;
using SferaCandidate.Domain.Common;
using SferaCandidate.Domain.Recommendations;

namespace SferaCandidate.Domain.Tests;

public sealed class AssessmentTests
{
    [Fact]
    public void Assessment_HappyPath_TransitionsToResultGeneratedWithRulesetVersion()
    {
        var createdAt = new DateTimeOffset(2026, 8, 16, 8, 0, 0, TimeSpan.Zero);
        var assessment = Assessment.Create(Guid.NewGuid(), createdAt);

        assessment.SetParticipantType(
            ParticipantType.TraderOrSupplier,
            createdAt.AddMinutes(1));

        assessment.MarkReadyForEvaluation(createdAt.AddMinutes(2));
        assessment.MarkResultGenerated(
            RecommendationEngine.CurrentRuleSetVersion,
            createdAt.AddMinutes(3));

        Assert.Equal(AssessmentStatus.ResultGenerated, assessment.Status);
        Assert.Equal(ParticipantType.TraderOrSupplier, assessment.ParticipantType);
        Assert.Equal(RecommendationEngine.CurrentRuleSetVersion, assessment.RuleSetVersion);
    }

    [Fact]
    public void Assessment_CannotBecomeReadyWithoutParticipantType()
    {
        var assessment = Assessment.Create(Guid.NewGuid(), DateTimeOffset.UtcNow);

        var exception = Assert.Throws<DomainValidationException>(() =>
            assessment.MarkReadyForEvaluation(DateTimeOffset.UtcNow));

        Assert.Contains("participant type", exception.Message, StringComparison.OrdinalIgnoreCase);
    }
}
