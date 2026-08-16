using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SferaCandidate.Domain.Assessments;
using SferaCandidate.Domain.Roi;

namespace SferaCandidate.Infrastructure.Persistence.Configurations;

internal sealed class RoiScenarioConfiguration : IEntityTypeConfiguration<RoiScenario>
{
    public void Configure(EntityTypeBuilder<RoiScenario> builder)
    {
        builder.ToTable("RoiScenarios");
        builder.HasKey(scenario => scenario.Id);

        builder.Property(scenario => scenario.ScenarioName).IsRequired();
        builder.Property(scenario => scenario.CasesPerMonth).HasPrecision(18, 2);
        builder.Property(scenario => scenario.MinutesSavedPerCase).HasPrecision(18, 2);
        builder.Property(scenario => scenario.LoadedHourlyCost).HasPrecision(18, 2);
        builder.Property(scenario => scenario.AnnualOperatingCost).HasPrecision(18, 2);
        builder.Property(scenario => scenario.ImplementationCost).HasPrecision(18, 2);
        builder.Property(scenario => scenario.CasesPerYear).HasPrecision(18, 2);
        builder.Property(scenario => scenario.AnnualHoursSaved).HasPrecision(18, 2);
        builder.Property(scenario => scenario.AnnualTimeValue).HasPrecision(18, 2);
        builder.Property(scenario => scenario.AnnualNetBenefit).HasPrecision(18, 2);
        builder.Property(scenario => scenario.SimplePaybackMonths).HasPrecision(18, 2);
        builder.Property(scenario => scenario.CreatedAt).IsRequired();
        builder.Property(scenario => scenario.UpdatedAt).IsRequired();

        builder.HasIndex(scenario => new { scenario.AssessmentId, scenario.ScenarioName })
            .IsUnique();

        builder.HasOne<Assessment>()
            .WithMany()
            .HasForeignKey(scenario => scenario.AssessmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
