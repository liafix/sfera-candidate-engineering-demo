using Microsoft.Extensions.DependencyInjection;
using SferaCandidate.Application.Assessments;
using SferaCandidate.Application.Roi;
using SferaCandidate.Domain.Recommendations;
using SferaCandidate.Domain.Roi;

namespace SferaCandidate.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<RecommendationEngine>();
        services.AddSingleton<RoiCalculator>();

        services.AddScoped<CreateAssessmentHandler>();
        services.AddScoped<GetAssessmentHandler>();
        services.AddScoped<SaveAnswerHandler>();
        services.AddScoped<EvaluateAssessmentHandler>();
        services.AddScoped<GetAssessmentResultHandler>();
        services.AddScoped<CalculateRoiHandler>();

        return services;
    }
}
