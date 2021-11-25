# login in Ubuntu
az login az login --use-device-code /
az login --tenant 429950a6-2916-4b6f-8bd1-09b5071951d4
#Create a resource group
resourceGroup=DL-LEARNING-RG
az group create --name $resourceGroup --location southeastasia  #/////canadacentral

#delete resouce group 
az group delete --name $resourceGroup

# Vnet 
#resourceGroup='DL-LEARNING-RG'
subscription='909efc0a-aa87-4bd2-884c-c93b75692357'
vnetName='aks-vnet-eshop'

az network vnet create -g $resourceGroup --subscription $subscription -n $vnetName  -l southeastasia --address-prefix 10.10.0.0/17 --subnet-name eshopsubnet --subnet-prefix 10.10.0.0/18

subnetId=$(az network vnet subnet show --resource-group $resourceGroup --subscription $subscription --vnet-name $vnetName --name eshopsubnet --query id -o tsv)

# Create a private container registry
#######################################################################################
# Create a resource group for acr
acrrg=DL-PRIVATE-RG 
az group create --name $acrrg --location southeastasia ///eastus 
# Create a container registry
az acr create --resource-group $acrrg \
  --name heigoo --sku Basic                     ###// Standard Premium

#Log in to registry
 az acr login --name heigoo #geCqSifODg7Zs8KCni//P/f295oI8uUr

 #Push image to registry
 docker pull mcr.microsoft.com/hello-world
 docker tag mcr.microsoft.com/hello-world heigoo.azurecr.io/hello-world:v1
 docker push heigoo.azurecr.io/hello-world:v1
 docker rmi heigoo.azurecr.io/hello-world:v1

 #List container images
 az acr repository list --name heigoo --output table
 az acr repository show-tags --name heigoo --repository hello-world --output table

 #Run image from registry
 docker run heigoo.azurecr.io/hello-world:v1
#Clean up resources
 az group delete --name DL-PRIVATE-RG

# crete aks Cluster
clusterName='eShop'
acr=$(az acr show --name heigoo --resource-group $acrrg --query "id" --output tsv)

az aks create -n $clusterName --resource-group $resourceGroup --subscription $subscription --kubernetes-version 1.21.1 --network-plugin azure --enable-managed-identity  --generate-ssh-keys --attach-acr $acr --node-count 2  --vnet-subnet-id $subnetId


## if acr already created
az aks update --name myAKSCluster --resource-group myResourceGroup --subscription mySubscription --attach-acr <acr-resource-id>
az acr show --name acrName --resource-group myResourceGroup --subscription mySubscription --query "id"


## deploy
az account set --subscription 909efc0a-aa87-4bd2-884c-c93b75692357
az aks get-credentials --resource-group DL-LEARNING-RG --name eShop
kubectl get all -n cert-manager -o wide




# install ingress-nginx
#cd D:\temp\microservice\eShopOnContainers\deploy\k8s\nginx-ingress
kubectl apply -f mandatory.yaml
kubectl apply -f local-cm.yaml #(add large-client-header-buffers: "4 16k")
kubectl apply -f local-svc.yaml


#cd D:\temp\microservice\eShopOnContainers\deploy\k8s\helm

#.\deploy-all.ps1 -externalDns aks -aksName eShop -aksRg DL-LEARNING-RG -imageTag linux-latest -registry heigoo.azurecr.io -dockerUser heigoo -dockerPassword tuQbbDDaFxYPV6NMBpEylhw -useMesh $false

.\deploy-all.ps1 -externalDns eshop.anniedesign.xyz -imageTag linux-latest -registry heigoo.azurecr.io -dockerUser heigoo -dockerPassword geCqSifODg7Zs8KCni//P/f295oI8uUr -useMesh $false -sslSupport staging
.\deploy-all.ps1 -externalDns eshop.anniedesign.xyz -imageTag linux-latest -registry heigoo.azurecr.io -dockerUser heigoo -dockerPassword geCqSifODg7Zs8KCni//P/f295oI8uUr -useMesh $false -sslSupport prod
#.\deploy-all.ps1 -externalDns eshop.anniedesign.xyz -aksName eShop -aksRg DL-LEARNING-RG -imageTag linux-dev -useMesh $false

