$ErrorActionPreference = 'Stop'

Write-Host '=== SFÉRA Candidate Demo — Phase 4 verification ==='

Write-Host '[1/8] Runtime versions'
node --version
npm --version

dotnet --version

Write-Host '[2/8] Backend restore/build/tests'
dotnet restore .\backend\SferaCandidate.sln
dotnet build .\backend\SferaCandidate.sln --configuration Release --no-restore
dotnet test .\backend\SferaCandidate.sln --configuration Release --no-build

Push-Location .\frontend
try {
    Write-Host '[3/8] Frontend dependency install'
    if (Test-Path .\package-lock.json) {
        npm ci
    } else {
        Write-Warning 'package-lock.json is not present because the source-generation environment could not access npm. Running npm install.'
        npm install
    }

    Write-Host '[4/8] Frontend typecheck'
    npm run typecheck

    Write-Host '[5/8] Frontend lint'
    npm run lint

    Write-Host '[6/8] Frontend production build'
    npm run build

    Write-Host '[7/8] Dependency metadata'
    npm ls next react react-dom tailwindcss --depth=0
}
finally {
    Pop-Location
}

Write-Host '[8/8] Gate'
Write-Host 'PHASE 4 BUILD GATE: PASS' -ForegroundColor Green
