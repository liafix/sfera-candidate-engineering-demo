using SferaCandidate.Application.Abstractions.Persistence;
using SferaCandidate.Domain.Auditing;

namespace SferaCandidate.Infrastructure.Persistence.Repositories;

internal sealed class AuditEventRepository(SferaCandidateDbContext dbContext)
    : IAuditEventRepository
{
    public Task AddAsync(AuditEvent auditEvent, CancellationToken cancellationToken = default) =>
        dbContext.AuditEvents.AddAsync(auditEvent, cancellationToken).AsTask();
}
