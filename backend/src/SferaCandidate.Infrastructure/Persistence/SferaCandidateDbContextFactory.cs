using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SferaCandidate.Infrastructure.Persistence;

public sealed class SferaCandidateDbContextFactory : IDesignTimeDbContextFactory<SferaCandidateDbContext>
{
    public SferaCandidateDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SferaCandidateDbContext>();
        optionsBuilder.UseSqlite("Data Source=sfera-candidate.db");

        return new SferaCandidateDbContext(optionsBuilder.Options);
    }
}
