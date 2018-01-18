#!/usr/bin/env bash

# http://redsymbol.net/articles/unofficial-bash-strict-mode/
set -euo pipefail

# This script is comparable to the PowerShell script deploy.ps1 but to be used from a Mac bash environment.
# There are, however, the following few differences/limitations:
 
# It assumes docker/container registry login was already performed
# It assumes K8s was given access to the registry—does not create any K8s secrets
# It does not support explicit kubectl config file (relies on kubectl config use-context to point kubectl at the right cluster/namespace)
# It always deploys infrastructure bits (redis, SQL Server etc)
# The script was tested only with Azure Container Registry (not Docker Hub, although it is expected to work with Docker Hub too)
 
# Feel free to submit a PR in order to improve it.

usage()
{
    cat <<END
deploy.sh: deploys eShopOnContainers application to Kubernetes cluster
Parameters:
  -r | --registry <container registry> 
    Specifies container registry (ACR) to use (required), e.g. myregistry.azurecr.io
  -t | --tag <docker image tag> 
    Default: newly created, date-based timestamp, with 1-minute resolution
  -b | --build-solution
    Force solution build before deployment (default: false)
  --skip-image-build
    Do not build images (default is to build all images)
  --skip-image-push
    Do not upload images to the container registry (just run the Kubernetes deployment portion)
    Default is to push images to container registry
  -h | --help
    Displays this help text and exits the script

It is assumed that the Kubernetes AKS cluster has been granted access to ACR registry.
For more info see 
https://docs.microsoft.com/en-us/azure/container-registry/container-registry-auth-aks

WARNING! THE SCRIPT WILL COMPLETELY DESTROY ALL DEPLOYMENTS AND SERVICES VISIBLE
FROM THE CURRENT CONFIGURATION CONTEXT.
It is recommended that you create a separate namespace and confguration context
for the eShopOnContainers application, to isolate it from other applications on the cluster.
For more information see https://kubernetes.io/docs/tasks/administer-cluster/namespaces/
You can use eshop-namespace.yaml file (in the same directory) to create the namespace.

END
}

image_tag=$(date '+%Y%m%d%H%M')
build_solution=''
container_registry=''
build_images='yes'
push_images='yes'

while [[ $# -gt 0 ]]; do
  case "$1" in
    -r | --registry )
        container_registry="$2"; shift 2 ;;
    -t | --tag )
        image_tag="$2"; shift 2 ;;
    -b | --build-solution )
        build_solution='yes'; shift ;;
    --skip-image-build )
        build_images=''; shift ;;
    --skip-image-push )
        push_images=''; shift ;;
    -h | --help )
        usage; exit 1 ;;
    *)
        echo "Unknown option $1"
        usage; exit 2 ;;
  esac
done

if [[ ! $container_registry ]]; then
    echo 'Container registry must be specified (e.g. myregistry.azurecr.io)'
    echo ''
    usage
    exit 3
fi

if [[ $build_solution ]]; then
    echo "#################### Building eShopOnContainers solution ####################"
    dotnet publish -o obj/Docker/publish ../eShopOnContainers-ServicesAndWebApps.sln
fi

export TAG=$image_tag

if [[ $build_images ]]; then
    echo "#################### Building eShopOnContainers Docker images ####################"
    docker-compose -p .. -f ../docker-compose.yml build

    # Remove temporary images
    docker rmi $(docker images -qf "dangling=true")
fi

if [[ $push_images ]]; then
    echo "#################### Pushing images to registry ####################"
    services=(basket.api catalog.api identity.api ordering.api marketing.api payment.api locations.api webmvc webspa webstatus)

    for service in "${services[@]}"
    do
        echo "Pushing image for service $service..."
        docker tag "eshop/$service:$image_tag" "$container_registry/$service:$image_tag"
        docker push "$container_registry/$service:$image_tag"
    done
fi

echo "#################### Cleaning up old deployment ####################"
kubectl delete deployments --all
kubectl delete services --all
kubectl delete configmap config-files || true
kubectl delete configmap urls || true
kubectl delete configmap externalcfg || true

echo "#################### Deploying infrastructure components ####################"
kubectl create configmap config-files --from-file=nginx-conf=nginx.conf
kubectl label configmap config-files app=eshop
kubectl create -f sql-data.yaml -f basket-data.yaml -f keystore-data.yaml -f rabbitmq.yaml -f nosql-data.yaml

echo "#################### Creating application service definitions ####################"
kubectl create -f services.yaml -f frontend.yaml

echo "#################### Waiting for Azure to provision external IP ####################"

ip_regex='([0-9]{1,3}\.){3}[0-9]{1,3}'
while true; do
    printf "."
    frontendUrl=$(kubectl get svc frontend -o=jsonpath="{.status.loadBalancer.ingress[0].ip}")
    if [[ $frontendUrl =~ $ip_regex ]]; then
        break
    fi
    sleep 5s
