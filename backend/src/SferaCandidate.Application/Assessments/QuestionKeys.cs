namespace SferaCandidate.Application.Assessments;

public static class QuestionKeys
{
    public const string OrganizationName = "organizationName";
    public const string ParticipantType = "participantType";
    public const string PrimaryNeed = "primaryNeed";
    public const string ManagesWholesaleContracts = "managesWholesaleContracts";
    public const string NeedsTradingOrPlanningSupport = "needsTradingOrPlanningSupport";

    public static readonly IReadOnlySet<string> Supported = new HashSet<string>(StringComparer.Ordinal)
    {
        OrganizationName,
        ParticipantType,
        PrimaryNeed,
        ManagesWholesaleContracts,
        NeedsTradingOrPlanningSupport
    };

    public static readonly IReadOnlySet<string> RequiredForEvaluation = new HashSet<string>(StringComparer.Ordinal)
    {
        ParticipantType,
        PrimaryNeed,
        ManagesWholesaleContracts,
        NeedsTradingOrPlanningSupport
    };
}
