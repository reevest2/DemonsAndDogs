git checkout develop
git pull origin develop
git merge master --ff-only
git push origin develop
Write-Host "develop is now in sync with master."
