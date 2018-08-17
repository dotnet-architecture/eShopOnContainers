Param(
    [Parameter(Mandatory=$true)][string]$agentsFqdn,
    [Parameter(Mandatory=$true)][string]$registry
)

(Get-Content template.json) |
    Foreach-Object {
        $_.Replace("AGENTS_FQDN", $agentsFqdn).Replace("REGISTRY", $registry)
    } | Set-Content eShopOnContainers.json