# Phase 2 Report — Domain Model, Recommendation Engine & ROI Calculator

## Status

Implementation complete in source code. Local .NET build/test gate must still be run on a machine with the .NET SDK installed.

## Implemented domain model

### Assessments
- `Assessment`
- `AssessmentAnswer`
- `AssessmentStatus`
- `ParticipantType`
- `NeedCategory`

### Recommendations
- `RecommendationInput`
- `RecommendationDecision`
- `RecommendationEngine`
- `Recommendation`
- `RecommendationStatus`

### ROI
- `RoiCalculationInput`
- `RoiCalculationResult`
- `RoiCalculator`
- `RoiScenario`
- `RoiScenarioName`

### Sales workspace foundation
- `SyntheticLead`
- `SyntheticLeadStatus`

### Audit foundation
- `AuditEvent`
- `AuditAction`

### Common
- `DomainValidationException`

## Deterministic Recommendation Engine

Ruleset version:

`candidate-demo-2026.08-v1`

The engine currently implements one named candidate-demo product path: XMtrade / ETRM.
Unsupported combinations fail safely into expert review rather than inventing another product recommendation.

Every product result remains subject to expert validation.

## ROI Calculator

Implements transparent arithmetic for:

- cases per year
- annual hours saved
- annual time value
- annual net benefit
- simple payback in months

No payback is claimed when annual net benefit is zero or negative.

## EF Core mapping added

The SQLite model now includes:

- Assessments
- AssessmentAnswers
- Recommendations
- RoiScenarios
- SyntheticLeads
- AuditEvents

Important constraints include:

- unique `(AssessmentId, QuestionKey)` answers
- max one recommendation per assessment
- unique `(AssessmentId, ScenarioName)` ROI scenarios
- max one synthetic lead per assessment
- indexes for status, creation time and audit lookup

A migration is intentionally not hand-written in this environment. Generate it with EF tooling only after the real `dotnet build` gate passes.

## Unit tests added

Recommendation Engine:

1. trader/supplier + wholesale contracts -> ETRM pathway
2. unsupported combination -> expert review
3. same input -> same decision data
4. missing participant type -> validation error
5. score remains in allowed range

ROI Calculator:

1. known scenario -> exact expected arithmetic
2. non-positive benefit -> no payback
3. zero implementation cost -> zero-month payback when benefit is positive
4. negative input -> validation error

Assessment entity:

1. valid state transition to result generated
2. cannot become ready without participant type

Existing foundation and API health tests remain in place.

## Verification limitation

The current execution container has no `dotnet`, `csc`, `msbuild`, or Mono compiler available. Therefore this report does **not** claim a successful compile or test run.

Static verification performed here covers:

- project/file structure
- XML parsing for project/package files
- JSON parsing for appsettings
- namespace/reference consistency checks
- deterministic formula review
- test-to-rule consistency review

The authoritative Phase 2 gate is:

```powershell
.\scripts\verify-phase2.ps1
```

A PASS requires restore, Release build and all tests to pass on the user's machine.
