# eShopOnContainers on Kubernetes
This directory contains Kubernetes configuration for the eShopOnContainers app and a PowerShell script to deploy it to a cluster. Each microservice has a deployment configuration in `deployments.yaml`, and is exposed to the cluster by a service in `services.yaml`. The microservices are exposed externally on individual routes (`/basket-api`, `/webmvc`, etc.) by an nginx reverse proxy as specified in `frontend.yaml` and `nginx.conf`.

## Deploying the application
### Prerequisites
* A Docker build host.
* A private Docker registry. Follow Azure Container Registry's [guide](https://docs.microsoft.com/en-us/azure/container-registry/container-registry-get-started-portal) to create one.
* A Kubernetes cluster. Follow Azure Container Service's [walkthrough](https://docs.microsoft.com/en-us/azure/container-service/container-service-kubernetes-walkthrough) to create one.

### Run the deployment script
1. Open a PowerShell command line at `eShopOnContainers/k8s`.
1. Ensure `docker`, `docker-compose`, and `kubectl` are on the path, and configured for your Docker machine and Kubernetes cluster.
1. Run `deploy.ps1` with your registry information. For example:
    ```
    ./deploy.ps1 -registry myregistry.azurecr.io -dockerUser User -dockerPassword SecretPassword
    ```
    The Docker username and password are provided by Azure Container Registry, and can be retrieved from the Azure portal.

The script will build the code and corresponding Docker images, push the latter to your registry, and deploy the application to your Kubernetes cluster.

TODOs
=====
* Host WebSPA at `/webspa`
    * This is blocked on correct relative URLs for images. Presently these are set at build by webpack, which isn't aware of where the app will be sited. An Angular solution might exist. Another option is to encode the images in base64.
* Debug microservice resiliency issues--some microservices can enter failure states requiring their pod to be recreated.
* Respond to `kubectl` failures in `deploy.ps1`.