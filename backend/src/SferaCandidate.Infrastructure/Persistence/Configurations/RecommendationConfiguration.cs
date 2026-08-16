using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SferaCandidate.Domain.Assessments;
using SferaCandidate.Domain.Recommendations;

namespace SferaCandidate.Infrastructure.Persistence.Configurations;

internal sealed class RecommendationConfiguration : IEntityTypeConfiguration<Recommendation>
{
    public void Configure(EntityTypeBuilder<Recommendation> builder)
    {
        builder.ToTable("Recommendations");
        builder.HasKey(recommendation => recommendation.Id);

        builder.Property(recommendation => recommendation.ProductCode)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(recommendation => recommendation.DisplayName)
            .HasMaxLength(160)
            .IsRequired();

        builder.Property(recommendation => recommendation.FitScore).IsRequired();
        builder.Property(recommendation => recommendation.Status).IsRequired();
        builder.Property(recommendation => recommendation.RequiresExpertReview).IsRequired();

        builder.Property(recommendation => recommendation.ReasonsJson)
            .HasColumnType("TEXT")
            .IsRequired();

        builder.Property(recommendation => recommendation.RuleSetVersion)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(recommendation => recommendation.CreatedAt).IsRequired();

        builder.HasIndex(recommendation => recommendation.AssessmentId)
            .IsUnique();

        builder.HasOne<Assessment>()
            .WithOne()
            .HasForeignKey<Recommendation>(recommendation => recommendation.AssessmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
