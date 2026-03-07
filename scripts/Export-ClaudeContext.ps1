param(
    [string]$SolutionRoot = (Get-Location).Path,
    [string]$OutputFile = "",
    [string[]]$ExcludeProjects = @()
)

$IncludeExtensions = @(".cs", ".razor", ".json", ".md", ".csproj", ".sln")

$ExcludeFolders = @(
    "bin", "obj", ".git", ".vs", "node_modules",
    ".idea", "TestResults", "publish", "artifacts"
)

$ExcludeFilePatterns = @(
    "*.Designer.cs",
    "*.generated.cs",
    "*.g.cs",
    "*.g.i.cs",
    "AssemblyInfo.cs",
    "GlobalUsings.g.cs",
    "launchSettings.json"
)

$ExcludeFileNames = @(
    "package-lock.json",
    "package.json",
    "tsconfig.json",
    "appsettings.Production.json"
)

if ($OutputFile -eq "") {
    $OutputFile = Join-Path $SolutionRoot "claude-context.txt"
}

if (-not (Test-Path $SolutionRoot)) {
    Write-Error "Solution root not found: $SolutionRoot"
    exit 1
}

$allExcludeFolders = $ExcludeFolders + $ExcludeProjects
$output = [System.Text.StringBuilder]::new()
$fileCount = 0
$skippedCount = 0

Write-Host ""
Write-Host "Exporting Claude context from: $SolutionRoot" -ForegroundColor Cyan
Write-Host "Output file: $OutputFile" -ForegroundColor Cyan
Write-Host ""

[void]$output.AppendLine("# Claude Project Context")
[void]$output.AppendLine("# Generated: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')")
[void]$output.AppendLine("# Solution: $(Split-Path $SolutionRoot -Leaf)")
[void]$output.AppendLine("# ================================================================")
[void]$output.AppendLine("")

$allFiles = Get-ChildItem -Path $SolutionRoot -Recurse -File |
    Where-Object {
        $IncludeExtensions -contains $_.Extension.ToLower()
    } |
    Where-Object {
        $relativePath = $_.FullName.Substring($SolutionRoot.Length)
        $sep = [System.IO.Path]::DirectorySeparatorChar
        $pathParts = $relativePath -split [regex]::Escape($sep)
        $excluded = $false
        foreach ($part in $pathParts) {
            if ($allExcludeFolders -contains $part) {
                $excluded = $true
                break
            }
        }
        -not $excluded
    } |
    Where-Object {
        $matched = $false
        foreach ($pattern in $ExcludeFilePatterns) {
            if ($_.Name -like $pattern) {
                $matched = $true
                break
            }
        }
        -not $matched
    } |
    Where-Object {
        $ExcludeFileNames -notcontains $_.Name
    } |
    Sort-Object FullName

foreach ($file in $allFiles) {
    $relativePath = $file.FullName.Substring($SolutionRoot.Length).TrimStart('\', '/')

    if ($file.Length -eq 0) {
        $skippedCount++
        continue
    }

    try {
        $content = Get-Content -Path $file.FullName -Raw -Encoding UTF8 -ErrorAction Stop

        if ([string]::IsNullOrWhiteSpace($content)) {
            $skippedCount++
            continue
        }

        [void]$output.AppendLine("## FILE: $relativePath")
        [void]$output.AppendLine('```' + $file.Extension.TrimStart('.'))
        [void]$output.AppendLine($content.TrimEnd())
        [void]$output.AppendLine('```')
        [void]$output.AppendLine("")

        $fileCount++
        Write-Host "  + $relativePath" -ForegroundColor Gray
    }
    catch {
        Write-Warning "Could not read: $relativePath"
        $skippedCount++
    }
}

[void]$output.AppendLine("# ================================================================")
[void]$output.AppendLine("# END OF CONTEXT - $fileCount files exported, $skippedCount skipped")

$output.ToString() | Set-Content -Path $OutputFile -Encoding UTF8

$fileSizeKB = [math]::Round((Get-Item $OutputFile).Length / 1KB, 1)

Write-Host ""
Write-Host "Done! $fileCount files exported ($skippedCount skipped)" -ForegroundColor Green
Write-Host "Output: $OutputFile ($fileSizeKB KB)" -ForegroundColor Green
Write-Host ""
Write-Host "Upload claude-context.txt to your Claude Project to sync context." -ForegroundColor Yellow
