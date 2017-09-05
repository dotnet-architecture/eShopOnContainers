# Deploying Resources On Azure

## Pre-requisites
1. [Azure CLI 2.0 Installed](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli)
2. Azure subscription created

Login into your azure subscription by typing `az login` (note that you maybe need to use `az account set` to set the subscription to use). Refer to [this article](https://docs.microsoft.com/en-us/cli/azure/authenticate-azure-cli) for more details

## Deploying using CLI

## Deploying Virtual machines to host the services

1. [Deploying a Linux VM to run single-server development environment using docker-machine (**Recommended for development environments**)](az/vms/docker-machine.md)
2. [Deploying a Linux VM or Windows Server 2016 to run a single-server development environment using ARM template (**Recommended for creating testing environments**)](az/vms/plain-vm.md)

Using `docker-machine` is the recommended way to create a VM with docker installed. But it is limited to Linux based VMs.

## Deploying Azure resources used by the services

1. [Deploying SQL Server and databases](az/sql/readme.md)
2. [Deploying Azure Service Bus](az/servicebus/readme.md)
3. [Deploying Redis Cache](az/redis/readme.md)
4. [Deploying Cosmosdb](az/cosmos/readme.md)
5. [Deploying Catalog Storage](az/storage/catalog/readme.md)
6. [Deploying Marketing Storage](az/storage/marketing/readme.md)
7. [Deploying Marketing Azure functions](az/azurefunctions/readme.md)





