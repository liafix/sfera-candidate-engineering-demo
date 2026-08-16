using SferaCandidate.Application.Abstractions.Persistence;
using SferaCandidate.Application.Common;

namespace SferaCandidate.Application.Assessments;

public sealed class GetAssessmentHandler(
    IAssessmentRepository assessments,
    IAssessmentAnswerRepository answers)
{
    public async Task<AssessmentDto> HandleAsync(
        Guid assessmentId,
        CancellationToken cancellationToken = default)
    {
        var assessment = await assessments.GetByIdAsync(assessmentId, cancellationToken)
            ?? throw new NotFoundException($"Assessment '{assessmentId}' was not found.");

        var storedAnswers = await answers.ListByAssessmentAsync(assessmentId, cancellationToken);
        var answerMap = storedAnswers.ToDictionary(
            answer => answer.QuestionKey,
            answer => answer.Value,
            StringComparer.Ordinal);

        return new AssessmentDto(
            assessment.Id,
            assessment.Status,
            assessment.ParticipantType,
            assessment.RuleSetVersion,
            assessment.CreatedAt,
            assessment.UpdatedAt,
            answerMap);
    }
}
