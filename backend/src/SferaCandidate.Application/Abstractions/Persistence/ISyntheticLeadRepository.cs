using SferaCandidate.Domain.Leads;

namespace SferaCandidate.Application.Abstractions.Persistence;

public interface ISyntheticLeadRepository
{
    Task<SyntheticLead?> GetByAssessmentIdAsync(
        Guid assessmentId,
        CancellationToken cancellationToken = default);

    Task AddAsync(SyntheticLead lead, CancellationToken cancellationToken = default);
}
