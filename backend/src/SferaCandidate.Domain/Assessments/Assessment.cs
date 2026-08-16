using SferaCandidate.Domain.Common;

namespace SferaCandidate.Domain.Assessments;

public sealed class Assessment
{
    private Assessment()
    {
    }

    private Assessment(Guid id, DateTimeOffset createdAt)
    {
        Id = id;
        Status = AssessmentStatus.Draft;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    public Guid Id { get; private set; }

    public AssessmentStatus Status { get; private set; }

    public ParticipantType? ParticipantType { get; private set; }

    public string? RuleSetVersion { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public static Assessment Create(Guid id, DateTimeOffset createdAt)
    {
        if (id == Guid.Empty)
        {
            throw new DomainValidationException("Assessment id must not be empty.");
        }

        return new Assessment(id, createdAt);
    }

    public void SetParticipantType(ParticipantType participantType, DateTimeOffset updatedAt)
    {
        if (participantType == SferaCandidate.Domain.Assessments.ParticipantType.Unknown)
        {
            throw new DomainValidationException("Participant type must be specified.");
        }

        EnsureEditable();

        ParticipantType = participantType;
        Status = AssessmentStatus.InProgress;
        UpdatedAt = updatedAt;
    }

    public void MarkInProgress(DateTimeOffset updatedAt)
    {
        EnsureEditable();

        if (Status == AssessmentStatus.Draft)
        {
            Status = AssessmentStatus.InProgress;
        }

        UpdatedAt = updatedAt;
    }

    public void MarkReadyForEvaluation(DateTimeOffset updatedAt)
    {
        EnsureEditable();

        if (ParticipantType is null)
        {
            throw new DomainValidationException(
                "Assessment cannot be evaluated before participant type is provided.");
        }

        Status = AssessmentStatus.ReadyForResult;
        UpdatedAt = updatedAt;
    }

    public void MarkResultGenerated(string ruleSetVersion, DateTimeOffset updatedAt)
    {
        if (Status != AssessmentStatus.ReadyForResult && Status != AssessmentStatus.ResultGenerated)
        {
            throw new DomainValidationException(
                "Assessment must be ready for evaluation before a result can be generated.");
        }

        if (string.IsNullOrWhiteSpace(ruleSetVersion))
        {
            throw new DomainValidationException("Ruleset version is required.");
        }

        RuleSetVersion = ruleSetVersion.Trim();
        Status = AssessmentStatus.ResultGenerated;
        UpdatedAt = updatedAt;
    }

    private void EnsureEditable()
    {
        if (Status == AssessmentStatus.ReadyForResult ||
            Status == AssessmentStatus.ResultGenerated ||
            Status == AssessmentStatus.Completed ||
            Status == AssessmentStatus.Abandoned ||
            Status == AssessmentStatus.Expired ||
            Status == AssessmentStatus.Deleted)
        {
            throw new DomainValidationException(
                $"Assessment in status '{Status}' cannot be edited.");
        }
    }
}
