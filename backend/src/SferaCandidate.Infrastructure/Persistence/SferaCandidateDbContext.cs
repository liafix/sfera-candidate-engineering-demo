using Microsoft.EntityFrameworkCore;
using SferaCandidate.Domain.Assessments;
using SferaCandidate.Domain.Auditing;
using SferaCandidate.Domain.Leads;
using SferaCandidate.Domain.Recommendations;
using SferaCandidate.Domain.Roi;

namespace SferaCandidate.Infrastructure.Persistence;

public sealed class SferaCandidateDbContext(DbContextOptions<SferaCandidateDbContext> options)
    : DbContext(options)
{
    public DbSet<Assessment> Assessments => Set<Assessment>();

    public DbSet<AssessmentAnswer> AssessmentAnswers => Set<AssessmentAnswer>();

    public DbSet<Recommendation> Recommendations => Set<Recommendation>();

    public DbSet<RoiScenario> RoiScenarios => Set<RoiScenario>();

    public DbSet<SyntheticLead> SyntheticLeads => Set<SyntheticLead>();

    public DbSet<AuditEvent> AuditEvents => Set<AuditEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SferaCandidateDbContext).Assembly);
    }
}
