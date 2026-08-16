using SferaCandidate.Domain.Assessments;

namespace SferaCandidate.Application.Abstractions.Persistence;

public interface IAssessmentRepository
{
    Task<Assessment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(Assessment assessment, CancellationToken cancellationToken = default);
}
