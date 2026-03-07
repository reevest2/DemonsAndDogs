param(
    [string]$SolutionRoot = (Get-Location).Path
)

$DocsFolder = Join-Path $SolutionRoot "docs"
$IndexFile = Join-Path $DocsFolder "index.md"
$CurrentStateFile = Join-Path $DocsFolder "current-state.md"
$RoadmapFile = Join-Path $DocsFolder "roadmap.md"

$ErrorCount = 0
$WarningCount = 0
$PassCount = 0

Write-Host ""
Write-Host "Validating Documentation..." -ForegroundColor Cyan
Write-Host ""

if (-not (Test-Path $DocsFolder)) {
    Write-Error "Docs folder not found: $DocsFolder"
    exit 1
}

if (-not (Test-Path $IndexFile)) {
    Write-Error "index.md not found in $DocsFolder"
    exit 1
}

# 1. Read index.md
$IndexContent = Get-Content -Path $IndexFile -Raw

# 2. Extract links from index.md (all links)
$AllLinksInIndex = [regex]::Matches($IndexContent, '\[[^\]]+\]\(([^)]+\.md)\)') | ForEach-Object { $_.Groups[1].Value }

# 3. Extract links specifically from the Documentation Index table
# We look for the table after the "## Documentation Index" header
$TableRegex = "(?s)## Documentation Index\s+(.*?)(?:\n##|$)"
$TableContent = ""
if ($IndexContent -match $TableRegex) {
    $TableContent = $Matches[1]
}
$TableLinks = [regex]::Matches($TableContent, '\[[^\]]+\]\(([^)]+\.md)\)') | ForEach-Object { $_.Groups[1].Value }

# 4. Check every .md file in docs/ has a corresponding entry in index.md
$AllMdFiles = Get-ChildItem -Path $DocsFolder -Filter "*.md" | Where-Object { $_.Name -ne "index.md" }

foreach ($file in $AllMdFiles) {
    if ($AllLinksInIndex -contains $file.Name) {
        $PassCount++
    } else {
        Write-Host "ERROR: $($file.Name) is missing from index.md" -ForegroundColor Red
        $ErrorCount++
    }
}

# 5. Check every link in index.md's Documentation Index table points to a file that exists
foreach ($link in $TableLinks) {
    $fullPath = Join-Path $DocsFolder $link
    if (Test-Path $fullPath) {
        $PassCount++
    } else {
        Write-Host "ERROR: Broken link in index.md table points to non-existent file: $link" -ForegroundColor Red
        $ErrorCount++
    }
}

# 6. current-state.md exists and warn if it hasn't been modified in > 7 days
if (Test-Path $CurrentStateFile) {
    $lastWrite = (Get-Item $CurrentStateFile).LastWriteTime
    $daysOld = (New-TimeSpan -Start $lastWrite -End (Get-Date)).Days
    
    if ($daysOld -gt 7) {
        Write-Host "WARNING: current-state.md hasn't been updated in $daysOld days" -ForegroundColor Yellow
        $WarningCount++
    } else {
        $PassCount++
    }
} else {
    Write-Host "ERROR: current-state.md is missing" -ForegroundColor Red
    $ErrorCount++
}

# 7. roadmap.md exists and contains at least one In Progress item
if (Test-Path $RoadmapFile) {
    $RoadmapContent = Get-Content -Path $RoadmapFile -Raw
    if ($RoadmapContent -like "*In Progress*") {
        $PassCount++
    } else {
        Write-Host "ERROR: roadmap.md does not contain any 'In Progress' items" -ForegroundColor Red
        $ErrorCount++
    }
} else {
    Write-Host "ERROR: roadmap.md is missing" -ForegroundColor Red
    $ErrorCount++
}

# Summary
Write-Host ""
Write-Host "Summary:" -ForegroundColor Cyan
Write-Host "  Checks Passed: $PassCount" -ForegroundColor Green
Write-Host "  Warnings:      $WarningCount" -ForegroundColor Yellow
Write-Host "  Errors:        $ErrorCount" -ForegroundColor Red
Write-Host ""

if ($ErrorCount -gt 0) {
    exit 1
}
exit 0
