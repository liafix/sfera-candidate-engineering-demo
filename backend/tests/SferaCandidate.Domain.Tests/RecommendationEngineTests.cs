using SferaCandidate.Domain.Assessments;
using SferaCandidate.Domain.Common;
using SferaCandidate.Domain.Recommendations;

namespace SferaCandidate.Domain.Tests;

public sealed class RecommendationEngineTests
{
    private readonly RecommendationEngine _engine = new();

    [Fact]
    public void Evaluate_TraderOrSupplierWithWholesaleContracts_ReturnsEtrmPathway()
    {
        var input = new RecommendationInput(
            ParticipantType.TraderOrSupplier,
            NeedCategory.Other,
            ManagesWholesaleContracts: true,
            NeedsTradingOrPlanningSupport: false);

        var result = _engine.Evaluate(input);

        Assert.Equal("XMTRADE_ETRM", result.ProductCode);
        Assert.Equal("XMtrade / ETRM", result.DisplayName);
        Assert.Equal(75, result.FitScore);
        Assert.Equal(RecommendationStatus.Suggested, result.Status);
        Assert.True(result.RequiresExpertReview);
        Assert.Equal(RecommendationEngine.CurrentRuleSetVersion, result.RuleSetVersion);
        Assert.Contains(
            result.Reasons,
            reason => reason.Contains("Wholesale contract management", StringComparison.Ordinal));
    }

    [Fact]
    public void Evaluate_UnsupportedCombination_RequiresExpertReview()
    {
        var input = new RecommendationInput(
            ParticipantType.IndustrialConsumer,
            NeedCategory.Other,
            ManagesWholesaleContracts: false,
            NeedsTradingOrPlanningSupport: false);

        var result = _engine.Evaluate(input);

        Assert.Equal("EXPERT_REVIEW", result.ProductCode);
        Assert.Equal(RecommendationStatus.ExpertReviewRequired, result.Status);
        Assert.True(result.RequiresExpertReview);
        Assert.Contains(
            result.Reasons,
            reason => reason.Contains("does not contain an approved deterministic product rule", StringComparison.Ordinal));
    }

    [Fact]
    public void Evaluate_SameInput_ReturnsSameDecisionData()
    {
        var input = new RecommendationInput(
            ParticipantType.TraderOrSupplier,
            NeedCategory.TradingAndSupply,
            ManagesWholesaleContracts: true,
            NeedsTradingOrPlanningSupport: true);

        var first = _engine.Evaluate(input);
        var second = _engine.Evaluate(input);

        Assert.Equal(first.ProductCode, second.ProductCode);
        Assert.Equal(first.DisplayName, second.DisplayName);
        Assert.Equal(first.FitScore, second.FitScore);
        Assert.Equal(first.Status, second.Status);
        Assert.Equal(first.RequiresExpertReview, second.RequiresExpertReview);
        Assert.Equal(first.RuleSetVersion, second.RuleSetVersion);
        Assert.Equal(first.Reasons, second.Reasons);
    }

    [Fact]
    public void Evaluate_MissingRequiredParticipantType_ThrowsValidationError()
    {
        var input = new RecommendationInput(
            ParticipantType.Unknown,
            NeedCategory.TradingAndSupply,
            ManagesWholesaleContracts: true,
            NeedsTradingOrPlanningSupport: true);

        var exception = Assert.Throws<DomainValidationException>(() => _engine.Evaluate(input));

        Assert.Contains("Participant type", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Evaluate_FitScore_NeverExceedsOneHundred()
    {
        var input = new RecommendationInput(
            ParticipantType.TraderOrSupplier,
            NeedCategory.TradingAndSupply,
            ManagesWholesaleContracts: true,
            NeedsTradingOrPlanningSupport: true);

        var result = _engine.Evaluate(input);

        Assert.InRange(result.FitScore, 0, 100);
        Assert.Equal(95, result.FitScore);
    }
}
