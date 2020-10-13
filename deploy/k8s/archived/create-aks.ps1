Param(
    [parameter(Mandatory=$true)][string]$resourceGroupName,
    [parameter(Mandatory=$true)][string]$location,
    [parameter(Mandatory=$true)][string]$serviceName,
    [parameter(Mandatory=$true)][string]$dnsNamePrefix,
    [parameter(Mandatory=$false)][string]$registryName,
    [parameter(Mandatory=$true)][bool]$createAcr=$true,
    [parameter(Mandatory=$false)][int]$nodeCount=3,
    [parameter(Mandatory=$false)][string]$nodeVMSize="Standard_D2_v2",
    [parameter(Mandatory=$false)][bool]$enableHttpApplicationAddon=$true,
    [parameter(Mandatory=$false)][bool]$enableAzureMonitoring=$false,
    [parameter(Mandatory=$false)][ValidateSet("VirtualMachineScaleSets","AvailabilitySet",IgnoreCase=$true)]$vmSetType="VirtualMachineScaleSets"
)

# Create resource group
Write-Host "Creating Azure Resource Group..." -ForegroundColor Yellow
az group create --name=$resourceGroupName --location=$location

if ($createAcr -eq $true) {
    # Create Azure Container Registry
    if ([string]::IsNullOrEmpty($registryName)) {
        $registryName=$serviceName
    }
    Write-Host "Creating Azure Container Registry named $registryName" -ForegroundColor Yellow
    az acr create -n $registryName -g $resourceGroupName -l $location  --admin-enabled true --sku Basic
}

# Create kubernetes cluster in AKS
Write-Host "Creating AKS $resourceGroupName/$serviceName" -ForegroundColor Yellow
az aks create --resource-group=$resourceGroupName --name=$serviceName --dns-name-prefix=$dnsNamePrefix --generate-ssh-keys --node-count=$nodeCount --node-vm-size=$nodeVMSize --vm-set-type $vmSetType

if ($enableHttpApplicationAddon) {
    Write-Host "Enabling Http Applciation Routing in AKS $serviceName" -ForegroundColor Yellow
    az aks enable-addons --resource-group $resourceGroupName --name $serviceName --addons http_application_routing
}

if ($enableAzureMonitoring) {
    Write-Host "Enabling Azure Monitoring in AKS $serviceName" -ForegroundColor Yellow
    az aks enable-addons --resource-group $resourceGroupName --name $serviceName --addons monitoring
}

# Retrieve kubernetes cluster configuration and save it under ~/.kube/config
Write-Host "Getting Kubernetes config..." -ForegroundColor Yellow
az aks get-credentials --resource-group=$resourceGroupName --name=$serviceName

if ($createAcr -eq $true) {
    # Show ACR credentials
    Write-Host "ACR $registryName credentials:" -ForegroundColor Yellow
    az acr credential show -n $registryName
}