# enable tls-support 
# https://github.com/dotnet-architecture/eShopOnContainers/wiki/AKS-TLS
# cd D:\temp\microservice\eShopOnContainers\deploy\k8s
#run .\enable-tls.ps1
# rename values-staging.yaml(values-prod.yaml) to values.yaml() and ingressClass to nginx
# cd D:\temp\microservice\eShopOnContainers\deploy\k8s\helm

#kubectl apply -f cert-manager.yaml(if no running .\enable-tls.ps1)
helm install eshop-tls-support tls-support
kubectl get issuer
kubectl get cert -o wide
helm uninstall eshop-tls-support #(change server and environment to pord server ) redeploy

# check deploy status
kubectl get deployment
kubectl get ingress #check external IP to bind it on Godaddy (or other DNS provider) with the DNS name
kubectl get cert # check certificate

kubectl get certificaterequest
kubectl get order 
kubectl get challenges

kubectl get Issuers,ClusterIssuers,Certificates,CertificateRequests,Orders,Challenges --all-namespaces

# CD D:\temp\microservice\eShopOnContainers\deploy\k8s\nodeports to change sql-service.yaml from NodePort to LoadBalancer
kubectl apply -f sql-service1.yaml
#get db external IP(lb) to connect to DB to change all http to https (eg.  20.44.192.98:1433 sa/Pass@word)

# update clients set ClientUri= replace(clientUri,'http://eshop.','https://eshop.')
# update ClientRedirectUris set RedirectUri = replace(RedirectUri,'http://eshop.','https://eshop.') where clientid <>3
# update ClientPostLogoutRedirectUris set PostLogoutRedirectUri = replace(PostLogoutRedirectUri,'http://eshop.','https://eshop.') where clientid <>3
# webmvc unauthorized client issue (change back RedirectUri to http for temporary usage)
##uninstall

helm uninstall $(helm ls --filter eshop -q) --dry-run

# https://github.com/dotnet-architecture/eShopOnContainers/issues/1513
#azure devops pipeline
# https://github.com/dotnet-architecture/eShopOnContainers/tree/main/build/azure-devops
#https://docs.microsoft.com/en-us/azure/devops/pipelines/library/service-endpoints?view=azure-devops&tabs=yaml
#  Project settings > Service connections.
# Select + New service connection, select the type of service connection that you need, and then select Next.
# create a new docker Registry service heigooRegistry with other docker registry option
# using service principal id and password username/password: 7a304605-08fa-47e2-adea-49d529dcabc4/kv59J2RHiknv-v_uLzQIj37_zHjvX4QgZc
































####service principle
#!/bin/bash
# This script requires Azure CLI version 2.25.0 or later. Check version with `az --version`.

# Modify for your environment.
# ACR_NAME: The name of your Azure Container Registry
# SERVICE_PRINCIPAL_NAME: Must be unique within your AD tenant
ACR_NAME=heigoo
SERVICE_PRINCIPAL_NAME=acr-service-principal

# Obtain the full registry ID for subsequent command args
ACR_REGISTRY_ID=$(az acr show --name $ACR_NAME --query "id" --output tsv)

# Create the service principal with rights scoped to the registry.
# Default permissions are for docker pull access. Modify the '--role'
# argument value as desired:
# acrpull:     pull only
# acrpush:     push and pull
# owner:       push, pull, and assign roles
PASSWORD=$(az ad sp create-for-rbac --name $SERVICE_PRINCIPAL_NAME --scopes $ACR_REGISTRY_ID --role acrpush --query "password" --output tsv)
USER_NAME=$(az ad sp list --display-name $SERVICE_PRINCIPAL_NAME --query "[].appId" --output tsv)

# Output the service principal's credentials; use these in your services and
# applications to authenticate to the container registry.
echo "Service principal ID: $USER_NAME"
echo "Service principal password: $PASSWORD"



#############################################################################################
 ## This creates a working single node Azure Kubernetes Cluster
## and with an Azure Container Registry. Note, the ACR is in 
## the same resource group as the AKS for demo purposes. For
## dev you should have ACR in separate resource group.

