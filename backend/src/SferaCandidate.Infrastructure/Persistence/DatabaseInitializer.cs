using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace SferaCandidate.Infrastructure.Persistence;

public static class DatabaseInitializer
{
    public static async Task InitializeCandidateDatabaseAsync(
        this IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SferaCandidateDbContext>();
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);
    }
}
