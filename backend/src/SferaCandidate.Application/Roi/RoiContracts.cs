using SferaCandidate.Domain.Roi;

namespace SferaCandidate.Application.Roi;

public sealed record CalculateRoiCommand(
    RoiScenarioName ScenarioName,
    decimal CasesPerMonth,
    decimal MinutesSavedPerCase,
    decimal LoadedHourlyCost,
    decimal AnnualOperatingCost,
    decimal ImplementationCost);

public sealed record RoiScenarioDto(
    Guid Id,
    Guid AssessmentId,
    RoiScenarioName ScenarioName,
    decimal CasesPerMonth,
    decimal MinutesSavedPerCase,
    decimal LoadedHourlyCost,
    decimal AnnualOperatingCost,
    decimal ImplementationCost,
    decimal CasesPerYear,
    decimal AnnualHoursSaved,
    decimal AnnualTimeValue,
    decimal AnnualNetBenefit,
    decimal? SimplePaybackMonths,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
