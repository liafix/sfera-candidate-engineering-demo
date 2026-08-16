using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SferaCandidate.Domain.Assessments;

namespace SferaCandidate.Infrastructure.Persistence.Configurations;

internal sealed class AssessmentConfiguration : IEntityTypeConfiguration<Assessment>
{
    public void Configure(EntityTypeBuilder<Assessment> builder)
    {
        builder.ToTable("Assessments");
        builder.HasKey(assessment => assessment.Id);

        builder.Property(assessment => assessment.Status).IsRequired();
        builder.Property(assessment => assessment.ParticipantType);
        builder.Property(assessment => assessment.RuleSetVersion).HasMaxLength(64);
        builder.Property(assessment => assessment.CreatedAt).IsRequired();
        builder.Property(assessment => assessment.UpdatedAt).IsRequired();

        builder.HasIndex(assessment => assessment.Status);
        builder.HasIndex(assessment => assessment.CreatedAt);
    }
}
