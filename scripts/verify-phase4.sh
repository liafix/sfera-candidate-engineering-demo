#!/usr/bin/env bash
set -euo pipefail

echo '=== SFÉRA Candidate Demo — Phase 4 verification ==='

echo '[1/8] Runtime versions'
node --version
npm --version
dotnet --version

echo '[2/8] Backend restore/build/tests'
dotnet restore ./backend/SferaCandidate.sln
dotnet build ./backend/SferaCandidate.sln --configuration Release --no-restore
dotnet test ./backend/SferaCandidate.sln --configuration Release --no-build

cd frontend

echo '[3/8] Frontend dependency install'
if [[ -f package-lock.json ]]; then
  npm ci
else
  echo 'WARNING: package-lock.json absent; running npm install.' >&2
  npm install
fi

echo '[4/8] Frontend typecheck'
npm run typecheck

echo '[5/8] Frontend lint'
npm run lint

echo '[6/8] Frontend production build'
npm run build

echo '[7/8] Dependency metadata'
npm ls next react react-dom tailwindcss --depth=0

echo '[8/8] Gate'
echo 'PHASE 4 BUILD GATE: PASS'
