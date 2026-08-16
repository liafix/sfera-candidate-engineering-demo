using SferaCandidate.Domain.Common;

namespace SferaCandidate.Domain.Assessments;

public sealed class AssessmentAnswer
{
    private AssessmentAnswer()
    {
        QuestionKey = string.Empty;
        Value = string.Empty;
    }

    private AssessmentAnswer(
        Guid id,
        Guid assessmentId,
        string questionKey,
        string value,
        DateTimeOffset createdAt)
    {
        Id = id;
        AssessmentId = assessmentId;
        QuestionKey = questionKey;
        Value = value;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    public Guid Id { get; private set; }

    public Guid AssessmentId { get; private set; }

    public string QuestionKey { get; private set; }

    public string Value { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public static AssessmentAnswer Create(
        Guid id,
        Guid assessmentId,
        string questionKey,
        string value,
        DateTimeOffset createdAt)
    {
        if (id == Guid.Empty)
        {
            throw new DomainValidationException("Assessment answer id must not be empty.");
        }

        if (assessmentId == Guid.Empty)
        {
            throw new DomainValidationException("Assessment id must not be empty.");
        }

        ValidateText(questionKey, value);

        return new AssessmentAnswer(
            id,
            assessmentId,
            questionKey.Trim(),
            value.Trim(),
            createdAt);
    }

    public void UpdateValue(string value, DateTimeOffset updatedAt)
    {
        ValidateText(QuestionKey, value);
        Value = value.Trim();
        UpdatedAt = updatedAt;
    }

    private static void ValidateText(string questionKey, string value)
    {
        if (string.IsNullOrWhiteSpace(questionKey))
        {
            throw new DomainValidationException("Question key is required.");
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainValidationException("Answer value is required.");
        }
    }
}
