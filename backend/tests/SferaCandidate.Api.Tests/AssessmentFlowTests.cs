using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using SferaCandidate.Api.Tests.Infrastructure;

namespace SferaCandidate.Api.Tests;

public sealed class AssessmentFlowTests : IClassFixture<CandidateApiFactory>
{
    private readonly HttpClient _client;

    public AssessmentFlowTests(CandidateApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task FullAssessmentFlow_PersistsAnswers_EvaluatesAndCalculatesRoi()
    {
        var assessmentId = await CreateAssessmentAsync();

        await SaveAnswerAsync(assessmentId, "organizationName", "Demo Energy Trading SK");
        await SaveAnswerAsync(assessmentId, "participantType", "trader_or_supplier");
        await SaveAnswerAsync(assessmentId, "primaryNeed", "trading_and_supply");
        await SaveAnswerAsync(assessmentId, "managesWholesaleContracts", "true");
        await SaveAnswerAsync(assessmentId, "needsTradingOrPlanningSupport", "true");

        using (var persisted = await _client.GetAsync($"/api/v1/assessments/{assessmentId}", TestContext.Current.CancellationToken))
        {
            Assert.Equal(HttpStatusCode.OK, persisted.StatusCode);
            using var json = JsonDocument.Parse(await persisted.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
            Assert.Equal("Demo Energy Trading SK", json.RootElement.GetProperty("answers").GetProperty("organizationName").GetString());
        }

        Guid recommendationId;
        using (var evaluation = await _client.PostAsync($"/api/v1/assessments/{assessmentId}/evaluate", null, TestContext.Current.CancellationToken))
        {
            Assert.Equal(HttpStatusCode.OK, evaluation.StatusCode);
            using var json = JsonDocument.Parse(await evaluation.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
            var root = json.RootElement;
            recommendationId = root.GetProperty("recommendationId").GetGuid();
            Assert.Equal("XMTRADE_ETRM", root.GetProperty("productCode").GetString());
            Assert.Equal("XMtrade / ETRM", root.GetProperty("displayName").GetString());
            Assert.Equal(95, root.GetProperty("fitScore").GetInt32());
            Assert.True(root.GetProperty("requiresExpertReview").GetBoolean());
            Assert.Equal("candidate-demo-2026.08-v1", root.GetProperty("ruleSetVersion").GetString());
            Assert.NotEmpty(root.GetProperty("reasons").EnumerateArray().ToArray());
        }

        using (var result = await _client.GetAsync($"/api/v1/assessments/{assessmentId}/result", TestContext.Current.CancellationToken))
        {
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            using var json = JsonDocument.Parse(await result.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
            Assert.Equal(recommendationId, json.RootElement.GetProperty("recommendationId").GetGuid());
        }

        var roiPayload = new
        {
            scenarioName = "reference",
            casesPerMonth = 100m,
            minutesSavedPerCase = 30m,
            loadedHourlyCost = 40m,
            annualOperatingCost = 6000m,
            implementationCost = 12000m
        };

        using (var roi = await _client.PostAsJsonAsync($"/api/v1/assessments/{assessmentId}/roi", roiPayload, TestContext.Current.CancellationToken))
        {
            Assert.Equal(HttpStatusCode.OK, roi.StatusCode);
            using var json = JsonDocument.Parse(await roi.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
            var root = json.RootElement;
            Assert.Equal(1200m, root.GetProperty("casesPerYear").GetDecimal());
            Assert.Equal(600m, root.GetProperty("annualHoursSaved").GetDecimal());
            Assert.Equal(24000m, root.GetProperty("annualTimeValue").GetDecimal());
            Assert.Equal(18000m, root.GetProperty("annualNetBenefit").GetDecimal());
            Assert.Equal(8m, root.GetProperty("simplePaybackMonths").GetDecimal());
        }
    }

    [Fact]
    public async Task Evaluate_RepeatedRequest_IsIdempotent()
    {
        var assessmentId = await CreateReadyAssessmentAsync();

        using var first = await _client.PostAsync($"/api/v1/assessments/{assessmentId}/evaluate", null, TestContext.Current.CancellationToken);
        using var second = await _client.PostAsync($"/api/v1/assessments/{assessmentId}/evaluate", null, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        Assert.Equal(HttpStatusCode.OK, second.StatusCode);

        using var firstJson = JsonDocument.Parse(await first.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
        using var secondJson = JsonDocument.Parse(await second.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));

        Assert.Equal(
            firstJson.RootElement.GetProperty("recommendationId").GetGuid(),
            secondJson.RootElement.GetProperty("recommendationId").GetGuid());
        Assert.Equal(
            firstJson.RootElement.GetProperty("syntheticLeadId").GetGuid(),
            secondJson.RootElement.GetProperty("syntheticLeadId").GetGuid());
    }

    [Fact]
    public async Task Evaluate_WithMissingRequiredAnswers_ReturnsValidationError()
    {
        var assessmentId = await CreateAssessmentAsync();
        await SaveAnswerAsync(assessmentId, "participantType", "trader_or_supplier");

        using var response = await _client.PostAsync($"/api/v1/assessments/{assessmentId}/evaluate", null, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.Contains(
            response.Headers,
            header => string.Equals(header.Key, "X-Correlation-ID", StringComparison.OrdinalIgnoreCase));

        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
        Assert.Equal("VALIDATION_FAILED", json.RootElement.GetProperty("error").GetProperty("code").GetString());
        Assert.Contains(
            "Missing required answers",
            json.RootElement.GetProperty("error").GetProperty("message").GetString() ?? string.Empty,
            StringComparison.Ordinal);
    }

    [Fact]
    public async Task SaveAnswer_AfterEvaluation_ReturnsConflict()
    {
        var assessmentId = await CreateReadyAssessmentAsync();
        using var evaluation = await _client.PostAsync($"/api/v1/assessments/{assessmentId}/evaluate", null, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, evaluation.StatusCode);

        using var response = await _client.PutAsJsonAsync(
            $"/api/v1/assessments/{assessmentId}/answers/primaryNeed",
            new { value = "other" },
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
        Assert.Equal("CONFLICT", json.RootElement.GetProperty("error").GetProperty("code").GetString());
    }

    [Fact]
    public async Task Roi_SameScenario_RecalculatesWithoutCreatingDuplicateScenario()
    {
        var assessmentId = await CreateReadyAssessmentAsync();
        using var evaluation = await _client.PostAsync($"/api/v1/assessments/{assessmentId}/evaluate", null, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, evaluation.StatusCode);

        var firstPayload = new
        {
            scenarioName = "reference",
            casesPerMonth = 100m,
            minutesSavedPerCase = 30m,
            loadedHourlyCost = 40m,
            annualOperatingCost = 6000m,
            implementationCost = 12000m
        };

        var secondPayload = new
        {
            scenarioName = "reference",
            casesPerMonth = 100m,
            minutesSavedPerCase = 45m,
            loadedHourlyCost = 40m,
            annualOperatingCost = 6000m,
            implementationCost = 12000m
        };

        using var first = await _client.PostAsJsonAsync($"/api/v1/assessments/{assessmentId}/roi", firstPayload, TestContext.Current.CancellationToken);
        using var second = await _client.PostAsJsonAsync($"/api/v1/assessments/{assessmentId}/roi", secondPayload, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        Assert.Equal(HttpStatusCode.OK, second.StatusCode);

        using var firstJson = JsonDocument.Parse(await first.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
        using var secondJson = JsonDocument.Parse(await second.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));

        Assert.Equal(firstJson.RootElement.GetProperty("id").GetGuid(), secondJson.RootElement.GetProperty("id").GetGuid());
        Assert.Equal(45m, secondJson.RootElement.GetProperty("minutesSavedPerCase").GetDecimal());
        Assert.True(
            secondJson.RootElement.GetProperty("annualNetBenefit").GetDecimal() >
            firstJson.RootElement.GetProperty("annualNetBenefit").GetDecimal());
    }

    [Fact]
    public async Task SaveAnswer_UnsupportedQuestionKey_ReturnsValidationError()
    {
        var assessmentId = await CreateAssessmentAsync();

        using var response = await _client.PutAsJsonAsync(
            $"/api/v1/assessments/{assessmentId}/answers/unknownQuestion",
            new { value = "anything" },
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
        Assert.Equal("VALIDATION_FAILED", json.RootElement.GetProperty("error").GetProperty("code").GetString());
    }

    [Fact]
    public async Task Roi_BeforeEvaluation_ReturnsConflict()
    {
        var assessmentId = await CreateAssessmentAsync();

        using var response = await _client.PostAsJsonAsync(
            $"/api/v1/assessments/{assessmentId}/roi",
            new
            {
                scenarioName = "reference",
                casesPerMonth = 100m,
                minutesSavedPerCase = 30m,
                loadedHourlyCost = 40m,
                annualOperatingCost = 6000m,
                implementationCost = 12000m
            },
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
        Assert.Equal("CONFLICT", json.RootElement.GetProperty("error").GetProperty("code").GetString());
    }

    [Fact]
    public async Task UnknownAssessment_ReturnsNotFoundErrorModel()
    {
        var unknownId = Guid.NewGuid();
        using var response = await _client.GetAsync($"/api/v1/assessments/{unknownId}", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
        var error = json.RootElement.GetProperty("error");
        Assert.Equal("NOT_FOUND", error.GetProperty("code").GetString());
        Assert.False(string.IsNullOrWhiteSpace(error.GetProperty("correlationId").GetString()));
    }

    private async Task<Guid> CreateAssessmentAsync()
    {
        using var response = await _client.PostAsync("/api/v1/assessments", null, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
        return json.RootElement.GetProperty("id").GetGuid();
    }

    private async Task<Guid> CreateReadyAssessmentAsync()
    {
        var id = await CreateAssessmentAsync();
        await SaveAnswerAsync(id, "participantType", "trader_or_supplier");
        await SaveAnswerAsync(id, "primaryNeed", "trading_and_supply");
        await SaveAnswerAsync(id, "managesWholesaleContracts", "true");
        await SaveAnswerAsync(id, "needsTradingOrPlanningSupport", "true");
        return id;
    }

    private async Task SaveAnswerAsync(Guid assessmentId, string questionKey, string value)
    {
        using var response = await _client.PutAsJsonAsync(
            $"/api/v1/assessments/{assessmentId}/answers/{questionKey}",
            new { value },
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
