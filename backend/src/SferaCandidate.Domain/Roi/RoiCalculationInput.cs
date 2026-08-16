namespace SferaCandidate.Domain.Roi;

public sealed record RoiCalculationInput(
    decimal CasesPerMonth,
    decimal MinutesSavedPerCase,
    decimal LoadedHourlyCost,
    decimal AnnualOperatingCost,
    decimal ImplementationCost);
