$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$solution = Join-Path $repoRoot "backend\SferaCandidate.sln"

Write-Host "=== SFÉRA Candidate Demo — Phase 2 verification ==="
Write-Host "Repo: $repoRoot"

Write-Host "`n[1/5] .NET SDK"
dotnet --version

Write-Host "`n[2/5] Restore"
dotnet restore $solution

Write-Host "`n[3/5] Release build"
dotnet build $solution --configuration Release --no-restore

Write-Host "`n[4/5] Domain tests"
dotnet test (Join-Path $repoRoot "backend\tests\SferaCandidate.Domain.Tests\SferaCandidate.Domain.Tests.csproj") `
  --configuration Release `
  --no-build

Write-Host "`n[5/5] Full solution tests"
dotnet test $solution --configuration Release --no-build

Write-Host "`nPHASE 2 BUILD/TEST GATE: PASS" -ForegroundColor Green
