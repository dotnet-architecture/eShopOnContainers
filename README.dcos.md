# eShopOnContainers on DC/OS

## Prerequisites
* A private Docker registry. Follow Azure Container Registry's [guide](https://docs.microsoft.com/en-us/azure/container-registry/container-registry-get-started-portal) to create one.
* A DC/OS cluster. Follow Azure Container Service's [walkthrough](https://docs.microsoft.com/en-us/azure/container-service/container-service-deployment) to create one.
* The `dcos` CLI (DC/OS docs: [Installing the CLI](https://dcos.io/docs/1.8/usage/cli/install/))

## Prepare the Cluster
1. Install Marathon-LB. ([DC/OS docs](https://dcos.io/docs/1.8/usage/service-discovery/marathon-lb/usage/))
1. Mount an Azure file share on each agent of the cluster at `/mnt/share`. See the Azure Container Service [documentation](https://docs.microsoft.com/en-us/azure/container-service/container-service-dcos-fileshare) for guidance.
1. Copy your private registry credentials to the mounted file share as described in the Azure Container Service [documentation](https://docs.microsoft.com/en-us/azure/container-service/container-service-dcos-acr).

## Deploy the Application
1. Build the eShopOnContainers Docker images, and push them to your registry.
1. Open an SSH tunnel to your cluster as described in the ACS documentation: [Connect with an ACS cluster](https://docs.microsoft.com/en-us/azure/container-service/container-service-connect#connect-to-a-dcos-or-swarm-cluster)
1. Open a PowerShell window with the `dcos` CLI on your path.
1. Navigate to your local `eShopOnContainers` repository's `dcos` directory.
1. Run `generate-config.ps1` to generate configuration for the eShopOnContainers services. The script requires URLs for your cluster's agents and your private registry. Your agents' URL is of the form `[dns prefix]agents.[Azure region].cloudapp.azure.com`:
>```powershell
>./generate-config.ps1 -agentsFqdn mydcosagents.centralus.cloudapp.azure.com -registry myregistry.azurecr.io
>```
6. Use the `dcos` CLI to deploy the application:
>```powershell
>dcos marathon group add eShopOnContainers.json
>```

# DC/OS 101
DC/OS is an operating system based on Apache Mesos, a distributed systems kernel. Similar to the way a desktop OS manages a single machine's compute resources, abstracting away the details of core scheduling and memory allocation, DC/OS manages the compute resources of a cluster of heterogenous machines.

Like a desktop OS, DC/OS is general-purpose; we won't deploy eShopOnContainers to it directly. Instead we'll deploy to Marathon, a container orchestration platform which runs atop DC/OS.

## A Marathon Application
The Maration API defines applications in JSON. Container application definitions are simple. For example, here's one for the eShopOnContainers SQL server:
>```json
>{
>  "id": "sql-data",
>  "container": {
>    "type": "DOCKER",
>    "docker": {
>      "image": "microsoft/mssql-server-linux:ctp1-4",
>      "network": "BRIDGE",
>      "portMappings": [
>        {
>          "hostPort": 1433,
>          "labels": {
>            "VIP_0": "eshopsql-data:1433"
>          }
>        }
>      ]
>    }
>  },
>  "env": {
>    "ACCEPT_EULA": "Y",
>    "SA_PASSWORD": "Pass@word"
>  },
>  "healthChecks": [
>    {
>      "protocol": "TCP",
>      "gracePeriodSeconds": 30,
>      "intervalSeconds": 60,
>      "timeoutSeconds": 30,
>      "maxConsecutiveFailures": 3,
>      "port": 1433
>    }
>  ],
>  "cpus": 0.1,
>  "mem": 1024,
>  "instances": 1
>},
>```
The definition straightforwardly specifies the image, environment variables, compute resources, and health checks for the container. Marathon's port configuration is more complex; for details, see the [Marathon documentation](https://mesosphere.github.io/marathon/docs/ports.html). Basically, the above definition binds the container's port 1433 to the host's port 1433. It also specifies a DC/OS Virtual IP, `eshopsql-data:1433`. Other containers can use this to find an IP for `sql-data` via Marathon's layer 4 load balancer using a simple naming convention: `eshopsql-data` becomes `eshopsql-data.marathon.l4lb.thisdcos.directory`.

## Exposing an Application
Marathon-LB is a useful tool for load balancing applications. It includes HAProxy and, using label metadata from the Marathon event bus, dynamically generates its configuration to match the state of Marathon applications. For example, here are the labels for the `webmvc` application:
>```json
>{
>  "id": "webmvc",
>  ...
>  "labels": {
>    "HAPROXY_GROUP": "external",
>    "HAPROXY_0_VHOST": "mydcosagents.centralus.cloudapp.azure.com",
>    "HAPROXY_0_MODE": "http",
>    "HAPROXY_0_PATH": "/webmvc"
>  },
>  ...
>}
>```
This tells the Marathon-LB application responsible for the "external" group to proxy `http://mydcosagents.centralus.cloudapp.azure.com/webmvc` to a `webmvc` container. When `webmvc` deploys, Marathon-LB generates appropriate configuration for, and restarts, HAProxy.

## Further Reading
* [DC/OS Docs](https://dcos.io/docs/1.8/)
* [Marathon Docs](https://mesosphere.github.io/marathon/docs/)
