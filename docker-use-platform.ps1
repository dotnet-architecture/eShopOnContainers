Param([ValidateSet(“nanowin", "linux")] [String] [Parameter(Mandatory=$true)] $Platform)

$scriptPath = Split-Path $script:MyInvocation.MyCommand.Path
 
Write-Host "Current script directory is $scriptPath" -ForegroundColor Yellow
Write-Host "Deleting eShop Docker images"
& "$ScriptPath\delete-images.ps1"

Write-Host "Changing Dockerfiles"
Copy-Item "$ScriptPath\src\Services\Basket\Basket.API\Dockerfile.$Platform"  "$ScriptPath\src\Services\Basket\Basket.API\Dockerfile" -Force
Copy-Item "$ScriptPath\src\Services\Catalog\Catalog.API\Dockerfile.$Platform"  "$ScriptPath\src\Services\Catalog\Catalog.API\Dockerfile" -Force
Copy-Item "$ScriptPath\src\Services\Identity\Identity.API\Dockerfile.$Platform"  "$ScriptPath\src\Services\Identity\Identity.API\Dockerfile" -Force
Copy-Item "$ScriptPath\src\Services\Ordering\Ordering.API\Dockerfile.$Platform"  "$ScriptPath\src\Services\Ordering\Ordering.API\Dockerfile" -Force

Copy-Item "$ScriptPath\src\Web\WebMVC\Dockerfile.$Platform"  "$ScriptPath\src\Web\WebMVC\Dockerfile" -Force
Copy-Item "$ScriptPath\src\Web\WebSPA\Dockerfile.$Platform"  "$ScriptPath\src\Web\WebSPA\Dockerfile" -Force

Write-Host "Replacing Docker-compose"
Copy-Item "$ScriptPath\_docker\$Platform\*.yml"  "$ScriptPath\" -Force
Write-Host "Done. Docker files are set for platform: $Platform"