using SferaCandidate.Domain.Roi;

namespace SferaCandidate.Application.Abstractions.Persistence;

public interface IRoiScenarioRepository
{
    Task<RoiScenario?> GetAsync(
        Guid assessmentId,
        RoiScenarioName scenarioName,
        CancellationToken cancellationToken = default);

    Task AddAsync(RoiScenario scenario, CancellationToken cancellationToken = default);
}
