Param(
    [ValidateSet(“nanowin", "linux")] [String] [Parameter(Mandatory=$true)] $Platform,
    [bool] $DeleteImages = $false

)

$scriptPath = Split-Path $script:MyInvocation.MyCommand.Path
$Platform = $Platform.ToLowerInvariant()
 
$SourcePerPlatformDockerFilesPath = "$ScriptPath\_docker\$Platform\extradf"
$TargetPerPlatformDockerFilesPath = "$ScriptPath\extradf"

Write-Host "Current script directory is $scriptPath" -ForegroundColor Yellow


If ($DeleteImages) {
    Write-Host "Deleting eShop Docker images"
    & "$ScriptPath\delete-images.ps1"
}

If (Test-Path $TargetPerPlatformDockerFilesPath) {
    Write-Host "Found per-platform extra Docker files. Removing..."
    Remove-Item "$TargetPerPlatformDockerFilesPath\" -Recurse -Force
}

If (Test-Path $SourcePerPlatformDockerFilesPath) {
    Write-Host "Copying per-platform extra Dockerfiles"
    Copy-Item "$SourcePerPlatformDockerFilesPath\*" "$ScriptPath\extradf\" -Recurse -Force
}
else {
    Write-Host "There are not extra Dockerfiles for platform $Platform"
}

Write-Host "Changing Dockerfiles"
Copy-Item "$ScriptPath\src\Services\Basket\Basket.API\Dockerfile.$Platform"  "$ScriptPath\src\Services\Basket\Basket.API\Dockerfile" -Force
Copy-Item "$ScriptPath\src\Services\Catalog\Catalog.API\Dockerfile.$Platform"  "$ScriptPath\src\Services\Catalog\Catalog.API\Dockerfile" -Force
Copy-Item "$ScriptPath\src\Services\Identity\Identity.API\Dockerfile.$Platform"  "$ScriptPath\src\Services\Identity\Identity.API\Dockerfile" -Force
Copy-Item "$ScriptPath\src\Services\Ordering\Ordering.API\Dockerfile.$Platform"  "$ScriptPath\src\Services\Ordering\Ordering.API\Dockerfile" -Force

Copy-Item "$ScriptPath\src\Web\WebMVC\Dockerfile.$Platform"  "$ScriptPath\src\Web\WebMVC\Dockerfile" -Force
Copy-Item "$ScriptPath\src\Web\WebSPA\Dockerfile.$Platform"  "$ScriptPath\src\Web\WebSPA\Dockerfile" -Force

Write-Host "Replacing Docker-compose"
Copy-Item "$ScriptPath\_docker\$Platform\*.yml"  "$ScriptPath\" -Force

Remove-Item "$ScriptPath\.eshopdocker_*" -Force -ErrorAction SilentlyContinue
New-Item "$ScriptPath\.eshopdocker_$Platform" -ItemType File | Out-Null

Write-Host "Done. Docker files are set for platform: $Platform"