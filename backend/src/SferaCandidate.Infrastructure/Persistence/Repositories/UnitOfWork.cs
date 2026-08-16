using SferaCandidate.Application.Abstractions.Persistence;

namespace SferaCandidate.Infrastructure.Persistence.Repositories;

internal sealed class UnitOfWork(SferaCandidateDbContext dbContext) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        dbContext.SaveChangesAsync(cancellationToken);
}
