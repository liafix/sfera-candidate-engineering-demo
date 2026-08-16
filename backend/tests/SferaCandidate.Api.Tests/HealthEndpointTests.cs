using System.Net;
using SferaCandidate.Api.Tests.Infrastructure;

namespace SferaCandidate.Api.Tests;

public sealed class HealthEndpointTests : IClassFixture<CandidateApiFactory>
{
    private readonly HttpClient _client;

    public HealthEndpointTests(CandidateApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task LiveHealth_ReturnsOk()
    {
        using var response = await _client.GetAsync(
            "/health/live",
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains(
            response.Headers,
            header => string.Equals(header.Key, "X-Correlation-ID", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task ReadyHealth_ReturnsOkWhenSqliteIsAvailable()
    {
        using var response = await _client.GetAsync(
            "/health/ready",
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
