# eShopOnContainers on Kubernetes
The k8s directory contains Kubernetes configuration for the eShopOnContainers app and a PowerShell script to deploy it to a cluster. Each eShopOnContainers microservice has a deployment configuration in `deployments.yaml`, and is exposed to the cluster by a service in `services.yaml`. The microservices are exposed externally on individual routes (`/basket-api`, `/webmvc`, etc.) by an nginx reverse proxy specified in `frontend.yaml` and `nginx.conf`.

## Prerequisites
* A Kubernetes cluster. Follow Azure Container Service's [walkthrough](https://docs.microsoft.com/en-us/azure/container-service/container-service-kubernetes-walkthrough) to create one. 
* A private Docker registry. Follow Azure Container Registry's [guide](https://docs.microsoft.com/en-us/azure/container-registry/container-registry-get-started-portal) to create one.
* Optionally, previous steps can be skipped if you run the **gen-k8s-env-aks.ps1** script to create an AKS cluster environment or gen-k8s-env.ps1 script to create an ACS for Kuberentes cluster environment including the creation of additional Azure environment needed like an Azure Resource Manager and ACR registry. 

Azure cli 2.0 must be previously installed [installation guide](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli). For example:

    **Important**: Note the parameter "-createAcr true". If you are creating the K8s cluster but you want to re-use and existing ACR, say "-createAcr false".


For AKS:

>```
>./gen-k8s-env-aks -resourceGroupName YoureShopAksResgroup -location centralus -serviceName YoureShopAksCluster -dnsNamePrefix youreshopaks -registryName YoureShopAcrRegistry -createAcr true -nodeCount 3 -nodeVMSize Standard_D2_v2
>```

For ACS:

>```
>./gen-k8s-env -resourceGroupName k8sGroup -location westeurope -registryName k8sregistry -createAcr true -orchestratorName k8s-cluster -dnsName k8s-dns
>```

* A Docker development environment with `docker` and `docker-compose`.
    * Visit [docker.com](https://docker.com) to download the tools and set up the environment. Docker's [installation guide](https://docs.docker.com/engine/getstarted/step_one/#step-3-verify-your-installation) covers verifying your Docker installation.
*  The Kubernetes command line client, `kubectl`.
    * This can be installed with the `az` tool as described in the Azure Container Service [walkthrough](https://docs.microsoft.com/en-us/azure/container-service/container-service-kubernetes-walkthrough). `az` is also helpful for getting the credentials `kubectl` needs to access your cluster. For other installation options, and information about configuring `kubectl` yourself, see the [Kubernetes documentation](https://kubernetes.io/docs/tasks/kubectl/install/).

## Deploy the application with the deployment script
1. Open a PowerShell command line at the `k8s` directory of your local eShopOnContainers repository.
1. Ensure `docker`, `docker-compose`, and `kubectl` are on the path, and configured for your Docker machine and Kubernetes cluster.
1. Run `deploy.ps1` with your registry information. The Docker username and password are provided by Azure Container Registry, and can be retrieved from the Azure portal. Optionally, ACR credentials can be obtained by running the following command:

>```
>az acr credential show -n eshopregistry
>```

Once the user and password are retrieved, run the following script for deployment. For example:

>```
>./deploy.ps1 -registry myregistry.azurecr.io -dockerUser User -dockerPassword SecretPassword -configFile file_with_config.yaml
>```

The parameter `configFile` is important (and mandatory) because it contains the configuration used for the Pods in Kubernetes. This allow deploying Pods that use your own resources in Azure or any other cloud provider. A configuration file `conf_local.yaml` is provided which configures Pods to use the infrastructure containers (that is sql server, rabbitmq, redis and mongodb must be deployed also in the k8s).

The script will build the code and corresponding Docker images, push the later to your registry, and deploy the application to your cluster. You can watch the deployment unfold from the Kubernetes web interface: run `kubectl proxy` and open a browser to [http://localhost:8001/ui](http://localhost:8001/ui)

### Pods configuration file

When deploying to k8s the script needs the `configFile` parameter with the location of the YAML configuration file. This file contains the configuration of the pods. The file is a .YAML file. For reference another configuration file (conf_cloud.yaml) is provided but without valid values.

If you deploy the infrastructure containers use `conf_local.yaml` as a value for `configFile` parameter. If you don't deploy the infrastructure containers use your own configuration file with the correct values.

### Parameters of the deploy.ps1 script

The script accepts following parameters:

+ `registry`: Name of the Docker registry to use. If not passed DockerHub is assumed
+ `dockerUser`: Login to use for the Docker registry (if needed)
+ `dockerPassword`: Password to use for the Docker registry (if needed)
+ `execPath`: Location of `kubectl` (if not in the path). If passed must finish with the path character.
+ `kubeconfigPath`: Location of the `kubectl` configuration file. **This parameter is used only in the CI pipeline**, so you don't need to pass it when invoking the script using the CLI.
+ `configFile`: Location of the Yaml file with the `externalcfg` configmap to be deployed. This configmap is used to configure the Pod's environment **This parameter is mandatory**
+ `imageTag`: Tag of the images to deploy to k8s. If not passed the name of the current branch is used.
+ `externalDns`: External DNS name of the k8s. This is only needed if you have configured a DNS that points to your k8s external IP. If you don't have any DNS configured do not pass this parameter.
+ `deployCI`: If `true` means that script is running under the context of a VSTS Hosted Build Agent. **You should never use this parameter from CLI**
+ `buildBits`: means that the source code of eShopOnContainers will be built. If you have built your code (and have all projects published in `obj/Docker/publish`) do not pass this parameter. Default value is `false`
+ `buildImages`: If `true` (default value) Docker images are built and pushed in the Docker registry. If you set this parameter to `false`, Docker images won't be built nor pushed in the Docker registry (but k8s' deployments and services will be redeployed).
+ `deployInfrastructure`: If `true` infrastructure containers (rabbitmq, mongo, redis, sql) will be deployed in k8s. If `false` those containers (and its related deployments and services in k8s) won't be deployed.
+ `dockerOrg`: Name of the organization in the registry where the images are (or will be pushed). Default value is `eshop` (which has images provided by Microsoft)

**Important:** If you **don't pass the `-buildBits $true` the script won't build and publish the projects** to their `obj/Docker/publish` folder. If any project is not published, you'll be receiving errors like:

```
ERROR: Service 'xxxxxxx' failed to build: COPY failed: stat /var/lib/docker/tmp/docker-builder123456789/obj/Docker/publish: no such file or directory
```

### Typical usages of the script:

Build all projects, and deploy all them in k8s including infrastructure containers in a organization called `foo` in Docker Hub. Images will be tagged with my current git branch and containers will use the configuration set in `conf_local.yml` file:

```
./deploy.ps1 -buildBits $true -dockerOrg foo -dockerUser MY_USER -dockerPassword MY_PASSWORD -configFile conf_local.yml
```

Do not build any project and don't rebuild docker images. Create k8s deployments that will pull images from my private repository, in the `foo` organization, using the tag `latest`. Containers will use the configuration set in `conf_cloud` file.

```
./deploy.ps1 -buildImages $false -dockerOrg foo -registry MY_REGISTRY_FQDN -dockerUser MY_USER -dockerPassword MY_PASSWORD -configFile conf_cloud.yml -imageTag master
```

Deploy k8s using public images that Microsoft provides:

```
./deploy.ps1 -buildImages $false -configFile conf_local.yml -imageTag master
```
