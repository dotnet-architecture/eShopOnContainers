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
  -c | --chart <name of chart>
    The name of the chart to upgrade (or install)
  -d | --dns <dns or ip address> | --dns aks
    Specifies the external DNS/ IP address of the Kubernetes cluster.
    If 'aks' is set as value, the DNS value is retrieved from the AKS. --aks-name and --aks-rg are needed.
    When --use-local-k8s is specified the external DNS is automatically set to localhost.
  -h | --help
    Displays this help text and exits the script.
  -n | --app-name <the name of the app>
    Specifies the name of the application (default: eshop).
  --namespace <namespace name>
    Specifies the namespace name to deploy the app. If it doesn't exists it will be created (default: eshop).
  -p | --docker-password <docker password>
    The Docker password used to logon to the custom registry, supplied using the -r parameter.
  -r | --registry <container registry>
    Specifies the container registry to use (required), e.g. myregistry.azurecr.io.
  --skip-clean
    Do not clean the Kubernetes helm chart. Default is to clean the chart.
  -t | --tag <docker image tag>
    The tag used for the newly created docker images. Default: latest.
  -u | --docker-username <docker username>
    The Docker username used to logon to the custom registry, supplied using the -r parameter.
  --use-local-k8s
    Deploy to a locally installed Kubernetes (default: false).

It is assumed that the Kubernetes cluster has been granted access to the container registry.
If using AKS and ACR see link for more info: 
https://docs.microsoft.com/en-us/azure/container-registry/container-registry-auth-aks

WARNING! THE SCRIPT WILL COMPLETELY DESTROY ALL DEPLOYMENTS AND SERVICES VISIBLE
FROM THE CURRENT CONFIGURATION CONTEXT AND NAMESPACE.
It is recommended that you check your selected namespace, 'eshop' by default, is already in use.
Every deployment and service done in the namespace will be deleted.
For more information see https://kubernetes.io/docs/tasks/administer-cluster/namespaces/

END
}

acr_connected=''
app_name='eshop'
aks_name=''
aks_rg=''
chart=''
clean='yes'
container_registry=''
docker_password=''
docker_username=''
dns=''
image_tag='latest'
skip_infrastructure=''
use_local_k8s=''
namespace='eshop'

while [[ $# -gt 0 ]]; do
  case "$1" in
    --acr-connected )
      acr_connected='yes'; shift ;;
    --aks-name )
      aks_name="$2"; shift 2;;
    --aks-rg )
      aks_rg="$2"; shift 2;;
    -c | --chart )
      chart="$2"; shift 2;;
    -d | --dns )
      dns="$2"; shift 2;;
    -h | --help )
      usage; exit 1 ;;
    -n | --app-name )
      app_name="$2"; shift 2;;
    -p | --docker-password )
      docker_password="$2"; shift 2;;
    -r | --registry )
      container_registry="$2"; shift 2;;
    --skip-clean )
      clean=''; shift ;;
    --image-build )
      build_images='yes'; shift ;;
    --image-push )
      push_images='yes'; shift ;;
    --skip-infrastructure )
      skip_infrastructure='yes'; shift ;;
    -t | --tag )
      image_tag="$2"; shift 2;;  
    -u | --docker-username )
      docker_username="$2"; shift 2;;
    --use-local-k8s )
      use_local_k8s='yes'; shift ;;
    --namespace )
      namespace="$2"; shift 2;;
    *)
      echo "Unknown option $1"
      usage; exit 2 ;;
  esac
done

export TAG=$image_tag

use_custom_registry=''

