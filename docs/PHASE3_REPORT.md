# Phase 3 Report — Application Use Cases, REST API & Integration Tests

## Status

Phase 3 source implementation is complete. The authoritative compile/test gate still has to run on a machine with the .NET 10 SDK because the generation environment has no `dotnet`, `csc`, or `msbuild` executable.

## What Phase 3 adds

### Application orchestration

The `Application` project now owns use cases rather than HTTP or EF Core details:

- `CreateAssessmentHandler`
- `GetAssessmentHandler`
- `SaveAnswerHandler`
- `EvaluateAssessmentHandler`
- `GetAssessmentResultHandler`
- `CalculateRoiHandler`

It also defines persistence ports:

- `IAssessmentRepository`
- `IAssessmentAnswerRepository`
- `IRecommendationRepository`
- `IRoiScenarioRepository`
- `ISyntheticLeadRepository`
- `IAuditEventRepository`
- `IUnitOfWork`

The application layer still does not reference EF Core or SQLite.

## Question contract

Phase 3 intentionally supports only the keys required by the current vertical slice:

```text
organizationName                         optional
participantType                          required for evaluation
primaryNeed                              required for evaluation
managesWholesaleContracts                required for evaluation
needsTradingOrPlanningSupport             required for evaluation
```

Supported participant values:

```text
trader_or_supplier
distribution_operator
market_operator
industrial_consumer
other
```

Supported primary-need values:

```text
trading_and_supply
distribution
market_operations
compliance_reporting
other
```

Boolean values must be `true` or `false`.

## Persistence adapters

`Infrastructure` now implements all application persistence interfaces with EF Core repositories and a shared `UnitOfWork` backed by `SferaCandidateDbContext`.

The API startup calls `EnsureCreatedAsync` for candidate-demo portability. This is deliberate for the short-lived demo and not presented as the desired production database deployment strategy. A production version should move to reviewed EF migrations and organization-approved release procedures.

## REST API

Prefix:

```text
/api/v1/assessments
```

Implemented endpoints:

```http
POST /api/v1/assessments
GET  /api/v1/assessments/{assessmentId}
PUT  /api/v1/assessments/{assessmentId}/answers/{questionKey}
POST /api/v1/assessments/{assessmentId}/evaluate
GET  /api/v1/assessments/{assessmentId}/result
POST /api/v1/assessments/{assessmentId}/roi
```

### Example assessment flow

```http
POST /api/v1/assessments
```

Then save answers such as:

```json
{ "value": "trader_or_supplier" }
```

Evaluate only after all four deterministic recommendation inputs are present.

## Evaluation idempotency

Sequential repeated evaluation of the same assessment returns the already-persisted recommendation and synthetic lead instead of creating duplicates.

Database constraints reinforce this:

```text
Recommendations UNIQUE(AssessmentId)
SyntheticLeads  UNIQUE(AssessmentId)
```

After a result is generated, answer mutation is rejected with HTTP `409 CONFLICT`. This makes the persisted result explainable against an immutable input set. To test different answers, create a new assessment.

## ROI recalculation

ROI requires an already-evaluated assessment.

Each assessment may contain one row per scenario name:

```text
Conservative
Reference
Growth
```

Posting the same scenario again recalculates the existing row instead of creating a duplicate. `CreatedAt` remains stable and `UpdatedAt` changes.

## Audit events

Application use cases append audit records for:

- assessment created,
- answer saved,
- assessment evaluated,
- recommendation generated,
- synthetic lead created,
- ROI calculated.

Answer audit metadata records the question key, not the answer value. This intentionally demonstrates a privacy-conscious audit pattern even though the demo uses synthetic data.

## Error contract

Handled application/domain errors use a consistent shape:

```json
{
  "error": {
    "code": "VALIDATION_FAILED",
    "message": "...",
    "correlationId": "...",
    "fields": []
  }
}
```

Mapped status codes:

```text
404 NOT_FOUND
409 CONFLICT
422 VALIDATION_FAILED
500 INTERNAL_ERROR
```

`X-Correlation-ID` is also returned in the response headers.

## API integration tests

The API test fixture uses a unique temporary SQLite database per factory and boots the real ASP.NET Core application through `WebApplicationFactory<Program>`.

Covered flows include:

1. liveness endpoint
2. SQLite readiness endpoint
3. full assessment -> persisted answers -> ETRM evaluation -> result -> ROI flow
4. repeated evaluation returns the same recommendation and synthetic lead
5. missing required answers return `422 VALIDATION_FAILED`
6. editing an evaluated assessment returns `409 CONFLICT`
7. recalculating the same ROI scenario reuses the row rather than duplicating it
8. unsupported question keys return `422 VALIDATION_FAILED`
9. ROI before evaluation returns `409 CONFLICT`
10. unknown assessment returns the consistent `404 NOT_FOUND` error model

Existing domain tests remain in the solution.

## Security / scope choices

Phase 3 still contains:

- no authentication,
- no real PII,
- no CRM connection,
- no production energy-system access,
- no generative recommendation logic,
- no real financial claims.

Authentication and role separation belong to a later production-oriented phase, not this interview vertical slice.

## Verification limitation

This environment cannot truthfully claim that `dotnet build` or `dotnet test` passed because no .NET SDK is installed.

Static checks performed before packaging include:

- XML parsing of all project/package files,
- JSON parsing of configuration,
- project-reference existence,
- repository/interface registration review,
- route/use-case consistency review,
- uniqueness/idempotency constraint review,
- brace/string balance heuristic for C# source,
- no-secret/no-real-email scan.

Run the authoritative gate locally:

```powershell
.\scripts\verify-phase3.ps1
```

Only the following output locks Phase 3:

```text
PHASE 3 BUILD/TEST GATE: PASS
```
