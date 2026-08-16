#!/usr/bin/env bash
set -euo pipefail

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
SOLUTION="$REPO_ROOT/backend/SferaCandidate.sln"
DOMAIN_TESTS="$REPO_ROOT/backend/tests/SferaCandidate.Domain.Tests/SferaCandidate.Domain.Tests.csproj"
API_TESTS="$REPO_ROOT/backend/tests/SferaCandidate.Api.Tests/SferaCandidate.Api.Tests.csproj"

echo "=== SFÉRA Candidate Demo — Phase 3 verification ==="
echo "Repo: $REPO_ROOT"

echo
echo "[1/6] .NET SDK"
dotnet --version

echo
echo "[2/6] Restore"
dotnet restore "$SOLUTION"

echo
echo "[3/6] Release build"
dotnet build "$SOLUTION" --configuration Release --no-restore

echo
echo "[4/6] Domain tests"
dotnet test "$DOMAIN_TESTS" --configuration Release --no-build

echo
echo "[5/6] API integration tests"
dotnet test "$API_TESTS" --configuration Release --no-build

echo
echo "[6/6] Full solution tests"
dotnet test "$SOLUTION" --configuration Release --no-build

echo
echo "PHASE 3 BUILD/TEST GATE: PASS"
