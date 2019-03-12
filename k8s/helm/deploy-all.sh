#!/usr/bin/env bash

# http://redsymbol.net/articles/unofficial-bash-strict-mode
set -euo pipefail

usage()
{
  cat <<END
deploy.sh: deploys the $app_name application to a Kubernetes cluster using Helm.
Parameters:
  --aks-name <AKS cluster name>
    The name of the AKS cluster. Required when the registry (using the -r parameter) is set to "aks".
  --aks-rg <AKS resource group>
    The resource group for the AKS cluster. Required when the registry (using the -r parameter) is set to "aks".
  -b | --build-solution
    Force a solution build before deployment (default: false).
  -d | --dns <dns or ip address>
    Specifies the external DNS/ IP address of the Kubernetes cluster. 
    When --use-local-k8s is specified the external DNS is automatically set to localhost.
  -h | --help
    Displays this help text and exits the script.
  -n | --app-name <the name of the app>
    Specifies the name of the application (default: eshop).
  -p | --docker-password <docker password>
    The Docker password used to logon to the custom registry, supplied using the -r parameter.
  -r | --registry <container registry>
    Specifies the container registry to use (required), e.g. myregistry.azurecr.io.
  --skip-clean
    Do not clean the Kubernetes cluster (default is to clean the cluster).
  --skip-image-build
    Do not build images (default is to build all images).
  --skip-image-push
    Do not upload images to the container registry (just run the Kubernetes deployment portion).
    Default is to push the images to the container registry.
  --skip-infrastructure
    Do not deploy infrastructure resources (like sql-data, no-sql or redis).
    This is useful for production environments where infrastructure is hosted outside the Kubernetes cluster.
  -t | --tag <docker image tag>
    The tag used for the newly created docker images. Default: newly created, date-based timestamp, with 1-minute resolution.
  -u | --docker-user <docker username>
    The Docker username used to logon to the custom registry, supplied using the -r parameter.
  --use-local-k8s
    Deploy to a locally installed Kubernetes (default: false).

It is assumed that the Kubernetes cluster has been granted access to the container registry.
If using AKS and ACR see link for more info: 
https://docs.microsoft.com/en-us/azure/container-registry/container-registry-auth-aks

WARNING! THE SCRIPT WILL COMPLETELY DESTROY ALL DEPLOYMENTS AND SERVICES VISIBLE
FROM THE CURRENT CONFIGURATION CONTEXT.
It is recommended that you create a separate namespace and confguration context
for the $app_name application, to isolate it from other applications on the cluster.
For more information see https://kubernetes.io/docs/tasks/administer-cluster/namespaces/
You can use namespace.yaml file (in the same directory) to create the namespace.

END
}

app_name='eshop'
aks_name=''
aks_rg=''
build_images='yes'
clean='yes'
build_solution=''
container_registry=''
docker_password=''
docker_username=''
dns=''
image_tag=$(date '+%Y%m%d%H%M')
push_images='yes'
skip_infrastructure=''
use_local_k8s=''

while [[ $# -gt 0 ]]; do
  case "$1" in
    --aks-name )
      aks_name="$2"; shift 2;;
    --aks-rg )
      aks_rg="$2"; shift 2;;
    -b | --build-solution )
      build_solution='yes'; shift ;;
    -d | --dns )
      dns="$2"; shift 2;;
    -h | --help )
      usage; exit 1 ;;
    -n | --app-name )
      app_name="$2"; shift 2;;
    -p | --docker-password )
      docker_password="$2"; shift;;
    -r | --registry )
      container_registry="$2"; shift 2;;
    --skip-clean )
      clean=''; shift ;;
    --skip-image-build )
      build_images=''; shift ;;
    --skip-image-push )
      push_images=''; shift ;;
    --skip-infrastructure )
      skip_infrastructure='yes'; shift ;;
    -t | --tag )
      image_tag="$2"; shift 2;;  
    -u | --docker-username )
      docker_username="$2"; shift 2;;
    --use-local-k8s )
      use_local_k8s='yes'; shift ;;
    *)
      echo "Unknown option $1"
      usage; exit 2 ;;
  esac
done

if [[ $build_solution ]]; then
  echo "#################### Building $app_name solution ####################"
  dotnet publish -o obj/Docker/publish ../../eShopOnContainers-ServicesAndWebApps.sln
