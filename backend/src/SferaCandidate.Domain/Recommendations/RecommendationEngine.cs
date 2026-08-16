using SferaCandidate.Domain.Assessments;
using SferaCandidate.Domain.Common;

namespace SferaCandidate.Domain.Recommendations;

public sealed class RecommendationEngine
{
    public const string CurrentRuleSetVersion = "candidate-demo-2026.08-v1";

    private const string EtrmProductCode = "XMTRADE_ETRM";
    private const string EtrmDisplayName = "XMtrade / ETRM";
    private const string ExpertReviewProductCode = "EXPERT_REVIEW";
    private const string ExpertReviewDisplayName = "Expert consultation required";

    public RecommendationDecision Evaluate(RecommendationInput input)
    {
        ArgumentNullException.ThrowIfNull(input);

        ValidateRequiredInput(input);

        var reasons = new List<string>();
        var fitScore = 0;

        if (input.ParticipantType == ParticipantType.TraderOrSupplier)
        {
            fitScore += 35;
            reasons.Add("Participant type was selected as trader or supplier.");
        }

        if (input.PrimaryNeed == NeedCategory.TradingAndSupply)
        {
            fitScore += 10;
            reasons.Add("The primary need was selected as trading and supply.");
        }

        if (input.ManagesWholesaleContracts)
        {
            fitScore += 40;
            reasons.Add("Wholesale contract management was selected.");
        }

        if (input.NeedsTradingOrPlanningSupport)
        {
            fitScore += 10;
            reasons.Add("Trading or planning support was selected.");
        }

        fitScore = Math.Min(fitScore, 95);

        var supportsEtrmPath =
            input.ParticipantType == ParticipantType.TraderOrSupplier &&
            (input.ManagesWholesaleContracts || input.NeedsTradingOrPlanningSupport);

        if (supportsEtrmPath)
        {
            reasons.Add(
                "The selected inputs satisfy this candidate demo's deterministic ETRM pathway rules.");

            return new RecommendationDecision(
                EtrmProductCode,
                EtrmDisplayName,
                fitScore,
                RecommendationStatus.Suggested,
                RequiresExpertReview: true,
                reasons.AsReadOnly(),
                CurrentRuleSetVersion);
        }

        reasons.Add(
            "This candidate demo does not contain an approved deterministic product rule for the selected combination.");

        return new RecommendationDecision(
            ExpertReviewProductCode,
            ExpertReviewDisplayName,
            fitScore,
            RecommendationStatus.ExpertReviewRequired,
            RequiresExpertReview: true,
            reasons.AsReadOnly(),
            CurrentRuleSetVersion);
    }

    private static void ValidateRequiredInput(RecommendationInput input)
    {
        if (input.ParticipantType == ParticipantType.Unknown)
        {
            throw new DomainValidationException("Participant type is required for evaluation.");
        }

        if (input.PrimaryNeed == NeedCategory.Unknown)
        {
            throw new DomainValidationException("Primary need is required for evaluation.");
        }
    }
}
