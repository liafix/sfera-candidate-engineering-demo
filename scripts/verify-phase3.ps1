$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$solution = Join-Path $repoRoot "backend\SferaCandidate.sln"
$domainTests = Join-Path $repoRoot "backend\tests\SferaCandidate.Domain.Tests\SferaCandidate.Domain.Tests.csproj"
$apiTests = Join-Path $repoRoot "backend\tests\SferaCandidate.Api.Tests\SferaCandidate.Api.Tests.csproj"

Write-Host "=== SFÉRA Candidate Demo — Phase 3 verification ==="
Write-Host "Repo: $repoRoot"

Write-Host "`n[1/6] .NET SDK"
dotnet --version

Write-Host "`n[2/6] Restore"
dotnet restore $solution

Write-Host "`n[3/6] Release build"
dotnet build $solution --configuration Release --no-restore

Write-Host "`n[4/6] Domain tests"
dotnet test $domainTests --configuration Release --no-build

Write-Host "`n[5/6] API integration tests"
dotnet test $apiTests --configuration Release --no-build

Write-Host "`n[6/6] Full solution tests"
dotnet test $solution --configuration Release --no-build

Write-Host "`nPHASE 3 BUILD/TEST GATE: PASS" -ForegroundColor Green
