using SferaCandidate.Domain.Common;

namespace SferaCandidate.Domain.Leads;

public sealed class SyntheticLead
{
    private SyntheticLead()
    {
        OrganizationName = string.Empty;
        Segment = string.Empty;
    }

    private SyntheticLead(
        Guid id,
        Guid assessmentId,
        string organizationName,
        string segment,
        SyntheticLeadStatus status,
        DateTimeOffset createdAt)
    {
        Id = id;
        AssessmentId = assessmentId;
        OrganizationName = organizationName;
        Segment = segment;
        Status = status;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }

    public Guid AssessmentId { get; private set; }

    public string OrganizationName { get; private set; }

    public string Segment { get; private set; }

    public SyntheticLeadStatus Status { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public static SyntheticLead Create(
        Guid id,
        Guid assessmentId,
        string organizationName,
        string segment,
        SyntheticLeadStatus status,
        DateTimeOffset createdAt)
    {
        if (id == Guid.Empty || assessmentId == Guid.Empty)
        {
            throw new DomainValidationException("Lead and assessment ids must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(organizationName))
        {
            throw new DomainValidationException("Organization name is required.");
        }

        if (string.IsNullOrWhiteSpace(segment))
        {
            throw new DomainValidationException("Lead segment is required.");
        }

        return new SyntheticLead(
            id,
            assessmentId,
            organizationName.Trim(),
            segment.Trim(),
            status,
            createdAt);
    }
}
