using System.Text.Json;
using SferaCandidate.Application.Abstractions.Persistence;
using SferaCandidate.Application.Abstractions.Time;
using SferaCandidate.Application.Common;
using SferaCandidate.Domain.Assessments;
using SferaCandidate.Domain.Auditing;

namespace SferaCandidate.Application.Assessments;

public sealed class SaveAnswerHandler(
    IAssessmentRepository assessments,
    IAssessmentAnswerRepository answers,
    IAuditEventRepository auditEvents,
    IUnitOfWork unitOfWork,
    IClock clock)
{
    public async Task<SaveAnswerResult> HandleAsync(
        Guid assessmentId,
        string questionKey,
        string value,
        CancellationToken cancellationToken = default)
    {
        var assessment = await assessments.GetByIdAsync(assessmentId, cancellationToken)
            ?? throw new NotFoundException($"Assessment '{assessmentId}' was not found.");

        if (assessment.Status == AssessmentStatus.ResultGenerated)
        {
            throw new ConflictException(
                "Answers are immutable after evaluation. Create a new assessment to test different inputs.");
        }

        AnswerValueParser.Validate(questionKey, value);
        var now = clock.UtcNow;

        var answer = await answers.GetAsync(assessmentId, questionKey, cancellationToken);
        if (answer is null)
        {
            answer = AssessmentAnswer.Create(
                Guid.NewGuid(),
                assessmentId,
                questionKey,
                value,
                now);

            await answers.AddAsync(answer, cancellationToken);
        }
        else
        {
            answer.UpdateValue(value, now);
        }

        if (questionKey == QuestionKeys.ParticipantType)
        {
            assessment.SetParticipantType(
                AnswerValueParser.ParseParticipantType(value),
                now);
        }
        else
        {
            assessment.MarkInProgress(now);
        }

        await auditEvents.AddAsync(
            AuditEvent.Create(
                Guid.NewGuid(),
                nameof(Assessment),
                assessmentId,
                AuditAction.AnswerSaved,
                JsonSerializer.Serialize(new { questionKey }),
                now),
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new SaveAnswerResult(
            assessmentId,
            questionKey,
            answer.Value,
            assessment.Status,
            answer.UpdatedAt);
    }
}