fi

export TAG=$image_tag

if [[ $build_images ]]; then
  echo "#################### Building the $app_name Docker images ####################"
  docker-compose -p ../.. -f ../../docker-compose.yml build

  # Remove temporary images
  docker rmi $(docker images -qf "dangling=true") 
fi

if [[ $push_images ]]; then
  echo "#################### Pushing images to the container registry ####################"
  services=(basket.api catalog.api identity.api ordering.api marketing.api payment.api locations.api webmvc webspa webstatus)

  for service in "${services[@]}"
  do
    echo "Pushing image for service $service..."
    docker tag "eshop/$service:$image_tag" "$container_registry/$service:$image_tag"
    docker push "$container_registry/$service:$image_tag"
  done
fi

ingress_values_file="ingress_values.yaml"

if [[ $use_local_k8s ]]; then
  ingress_values_file="ingress_values_dockerk8s.yaml"
  dns="localhost"
fi

if [[ $dns == "aks" ]]; then
  echo "#################### Begin AKS discovery based on the --dns aks setting. ####################"
  if [[ -z $aks_name ]] || [[ -z $aks_rg ]]; then
    echo "Error: When using -dns aks, MUST set -aksName and -aksRg too."
    echo ''
    usage
    exit 1
  fi

  echo "Getting DNS of AKS of AKS $aks_name (in resource group $aks_rg)"
  dns="$(az aks show -n $aks_name -g $aks_rg --query addonProfiles.httpApplicationRouting.config.HTTPApplicationRoutingZoneName)"
  if [[ -z dns ]]; then
    echo "Error: when getting DNS of AKS $aks_name (in resource group $aks_rg). Please ensure AKS has httpRouting enabled AND Azure CLI is logged in and is of version 2.0.37 or higher."
    exit 1
  fi
  $dns=${dns//[\"]/""}
  echo "DNS base found is $dns. Will use $aks_name.$dns for the app!"
fi

# Initialization & check commands
if [[ -z $dns ]]; then
  echo "No DNS specified. Ingress resources will be bound to public IP."
fi

if [[ $clean ]]; then
  echo "Cleaning previous helm releases..."
  helm delete --purge $(helm ls -q) 
  echo "Previous releases deleted"
fi

use_custom_registry=''

if [[ -n $container_registry ]]; then 
  use_custom_registry='yes'
  if [[ -z $docker_user ]] || [[ -z $docker_password ]]; then
    echo "Error: Must use -u (--docker-username) AND -p (--docker-password) if specifying custom registry"
    exit 1
  fi
fi

echo "#################### Begin $app_name installation using Helm ####################"
infras=(sql-data nosql-data rabbitmq keystore-data basket-data)
charts=(eshop-common apigwmm apigwms apigwwm apigwws basket-api catalog-api identity-api locations-api marketing-api mobileshoppingagg ordering-api ordering-backgroundtasks ordering-signalrhub payment-api webmvc webshoppingagg webspa webstatus webhooks-api webhooks-web)

if [[ !$skip_infrastructure ]]; then
  for infra in "${infras[@]}"
  do
    echo "Installing infrastructure: $infra"
    helm install --values app.yaml --values inf.yaml --values $ingress_values_file --set app.name=$app_name --set inf.k8s.dns=$dns --name="$app_name-$infra" $infra     
  done  
fi

for chart in "${charts[@]}"
do
    echo "Installing: $chart"
    if [[ $use_custom_registry ]]; then 
      helm install --set inf.registry.server=$container_registry --set inf.registry.login=$docker_username --set inf.registry.pwd=$docker_password --set inf.registry.secretName=eshop-docker-scret --values app.yaml --values inf.yaml --values $ingress_values_file --set app.name=$app_name --set inf.k8s.dns=$dns --set image.tag=$image_tag --set image.pullPolicy=Always --name="$app_name-$chart" $chart 
    elif [[ $chart != "eshop-common" ]]; then  # eshop-common is ignored when no secret must be deployed
      helm install --values app.yaml --values inf.yaml --values $ingress_values_file --set app.name=$app_name --set inf.k8s.dns=$dns --set image.tag=$image_tag --set image.pullPolicy=Always --name="$app_name-$chart" $chart 
    fi
done

echo "FINISHED: Helm charts installed."
