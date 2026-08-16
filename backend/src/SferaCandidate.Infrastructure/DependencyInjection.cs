using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SferaCandidate.Application.Abstractions.Persistence;
using SferaCandidate.Application.Abstractions.Time;
using SferaCandidate.Infrastructure.Health;
using SferaCandidate.Infrastructure.Persistence;
using SferaCandidate.Infrastructure.Persistence.Repositories;
using SferaCandidate.Infrastructure.Time;

namespace SferaCandidate.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("CandidateDatabase")
            ?? throw new InvalidOperationException(
                "Connection string 'CandidateDatabase' is not configured.");

        services.AddDbContext<SferaCandidateDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped<IAssessmentRepository, AssessmentRepository>();
        services.AddScoped<IAssessmentAnswerRepository, AssessmentAnswerRepository>();
        services.AddScoped<IRecommendationRepository, RecommendationRepository>();
        services.AddScoped<IRoiScenarioRepository, RoiScenarioRepository>();
        services.AddScoped<ISyntheticLeadRepository, SyntheticLeadRepository>();
        services.AddScoped<IAuditEventRepository, AuditEventRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddSingleton<IClock, SystemClock>();

        services
            .AddHealthChecks()
            .AddCheck<SqliteDatabaseHealthCheck>(
                "sqlite",
                tags: ["ready"]);

        return services;
    }
}
