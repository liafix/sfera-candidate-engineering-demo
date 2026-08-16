#!/usr/bin/env bash
set -euo pipefail

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
SOLUTION="$REPO_ROOT/backend/SferaCandidate.sln"

echo "=== SFÉRA Candidate Demo — Phase 2 verification ==="
echo "Repo: $REPO_ROOT"

echo
echo "[1/5] .NET SDK"
dotnet --version

echo
echo "[2/5] Restore"
dotnet restore "$SOLUTION"

echo
echo "[3/5] Release build"
dotnet build "$SOLUTION" --configuration Release --no-restore

echo
echo "[4/5] Domain tests"
dotnet test "$REPO_ROOT/backend/tests/SferaCandidate.Domain.Tests/SferaCandidate.Domain.Tests.csproj" \
  --configuration Release \
  --no-build

echo
echo "[5/5] Full solution tests"
dotnet test "$SOLUTION" --configuration Release --no-build

echo
echo "PHASE 2 BUILD/TEST GATE: PASS"
