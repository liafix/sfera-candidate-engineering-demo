#!/usr/bin/env bash
set -euo pipefail

echo '=== SFERA Candidate Demo - Phase 5.3 verification ==='
echo '[1/11] Runtime versions'
node --version
npm --version
dotnet --version

echo '[2/11] Backend restore'
dotnet restore ./backend/SferaCandidate.sln

echo '[3/11] Backend build'
dotnet build ./backend/SferaCandidate.sln --configuration Release --no-restore

echo '[4/11] Backend tests'
dotnet test ./backend/SferaCandidate.sln --configuration Release --no-build

pushd ./frontend >/dev/null
echo '[5/11] Frontend dependency install'
if [[ -f package-lock.json ]]; then npm ci; else npm install; fi

echo '[6/11] Frontend production dependency audit'
npm audit --omit=dev --audit-level=high

echo '[7/11] Frontend typecheck'
npm run typecheck

echo '[8/11] Frontend lint'
npm run lint

echo '[9/11] Frontend production build'
npm run build

echo '[10/11] Dependency metadata'
npm ls next react react-dom tailwindcss --depth=0
popd >/dev/null

echo '[11/11] Phase 5.1 source contract checks'
grep -q '/api/v1/assessments/${assessmentId}/roi' ./frontend/src/lib/api/client.ts
grep -q 'requiresExpertReview' ./frontend/src/components/result/RecommendationResultView.tsx
grep -q 'Conservative' ./frontend/src/lib/roi/scenarios.ts
grep -q 'Reference' ./frontend/src/lib/roi/scenarios.ts
grep -q 'Growth' ./frontend/src/lib/roi/scenarios.ts
grep -q 'Microsoft.OpenApi" Version="2.7.5' ./Directory.Packages.props
grep -q 'SQLitePCLRaw.bundle_e_sqlite3" Version="2.1.12' ./Directory.Packages.props

echo 'PHASE 5.3 BUILD GATE: PASS'
