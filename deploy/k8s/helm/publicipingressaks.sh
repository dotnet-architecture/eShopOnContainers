
# 20.63.171.125
PUBLIC_IP_ID=$(az network public-ip list --query "[?ipAddress=='20.63.171.125'].id" -o tsv)
az network dns zone create  --resource-group k8sstudy   --name anniedesign.xyz
az network dns record-set a add-record   --resource-group k8sstudy  --record-set-name eshop  --zone-name anniedesign.xyz  --ipv4-address 1.1.1.1
az network dns record-set a update --name eshop  --resource-group k8sstudy  --zone-name anniedesign.xyz   --target-resource /subscriptions/e25379c9-941e-4fe6-81ff-f0e62becf996/resourceGroups/mc_k8sstudy_eshop_japaneast/providers/Microsoft.Network/publicIPAddresses/kubernetes-ae87c596c80514b95839a76c3ed683df
az network dns zone show  --resource-group k8sstudy  --name anniedesign.xyz  --query nameServers
# ns1-03.azure-dns.com.
# ns2-03.azure-dns.net.
# ns3-03.azure-dns.org.
# ns4-03.azure-dns.info.
# ---------------------------
REGISTRY_NAME=heigoo
CONTROLLER_REGISTRY=k8s.gcr.io
CONTROLLER_IMAGE=ingress-nginx/controller
CONTROLLER_TAG=v0.48.1
PATCH_REGISTRY=docker.io
PATCH_IMAGE=jettech/kube-webhook-certgen
PATCH_TAG=v1.5.1
DEFAULTBACKEND_REGISTRY=k8s.gcr.io
DEFAULTBACKEND_IMAGE=defaultbackend-amd64
DEFAULTBACKEND_TAG=1.5
CERT_MANAGER_REGISTRY=quay.io
CERT_MANAGER_TAG=v1.3.1
CERT_MANAGER_IMAGE_CONTROLLER=jetstack/cert-manager-controller
CERT_MANAGER_IMAGE_WEBHOOK=jetstack/cert-manager-webhook
CERT_MANAGER_IMAGE_CAINJECTOR=jetstack/cert-manager-cainjector

az acr import --name $REGISTRY_NAME --source $CONTROLLER_REGISTRY/$CONTROLLER_IMAGE:$CONTROLLER_TAG --image $CONTROLLER_IMAGE:$CONTROLLER_TAG
az acr import --name $REGISTRY_NAME --source $PATCH_REGISTRY/$PATCH_IMAGE:$PATCH_TAG --image $PATCH_IMAGE:$PATCH_TAG
az acr import --name $REGISTRY_NAME --source $DEFAULTBACKEND_REGISTRY/$DEFAULTBACKEND_IMAGE:$DEFAULTBACKEND_TAG --image $DEFAULTBACKEND_IMAGE:$DEFAULTBACKEND_TAG
az acr import --name $REGISTRY_NAME --source $CERT_MANAGER_REGISTRY/$CERT_MANAGER_IMAGE_CONTROLLER:$CERT_MANAGER_TAG --image $CERT_MANAGER_IMAGE_CONTROLLER:$CERT_MANAGER_TAG
az acr import --name $REGISTRY_NAME --source $CERT_MANAGER_REGISTRY/$CERT_MANAGER_IMAGE_WEBHOOK:$CERT_MANAGER_TAG --image $CERT_MANAGER_IMAGE_WEBHOOK:$CERT_MANAGER_TAG
az acr import --name $REGISTRY_NAME --source $CERT_MANAGER_REGISTRY/$CERT_MANAGER_IMAGE_CAINJECTOR:$CERT_MANAGER_TAG --image $CERT_MANAGER_IMAGE_CAINJECTOR:$CERT_MANAGER_TAG


az aks show --resource-group k8sstudy --name eshop --query nodeResourceGroup -o tsv   --MC_k8sstudy_eShop_japaneast

az network public-ip create --resource-group MC_k8sstudy_eShop_japaneast  --name myAKSPublicIP --sku Standard --allocation-method static --query publicIp.ipAddress -o tsv  --20.194.219.173
-----
# Create a namespace for your ingress resources
kubectl create namespace ingress-basic

# Add the ingress-nginx repository
helm repo add ingress-nginx https://kubernetes.github.io/ingress-nginx

# Set variable for ACR location to use for pulling images
ACR_URL=heigoo.azurecr.io
STATIC_IP=20.194.219.173
DNS_LABEL=eshop

# Use Helm to deploy an NGINX ingress controller



kubectl --namespace ingress-basic get services -o wide -w nginx-ingress-ingress-nginx-controller

az network public-ip list --resource-group MC_myResourceGroup_myAKSCluster_eastus --query "[?name=='myAKSPublicIP'].[dnsSettings.fqdn]" -o tsv


# ===============
az network public-ip create \
    --resource-group k8sstudy \
    --name myAKSPublicIP \
    --sku Standard \
    --allocation-method static

az network public-ip list

az network public-ip show --resource-group k8sstudy --name myAKSPublicIP --query ipAddress --output tsv

az role assignment create \
    --assignee eShop \
    --role "Network Contributor" \
    --scope /subscriptions/e25379c9-941e-4fe6-81ff-f0e62becf996/resourceGroups/k8sstudy

# ==================
az aks create --name myAKSCluster --resource-group myResourceGroup
service principle
az ad sp create-for-rbac --skip-assignment --name myAKSClusterServicePrincipal
Specify a service principal for an AKS cluster
az aks create \
    --resource-group myResourceGroup \
    --name myAKSCluster \
    --service-principal <appId> \
    --client-secret <password>

# Delegate access to other Azure resources

