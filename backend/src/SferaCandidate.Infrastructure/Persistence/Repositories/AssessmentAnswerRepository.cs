using Microsoft.EntityFrameworkCore;
using SferaCandidate.Application.Abstractions.Persistence;
using SferaCandidate.Domain.Assessments;

namespace SferaCandidate.Infrastructure.Persistence.Repositories;

internal sealed class AssessmentAnswerRepository(SferaCandidateDbContext dbContext)
    : IAssessmentAnswerRepository
{
    public Task<AssessmentAnswer?> GetAsync(
        Guid assessmentId,
        string questionKey,
        CancellationToken cancellationToken = default) =>
        dbContext.AssessmentAnswers.SingleOrDefaultAsync(
            answer => answer.AssessmentId == assessmentId && answer.QuestionKey == questionKey,
            cancellationToken);

    public async Task<IReadOnlyList<AssessmentAnswer>> ListByAssessmentAsync(
        Guid assessmentId,
        CancellationToken cancellationToken = default) =>
        await dbContext.AssessmentAnswers
            .Where(answer => answer.AssessmentId == assessmentId)
            .OrderBy(answer => answer.QuestionKey)
            .ToListAsync(cancellationToken);

    public Task AddAsync(AssessmentAnswer answer, CancellationToken cancellationToken = default) =>
        dbContext.AssessmentAnswers.AddAsync(answer, cancellationToken).AsTask();
}
