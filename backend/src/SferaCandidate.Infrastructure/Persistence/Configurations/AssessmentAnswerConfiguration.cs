using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SferaCandidate.Domain.Assessments;

namespace SferaCandidate.Infrastructure.Persistence.Configurations;

internal sealed class AssessmentAnswerConfiguration : IEntityTypeConfiguration<AssessmentAnswer>
{
    public void Configure(EntityTypeBuilder<AssessmentAnswer> builder)
    {
        builder.ToTable("AssessmentAnswers");
        builder.HasKey(answer => answer.Id);

        builder.Property(answer => answer.QuestionKey)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(answer => answer.Value)
            .HasMaxLength(2048)
            .IsRequired();

        builder.Property(answer => answer.CreatedAt).IsRequired();
        builder.Property(answer => answer.UpdatedAt).IsRequired();

        builder.HasIndex(answer => new { answer.AssessmentId, answer.QuestionKey })
            .IsUnique();

        builder.HasOne<Assessment>()
            .WithMany()
            .HasForeignKey(answer => answer.AssessmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
