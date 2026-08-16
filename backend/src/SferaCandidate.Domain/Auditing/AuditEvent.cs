using SferaCandidate.Domain.Common;

namespace SferaCandidate.Domain.Auditing;

public sealed class AuditEvent
{
    private AuditEvent()
    {
        EntityType = string.Empty;
        MetadataJson = "{}";
    }

    private AuditEvent(
        Guid id,
        string entityType,
        Guid entityId,
        AuditAction action,
        string metadataJson,
        DateTimeOffset createdAt)
    {
        Id = id;
        EntityType = entityType;
        EntityId = entityId;
        Action = action;
        MetadataJson = metadataJson;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }

    public string EntityType { get; private set; }

    public Guid EntityId { get; private set; }

    public AuditAction Action { get; private set; }

    public string MetadataJson { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public static AuditEvent Create(
        Guid id,
        string entityType,
        Guid entityId,
        AuditAction action,
        string metadataJson,
        DateTimeOffset createdAt)
    {
        if (id == Guid.Empty || entityId == Guid.Empty)
        {
            throw new DomainValidationException("Audit event and entity ids must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(entityType))
        {
            throw new DomainValidationException("Audit entity type is required.");
        }

        if (string.IsNullOrWhiteSpace(metadataJson))
        {
            throw new DomainValidationException("Audit metadata JSON is required.");
        }

        return new AuditEvent(
            id,
            entityType.Trim(),
            entityId,
            action,
            metadataJson,
            createdAt);
    }
}
