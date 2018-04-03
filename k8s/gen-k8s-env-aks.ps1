Param(
    [parameter(Mandatory=$true)][string]$resourceGroupName,
    [parameter(Mandatory=$true)][string]$location,
    [parameter(Mandatory=$false)][string]$registryName,
    [parameter(Mandatory=$true)][string]$serviceName,
    [parameter(Mandatory=$true)][string]$dnsName,
    [parameter(Mandatory=$true)][string]$createAcr=$true,
    [parameter(Mandatory=$false)][int]$nodeCount=2,
    [parameter(Mandatory=$false)][string]$nodeVMSize="Standard_D2_v2",
	[parameter(Mandatory=$false)][string]$kubernetesVersion="1.7.7"
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
az aks create --resource-group=$resourceGroupName --name=$serviceName --dns-name-prefix=$dnsName --generate-ssh-keys --node-count=$nodeCount --node-vm-size=$nodeVMSize --kubernetes-version $kubernetesVersion

# Retrieve kubernetes cluster configuration and save it under ~/.kube/config 
az aks get-credentials --resource-group=$resourceGroupName --name=$serviceName

if ($createAcr -eq $true) {
    # Show ACR credentials
    az acr credential show -n $registryName
}