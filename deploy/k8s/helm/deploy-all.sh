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
  -d | --dns <dns or ip address> | --dns aks
    Specifies the external DNS/ IP address of the Kubernetes cluster.
    If 'aks' is set as value, the DNS value is retrieved from the AKS. --aks-name and --aks-rg are needed.
    When --use-local-k8s is specified the external DNS is automatically set to localhost.
  -h | --help
    Displays this help text and exits the script.
  --image-build
    Build images (default is to not build all images).
  --image-push
    Upload images to the container registry (default is not pushing to the custom registry)
  -n | --app-name <the name of the app>
    Specifies the name of the application (default: eshop).
  --namespace <namespace name>
    Specifies the namespace name to deploy the app. If it doesn't exists it will be created (default: eshop).
  -p | --docker-password <docker password>
    The Docker password used to logon to the custom registry, supplied using the -r parameter.
  -r | --registry <container registry>
    Specifies the container registry to use (required), e.g. myregistry.azurecr.io.
  --skip-clean
    Do not clean the Kubernetes cluster (default is to clean the cluster).
  --skip-infrastructure
    Do not deploy infrastructure resources (like sql-data, no-sql or redis).
    This is useful for production environments where infrastructure is hosted outside the Kubernetes cluster.
  -t | --tag <docker image tag>
    The tag used for the newly created docker images. Default: latest.
  -u | --docker-username <docker username>
    The Docker username used to logon to the custom registry, supplied using the -r parameter.
  --use-local-k8s
    Deploy to a locally installed Kubernetes (default: false).
  --use-linkerd
    Use Linkerd as service mesh
  --use-istio
    Use Istio as service mesh
  --ingress-mesh-annotations-file )
    e.g. ingress_values_istio.yaml
  --image-pull-policy <policy>
    Image Pull Policy: Always, IfNotPresent, Never (default: Always)
  --ssl-enabled
    Enable SSL for the application.
  --ssl-support <ssl support>
    SSL support: prod, staging, custom, none (default) 
  --tls-secret-name <secret name>
    The name of the ssl cert.

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

app_name='eshop'
aks_name=''
aks_rg=''
build_images=''
clean='yes'
build_solution=''
container_registry=''
docker_password=''
docker_username=''
dns=''
image_tag='latest'
push_images=''
skip_infrastructure=''
use_local_k8s=''
namespace='eshop'
use_linkerd=''
use_istio=''
istio_gateway_name='istio-system/default-gateway'
ingress_mesh_annotations_file='ingress_values_linkerd.yaml'
imagePullPolicy='Always'
ssl_enabled=false
ssl_issuer=""
ssl_support="none"
ssl_options=""
tls_secret_name='eshop-tls-custom'


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
    --use-linkerd )
      use_linkerd='yes'; shift ;;
    --use-istio )
      use_istio='yes'; shift ;;
    --istio-gateway-name )
      istio_gateway_name="$2"; shift 2;;
    --ingress-mesh-annotations-file )
      ingress_mesh_annotations_file="$2"; shift 2;;
    --image-pull-policy )
      imagePullPolicy="$2"; shift 2;;
    --ssl-enabled )
      ssl_enabled='yes'; shift ;;
    --ssl-support )
      ssl_support="$2"; shift 2 ;;
    --tls-secret-name )
      tls_secret_name="$2"; shift 2;;
    *)
      echo "Unknown option $1"
      usage; exit 2 ;;
  esac
done

if [[ $imagePullPolicy != "Always" && $imagePullPolicy != "Never" && $imagePullPolicy != "IfNotPresent" ]]; then
  echo "--image-pull-policy needs to be a valid value: Always, IfNotPresent, Never"
  usage; exit 2;
fi

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

case "$ssl_support" in
  "staging")
    ssl_enabled=true
    tls_secret_name="eshop-letsencrypt-staging"
    ssl_issuer="letsencrypt-staging"
    ;;
  "prod")
    ssl_enabled=true
    tls_secret_name="eshop-letsencrypt-prod"
    ssl_issuer="letsencrypt-prod"
    ;;
  "custom")
    ssl_enabled=true
    ;;
esac

if [ -z "$dns" ]; then
  echo "No DNS specified. Ingress resources will be bound to public IP" >&2
  if [ $ssl_enabled ]; then
    echo "Can't bind SSL to public IP. DNS is mandatory when using TLS" >&2
    exit 1
  fi
fi

if [[ $use_istio && $use_linkerd ]]; then
  echo "You cannot enable both Istio and Linkerd." >&2
  exit 1
fi

if [[ $use_local_k8s && $ssl_enabled ]]; then
  echo "SSL cannot be enabled on local K8s." >&2
  exit 1
fi

use_custom_registry=''

