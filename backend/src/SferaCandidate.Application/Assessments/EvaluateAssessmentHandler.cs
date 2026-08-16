using System.Text.Json;
using SferaCandidate.Application.Abstractions.Persistence;
using SferaCandidate.Application.Abstractions.Time;
using SferaCandidate.Application.Common;
using SferaCandidate.Domain.Assessments;
using SferaCandidate.Domain.Auditing;
using SferaCandidate.Domain.Common;
using SferaCandidate.Domain.Leads;
using SferaCandidate.Domain.Recommendations;

namespace SferaCandidate.Application.Assessments;

public sealed class EvaluateAssessmentHandler(
    IAssessmentRepository assessments,
    IAssessmentAnswerRepository answers,
    IRecommendationRepository recommendations,
    ISyntheticLeadRepository leads,
    IAuditEventRepository auditEvents,
    IUnitOfWork unitOfWork,
    IClock clock,
    RecommendationEngine recommendationEngine)
{
    public async Task<RecommendationResultDto> HandleAsync(
        Guid assessmentId,
        CancellationToken cancellationToken = default)
    {
        var assessment = await assessments.GetByIdAsync(assessmentId, cancellationToken)
            ?? throw new NotFoundException($"Assessment '{assessmentId}' was not found.");

        var existingRecommendation = await recommendations.GetByAssessmentIdAsync(
            assessmentId,
            cancellationToken);

        if (existingRecommendation is not null)
        {
            var existingLead = await leads.GetByAssessmentIdAsync(assessmentId, cancellationToken)
                ?? throw new ConflictException(
                    "Evaluation exists but its synthetic lead is missing. The demo data is inconsistent.");

            return Map(existingRecommendation, existingLead.Id);
        }

        var storedAnswers = await answers.ListByAssessmentAsync(assessmentId, cancellationToken);
        var answerMap = storedAnswers.ToDictionary(
            answer => answer.QuestionKey,
            answer => answer.Value,
            StringComparer.Ordinal);

        var missing = QuestionKeys.RequiredForEvaluation
            .Where(key => !answerMap.ContainsKey(key))
            .OrderBy(key => key, StringComparer.Ordinal)
            .ToArray();

        if (missing.Length > 0)
        {
            throw new DomainValidationException(
                $"Assessment cannot be evaluated. Missing required answers: {string.Join(", ", missing)}.");
        }

        var input = new RecommendationInput(
            AnswerValueParser.ParseParticipantType(answerMap[QuestionKeys.ParticipantType]),
            AnswerValueParser.ParseNeedCategory(answerMap[QuestionKeys.PrimaryNeed]),
            AnswerValueParser.ParseBoolean(answerMap[QuestionKeys.ManagesWholesaleContracts]),
            AnswerValueParser.ParseBoolean(answerMap[QuestionKeys.NeedsTradingOrPlanningSupport]));

        var now = clock.UtcNow;
        assessment.MarkReadyForEvaluation(now);
        var decision = recommendationEngine.Evaluate(input);
        var recommendation = Recommendation.Create(
            Guid.NewGuid(),
            assessmentId,
            decision,
            now);

        assessment.MarkResultGenerated(decision.RuleSetVersion, now);
        await recommendations.AddAsync(recommendation, cancellationToken);

        var organizationName = answerMap.TryGetValue(QuestionKeys.OrganizationName, out var storedOrganization)
            ? storedOrganization
            : "Synthetic Candidate Organization";

        var leadStatus = decision.RequiresExpertReview
            ? SyntheticLeadStatus.ReviewRequired
            : SyntheticLeadStatus.Evaluated;

        var lead = SyntheticLead.Create(
            Guid.NewGuid(),
            assessmentId,
            organizationName,
            input.ParticipantType.ToString(),
            leadStatus,
            now);

        await leads.AddAsync(lead, cancellationToken);

        await auditEvents.AddAsync(
            AuditEvent.Create(
                Guid.NewGuid(),
                nameof(Assessment),
                assessmentId,
                AuditAction.AssessmentEvaluated,
                JsonSerializer.Serialize(new { ruleSetVersion = decision.RuleSetVersion }),
                now),
            cancellationToken);

        await auditEvents.AddAsync(
            AuditEvent.Create(
                Guid.NewGuid(),
                nameof(Recommendation),
                recommendation.Id,
                AuditAction.RecommendationGenerated,
                JsonSerializer.Serialize(new
                {
                    assessmentId,
                    recommendation.ProductCode,
                    recommendation.RuleSetVersion
                }),
                now),
            cancellationToken);

        await auditEvents.AddAsync(
            AuditEvent.Create(
                Guid.NewGuid(),
                nameof(SyntheticLead),
                lead.Id,
                AuditAction.LeadCreated,
                JsonSerializer.Serialize(new { assessmentId, synthetic = true }),
                now),
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(recommendation, lead.Id);
    }

    private static RecommendationResultDto Map(Recommendation recommendation, Guid leadId)
    {
        var reasons = JsonSerializer.Deserialize<string[]>(recommendation.ReasonsJson) ?? [];

        return new RecommendationResultDto(
            recommendation.Id,
            recommendation.AssessmentId,
            recommendation.ProductCode,
            recommendation.DisplayName,
            recommendation.FitScore,
            recommendation.Status,
            recommendation.RequiresExpertReview,
            reasons,
            recommendation.RuleSetVersion,
            recommendation.CreatedAt,
            leadId);
    }
}
