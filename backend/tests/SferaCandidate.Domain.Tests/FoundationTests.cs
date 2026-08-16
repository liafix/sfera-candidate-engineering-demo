using SferaCandidate.Domain;

namespace SferaCandidate.Domain.Tests;

public sealed class FoundationTests
{
    [Fact]
    public void DomainAssembly_HasExpectedName()
    {
        var assemblyName = typeof(DomainAssemblyMarker).Assembly.GetName().Name;

        Assert.Equal("SferaCandidate.Domain", assemblyName);
    }
}
