using SferaCandidate.Domain.Assessments;

namespace SferaCandidate.Domain.Recommendations;

public sealed record RecommendationInput(
    ParticipantType ParticipantType,
    NeedCategory PrimaryNeed,
    bool ManagesWholesaleContracts,
    bool NeedsTradingOrPlanningSupport);
