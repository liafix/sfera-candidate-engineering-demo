namespace SferaCandidate.Domain.Roi;

public sealed record RoiCalculationResult(
    decimal CasesPerYear,
    decimal AnnualHoursSaved,
    decimal AnnualTimeValue,
    decimal AnnualNetBenefit,
    decimal? SimplePaybackMonths)
{
    public bool PaybackReached => SimplePaybackMonths.HasValue;
}
