# Architecture — through Phase 5

## Goal

Keep the candidate demo small, explainable and testable while demonstrating a real HTTP -> application -> domain -> persistence workflow.

## Logical view

```text
                 ┌──────────────────────────────┐
                 │ Browser / Next.js — Phase 4 │
                 └──────────────┬───────────────┘
                                │ REST / JSON
                                ▼
┌─────────────────────────────────────────────────────────────┐
│ SferaCandidate.Api                                          │
│ endpoints • JSON contract • CORS • errors • correlation ID  │
│ health • OpenAPI                                            │
└──────────────────────────┬──────────────────────────────────┘
                           │
                           ▼
┌─────────────────────────────────────────────────────────────┐
│ SferaCandidate.Application                                  │
│ CreateAssessment • SaveAnswer • Evaluate • GetResult • ROI  │
│ persistence ports • orchestration • error semantics          │
└────────────────┬───────────────────────────┬────────────────┘
                 │                           │ interfaces
                 ▼                           ▼
┌──────────────────────────────┐   ┌───────────────────────────┐
│ SferaCandidate.Domain        │   │ Infrastructure            │
│ Assessment state             │   │ EF Core repositories      │
│ RecommendationEngine         │   │ UnitOfWork                │
│ RoiCalculator                │   │ SQLite                    │
│ Recommendation/ROI/Lead      │   │ SystemClock / health      │
│ Audit entities               │   └─────────────┬─────────────┘
└──────────────────────────────┘                 │
                                               ▼
                                          relational DB
```

## Dependency rules

- `Domain` references no solution project and no persistence/web framework.
- `Application` references `Domain` and defines the persistence interfaces it needs.
- `Infrastructure` implements those interfaces and references `Application` + `Domain`.
- `Api` composes `Application` + `Infrastructure` and owns the HTTP boundary.
- API integration tests boot the real `Api` project.

## Application use cases

```text
CreateAssessmentHandler
GetAssessmentHandler
SaveAnswerHandler
EvaluateAssessmentHandler
GetAssessmentResultHandler
CalculateRoiHandler
```

Handlers own orchestration. They do not contain SQL and they do not know that SQLite exists.

## Request path: save answer

```text
PUT /answers/{questionKey}
        │
        ▼
SaveAnswerHandler
        │
        ├─ validate supported question/value
        ├─ load assessment
        ├─ create/update AssessmentAnswer
        ├─ update assessment lifecycle
        ├─ append AuditEvent (question key only)
        ▼
IUnitOfWork.SaveChangesAsync
```

## Request path: evaluate

```text
POST /evaluate
      │
      ▼
EvaluateAssessmentHandler
      │
      ├─ existing recommendation? ── yes ──> return persisted result
      │
      ├─ load required answers
      ├─ validate complete input set
      ├─ RecommendationEngine.Evaluate
      ├─ persist Recommendation
      ├─ persist SyntheticLead
      ├─ mark Assessment ResultGenerated + ruleset version
      ├─ append audit events
      ▼
UnitOfWork
```

The result input set becomes immutable after evaluation. This prevents a stored recommendation from silently drifting away from the answers that produced it.

## Idempotency

Sequential repeat requests to `POST /evaluate` return the existing recommendation and lead.

Relational constraints provide a second line of defense:

```text
Recommendations UNIQUE(AssessmentId)
SyntheticLeads  UNIQUE(AssessmentId)
```

For a production high-concurrency API, an explicit transactional/concurrency strategy would be added around the evaluation command. Phase 3 does not pretend that sequential demo idempotency is the full distributed-systems solution.

## ROI update semantics

`RoiScenarios` remain unique by:

```text
AssessmentId + ScenarioName
```

A repeated request for the same scenario recalculates the existing entity and updates `UpdatedAt`; it does not create a duplicate.

## Persistence

Tables:

```text
Assessments
AssessmentAnswers
Recommendations
RoiScenarios
SyntheticLeads
AuditEvents
```

Key constraints:

