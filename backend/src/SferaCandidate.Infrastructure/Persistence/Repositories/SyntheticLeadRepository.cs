using Microsoft.EntityFrameworkCore;
using SferaCandidate.Application.Abstractions.Persistence;
using SferaCandidate.Domain.Leads;

namespace SferaCandidate.Infrastructure.Persistence.Repositories;

internal sealed class SyntheticLeadRepository(SferaCandidateDbContext dbContext)
    : ISyntheticLeadRepository
{
    public Task<SyntheticLead?> GetByAssessmentIdAsync(
        Guid assessmentId,
        CancellationToken cancellationToken = default) =>
        dbContext.SyntheticLeads.SingleOrDefaultAsync(
            lead => lead.AssessmentId == assessmentId,
            cancellationToken);

    public Task AddAsync(SyntheticLead lead, CancellationToken cancellationToken = default) =>
        dbContext.SyntheticLeads.AddAsync(lead, cancellationToken).AsTask();
}
