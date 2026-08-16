using SferaCandidate.Application.Abstractions.Time;

namespace SferaCandidate.Infrastructure.Time;

internal sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
