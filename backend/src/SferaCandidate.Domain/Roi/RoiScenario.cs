using SferaCandidate.Domain.Common;

namespace SferaCandidate.Domain.Roi;

public sealed class RoiScenario
{
    private RoiScenario()
    {
    }

    private RoiScenario(
        Guid id,
        Guid assessmentId,
        RoiScenarioName scenarioName,
        RoiCalculationInput input,
        RoiCalculationResult result,
        DateTimeOffset createdAt)
    {
        Id = id;
        AssessmentId = assessmentId;
        ScenarioName = scenarioName;
        Apply(input, result);
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    public Guid Id { get; private set; }

    public Guid AssessmentId { get; private set; }

    public RoiScenarioName ScenarioName { get; private set; }

    public decimal CasesPerMonth { get; private set; }

    public decimal MinutesSavedPerCase { get; private set; }

    public decimal LoadedHourlyCost { get; private set; }

    public decimal AnnualOperatingCost { get; private set; }

    public decimal ImplementationCost { get; private set; }

    public decimal CasesPerYear { get; private set; }

    public decimal AnnualHoursSaved { get; private set; }

    public decimal AnnualTimeValue { get; private set; }

    public decimal AnnualNetBenefit { get; private set; }

    public decimal? SimplePaybackMonths { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public static RoiScenario Create(
        Guid id,
        Guid assessmentId,
        RoiScenarioName scenarioName,
        RoiCalculationInput input,
        RoiCalculationResult result,
        DateTimeOffset createdAt)
    {
        if (id == Guid.Empty)
        {
            throw new DomainValidationException("ROI scenario id must not be empty.");
        }

        if (assessmentId == Guid.Empty)
        {
            throw new DomainValidationException("Assessment id must not be empty.");
        }

        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(result);

        return new RoiScenario(
            id,
            assessmentId,
            scenarioName,
            input,
            result,
            createdAt);
    }

    public void Recalculate(
        RoiCalculationInput input,
        RoiCalculationResult result,
        DateTimeOffset updatedAt)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(result);

        Apply(input, result);
        UpdatedAt = updatedAt;
    }

    private void Apply(RoiCalculationInput input, RoiCalculationResult result)
    {
        CasesPerMonth = input.CasesPerMonth;
        MinutesSavedPerCase = input.MinutesSavedPerCase;
        LoadedHourlyCost = input.LoadedHourlyCost;
        AnnualOperatingCost = input.AnnualOperatingCost;
        ImplementationCost = input.ImplementationCost;
        CasesPerYear = result.CasesPerYear;
        AnnualHoursSaved = result.AnnualHoursSaved;
        AnnualTimeValue = result.AnnualTimeValue;
        AnnualNetBenefit = result.AnnualNetBenefit;
        SimplePaybackMonths = result.SimplePaybackMonths;
    }
}
