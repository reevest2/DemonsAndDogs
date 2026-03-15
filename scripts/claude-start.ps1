param(
    [Parameter(Mandatory=$true)]
    [string]$FeatureName
)

git checkout develop
git pull origin develop
git checkout -b "feature/$FeatureName"
Write-Host "Branch feature/$FeatureName created off develop. Ready for Claude session."
