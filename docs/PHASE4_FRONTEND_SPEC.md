# Phase 4 Frontend Spec — Candidate Demo Landing + Assessment Wizard

## Goal

Turn the verified backend architecture into a browser-presentable candidate experience without moving domain logic into React.

The frontend must demonstrate three things quickly:

1. this is an independent candidate project, not an official SFÉRA product;
2. the browser talks to a real ASP.NET Core API and persists assessment state;
3. the UI makes deterministic/explainable system behavior visible without pretending to know SFÉRA internals.

## Surface inventory

### 1. Global candidate shell

- dark technical header with text-only candidate identity;
- permanent amber disclaimer rail;
- no SFÉRA logo recreation;
- no claim that the visual language is SFÉRA's product design.

### 2. Landing

First viewport:

- strong engineering-focused headline;
- one primary CTA: `Spustiť candidate assessment`;
- technical boundary panel showing frontend / API / rules / persistence;
- synthetic-data and unofficial status remain visible.

Downstream sections:

- what the demo proves: workflow, rules, boundaries, failure modes;
- `What this is` / `What this is not` clarification.

### 3. Assessment Wizard

- one question at a time;
- five backend-supported question keys only;
- visible progress;
- persisted answer on Continue;
- restore/resume from GET assessment;
- required validation in UI, backend still source of truth;
- engineering note rail on desktop;
- compact single-column layout on mobile.

### 4. Phase 4 completion state

Phase 4 may show the persisted Recommendation result after evaluation, but only as a minimal technical confirmation.

The polished recommendation / ROI / business-case interface is deliberately deferred.

## Visual direction

**Paradigm:** enterprise operational software, not marketing-agency presentation.

- Background: slate / true white surfaces.
- Hero: deep navy technical canvas.
- Accent: sky/cyan for focus and system state.
- Warning: amber only for unofficial/expert-validation disclosures.
- Geometry: restrained 6–12 px radii, thin borders, low shadows.
- Typography: system UI stack for zero external font dependency.
- Density: medium; enough information for engineering credibility without dashboard clutter.
- Motion: only progress/focus/state transitions; `prefers-reduced-motion` respected.

## Allowed technical claims

The UI may state facts implemented in this repository:

- Next.js + React + TypeScript frontend;
- ASP.NET Core REST API;
- EF Core + relational SQLite persistence;
- deterministic versioned recommendation rules;
- sequential idempotent evaluation;
- audit events;
- explicit reasons / expert-review fallback.

The UI must not claim:

- official SFÉRA architecture;
- production-grade concurrency guarantees;
- access to confidential data;
- validated ROI;
- official product fit;
- official SFÉRA UI/UX.

## API mapping

```text
Landing CTA
  -> POST /api/v1/assessments
  -> route /assessment/{id}

Wizard load
  -> GET /api/v1/assessments/{id}

Continue
  -> PUT /api/v1/assessments/{id}/answers/{questionKey}

Final Continue
  -> PUT final answer
  -> POST /api/v1/assessments/{id}/evaluate

Resume evaluated assessment
  -> GET assessment
  -> GET result
```

## Responsive rules

### >= 1024 px

- two-column hero;
- wizard content + engineering note rail;
- max content width around 1152–1280 px.

### 640–1023 px

- stacked hero;
- full-width wizard;
- engineering notes below main form.

### < 640 px

- compact header identity;
- disclaimer wraps naturally;
- option cards remain full width;
- action buttons can wrap but never clip.

## Accessibility baseline

- semantic landmarks;
- real radio inputs inside selectable option cards;
- visible focus states;
- progressbar ARIA metadata;
- error regions use `role=alert`;
- no interaction depends only on color;
- reduced-motion support.