done

printf "\n"
externalDns=$frontendUrl
echo "Using $externalDns as the external DNS/IP of the K8s cluster"

echo "#################### Creating application configuration ####################"

# urls configmap
kubectl create configmap urls \
    "--from-literal=BasketUrl=http://basket" \
    "--from-literal=BasketHealthCheckUrl=http://basket/hc" \
    "--from-literal=CatalogUrl=http://$externalDns/catalog-api" \
    "--from-literal=CatalogHealthCheckUrl=http://catalog/hc" \
    "--from-literal=PicBaseUrl=http://$externalDns/catalog-api/api/v1/catalog/items/[0]/pic/" \
    "--from-literal=Marketing_PicBaseUrl=http://$externalDns/marketing-api/api/v1/campaigns/[0]/pic/" \
    "--from-literal=IdentityUrl=http://$externalDns/identity" \
    "--from-literal=IdentityHealthCheckUrl=http://identity/hc" \
    "--from-literal=OrderingUrl=http://ordering" \
    "--from-literal=OrderingHealthCheckUrl=http://ordering/hc" \
    "--from-literal=MvcClientExternalUrl=http://$externalDns/webmvc" \
    "--from-literal=WebMvcHealthCheckUrl=http://webmvc/hc" \
    "--from-literal=MvcClientOrderingUrl=http://ordering" \
    "--from-literal=MvcClientCatalogUrl=http://catalog" \
    "--from-literal=MvcClientBasketUrl=http://basket" \
    "--from-literal=MvcClientMarketingUrl=http://marketing" \
    "--from-literal=MvcClientLocationsUrl=http://locations" \
    "--from-literal=MarketingHealthCheckUrl=http://marketing/hc" \
    "--from-literal=WebSpaHealthCheckUrl=http://webspa/hc" \
    "--from-literal=SpaClientMarketingExternalUrl=http://$externalDns/marketing-api" \
    "--from-literal=SpaClientOrderingExternalUrl=http://$externalDns/ordering-api" \
    "--from-literal=SpaClientCatalogExternalUrl=http://$externalDns/catalog-api" \
    "--from-literal=SpaClientBasketExternalUrl=http://$externalDns/basket-api" \
    "--from-literal=SpaClientIdentityExternalUrl=http://$externalDns/identity" \
    "--from-literal=SpaClientLocationsUrl=http://$externalDns/locations-api" \
    "--from-literal=LocationsHealthCheckUrl=http://locations/hc" \
    "--from-literal=SpaClientExternalUrl=http://$externalDns" \
    "--from-literal=LocationApiClient=http://$externalDns/locations-api" \
    "--from-literal=MarketingApiClient=http://$externalDns/marketing-api" \
    "--from-literal=BasketApiClient=http://$externalDns/basket-api" \
    "--from-literal=OrderingApiClient=http://$externalDns/ordering-api" \
    "--from-literal=PaymentHealthCheckUrl=http://payment/hc"

kubectl label configmap urls app=eshop

# externalcfg configmap -- points to local infrastructure components (rabbitmq, SQL Server etc)
kubectl create -f conf_local.yml

# Create application pod deployments
kubectl create -f deployments.yaml

echo "#################### Deploying application pods ####################"

# update deployments with the correct image (with tag and/or registry)
kubectl set image deployments/basket "basket=$container_registry/basket.api:$image_tag"
kubectl set image deployments/catalog "catalog=$container_registry/catalog.api:$image_tag"
kubectl set image deployments/identity "identity=$container_registry/identity.api:$image_tag"
kubectl set image deployments/ordering "ordering=$container_registry/ordering.api:$image_tag"
kubectl set image deployments/marketing "marketing=$container_registry/marketing.api:$image_tag"
kubectl set image deployments/locations "locations=$container_registry/locations.api:$image_tag"
kubectl set image deployments/payment "payment=$container_registry/payment.api:$image_tag"
kubectl set image deployments/webmvc "webmvc=$container_registry/webmvc:$image_tag"
kubectl set image deployments/webstatus "webstatus=$container_registry/webstatus:$image_tag"
kubectl set image deployments/webspa "webspa=$container_registry/webspa:$image_tag"

kubectl rollout resume deployments/basket
kubectl rollout resume deployments/catalog
kubectl rollout resume deployments/identity
kubectl rollout resume deployments/ordering
kubectl rollout resume deployments/marketing
kubectl rollout resume deployments/locations
kubectl rollout resume deployments/payment
kubectl rollout resume deployments/webmvc
kubectl rollout resume deployments/webstatus
kubectl rollout resume deployments/webspa

echo "WebSPA is exposed at http://$externalDns, WebMVC at http://$externalDns/webmvc, WebStatus at http://$externalDns/webstatus"
echo "eShopOnContainers deployment is DONE"
