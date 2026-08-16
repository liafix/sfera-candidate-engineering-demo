# Phase 5 — Recommendation Result + ROI Scenario Model

## Objective

Turn the persisted Phase 3 recommendation into the strongest interview-facing technical screen without moving domain logic into the browser.

The result experience must show four things clearly:

1. **what the deterministic ruleset returned,**
2. **why it returned it,**
3. **which parts still require domain-expert validation,**
4. **how an illustrative business case changes when visible assumptions change.**

> **UNOFFICIAL CANDIDATE DEMO.** The screen uses public context, candidate-defined rules and synthetic assumptions. It is not an official SFÉRA product, pricing tool, forecast or internal system.

## Screen anatomy

```text
Recommendation status rail
        │
        ├── Suggested solution pathway
        ├── Organization / product code / expert review metadata
        ├── Explainable rule reasons
        ├── Demo fit score + ruleset metadata
        │
        ▼
Transparent Business Case
        │
        ├── Conservative / Reference / Growth scenario presets
        ├── Editable assumptions
        ├── POST /api/v1/assessments/{id}/roi
        ├── Annual hours saved
        ├── Annual time value
        ├── Annual net benefit
        └── Simple payback
```

## Design direction

Phase 5 remains inside the Phase 4 enterprise product system:

- slate/white information surfaces,
- dark system rails only where hierarchy benefits,
- restrained sky accent,
- no marketing illustration,
- no fake energy charts,
- no decorative AI language,
- tables/rows/metrics rather than bento-card overload,
- compact mono metadata for technical state,
- strong visible disclaimers.

The result page should feel like an operational review screen rather than a sales landing page.

## Recommendation result rules

The browser renders only the persisted API result:

```text
RecommendationResultDto
  recommendationId
  assessmentId
  productCode
  displayName
  fitScore
  status
  requiresExpertReview
  reasons[]
  ruleSetVersion
  createdAt
  syntheticLeadId
```

The frontend does not reconstruct or re-score recommendation logic.

### Fit score presentation

The gauge is a visualization of the returned candidate-demo score only.

It must explicitly state that it is **not**:

- probability of project success,
- commercial rating,
- official SFÉRA metric,
- machine-learning confidence.

## ROI scenario presets

All preset values are synthetic and explicitly labelled as model assumptions.

### Conservative

```text
casesPerMonth          70
minutesSavedPerCase    20
loadedHourlyCost       35 EUR/h
annualOperatingCost    7,000 EUR
implementationCost    14,000 EUR
```

### Reference

```text
casesPerMonth         100
minutesSavedPerCase    30
loadedHourlyCost       40 EUR/h
annualOperatingCost    6,000 EUR
implementationCost    12,000 EUR
```

### Growth

```text
casesPerMonth         160
minutesSavedPerCase    40
loadedHourlyCost       45 EUR/h
annualOperatingCost    7,000 EUR
implementationCost    14,000 EUR
```

These numbers are not estimates for SFÉRA or its customers.

## Server authority

The browser sends assumptions to:

```http
POST /api/v1/assessments/{assessmentId}/roi
```

The ASP.NET Core domain calculator remains the authoritative calculator.

The frontend displays the returned fields:

```text
casesPerYear
annualHoursSaved
annualTimeValue
annualNetBenefit
simplePaybackMonths
updatedAt
```

It does not implement a second hidden ROI engine.

## Scenario behavior

- The reference scenario is calculated on first result load.
- Selecting another preset sends that preset to the API.
- Editing an assumption does not silently change displayed server metrics.
- The UI marks edited inputs as **not yet recalculated**.
- Pressing **Prepočítať model** sends the new assumptions to the API.
- Reusing the same scenario name exercises the existing Phase 3 idempotent-recalculation behavior.

## Failure states

If ROI calculation fails:

- recommendation result remains visible,
- no local fallback calculation is invented,
- the safe API error is shown,
- correlation ID is surfaced when available,
- the user can retry by calculating again.

## Accessibility

- scenario controls use `aria-pressed`,
- metrics are text, not chart-only meaning,
- input fields have visible labels,
- loading/result messages use `aria-live`,
- keyboard focus rings remain visible,
- the score gauge also has an accessible text label,
- no meaning depends on colour alone.

## Phase 5 Definition of Done

- [x] persisted recommendation is rendered as the primary result
- [x] rule reasons are visibly enumerated
- [x] ruleset version is visible
- [x] fit-score disclaimer is visible
- [x] expert-review boundary is visible
- [x] ROI API client contract exists
- [x] Conservative / Reference / Growth presets exist
- [x] assumptions are editable
- [x] changed assumptions are marked stale until server recalculation
- [x] ROI metrics come from API response
- [x] payback `null` renders as `Not reached`
- [x] business-case disclaimer is visible
- [x] frontend source passes TypeScript syntax/transpile audit in the generation environment
- [ ] full `npm run typecheck`
- [ ] full `npm run lint`
- [ ] full `npm run build`
- [ ] live browser + API walkthrough on a machine with npm packages and .NET SDK

The unchecked items are runtime/toolchain gates and must not be claimed as passed until executed successfully.
