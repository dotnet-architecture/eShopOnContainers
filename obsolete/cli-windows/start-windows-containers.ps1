# rootPath: Root path of the repo (where docker-compose*.yml are). If not passed ../cli-windows/ is assumed
# buildBits: If the projects must be built before. Default value: $true
# customEventBusLoginPassword: If a custom RabbitMQ image is used that do not use the default user login/pwd. Default: $false (means assume use default spring2/rabbitmq image)

Param(
    [parameter(Mandatory=$false)][string] $rootPath,
    [parameter(Mandatory=$false)][bool] $customEventBusLoginPassword=$false,
    [parameter(Mandatory=$false)][bool]$buildBits=$false
)

$scriptPath = Split-Path $script:MyInvocation.MyCommand.Path
if ([string]::IsNullOrEmpty($rootPath)) {
    $rootPath = "$scriptPath\.."
}
Write-Host "Root path used is $rootPath" -ForegroundColor Yellow


if ($buildBits) {
    & $scriptPath\build-bits.ps1 -rootPath $rootPath
}


$env:ESHOP_EXTERNAL_DNS_NAME_OR_IP = "10.0.75.1"
$env:ESHOP_AZURE_STORAGE_CATALOG_URL ="http://10.0.75.1:5101/api/v1/catalog/items/[0]/pic/"
$env:ESHOP_AZURE_STORAGE_MARKETING_URL ="http://10.0.75.1:5110/api/v1/campaigns/[0]/pic/"
$env:ESHOP_OCELOT_VOLUME_SPEC ="C:\app\configuration"

if (-Not $customEventBusLoginPassword) {
    docker-compose -f "$rootPath\docker-compose.yml" -f "$rootPath\docker-compose.override.yml" -f "$rootPath\docker-compose.windows.yml" -f "$rootPath\docker-compose.override.windows.yml" up
}
else {
    docker-compose -f "$rootPath\docker-compose.yml" -f "$rootPath\docker-compose.override.yml" -f "$rootPath\docker-compose.windows.yml"   up
}
