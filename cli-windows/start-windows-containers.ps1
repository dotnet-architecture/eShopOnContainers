Param(
    [parameter(Mandatory=$false)][string] $rootPath,
    [parameter(Mandatory=$false)][bool]$buildBits=$true
)

$scriptPath = Split-Path $script:MyInvocation.MyCommand.Path
if ([string]::IsNullOrEmpty($rootPath)) {
    $rootPath = "$scriptPath\.."
}
Write-Host "Root path used is $rootPath" -ForegroundColor Yellow


if ($buildBits) {
    & $scriptPath\build-bits.ps1 -rootPath $rootPath
}
docker-compose -f "$rootPath\docker-compose-windows.yml" -f "$rootPath\docker-compose.override.yml"  up
