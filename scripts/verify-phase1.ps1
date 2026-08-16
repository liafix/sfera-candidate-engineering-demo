$ErrorActionPreference = 'Stop'

$repoRoot = Split-Path -Parent $PSScriptRoot
$solution = Join-Path $repoRoot 'backend\SferaCandidate.sln'

Write-Host '== SFÉRA Candidate Demo: Phase 1 verification ==' -ForegroundColor Cyan

Write-Host "`n[1/4] .NET SDK"
dotnet --version

Write-Host "`n[2/4] Restore"
dotnet restore $solution

Write-Host "`n[3/4] Build"
dotnet build $solution --no-restore --configuration Release

Write-Host "`n[4/4] Tests"
dotnet test $solution --no-build --configuration Release

Write-Host "`nPHASE 1 BUILD/TEST GATE: PASS" -ForegroundColor Green
Write-Host 'Run the API separately and verify /health/live and /health/ready for the runtime gate.'
