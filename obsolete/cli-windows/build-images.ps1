Param([string] $imageTag)

$scriptPath = Split-Path $script:MyInvocation.MyCommand.Path

if ([string]::IsNullOrEmpty($imageTag)) {
    $imageTag =  $(git rev-parse --abbrev-ref HEAD)
}

Write-Host "Building images with tag $imageTag" -ForegroundColor Yellow
$env:TAG=$imageTag
docker-compose -f "$scriptPath\..\docker-compose.yml" -f "$scriptPath\..\docker-compose.windows.yml" build