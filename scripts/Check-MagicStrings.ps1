param(
    [string]$SolutionRoot = (Get-Location).Path,
    [switch]$Fix
)

# Directories to scan (exclude tests, obj, AppConstants itself)
$ScanDirs = @(
    "API",
    "API.Services",
    "API.Services.Mock",
    "DataAccess",
    "Mediator",
    "Models",
    "UIComponents"
)

# Load all existing constants from AppConstants
$AppConstantsDir = Join-Path $SolutionRoot "AppConstants"
$DefinedConstants = @{}

Get-ChildItem -Path $AppConstantsDir -Filter "*.cs" -Recurse | ForEach-Object {
    $content = Get-Content $_.FullName -Raw
    $matches = [regex]::Matches($content, 'public const string \w+ = "([^"]+)"')
    foreach ($m in $matches) {
        $value = $m.Groups[1].Value
        $DefinedConstants[$value] = $_.Name
    }
}

Write-Host "Loaded $($DefinedConstants.Count) constants from AppConstants." -ForegroundColor Cyan
Write-Host ""

$Findings = @()

foreach ($dir in $ScanDirs) {
    $fullDir = Join-Path $SolutionRoot $dir
    if (-not (Test-Path $fullDir)) { continue }

    Get-ChildItem -Path $fullDir -Filter "*.cs" -Recurse | Where-Object {
        $_.FullName -notmatch "\\obj\\" -and
        $_.FullName -notmatch "\.Tests\."
    } | ForEach-Object {
        $file = $_
        $lines = Get-Content $file.FullName
        $lineNum = 0

        foreach ($line in $lines) {
            $lineNum++

            # Skip comments, using directives, namespace/class declarations
            $trimmed = $line.Trim()
            if ($trimmed.StartsWith("//") -or
                $trimmed.StartsWith("*") -or
                $trimmed.StartsWith("using ") -or
                $trimmed.StartsWith("namespace ") -or
                $trimmed.StartsWith("[") -or
                $trimmed.StartsWith("//")) {
                continue
            }

            # Find string literals
            $stringMatches = [regex]::Matches($line, '"([^"\\]{3,})"')
            foreach ($sm in $stringMatches) {
                $value = $sm.Groups[1].Value

                # Skip interpolated string content, format strings, log messages
                if ($value -match '\{') { continue }
                # Skip HTML/CSS-like content
                if ($value -match '<|>|\.css|\.js|\.html') { continue }
                # Skip exception messages (they're OK as literals)
                if ($line -match 'throw new|Exception\(|ArgumentException\(|KeyNotFoundException\(') { continue }
                # Skip test data/JSON literals
                if ($value -match '^\s*\{' -or $value -match '""') { continue }
                # Skip lines inside verbatim strings (@"...") — contains escaped quotes ""
                if ($line -match '""') { continue }
                # Skip JSON property keys (lines that look like JSON object content)
                if ($trimmed -match '^""') { continue }

                # Check if this value matches a defined constant
                if ($DefinedConstants.ContainsKey($value)) {
                    $Findings += [PSCustomObject]@{
                        File    = $file.FullName.Replace($SolutionRoot, "").TrimStart('\')
                        Line    = $lineNum
                        Value   = $value
                        Constant = $DefinedConstants[$value]
                    }
                }
            }
        }
    }
}

if ($Findings.Count -eq 0) {
    Write-Host "No magic strings found that match existing AppConstants." -ForegroundColor Green
} else {
    Write-Host "Found $($Findings.Count) magic string(s) that should use AppConstants:" -ForegroundColor Yellow
    Write-Host ""
    foreach ($f in $Findings) {
        Write-Host "  $($f.File):$($f.Line)" -ForegroundColor White
        Write-Host "    Value:    `"$($f.Value)`"" -ForegroundColor Gray
        Write-Host "    Constant: defined in $($f.Constant)" -ForegroundColor Gray
        Write-Host ""
    }
    exit 1
}
