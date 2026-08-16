# SFÉRA Energy Solution & ROI Configurator — Candidate Engineering Demo

> **UNOFFICIAL CANDIDATE DEMO** — independent candidate project by Dušan Cabala. It is not an official SFÉRA, a. s. product, does not represent SFÉRA's internal architecture, and uses only public context plus synthetic/demo data.

## Current status

This repository is at **Phase 5 — Polished Recommendation Result + Transparent ROI Scenario Model + Business Case View**.

Implemented through Phase 5:

- layered ASP.NET Core backend (`Domain`, `Application`, `Infrastructure`, `Api`),
- deterministic recommendation engine,
- transparent ROI calculator,
- EF Core + SQLite relational persistence,
- repository abstractions and infrastructure adapters,
- application handlers for the assessment workflow,
- versioned REST API,
- persistent synthetic leads and append-only audit events,
- sequential evaluation idempotency,
- consistent API error envelope + correlation IDs,
- xUnit domain tests,
- full ASP.NET Core API integration tests using temporary SQLite databases,
- liveness/readiness health checks and OpenAPI foundation,
- Next.js/React candidate landing,
- typed browser API client with correlation-aware error handling,
- persisted one-question-at-a-time assessment wizard,
- resume-after-reload behavior,
- final evaluation call, polished persisted recommendation review and expert-validation boundary,
- Conservative / Reference / Growth synthetic ROI presets,
- editable assumptions with stale-result protection,
- server-authoritative ROI recalculation through the existing ASP.NET Core endpoint,
- business-case metrics and transparent formula presentation.

Phase 5 turns the browser layer into an interview-ready result experience while keeping recommendation and ROI truth in the backend.

## Technology baseline

- .NET 10 / ASP.NET Core
- C#
- Entity Framework Core
- SQLite for candidate-demo portability
- xUnit v3
- Next.js 16.2.11 / React 19.2.7
- TypeScript + Tailwind CSS 4.3

SQLite is an infrastructure choice, not a domain dependency. The business logic has no EF Core or SQLite dependency.

## Architecture

```text
HTTP / JSON
    │
    ▼
SferaCandidate.Api
    │
    ▼
Application handlers
    │             │
    │             └────> Persistence interfaces
    ▼                         ▲
Domain services               │
RecommendationEngine          │
RoiCalculator                 │
                              │
                    Infrastructure / EF Core
                              │
                              ▼
                            SQLite
```

Dependencies point inward:

```text
Api -> Application -> Domain
Api -> Infrastructure -> Application
Infrastructure -> Domain
```

## Candidate-demo API

Base prefix:

```text
/api/v1/assessments
```

Endpoints:

```http
POST /api/v1/assessments
GET  /api/v1/assessments/{assessmentId}
PUT  /api/v1/assessments/{assessmentId}/answers/{questionKey}
POST /api/v1/assessments/{assessmentId}/evaluate
GET  /api/v1/assessments/{assessmentId}/result
POST /api/v1/assessments/{assessmentId}/roi
```

Health/OpenAPI:

```text
GET /health
GET /health/live
GET /health/ready
GET /openapi/v1.json    # Development only
```

## Supported assessment keys

```text
organizationName                         optional
participantType                          required
primaryNeed                              required
managesWholesaleContracts                required
needsTradingOrPlanningSupport             required
```

Example answer payload:

```json
{
  "value": "trader_or_supplier"
}
```

Recommendation rules are documented in `docs/DOMAIN_RULES.md`.

## Recommendation safety

Ruleset:

```text
candidate-demo-2026.08-v1
```

The demo deliberately implements only one named path: **XMtrade / ETRM**.

Unsupported combinations return **Expert consultation required** rather than inventing a product path. Every named-product result remains marked for expert review.

After evaluation, answers become immutable. A different input set requires a new assessment. This keeps the persisted result explainable against the exact inputs that created it.

## ROI safety

ROI is illustrative arithmetic only:

```text
casesPerYear = casesPerMonth × 12
annualHoursSaved = casesPerYear × minutesSavedPerCase / 60
annualTimeValue = annualHoursSaved × loadedHourlyCost
annualNetBenefit = annualTimeValue - annualOperatingCost
simplePaybackMonths = implementationCost / annualNetBenefit × 12
```

If annual net benefit is zero or negative, no payback is claimed.

Posting the same scenario (`conservative`, `reference`, `growth`) again recalculates the existing row rather than creating a duplicate.

## Run locally

Prerequisites: compatible .NET 10 SDK plus Node.js 20.9+ / npm.

### Backend

```powershell
dotnet --version
dotnet run --project .\backend\src\SferaCandidate.Api\SferaCandidate.Api.csproj
```

Default launch profile URL:

```text
http://localhost:5158
```

The candidate database is created locally as SQLite for portability.

### Frontend

```powershell
cd .\frontend
Copy-Item .env.example .env.local
npm install
npm run dev
```

Open:

```text
http://localhost:3000
```

The frontend calls the real ASP.NET Core API through `NEXT_PUBLIC_API_BASE_URL`.

## Phase 5 verification

Windows:

```powershell
.\scripts\verify-phase5.1.ps1
```

Bash/Linux/macOS:

```bash
./scripts/verify-phase5.sh
```

The verifier runs backend restore/build/tests, installs frontend dependencies, then runs TypeScript typecheck, ESLint and the Next.js production build.

Authoritative success output:

```text
PHASE 5 BUILD GATE: PASS
```

The source-generation environment cannot access the npm registry reliably and does not have the .NET SDK, so this repository does **not** claim backend or frontend build/test PASS until the Phase 5 verifier succeeds on a properly equipped machine.

## Database deployment note

Phase 3 uses `EnsureCreatedAsync` to make the interview demo portable and self-starting. This is explicitly not presented as the desired production schema-release process.

For a production path, replace it with reviewed EF Core migrations and the deployment controls required by the target environment.

## Documentation

```text
docs/ARCHITECTURE.md
docs/DISCLAIMER.md
docs/DOMAIN_RULES.md
docs/PHASE1_REPORT.md
docs/PHASE2_REPORT.md
docs/PHASE3_REPORT.md
docs/PHASE4_FRONTEND_SPEC.md
docs/PHASE4_REPORT.md
docs/PHASE5_RESULT_ROI_SPEC.md
docs/PHASE5_REPORT.md
```

## Next phase

**Phase 6 — Mini Sales Workspace + synthetic lead list/detail + audit timeline, reusing the persisted lead and audit data already produced by the backend.**
