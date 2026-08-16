using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SferaCandidate.Infrastructure.Persistence;

namespace SferaCandidate.Api.Tests.Infrastructure;

public sealed class CandidateApiFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection;
    private bool _connectionDisposed;

    public CandidateApiFactory()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Cors:AllowedOrigins:0"] = "http://localhost:3000"
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<SferaCandidateDbContext>();
            services.RemoveAll<DbContextOptions<SferaCandidateDbContext>>();

            // One open low-level SQLite connection keeps this in-memory database
            // alive for the complete lifetime of the WebApplicationFactory fixture.
            // Each fixture gets its own isolated database and no temp files need cleanup.
            services.AddDbContext<SferaCandidateDbContext>(options =>
                options.UseSqlite(_connection));
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            DisposeConnection();
        }
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        DisposeConnection();
        GC.SuppressFinalize(this);
    }

    private void DisposeConnection()
    {
        if (_connectionDisposed)
        {
            return;
        }

        _connection.Dispose();
        _connectionDisposed = true;
    }
}
