# Phase 5.4 Fix Report

## Trigger

Phase 5.3 compiled successfully and 21/22 tests passed on the user's Windows/.NET 10 environment. The remaining problems were:

1. `Evaluate_WithMissingRequiredAnswers_ReturnsValidationError` returned the expected `422`, but the final handled error response did not contain `X-Correlation-ID`.
2. xUnit class-fixture cleanup reported that temporary SQLite files were still locked when `CandidateApiFactory` attempted to delete them.

## Fix A — correlation ID survives handled exceptions

`CorrelationIdMiddleware` now registers the response header with `HttpResponse.OnStarting`. This makes the header part of the final response even if ASP.NET Core exception handling clears and rewrites the response body/status.

Middleware order is now:

```text
CorrelationIdMiddleware
  -> ExceptionHandlerMiddleware
     -> CORS/endpoints
```

This also ensures the correlation/trace identifier exists before the exception handler logs or serializes an error.

## Fix B — isolated in-memory SQLite for integration tests

`CandidateApiFactory` no longer creates/deletes temporary `.db` files. Each xUnit class fixture now owns one open `Microsoft.Data.Sqlite.SqliteConnection` using `Data Source=:memory:` and injects that connection into `SferaCandidateDbContext`.

The connection stays open for the full fixture lifetime so EF Core contexts see the same in-memory database. It is disposed on both synchronous and asynchronous factory cleanup; the async path is important because xUnit v3 prefers `DisposeAsync()` for fixtures that provide it.

Benefits:

- no file-lock cleanup race on Windows;
- isolation between test classes;
- no risk of touching the candidate-demo file database;
- faster deterministic integration tests.

## Gate

Run from repository root in PowerShell:

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force
.\scripts\verify-phase5.4.ps1
```

Expected final line:

```text
PHASE 5.4 BUILD GATE: PASS
```

## Verification limitation

The generation environment does not contain the .NET SDK, so the new C# changes were statically audited here. The authoritative compile/test result is the verifier output from the user's Windows .NET 10 environment.
