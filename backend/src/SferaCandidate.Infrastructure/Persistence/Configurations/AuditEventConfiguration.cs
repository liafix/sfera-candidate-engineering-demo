using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SferaCandidate.Domain.Auditing;

namespace SferaCandidate.Infrastructure.Persistence.Configurations;

internal sealed class AuditEventConfiguration : IEntityTypeConfiguration<AuditEvent>
{
    public void Configure(EntityTypeBuilder<AuditEvent> builder)
    {
        builder.ToTable("AuditEvents");
        builder.HasKey(auditEvent => auditEvent.Id);

        builder.Property(auditEvent => auditEvent.EntityType)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(auditEvent => auditEvent.Action).IsRequired();

        builder.Property(auditEvent => auditEvent.MetadataJson)
            .HasColumnType("TEXT")
            .IsRequired();

        builder.Property(auditEvent => auditEvent.CreatedAt).IsRequired();

        builder.HasIndex(auditEvent => new
        {
            auditEvent.EntityType,
            auditEvent.EntityId,
            auditEvent.CreatedAt
        });
    }
}
