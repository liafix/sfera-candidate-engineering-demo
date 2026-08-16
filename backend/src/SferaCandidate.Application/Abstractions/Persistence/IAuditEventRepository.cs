using SferaCandidate.Domain.Auditing;

namespace SferaCandidate.Application.Abstractions.Persistence;

public interface IAuditEventRepository
{
    Task AddAsync(AuditEvent auditEvent, CancellationToken cancellationToken = default);
}
