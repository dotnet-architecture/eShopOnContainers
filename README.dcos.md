# eShopOnContainers on DC/OS

## Prerequisites
* A DC/OS cluster. Follow Azure Container Service's [walkthrough](https://docs.microsoft.com/en-us/azure/container-service/container-service-deployment) to create one.
* A private Docker registry. Follow Azure Container Registry's [guide](https://docs.microsoft.com/en-us/azure/container-registry/container-registry-get-started-portal) to create one.
* The `dcos` CLI. See [DC/OS: Installing the CLI](https://dcos.io/docs/1.8/usage/cli/install/) for more details.

## Prepare the Cluster
1. Install Marathon-LB to your cluster. This can be done with the DC/OS cli (`dcos package install marathon-lb`) or web interface. See the [DC/OS docs](https://dcos.io/docs/1.8/usage/service-discovery/marathon-lb/usage/) for more information.
1. Provide Marathon credentials for your container registry.
    * Mount an Azure file share on each agent of the cluster at `/mnt/share`. See the Azure Container Service [documentation](https://docs.microsoft.com/en-us/azure/container-service/container-service-dcos-fileshare) for guidance.
    * Copy your private registry credentials to the mounted share as described in the Azure Container Service [documentation](https://docs.microsoft.com/en-us/azure/container-service/container-service-dcos-acr).

## Deploy the Application
1. Build the eShopOnContainers Docker images.
1. Open a PowerShell window in your local `eShopOnContainers` repository's `dcos` directory.
1. Tag the `eshop/...` images with your registry, then push them.
    * `tagpush.ps1` can do this for you, e.g.:
    >```
    >./tagpush.ps1 myregistry.azurecr.io
    >```
1. Open an SSH tunnel to your cluster as described in the ACS documentation: [Connect with an ACS cluster](https://docs.microsoft.com/en-us/azure/container-service/container-service-connect#connect-to-a-dcos-or-swarm-cluster)
1. Ensure `dcos` is on your path.
1. Run `generate-config.ps1` to generate configuration for the eShopOnContainers services. The script requires hostnames for your cluster's agents and your private registry. Your agents' hostname is of the form `[dns prefix]agents.[Azure region].cloudapp.azure.com`:
>```
>./generate-config.ps1 -agentsFqdn mydcosagents.centralus.cloudapp.azure.com -registry myregistry.azurecr.io
>```
7. Use the `dcos` CLI to deploy the application:
>```
>dcos marathon group add eShopOnContainers.json
>```
You can watch the deployment progress in the DC/OS web interface at [http://localhost/#/services/](http://localhost/#/services/).
