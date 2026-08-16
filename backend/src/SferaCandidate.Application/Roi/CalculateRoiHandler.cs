using System.Text.Json;
using SferaCandidate.Application.Abstractions.Persistence;
using SferaCandidate.Application.Abstractions.Time;
using SferaCandidate.Application.Common;
using SferaCandidate.Domain.Assessments;
using SferaCandidate.Domain.Auditing;
using SferaCandidate.Domain.Roi;

namespace SferaCandidate.Application.Roi;

public sealed class CalculateRoiHandler(
    IAssessmentRepository assessments,
    IRoiScenarioRepository roiScenarios,
    IAuditEventRepository auditEvents,
    IUnitOfWork unitOfWork,
    IClock clock,
    RoiCalculator calculator)
{
    public async Task<RoiScenarioDto> HandleAsync(
        Guid assessmentId,
        CalculateRoiCommand command,
        CancellationToken cancellationToken = default)
    {
        var assessment = await assessments.GetByIdAsync(assessmentId, cancellationToken)
            ?? throw new NotFoundException($"Assessment '{assessmentId}' was not found.");

        if (assessment.Status != AssessmentStatus.ResultGenerated)
        {
            throw new ConflictException(
                "ROI can be calculated only after the assessment has been evaluated.");
        }

        if (!Enum.IsDefined(command.ScenarioName))
        {
            throw new SferaCandidate.Domain.Common.DomainValidationException(
                "ROI scenario name is not supported.");
        }

        var input = new RoiCalculationInput(
            command.CasesPerMonth,
            command.MinutesSavedPerCase,
            command.LoadedHourlyCost,
            command.AnnualOperatingCost,
            command.ImplementationCost);

        var result = calculator.Calculate(input);
        var now = clock.UtcNow;

        var scenario = await roiScenarios.GetAsync(
            assessmentId,
            command.ScenarioName,
            cancellationToken);

        if (scenario is null)
        {
            scenario = RoiScenario.Create(
                Guid.NewGuid(),
                assessmentId,
                command.ScenarioName,
                input,
                result,
                now);

            await roiScenarios.AddAsync(scenario, cancellationToken);
        }
        else
        {
            scenario.Recalculate(input, result, now);
        }

        await auditEvents.AddAsync(
            AuditEvent.Create(
                Guid.NewGuid(),
                nameof(RoiScenario),
                scenario.Id,
                AuditAction.RoiCalculated,
                JsonSerializer.Serialize(new
                {
                    assessmentId,
                    scenario = command.ScenarioName.ToString(),
                    illustrative = true
                }),
                now),
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Map(scenario);
    }

    private static RoiScenarioDto Map(RoiScenario scenario) => new(
        scenario.Id,
        scenario.AssessmentId,
        scenario.ScenarioName,
        scenario.CasesPerMonth,
        scenario.MinutesSavedPerCase,
        scenario.LoadedHourlyCost,
        scenario.AnnualOperatingCost,
        scenario.ImplementationCost,
        scenario.CasesPerYear,
        scenario.AnnualHoursSaved,
        scenario.AnnualTimeValue,
        scenario.AnnualNetBenefit,
        scenario.SimplePaybackMonths,
        scenario.CreatedAt,
        scenario.UpdatedAt);
}
