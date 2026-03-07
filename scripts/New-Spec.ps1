param(
    [Parameter(Mandatory = $true)]
    [string]$Name,

    [Parameter(Mandatory = $true)]
    [string]$Title,

    [string]$Milestone = "Backlog",

    [string]$SolutionRoot = (Get-Location).Path
)

# Set script to stop on error
$ErrorActionPreference = "Stop"

# Ensure kebab-case for Name
if ($Name -cmatch "[A-Z ]") {
    Write-Error "Name must be in kebab-case (e.g. session-persistence). Received: $Name"
    exit 1
}

$DocsFolder = Join-Path $SolutionRoot "docs"
$FeaturesFolder = Join-Path $DocsFolder "features"
$SpecFile = Join-Path $FeaturesFolder "$Name.md"
$IndexFile = Join-Path $DocsFolder "state\index.md"
$RoadmapFile = Join-Path $DocsFolder "state\roadmap.md"

if (Test-Path $SpecFile) {
    Write-Error "Spec file already exists: $SpecFile"
    exit 1
}

Write-Host "Scaffolding new feature spec: $Title ($Name)" -ForegroundColor Cyan

# 1. Create the spec file
$SpecTemplate = @"
# $Title

## Overview
<!-- What is this feature and why does it exist? -->

## Goals
<!-- What specific outcomes does this feature achieve? -->

## Constraints
<!-- What must NOT change, what dependencies exist, what patterns must be followed? -->

## Data Model
<!-- Any new or modified records, entities, or database changes -->

## Flow
<!-- Step by step description of how this feature works end to end -->

## API Changes
<!-- New endpoints, modified contracts, new MediatR requests/handlers -->

## UI Changes
<!-- New or modified Blazor components -->

## Test Cases
<!-- These drive TDD - Junie writes these tests BEFORE implementation -->
### Happy Path
- [ ] $($Title)_HappyPath_Description

### Edge Cases
- [ ] $($Title)_EdgeCase_Description

### Error Cases
- [ ] $($Title)_ErrorCase_Description

## Open Questions
<!-- Unresolved decisions that need answers before implementation -->

## Implementation Notes
<!-- Any technical decisions made during implementation -->
"@

if (-not (Test-Path $FeaturesFolder)) {
    New-Item -Path $FeaturesFolder -ItemType Directory | Out-Null
}

$SpecTemplate | Set-Content -Path $SpecFile -Encoding UTF8
Write-Host "  + Created $SpecFile" -ForegroundColor Gray

# 2. Update index.md
$IndexContent = Get-Content -Path $IndexFile -Raw
$NewIndexRow = "| [features/$Name.md](../features/$Name.md) | $Title feature spec |"

if ($IndexContent -match "### Features") {
    # Section exists, find where to insert
    $Header = "### Features"
    $TableSep = "|---|---|"
    
    $HeaderPos = $IndexContent.IndexOf($Header)
    $SepPos = $IndexContent.IndexOf($TableSep, $HeaderPos)
    if ($SepPos -ge 0) {
        # Find the end of this table (until the next section or blank line)
        $EndSectionPos = $IndexContent.IndexOf("`n##", $SepPos)
        if ($EndSectionPos -lt 0) { $EndSectionPos = $IndexContent.Length }
        
        $Section = $IndexContent.Substring($SepPos, $EndSectionPos - $SepPos)
        # Find the last row in this section
        $LastRowIdx = $Section.LastIndexOf("`n|")
        if ($LastRowIdx -ge 0) {
            # Find the end of that last row
            $EndOfLine = $Section.IndexOf("`n", $LastRowIdx + 1)
            if ($EndOfLine -lt 0) { $EndOfLine = $Section.Length }
            
            $IndexContent = $IndexContent.Insert($SepPos + $EndOfLine, "`n$NewIndexRow")
        } else {
             # No rows yet, insert after TableSep
             $EndOfSep = $Section.IndexOf("`n", 0)
             if ($EndOfSep -lt 0) { $EndOfSep = $Section.Length }
             $IndexContent = $IndexContent.Insert($SepPos + $EndOfSep, "`n$NewIndexRow")
        }
    }
} else {
    # Create section. Usually after UI section or before State section.
    $FeaturesSection = @"

### Features
| Document | Description |
|---|---|
$NewIndexRow
"@
    if ($IndexContent -match "### State") {
        $IndexContent = $IndexContent -replace "### State", "$FeaturesSection`n`n### State"
    } else {
        $IndexContent = $IndexContent.TrimEnd() + "`n$FeaturesSection"
    }
}

$IndexContent | Set-Content -Path $IndexFile -Encoding UTF8
Write-Host "  + Updated $IndexFile" -ForegroundColor Gray

# 3. Update roadmap.md
$RoadmapContent = Get-Content -Path $RoadmapFile -Raw
$NewRoadmapRow = "| $Title spec | Planned | See docs/features/$Name.md |"

# Try to find existing milestone section.
$MilestoneRegex = "(?m)^## " + [regex]::Escape($Milestone) + ".*$"
if ($RoadmapContent -match $MilestoneRegex) {
    $HeaderMatch = $Matches[0]
    # Milestone exists, find the table after it
    $HeaderPos = $RoadmapContent.IndexOf($HeaderMatch)
    $TableSep = "|---|---|---|"
    $SepPos = $RoadmapContent.IndexOf($TableSep, $HeaderPos)
    
    if ($SepPos -ge 0) {
        # Find the end of this table
        $EndSectionPos = $RoadmapContent.IndexOf("`n##", $SepPos)
        if ($EndSectionPos -lt 0) { $EndSectionPos = $RoadmapContent.Length }
        
        $Section = $RoadmapContent.Substring($SepPos, $EndSectionPos - $SepPos)
        $LastRowIdx = $Section.LastIndexOf("`n|")
        if ($LastRowIdx -ge 0) {
            $EndOfLine = $Section.IndexOf("`n", $LastRowIdx + 1)
            if ($EndOfLine -lt 0) { $EndOfLine = $Section.Length }
            $RoadmapContent = $RoadmapContent.Insert($SepPos + $EndOfLine, "`n$NewRoadmapRow")
        } else {
             $EndOfSep = $Section.IndexOf("`n", 0)
             if ($EndOfSep -lt 0) { $EndOfSep = $Section.Length }
             $RoadmapContent = $RoadmapContent.Insert($SepPos + $EndOfSep, "`n$NewRoadmapRow")
        }
    }
} else {
    # Create milestone
    $NewMilestone = @"

## $Milestone

| Feature | Status | Notes |
|---|---|---|
$NewRoadmapRow
"@
    $RoadmapContent = $RoadmapContent.TrimEnd() + "`n$NewMilestone"
}

$RoadmapContent | Set-Content -Path $RoadmapFile -Encoding UTF8
Write-Host "  + Updated $RoadmapFile" -ForegroundColor Gray

# 4. Success summary and validation
Write-Host ""
Write-Host "Success! Summary:" -ForegroundColor Green
Write-Host "  Spec:      docs/features/$Name.md"
Write-Host "  Index:     Registered in docs/state/index.md"
Write-Host "  Roadmap:   Added to '$Milestone' in docs/state/roadmap.md"
Write-Host ""

Write-Host "Running Validate-Docs.ps1..." -ForegroundColor Cyan
& (Join-Path $SolutionRoot "scripts\Validate-Docs.ps1") -SolutionRoot $SolutionRoot
