using SferaCandidate.Domain.Common;

namespace SferaCandidate.Domain.Roi;

public sealed class RoiCalculator
{
    public RoiCalculationResult Calculate(RoiCalculationInput input)
    {
        ArgumentNullException.ThrowIfNull(input);
        Validate(input);

        var casesPerYear = input.CasesPerMonth * 12m;
        var annualHoursSaved = casesPerYear * input.MinutesSavedPerCase / 60m;
        var annualTimeValue = annualHoursSaved * input.LoadedHourlyCost;
        var annualNetBenefit = annualTimeValue - input.AnnualOperatingCost;

        decimal? paybackMonths = null;

        if (annualNetBenefit > 0m)
        {
            paybackMonths = input.ImplementationCost == 0m
                ? 0m
                : input.ImplementationCost / annualNetBenefit * 12m;
        }

        return new RoiCalculationResult(
            Round(casesPerYear),
            Round(annualHoursSaved),
            Round(annualTimeValue),
            Round(annualNetBenefit),
            paybackMonths.HasValue ? Round(paybackMonths.Value) : null);
    }

    private static void Validate(RoiCalculationInput input)
    {
        if (input.CasesPerMonth < 0m)
        {
            throw new DomainValidationException("Cases per month cannot be negative.");
        }

        if (input.MinutesSavedPerCase < 0m)
        {
            throw new DomainValidationException("Minutes saved per case cannot be negative.");
        }

        if (input.LoadedHourlyCost < 0m)
        {
            throw new DomainValidationException("Loaded hourly cost cannot be negative.");
        }

        if (input.AnnualOperatingCost < 0m)
        {
            throw new DomainValidationException("Annual operating cost cannot be negative.");
        }

        if (input.ImplementationCost < 0m)
        {
            throw new DomainValidationException("Implementation cost cannot be negative.");
        }
    }

    private static decimal Round(decimal value) =>
        decimal.Round(value, 2, MidpointRounding.AwayFromZero);
}
