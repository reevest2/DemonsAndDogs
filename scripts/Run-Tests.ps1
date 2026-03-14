param(
    [string]$SolutionRoot = (Get-Location).Path,
    [string]$Project = "",
    [switch]$Verbose
)

$ErrorActionPreference = "Stop"

$sln = Join-Path $SolutionRoot "DemonsAndDogs.sln"

if (-not (Test-Path $sln)) {
    Write-Error "Solution not found at $sln"
    exit 1
}

Write-Host "Running tests..." -ForegroundColor Cyan

$args = @("test", $sln, "--no-restore")

if ($Project) {
    $args += "--filter"
    $args += "FullyQualifiedName~$Project"
}

if (-not $Verbose) {
    $args += "--verbosity"
    $args += "quiet"
}

& dotnet @args

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "All tests passed." -ForegroundColor Green
} else {
    Write-Host ""
    Write-Host "Some tests failed." -ForegroundColor Red
    exit 1
}
