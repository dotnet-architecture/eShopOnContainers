Param (
[parameter(Mandatory=$false)][string]$aksName="",
[parameter(Mandatory=$false)][string]$aksRg=""
)

if ($aksName -and $aksRg) {

    $aks=$(az aks show -n $aksName -g $aksRg -o json | ConvertFrom-Json)
    if (-not $aks) {
        Write-Host "AKS $aksName not found in RG $aksRg" -ForegroundColor Red
        exit 1
    }

    Write-Host "Switching kubectl context to $aksRg/$aksName" -ForegroundColor Yellow
    az aks get-credentials -g $aksRg -n $aksName
}

Write-Host "Installing cert-manager on current cluster"

kubectl apply  --validate=false -f https://github.com/jetstack/cert-manager/releases/download/v0.11.0/cert-manager.yaml --validate=false
