# eShopOnContainers on Kubernetes
The k8s directory contains Kubernetes configuration for the eShopOnContainers app and a PowerShell script to deploy it to a cluster. Each eShopOnContainers microservice has a deployment configuration in `deployments.yaml`, and is exposed to the cluster by a service in `services.yaml`. The microservices are exposed externally on individual routes (`/basket-api`, `/webmvc`, etc.) by an nginx reverse proxy specified in `frontend.yaml` and `nginx.conf`.

## Prerequisites
* A Kubernetes cluster. Follow Azure Container Service's [walkthrough](https://docs.microsoft.com/en-us/azure/container-service/container-service-kubernetes-walkthrough) to create one. 
* A private Docker registry. Follow Azure Container Registry's [guide](https://docs.microsoft.com/en-us/azure/container-registry/container-registry-get-started-portal) to create one.
* Optionally, previous steps can be skipped if you run gen-k8s-env.ps1 script to automatically create the azure environment needed for kubernetes deployment. Azure cli 2.0 must be previously installed [installation guide](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli). For example:
>```
>./gen-k8s-env -resourceGroupName k8sGroup -location westeurope -registryName k8sregistry -orchestratorName k8s-cluster -dnsName k8s-dns
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
>./deploy.ps1 -registry myregistry.azurecr.io -dockerUser User -dockerPassword SecretPassword
>```
The script will build the code and corresponding Docker images, push the latter to your registry, and deploy the application to your cluster. You can watch the deployment unfold from the Kubernetes web interface: run `kubectl proxy` and open a browser to [http://localhost:8001/ui](http://localhost:8001/ui)
