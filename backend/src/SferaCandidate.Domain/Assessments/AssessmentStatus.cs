namespace SferaCandidate.Domain.Assessments;

public enum AssessmentStatus
{
    Draft = 0,
    InProgress = 1,
    ReadyForResult = 2,
    ResultGenerated = 3,
    Completed = 4,
    Abandoned = 5,
    Expired = 6,
    Deleted = 7
}
