using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SferaCandidate.Domain.Assessments;
using SferaCandidate.Domain.Leads;

namespace SferaCandidate.Infrastructure.Persistence.Configurations;

internal sealed class SyntheticLeadConfiguration : IEntityTypeConfiguration<SyntheticLead>
{
    public void Configure(EntityTypeBuilder<SyntheticLead> builder)
    {
        builder.ToTable("SyntheticLeads");
        builder.HasKey(lead => lead.Id);

        builder.Property(lead => lead.OrganizationName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(lead => lead.Segment)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(lead => lead.Status).IsRequired();
        builder.Property(lead => lead.CreatedAt).IsRequired();

        builder.HasIndex(lead => lead.AssessmentId)
            .IsUnique();

        builder.HasOne<Assessment>()
            .WithOne()
            .HasForeignKey<SyntheticLead>(lead => lead.AssessmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
