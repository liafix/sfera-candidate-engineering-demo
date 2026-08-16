using SferaCandidate.Domain.Recommendations;

namespace SferaCandidate.Application.Abstractions.Persistence;

public interface IRecommendationRepository
{
    Task<Recommendation?> GetByAssessmentIdAsync(
        Guid assessmentId,
        CancellationToken cancellationToken = default);

    Task AddAsync(Recommendation recommendation, CancellationToken cancellationToken = default);
}