az role assignment create --assignee <appId> --scope <resourceScope> --role Contributor
# ===========================================
# Create a new AKS cluster with ACR integration

# set this to the name of your Azure Container Registry.  It must be globally unique
MYACR=myContainerRegistry

# Run the following line to create an Azure Container Registry if you do not already have one
az acr create -n $MYACR -g myContainerRegistryResourceGroup --sku basic

# Create an AKS cluster with ACR integration
az aks create -n myAKSCluster -g myResourceGroup --generate-ssh-keys --attach-acr $MYACR

az aks create -n myAKSCluster -g myResourceGroup --generate-ssh-keys --attach-acr /subscriptions/<subscription-id>/resourceGroups/myContainerRegistryResourceGroup/providers/Microsoft.ContainerRegistry/registries/myContainerRegistry

# Configure ACR integration for existing AKS clusters
# =====
# =================managed identity==========================
az account show --query id -o tsv
az aks show -g k8sstudy -n eShop --query "servicePrincipalProfile"
# After verifying the cluster is using managed identities, you can find the control plane system-assigned identity's object ID with the following command:
az aks show -g k8sstudy -n eShop --query "identity"

az identity list --query "[].{Name:name, Id:id, Location:location}" -o table


# ==================inital aks==========
az group delete --name myResourceGroup --yes --no-wait   
# ======= acr azure registry============
az aks check-acr --name MyManagedCluster --resource-group MyResourceGroup --acr myacr.azurecr.io
# set this to the name of your Azure Container Registry.  It must be globally unique
MYACR=myContainerRegistry

# Run the following line to create an Azure Container Registry if you do not already have one
az acr create -n $MYACR -g myContainerRegistryResourceGroup --sku basic

# Create an AKS cluster with ACR integration
az aks create -n myAKSCluster -g myResourceGroup --generate-ssh-keys --attach-acr $MYACR
# -----------------


az aks update -n myAKSCluster -g myResourceGroup --attach-acr heigoo
# If you are using an ACR that is located in a different subscription from your AKS cluster, use the ACR resource ID when attaching or detaching from an AKS cluster.
az aks create -n myAKSCluster -g myResourceGroup --generate-ssh-keys --attach-acr /subscriptions/<subscription-id>/resourceGroups/myContainerRegistryResourceGroup/providers/Microsoft.ContainerRegistry/registries/myContainerRegistry

# -----

helm upgrade -i nginx-ingress ingress-nginx/ingress-nginx \
    --version 3.36.0 \
    --namespace ingress-basic \
    --set controller.replicaCount=2 \
    --set controller.nodeSelector."kubernetes\.io/os"=linux \
    --set controller.image.registry=$ACR_URL \
    --set controller.image.image=$CONTROLLER_IMAGE \
    --set controller.image.tag=$CONTROLLER_TAG \
    --set controller.image.digest="" \
    --set controller.admissionWebhooks.patch.nodeSelector."kubernetes\.io/os"=linux \
    --set controller.admissionWebhooks.patch.image.registry=$ACR_URL \
    --set controller.admissionWebhooks.patch.image.image=$PATCH_IMAGE \
    --set controller.admissionWebhooks.patch.image.tag=$PATCH_TAG \
    --set defaultBackend.nodeSelector."kubernetes\.io/os"=linux \
    --set defaultBackend.image.registry=$ACR_URL \
    --set defaultBackend.image.image=$DEFAULTBACKEND_IMAGE \
    --set defaultBackend.image.tag=$DEFAULTBACKEND_TAG


helm upgrade -i nginx-ingress ingress-nginx/ingress-nginx \
    --version 3.36.0 \
    --namespace ingress-basic \
    --set controller.replicaCount=2 \
    --set controller.nodeSelector."kubernetes\.io/os"=linux \
    --set controller.image.registry=$ACR_URL \
    --set controller.image.image=$CONTROLLER_IMAGE \
    --set controller.image.tag=$CONTROLLER_TAG \
    --set controller.image.digest="" \
    --set controller.admissionWebhooks.patch.nodeSelector."kubernetes\.io/os"=linux \
    --set controller.admissionWebhooks.patch.image.registry=$ACR_URL \
    --set controller.admissionWebhooks.patch.image.image=$PATCH_IMAGE \
    --set controller.admissionWebhooks.patch.image.tag=$PATCH_TAG \
    --set defaultBackend.nodeSelector."kubernetes\.io/os"=linux \
    --set defaultBackend.image.registry=$ACR_URL \
    --set defaultBackend.image.image=$DEFAULTBACKEND_IMAGE \
    --set defaultBackend.image.tag=$DEFAULTBACKEND_TAG \
    --set controller.service.loadBalancerIP=$STATIC_IP \
    --set controller.service.annotations."service\.beta\.kubernetes\.io/azure-dns-label-name"=$DNS_LABEL


az network dns record-set a add-record \
    --resource-group myResourceGroup \
    --zone-name anniedesign.xyz \
    --record-set-name "*" \
    --ipv4-address 20.89.163.19
# ============clear resources========================
kubectl delete namespace ingress-basic
kubectl delete -f certificates.yaml
kubectl delete -f cluster-issuer.yaml
helm list --all-namespaces
helm uninstall nginx-ingress cert-manager -n ingress-basic
kubectl delete -f aks-helloworld.yaml --namespace ingress-basic
kubectl delete -f ingress-demo.yaml --namespace ingress-basic
kubectl delete namespace ingress-basic
az network public-ip delete --resource-group MC_myResourceGroup_myAKSCluster_japaneast --name myAKSPublicIP

az group delete --name myResourceGroup --yes --no-wait
