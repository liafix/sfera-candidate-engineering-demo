using Microsoft.EntityFrameworkCore;
using SferaCandidate.Application.Abstractions.Persistence;
using SferaCandidate.Domain.Recommendations;

namespace SferaCandidate.Infrastructure.Persistence.Repositories;

internal sealed class RecommendationRepository(SferaCandidateDbContext dbContext)
    : IRecommendationRepository
{
    public Task<Recommendation?> GetByAssessmentIdAsync(
        Guid assessmentId,
        CancellationToken cancellationToken = default) =>
        dbContext.Recommendations.SingleOrDefaultAsync(
            recommendation => recommendation.AssessmentId == assessmentId,
            cancellationToken);

    public Task AddAsync(Recommendation recommendation, CancellationToken cancellationToken = default) =>
        dbContext.Recommendations.AddAsync(recommendation, cancellationToken).AsTask();
}
