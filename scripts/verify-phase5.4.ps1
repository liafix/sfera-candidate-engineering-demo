$ErrorActionPreference = 'Stop'

function Invoke-Checked {
    param(
        [Parameter(Mandatory = $true)] [scriptblock] $Command,
        [Parameter(Mandatory = $true)] [string] $Label
    )

    & $Command
    if ($LASTEXITCODE -ne 0) {
        throw "$Label failed with exit code $LASTEXITCODE."
    }
}

Write-Host '=== SFERA Candidate Demo - Phase 5.4 verification ==='

Write-Host '[1/11] Runtime versions'
Invoke-Checked { node --version } 'node --version'
Invoke-Checked { npm --version } 'npm --version'
Invoke-Checked { dotnet --version } 'dotnet --version'

Write-Host '[2/11] Backend restore'
Invoke-Checked { dotnet restore .\backend\SferaCandidate.sln } 'dotnet restore'

Write-Host '[3/11] Backend build'
Invoke-Checked { dotnet build .\backend\SferaCandidate.sln --configuration Release --no-restore } 'dotnet build'

Write-Host '[4/11] Backend tests'
Invoke-Checked { dotnet test .\backend\SferaCandidate.sln --configuration Release --no-build } 'dotnet test'

Push-Location .\frontend
try {
    Write-Host '[5/11] Frontend dependency install'
    if (Test-Path .\package-lock.json) {
        Invoke-Checked { npm ci } 'npm ci'
    }
    else {
        Write-Warning 'package-lock.json is not present. Running npm install to create the lock file.'
        Invoke-Checked { npm install } 'npm install'
    }

    Write-Host '[6/11] Frontend production dependency audit'
    Invoke-Checked { npm audit --omit=dev --audit-level=high } 'npm audit --omit=dev'

    Write-Host '[7/11] Frontend typecheck'
    Invoke-Checked { npm run typecheck } 'npm run typecheck'

    Write-Host '[8/11] Frontend lint'
    Invoke-Checked { npm run lint } 'npm run lint'

    Write-Host '[9/11] Frontend production build'
    Invoke-Checked { npm run build } 'npm run build'

    Write-Host '[10/11] Dependency metadata'
    Invoke-Checked { npm ls next react react-dom tailwindcss --depth=0 } 'npm ls'
}
finally {
    Pop-Location
}

Write-Host '[11/11] Phase 5.2 source contract checks'
$roiClient = Get-Content .\frontend\src\lib\api\client.ts -Raw
$resultView = Get-Content .\frontend\src\components\result\RecommendationResultView.tsx -Raw
$scenarioSource = Get-Content .\frontend\src\lib\roi\scenarios.ts -Raw
$packages = Get-Content .\Directory.Packages.props -Raw

if ($roiClient -notmatch '/api/v1/assessments/\$\{assessmentId\}/roi') {
    throw 'ROI API path is missing from the frontend client.'
}
if ($resultView -notmatch 'requiresExpertReview') {
    throw 'Result view no longer exposes the expert-review boundary.'
}
if ($scenarioSource -notmatch 'Conservative' -or $scenarioSource -notmatch 'Reference' -or $scenarioSource -notmatch 'Growth') {
    throw 'Expected ROI scenario labels are missing from scenarios.ts.'
}
if ($packages -notmatch 'Microsoft.OpenApi.*2\.7\.5') {
    throw 'Patched Microsoft.OpenApi 2.7.5 pin is missing.'
}
if ($packages -notmatch 'SQLitePCLRaw\.bundle_e_sqlite3.*2\.1\.12') {
    throw 'Patched SQLitePCLRaw bundle 2.1.12 pin is missing.'
}

$apiTestSources = Get-Content .\backend\tests\SferaCandidate.Api.Tests\*.cs -Raw
if ($apiTestSources -notmatch 'TestContext\.Current\.CancellationToken') {
    throw 'API integration tests are missing the xUnit cancellation-token pattern.'
}

Write-Host 'PHASE 5.4 BUILD GATE: PASS' -ForegroundColor Green
