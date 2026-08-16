using Microsoft.EntityFrameworkCore;
using SferaCandidate.Application.Abstractions.Persistence;
using SferaCandidate.Domain.Assessments;

namespace SferaCandidate.Infrastructure.Persistence.Repositories;

internal sealed class AssessmentRepository(SferaCandidateDbContext dbContext)
    : IAssessmentRepository
{
    public Task<Assessment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Assessments.SingleOrDefaultAsync(
            assessment => assessment.Id == id,
            cancellationToken);

    public Task AddAsync(Assessment assessment, CancellationToken cancellationToken = default) =>
        dbContext.Assessments.AddAsync(assessment, cancellationToken).AsTask();
}
