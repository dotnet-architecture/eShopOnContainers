Param(
    [parameter(Mandatory=$true)][string]$resourceGroupName,
    [parameter(Mandatory=$true)][string]$location,
    [parameter(Mandatory=$false)][string]$registryName,
    [parameter(Mandatory=$true)][string]$orchestratorName,
    [parameter(Mandatory=$true)][string]$dnsName,
    [parameter(Mandatory=$true)][string]$createAcr=$true,
    [parameter(Mandatory=$false)][int]$agentCount=2,
    [parameter(Mandatory=$false)][string]$agentVMSize="Standard_D2_v2",
    [parameter(Mandatory=$false)][int]$masterCount=1
)

# Create resource group
Write-Host "Creating resource group..." -ForegroundColor Yellow
az group create --name=$resourceGroupName --location=$location

if ($createAcr -eq $true) {
    # Create Azure Container Registry
    Write-Host "Creating Azure Container Registry..." -ForegroundColor Yellow
    az acr create -n $registryName -g $resourceGroupName -l $location  --admin-enabled true --sku Basic
}

# Create kubernetes orchestrator
Write-Host "Creating kubernetes orchestrator..." -ForegroundColor Yellow
az acs create --orchestrator-type=kubernetes --resource-group $resourceGroupName --name=$orchestratorName --dns-prefix=$dnsName --generate-ssh-keys --agent-count=$agentCount --agent-vm-size=$agentVMSize --master-count=$masterCount

# Retrieve kubernetes cluster configuration and save it under ~/.kube/config 
az acs kubernetes get-credentials --resource-group=$resourceGroupName --name=$orchestratorName

if ($createAcr -eq $true) {
    # Show ACR credentials
    az acr credential show -n $registryName
}