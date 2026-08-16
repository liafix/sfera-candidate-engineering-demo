#!/usr/bin/env bash
set -euo pipefail

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
SOLUTION="$REPO_ROOT/backend/SferaCandidate.sln"

echo '== SFÉRA Candidate Demo: Phase 1 verification =='
echo
echo '[1/4] .NET SDK'
dotnet --version

echo
echo '[2/4] Restore'
dotnet restore "$SOLUTION"

echo
echo '[3/4] Build'
dotnet build "$SOLUTION" --no-restore --configuration Release

echo
echo '[4/4] Tests'
dotnet test "$SOLUTION" --no-build --configuration Release

echo
echo 'PHASE 1 BUILD/TEST GATE: PASS'
echo 'Run the API separately and verify /health/live and /health/ready for the runtime gate.'