if [[ -n $container_registry ]] && [[ -z $acr_connected ]]; then 
  echo "################ Log into custom registry $container_registry ##################"
  use_custom_registry='yes'
  if [[ -z $docker_username ]] || [[ -z $docker_password ]]; then
    echo "Error: Must use -u (--docker-username) AND -p (--docker-password) if specifying custom registry"
    exit 1
  fi
  docker login -u $docker_username -p $docker_password $container_registry
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

  echo "Getting AKS cluster $aks_name  AKS (in resource group $aks_rg)"
  # JMESPath queries are case sensitive and httpapplicationrouting can be lowercase sometimes
  jmespath_dnsqueries=(\
    addonProfiles.httpApplicationRouting.config.HTTPApplicationRoutingZoneName \
    addonProfiles.httpapplicationrouting.config.HTTPApplicationRoutingZoneName \
  )
  for q in "${jmespath_dnsqueries[@]}"
  do
    dns="$(az aks show -n $aks_name -g $aks_rg --query $q -o tsv)"
    if [[ -n $dns ]]; then break; fi
  done
  if [[ -z $dns ]]; then
    echo "Error: when getting DNS of AKS $aks_name (in resource group $aks_rg). Please ensure AKS has httpRouting enabled AND Azure CLI is logged in and is of version 2.0.37 or higher."
    exit 1
  fi
  echo "DNS base found is $dns. Will use $aks_name.$dns for the app!"
  dns="$aks_name.$dns"
fi

# Initialization & check commands
if [[ -z $dns ]]; then
  echo "No DNS specified. Ingress resources will be bound to public IP."
fi

previous_install=''
if [[ -z $(helm ls -q --namespace $namespace | grep "$app_name-$chart") ]]; then
  echo "No previous release found"
else
  previous_install='yes'
fi
  
if [[ $clean ]] && [[ $previous_install ]]; then
  echo "Cleaning previous helm releases..."
  helm uninstall "$app_name-$chart" --namespace $namespace
  echo "Previous release deleted"
  waitsecs=5; while [ $waitsecs -gt 0 ]; do echo -ne "$waitsecs\033[0K\r"; sleep 1; : $((waitsecs--)); done
  previous_install=''
fi

echo "#################### Begin $app_name $chart installation using Helm ####################"
if [[ $use_custom_registry ]] || [[ $acr_connected ]]; then
  if [[ -z $acr_connected ]]; then
    if [[ -z $previous_install ]]; then
      helm upgrade --install "$app_name-$chart" --namespace $namespace --set "ingress.hosts={$dns}" --set inf.registry.server=$container_registry --set inf.registry.login=$docker_username --set inf.registry.pwd=$docker_password --set inf.registry.secretName=eshop-docker-scret --values app.yaml --values inf.yaml --values $ingress_values_file --set app.name=$app_name --set inf.k8s.dns=$dns --set image.tag=$image_tag --set image.pullPolicy=Always $chart 
    else
      helm upgrade --install "$app_name-$chart" --namespace $namespace --set "ingress.hosts={$dns}" --values app.yaml --values inf.yaml --values $ingress_values_file --set app.name=$app_name --set inf.k8s.dns=$dns --set image.tag=$image_tag --set image.pullPolicy=Always $chart 
    fi
  elif [[ $chart != "eshop-common" ]]; then
    # ACR is already connected, so we don't need username/password
    if [[ -z $previous_install ]]; then
      helm install "$app_name-$chart" --namespace $namespace --set "ingress.hosts={$dns}" --set inf.registry.server=$container_registry --values app.yaml --values inf.yaml --values $ingress_values_file --set app.name=$app_name --set inf.k8s.dns=$dns --set image.tag=$image_tag --set image.pullPolicy=Always $chart 
    else
      # don't set the image repo since it's already set
      helm upgrade "$app_name-$chart" --namespace $namespace --set "ingress.hosts={$dns}" --values app.yaml --values inf.yaml --values $ingress_values_file --set app.name=$app_name --set inf.k8s.dns=$dns --set image.tag=$image_tag --set image.pullPolicy=Always $chart 
    fi
  fi
elif [[ $chart != "eshop-common" ]]; then  # eshop-common is ignored when no secret must be deployed
  helm upgrade --install "$app_name-$chart" --namespace $namespace --set "ingress.hosts={$dns}" --values app.yaml --values inf.yaml --values $ingress_values_file --set app.name=$app_name --set inf.k8s.dns=$dns --set image.tag=$image_tag --set image.pullPolicy=Always $chart 
fi
echo "FINISHED: Helm chart installed."