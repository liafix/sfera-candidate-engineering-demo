using Microsoft.EntityFrameworkCore;
using SferaCandidate.Application.Abstractions.Persistence;
using SferaCandidate.Domain.Roi;

namespace SferaCandidate.Infrastructure.Persistence.Repositories;

internal sealed class RoiScenarioRepository(SferaCandidateDbContext dbContext)
    : IRoiScenarioRepository
{
    public Task<RoiScenario?> GetAsync(
        Guid assessmentId,
        RoiScenarioName scenarioName,
        CancellationToken cancellationToken = default) =>
        dbContext.RoiScenarios.SingleOrDefaultAsync(
            scenario => scenario.AssessmentId == assessmentId && scenario.ScenarioName == scenarioName,
            cancellationToken);

    public Task AddAsync(RoiScenario scenario, CancellationToken cancellationToken = default) =>
        dbContext.RoiScenarios.AddAsync(scenario, cancellationToken).AsTask();
}