if [[ -n $container_registry ]]; then 
  echo "################ Log into custom registry $container_registry ##################"
  use_custom_registry='yes'
  if [[ -z $docker_username ]] || [[ -z $docker_password ]]; then
    echo "Error: Must use -u (--docker-username) AND -p (--docker-password) if specifying custom registry"
    exit 1
  fi
  docker login -u $docker_username -p $docker_password $container_registry
fi

if [[ $push_images ]]; then
  echo "#################### Pushing images to the container registry ####################"
  services=(basket.api catalog.api identity.api ordering.api payment.api webmvc webspa webstatus)

  if [[ -z "$(docker image ls -q --filter=reference=eshop/$service:$image_tag)" ]]; then
    image_tag=linux-$image_tag
  fi

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

if [[ $clean ]]; then
  echo "Cleaning previous helm releases..."
  if [[ -z $(helm ls -q --namespace $namespace) ]]; then
    echo "No previous releases found"
  else
    helm --namespace $namespace uninstall $(helm ls -q --namespace $namespace)
    echo "Previous releases deleted"
    waitsecs=10; while [ $waitsecs -gt 0 ]; do echo -ne "$waitsecs\033[0K\r"; sleep 1; : $((waitsecs--)); done
  fi
fi

if [ "$ssl_enabled" == 'yes' ]; then
  ssl_options="--set ingress.tls[0].secretName=$tls_secret_name --set ingress.tls[0].hosts={$dns}"

  if [ "$ssl_support" != "custom" ]; then
    ssl_options="--set inf.tls.issuer=$ssl_issuer"
  fi
fi


echo "#################### Begin $app_name installation using Helm ####################"
infras=(sql-data nosql-data rabbitmq keystore-data basket-data)
charts=(eshop-common basket-api catalog-api identity-api mobileshoppingagg ordering-api ordering-backgroundtasks ordering-signalrhub payment-api webmvc webshoppingagg webspa webstatus webhooks-api webhooks-web)
gateways=(apigwms apigwws)

if [[ !$skip_infrastructure ]]; then
  for infra in "${infras[@]}"
  do
    echo "Installing infrastructure: $infra"
    helm install "$app_name-$infra" --namespace $namespace --set "ingress.hosts={$dns}" --set "ingress.gateways={$istio_gateway_name}" --values app.yaml --values inf.yaml --values $ingress_values_file --values $ingress_mesh_annotations_file --set app.name=$app_name --set inf.k8s.dns=$dns $infra --set inf.mesh.linkerd=$use_linkerd --set inf.mesh.istio=$use_istio --set inf.tls.enabled=$ssl_enabled $ssl_options
  done  
fi

for chart in "${charts[@]}"
do
    echo "Installing: $chart"
    if [[ $use_custom_registry ]]; then       
      helm install "$app_name-$chart" --namespace $namespace --set "ingress.hosts={$dns}" --set "ingress.gateways={$istio_gateway_name}" --set inf.registry.server=$container_registry --set inf.registry.login=$docker_username --set inf.registry.pwd=$docker_password --set inf.registry.secretName=eshop-docker-scret --values app.yaml --values inf.yaml --values $ingress_values_file --values $ingress_mesh_annotations_file  --set app.name=$app_name --set inf.k8s.dns=$dns --set image.tag=$image_tag --set image.pullPolicy=$imagePullPolicy $chart --set inf.mesh.linkerd=$use_linkerd --set inf.mesh.istio=$use_istio --set inf.tls.enabled=$ssl_enabled $ssl_options
    elif [[ $chart != "eshop-common" ]]; then  # eshop-common is ignored when no secret must be deployed      
      helm install "$app_name-$chart" --namespace $namespace --set "ingress.hosts={$dns}" --set "ingress.gateways={$istio_gateway_name}" --values app.yaml --values inf.yaml --values $ingress_values_file --values $ingress_mesh_annotations_file --set app.name=$app_name --set inf.k8s.dns=$dns --set image.tag=$image_tag --set image.pullPolicy=$imagePullPolicy $chart --set inf.mesh.linkerd=$use_linkerd --set inf.mesh.istio=$use_istio --set inf.tls.enabled=$ssl_enabled $ssl_options
    fi
done

for gw in "${gateways[@]}"
do
    echo "Installing gateway: $gw"
    helm install "$app_name-$gw" --namespace $namespace --set "ingress.hosts={$dns}" --set "ingress.gateways={$istio_gateway_name}" --values app.yaml --values inf.yaml --values $ingress_values_file --values $ingress_mesh_annotations_file --set app.name=$app_name --set inf.k8s.dns=$dns --set image.pullPolicy=$imagePullPolicy --set inf.tls.enabled=$ssl_enabled $ssl_options --set inf.mesh.linkerd=$use_linkerd --set inf.mesh.istio=$use_istio $gw 
done

echo "FINISHED: Helm charts installed."