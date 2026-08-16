# SFÉRA Candidate Demo — Phase 5.3 Fix Report

## Why Phase 5.2 still failed

The Windows verification run proved that restore and compilation now succeed, but 6 integration tests still failed.

Two independent root causes were identified from the real runtime output and source review:

1. **Test database isolation was not guaranteed.** `CandidateApiFactory` attempted to override the connection string through `ConfigureAppConfiguration`, while the production minimal-hosting startup had already registered `SferaCandidateDbContext` through `AddInfrastructure(builder.Configuration)`. Parallel `WebApplicationFactory` instances could therefore still reach the same default SQLite database. The visible symptom was `SQLite Error 1: 'table "Assessments" already exists'` during `EnsureCreatedAsync`.
2. **The repository ordered by `DateTimeOffset` in SQLite.** `AssessmentAnswerRepository.ListByAssessmentAsync` used `OrderBy(answer => answer.CreatedAt)`. SQLite/EF Core has provider limitations around ordering `DateTimeOffset`, and every evaluation request loads answers through this repository. That made evaluation endpoints fall into the generic 500 handler before validation/recommendation could complete.

## Fixes

### CandidateApiFactory

The integration test factory now replaces the registered `SferaCandidateDbContext` service directly with a unique temporary SQLite database for each factory instance. This avoids depending on configuration callback ordering and isolates test classes from the production `sfera-candidate.db` file.

Temporary `-wal` and `-shm` sidecar files are also removed on disposal when present.

### AssessmentAnswerRepository

The query now orders by `QuestionKey` instead of `CreatedAt`.

The application does not depend on chronological answer ordering; it immediately converts the rows to a dictionary. Ordering by the stable string key keeps output deterministic while avoiding the SQLite `DateTimeOffset` translation limitation.

## What was not weakened

- warnings-as-errors remains enabled;
- the xUnit cancellation-token analyzer remains enabled;
- the API exception handler was not changed to hide failures;
- persistence still uses relational SQLite + EF Core;
- production startup still initializes its own database normally;
- domain rules and ROI logic were not changed.

## Required verification

Run from the project root on Windows:

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force
.\scripts\verify-phase5.3.ps1
```

The authoritative success condition is:

```text
PHASE 5.3 BUILD GATE: PASS
```
