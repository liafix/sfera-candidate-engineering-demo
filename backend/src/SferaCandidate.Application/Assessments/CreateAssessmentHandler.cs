using System.Text.Json;
using SferaCandidate.Application.Abstractions.Persistence;
using SferaCandidate.Application.Abstractions.Time;
using SferaCandidate.Domain.Assessments;
using SferaCandidate.Domain.Auditing;

namespace SferaCandidate.Application.Assessments;

public sealed class CreateAssessmentHandler(
    IAssessmentRepository assessments,
    IAuditEventRepository auditEvents,
    IUnitOfWork unitOfWork,
    IClock clock)
{
    public async Task<AssessmentDto> HandleAsync(CancellationToken cancellationToken = default)
    {
        var now = clock.UtcNow;
        var assessment = Assessment.Create(Guid.NewGuid(), now);

        await assessments.AddAsync(assessment, cancellationToken);
        await auditEvents.AddAsync(
            AuditEvent.Create(
                Guid.NewGuid(),
                nameof(Assessment),
                assessment.Id,
                AuditAction.AssessmentCreated,
                JsonSerializer.Serialize(new { source = "candidate-demo-api" }),
                now),
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AssessmentDto(
            assessment.Id,
            assessment.Status,
            assessment.ParticipantType,
            assessment.RuleSetVersion,
            assessment.CreatedAt,
            assessment.UpdatedAt,
            new Dictionary<string, string>());
    }
}
