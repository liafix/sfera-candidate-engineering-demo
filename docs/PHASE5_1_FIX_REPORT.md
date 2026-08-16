# Phase 5.1 Verification Fix Report

Date: 2026-08-16

## Trigger

The first real Windows verification run exposed four classes of issues that could not be detected in the source-generation environment:

1. `Microsoft.OpenApi` 2.0.0 resolved transitively and is affected by GHSA-v5pm-xwqc-g5wc.
2. `SQLitePCLRaw.lib.e_sqlite3` 2.1.11 resolved transitively and is affected by GHSA-2m69-gcr7-jv3q.
3. xUnit analyzers are warnings-as-errors and rejected two `Assert.True(collection.Any(...))` assertions; related substring assertions were also normalized proactively.
4. The Phase 5 verifier looked for scenario labels in `RoiModeler.tsx`, while the source of truth is `src/lib/roi/scenarios.ts`.

The Windows run also reported 3 high-severity npm findings. React's July 2026 advisory identifies the affected React Server Components packages through 19.2.7 and the patched 19.2.8 line, so React/ReactDOM were upgraded to 19.2.8 and Next/eslint-config-next to 16.2.12.

## Changes

- Pin `Microsoft.OpenApi` to 2.7.5 and add it as a direct API dependency.
- Pin `SQLitePCLRaw.bundle_e_sqlite3` to 2.1.12 and add it as a direct Infrastructure dependency.
- Update xUnit assertions to analyzer-preferred forms.
- Update frontend to Next 16.2.12 + React/ReactDOM 19.2.8.
- Correct scenario contract check to inspect `scenarios.ts`.
- Make the PowerShell verifier actually fail immediately when a native command returns a non-zero exit code.
- Add a production npm audit gate (`npm audit --omit=dev --audit-level=high`).
- Replace non-ASCII verifier banner text to avoid Windows terminal mojibake.

## Required authoritative verification

Run from repository root:

```powershell
.\scripts\verify-phase5.1.ps1
```

The phase is locked only after the final line is:

```text
PHASE 5.1 BUILD GATE: PASS
```