```text
AssessmentAnswers UNIQUE(AssessmentId, QuestionKey)
Recommendations   UNIQUE(AssessmentId)
RoiScenarios      UNIQUE(AssessmentId, ScenarioName)
SyntheticLeads    UNIQUE(AssessmentId)
```

Phase 3 startup uses SQLite `EnsureCreatedAsync` only for candidate-demo portability. Production should use reviewed migrations.

## API errors

Application/domain exceptions are translated at the API boundary:

```text
NotFoundException          -> 404 NOT_FOUND
ConflictException          -> 409 CONFLICT
DomainValidationException  -> 422 VALIDATION_FAILED
unexpected exception       -> 500 INTERNAL_ERROR
```

All error envelopes include the request correlation ID. Stack traces are not sent to the client.

## Audit boundary

`AuditEvent` is append-only in the domain model; there is no update method.

Answer-save audit metadata intentionally stores only the question key, not the answer value. This avoids turning an audit log into a duplicate sensitive-data store.

## Test architecture

```text
CandidateApiFactory
  └─ real WebApplicationFactory<Program>
      └─ unique temporary SQLite database
```

The integration suite exercises the real routing, dependency injection, handlers, repositories, EF mappings and SQLite persistence rather than mocking the application boundary.

---

# Phase 4 — Browser layer

Phase 4 adds a Next.js/React presentation layer without moving recommendation or ROI logic into the browser.

```text
Browser / Next.js
       │
       │ REST + JSON
       ▼
ASP.NET Core API
       │
       ▼
Application handlers
       │
       ├── Domain rules
       └── Persistence abstractions
                │
                ▼
          EF Core / SQLite
```

## Frontend boundaries

The frontend owns:

- interaction state,
- presentation,
- assessment navigation,
- client-side required-field affordances,
- API error presentation,
- accessible controls.

The frontend does **not** own:

- product recommendation rules,
- ruleset versioning,
- assessment lifecycle truth,
- ROI mathematics,
- persistence constraints,
- authorization or production security policy.

`src/lib/assessment/questions.ts` is a presentation configuration for question copy and allowed API values. Backend validation remains authoritative.

## Resume model

The wizard does not treat React state as the source of truth across reloads.

```text
page open
  -> GET assessment
  -> inspect persisted answers
  -> resume at first unanswered required question
```

After an assessment is already evaluated, the page loads the stored recommendation instead of trying to mutate inputs.

## Failure behavior

The browser distinguishes safe API errors from transport failures. The API correlation ID is surfaced when available so a technical reviewer can trace a failing request without exposing stack traces.

---

# Phase 5 — Result and ROI browser flow

Phase 5 keeps the browser as a presentation/orchestration client. It does not duplicate the domain calculators.

```text
GET /assessment/{id}
        │
        ├── already evaluated?
        │       │
        │       └── GET /result
        │              │
        │              ▼
        │       RecommendationResultView
        │              │
        │              └── explicit reasons + ruleset + expert-review boundary
        │
        └── assessment wizard

RecommendationResultView
        │
        ▼
RoiModeler
        │
        ├── select synthetic scenario preset
        ├── edit visible assumptions
        └── POST /roi
                │
                ▼
        CalculateRoiHandler
                │
                ▼
          Domain RoiCalculator
                │
                ▼
         persisted RoiScenario
                │
                ▼
        server-confirmed metrics
```

## No duplicate ROI engine

The browser owns scenario presets and editable input state only. The returned metrics are rendered from `RoiScenarioDto`.

The browser deliberately does not calculate a hidden replacement value when the API is unavailable. This makes failure visible and keeps one source of calculation truth.

## Stale input protection

After a successful server calculation, the submitted assumptions are remembered in client state. If the user edits a field, the UI explains that the metrics still represent the last confirmed server result until **Prepočítať model** is pressed.

This prevents a visually subtle but important integrity bug where changed inputs could appear to correspond to old outputs.

## Synthetic scenario semantics

`Conservative`, `Reference` and `Growth` are candidate-defined input presets only. They are not SFÉRA scenarios, customer forecasts or commercial estimates.

Selecting the same scenario repeatedly uses the backend's existing `(AssessmentId, ScenarioName)` uniqueness and recalculation semantics.

