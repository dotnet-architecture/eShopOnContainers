# eShopOnContainers on Docker Swarm

## Prerequisites
1. A Docker Swarm cluster ([Azure Container Service](https://docs.microsoft.com/en-us/azure/container-service/container-service-deployment))
1. Docker tooling, including docker-compose, on your development machine ([docker.com](https://www.docker.com/))

## Deploying to a Swarm Cluster
1. Open a command line to your local eShopOnContainers repository.
2. Restore and publish the projects:
>```
>dotnet restore eShopOnContainers-ServicesAndWebApps.sln
>dotnet publish eShopOnContainers-ServicesAndWebApps.sln -c Release -o ./obj/Docker/publish
>```
3. Create an SSH tunnel to your cluster's master node (See [Connect with an ACS cluster](https://docs.microsoft.com/en-us/azure/container-service/container-service-connect#connect-to-a-dcos-or-swarm-cluster))
4. Set `SWARM_AGENTS_FQDN` to the domain name of your swarm agents' load balancer. This is of the form `[dns prefix]agents.[azure region].cloudapp.azure.com` (it can also be found in the Azure portal). For example:
>```bash
>export SWARM_AGENTS_FQDN=myswarmclusteragents.centralus.cloudapp.azure.com
>```
Or, in PowerShell:
>```powershell
>$env:SWARM_AGENTS_FQDN='myswarmclusteragents.centralus.cloudapp.azure.com'
>```
5. Deploy eShopOnContainers with `docker-compose`:
>```
>docker-compose -f docker-compose.yml -f docker-compose.swarm.yml up -d
>```

## Further Reading
* Docker: [Swarm mode overview](https://docs.docker.com/engine/swarm/)
