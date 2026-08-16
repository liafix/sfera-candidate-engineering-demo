using SferaCandidate.Domain.Common;
using SferaCandidate.Domain.Roi;

namespace SferaCandidate.Domain.Tests;

public sealed class RoiCalculatorTests
{
    private readonly RoiCalculator _calculator = new();

    [Fact]
    public void Calculate_KnownScenario_ReturnsExpectedTransparentMath()
    {
        var input = new RoiCalculationInput(
            CasesPerMonth: 100m,
            MinutesSavedPerCase: 30m,
            LoadedHourlyCost: 40m,
            AnnualOperatingCost: 6_000m,
            ImplementationCost: 12_000m);

        var result = _calculator.Calculate(input);

        Assert.Equal(1_200m, result.CasesPerYear);
        Assert.Equal(600m, result.AnnualHoursSaved);
        Assert.Equal(24_000m, result.AnnualTimeValue);
        Assert.Equal(18_000m, result.AnnualNetBenefit);
        Assert.Equal(8m, result.SimplePaybackMonths);
        Assert.True(result.PaybackReached);
    }

    [Fact]
    public void Calculate_NonPositiveAnnualNetBenefit_DoesNotClaimPayback()
    {
        var input = new RoiCalculationInput(
            CasesPerMonth: 10m,
            MinutesSavedPerCase: 10m,
            LoadedHourlyCost: 40m,
            AnnualOperatingCost: 1_000m,
            ImplementationCost: 5_000m);

        var result = _calculator.Calculate(input);

        Assert.Equal(-200m, result.AnnualNetBenefit);
        Assert.Null(result.SimplePaybackMonths);
        Assert.False(result.PaybackReached);
    }

    [Fact]
    public void Calculate_ZeroImplementationCostAndPositiveBenefit_ReturnsZeroMonthPayback()
    {
        var input = new RoiCalculationInput(
            CasesPerMonth: 10m,
            MinutesSavedPerCase: 60m,
            LoadedHourlyCost: 50m,
            AnnualOperatingCost: 0m,
            ImplementationCost: 0m);

        var result = _calculator.Calculate(input);

        Assert.Equal(0m, result.SimplePaybackMonths);
    }

    [Fact]
    public void Calculate_NegativeInput_ThrowsValidationError()
    {
        var input = new RoiCalculationInput(
            CasesPerMonth: -1m,
            MinutesSavedPerCase: 30m,
            LoadedHourlyCost: 40m,
            AnnualOperatingCost: 0m,
            ImplementationCost: 0m);

        var exception = Assert.Throws<DomainValidationException>(() => _calculator.Calculate(input));

        Assert.Contains("cannot be negative", exception.Message, StringComparison.Ordinal);
    }
}