echo "Beginning AKS Setup for Demo"
date

AKS_RESOURCE_GROUP=aks-rg1
AKS_CLUSTER_NAME=aks-c1
ACR_RESOURCE_GROUP=MC_aks-rg1_aks-c1_centralus 
ACR_NAME=aksacr122
SERVICE_PRINCIPAL_NAME=aks-sp-user
RG_LOCATION=CentralUS
DOCKER_USERNAME=$ACR_NAME
DOCKER_EMAIL={provide email address here}  #does not have to be an account with docker hub
#DOCKER_PASSWORD is applied a value later

az group create --location $RG_LOCATION --name $AKS_RESOURCE_GROUP

az aks create -g $AKS_RESOURCE_GROUP -n $AKS_CLUSTER_NAME --generate-ssh-keys --node-count 1 --node-vm-size Standard_F1s

az acr create --resource-group $ACR_RESOURCE_GROUP --name $ACR_NAME --sku Basic --admin-enabled true


CLIENT_ID=$(az aks show --resource-group $AKS_RESOURCE_GROUP --name $AKS_CLUSTER_NAME --query "servicePrincipalProfile.clientId" --output tsv)

# Get the ACR registry resource id
ACR_ID=$(az acr show --name $ACR_NAME --resource-group $ACR_RESOURCE_GROUP --query "id" --output tsv)

# Create role assignment
az role assignment create --assignee $CLIENT_ID --role Reader --scope $ACR_ID

# Populate the ACR login server and resource id.
ACR_LOGIN_SERVER=$(az acr show --name $ACR_NAME --query loginServer --output tsv)
ACR_REGISTRY_ID=$(az acr show --name $ACR_NAME --query id --output tsv)

# Create a contributor role assignment with a scope of the ACR resource.
SP_PASSWD=$(az ad sp create-for-rbac --name $SERVICE_PRINCIPAL_NAME --role Reader --scopes $ACR_REGISTRY_ID --query password --output tsv)

# Get the service principle client id.
CLIENT_ID=$(az ad sp show --id http://$SERVICE_PRINCIPAL_NAME --query appId --output tsv)

# Output used when creating Kubernetes secret.
echo "Service principal ID: $CLIENT_ID"
echo "Service principal password: $SP_PASSWD"

#connect to the aks environment
az aks get-credentials --resource-group $AKS_RESOURCE_GROUP --name $AKS_CLUSTER_NAME

ACR_HTTPS_LOGIN_SERVER="https://$ACR_LOGIN_SERVER"

### get password from ACR
DOCKER_PASSWORD=$(az acr credential show -n $ACR_NAME --query passwords[0].value -o tsv)
kubectl create secret docker-registry acrconnection --docker-server=$ACR_HTTPS_LOGIN_SERVER --docker-username=$DOCKER_USERNAME --docker-password=$DOCKER_PASSWORD --docker-email=$DOCKER_EMAIL

az acr login --name $ACR_NAME

echo "Completed AKS Setup"
date
#######################
# Post the following JSON payload to the endpoint, sending a valid Basic Access Token
# https://dev.azure.com/{organization}/{project}/_apis/serviceendpoint/endpoints?api-version=5.1-preview.2

{
    "authorization": {
        "scheme": "ServicePrincipal",
        "parameters": {
            "loginServer": "<ACRSERVER>.azurecr.io",
            "servicePrincipalId": "<APPLICATIONid OF SPN>",
            "tenantId": "<TENANTID>",
            "serviceprincipalkey": "<SPN kEY>"
        }
    },
    "description": "",
    "name": "Name of Connection",
    "type": "dockerregistry",
    "url": "https://<ACRSERVER>.azurecr.io",
    "isShared": false,
    "owner": "library",
    "data": {
        "registryId": "/subscriptions/<SUBSCRIPTIONID>/resourceGroups/<RESOURCEGROUP>/providers/Microsoft.ContainerRegistry/registries/<ACRSERVER>",
        "registrytype": "ACR",
        "spnObjectId": "",
        "subscriptionId": "<SUBSCRIPTIONID>",
        "subscriptionName": "<SUBSCRIPTIONNAME>"
    }
}