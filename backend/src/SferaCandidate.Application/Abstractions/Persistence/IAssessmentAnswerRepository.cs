using SferaCandidate.Domain.Assessments;

namespace SferaCandidate.Application.Abstractions.Persistence;

public interface IAssessmentAnswerRepository
{
    Task<AssessmentAnswer?> GetAsync(
        Guid assessmentId,
        string questionKey,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AssessmentAnswer>> ListByAssessmentAsync(
        Guid assessmentId,
        CancellationToken cancellationToken = default);

    Task AddAsync(AssessmentAnswer answer, CancellationToken cancellationToken = default);
}
