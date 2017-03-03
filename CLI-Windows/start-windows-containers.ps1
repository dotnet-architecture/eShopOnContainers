Param([string] $rootPath)
$scriptPath = Split-Path $script:MyInvocation.MyCommand.Path
if ([string]::IsNullOrEmpty($rootPath)) {
    $rootPath = "$scriptPath\.."
}
Write-Host "Root path used is $rootPath" -ForegroundColor Yellow

& .\build-bits.ps1 -rootPath $rootPath
docker-compose -f "$rootPath\docker-compose-windows.yml" -f "$rootPath\docker-compose-windows.override.yml"  up
