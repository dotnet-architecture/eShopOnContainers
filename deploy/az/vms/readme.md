# Deploy a VM to run the services

Follow these instructions to deploy a Linux-based VM with the Docker Host installed, or a VM with Windows Server 2016 plus
windows containers and Docker Daemon.

You can use this machine to installthe microservices and having a "development" environment (useful to develop and test the client apps).

Please note that this deployment is not a production deployment. In a production-based scenario, you should deploy all containers in ACS.

## Create the VM

Ensure you are logged in the desired subscription (use `az login` and `az account set` if needed. Refer to [this article](https://docs.microsoft.com/en-us/cli/azure/authenticate-azure-cli) for more details.

Go to `linux-vm` or `win-vm` folder (based on if you want a Linux or Windows VM). Then:

1. Edit the file `mvparams.json` with your desired values
2. Run the file `create-resources.cmd` from command-line to create the VM.

**Note:** To avoid errors, ARM template used (`azuredeploy.json`), generates unique names for:

1. VM used storage
2. Public DNS

Those public names are based on the parameters set in `mvparams.json` file.

### The mvparams.json file

This file contains the minimum set of parameters needed by the ARM template to deploy the VM. ARM template accepts some other parameters (set with
default values). Look the template for more info.

The parameters defined in this file are:

1. `newStorageAccountName`: Name of the storage created for the VM. To ensure uniqueness a unique suffix will be added to this value.
2. `adminUsername`: Admin login
3. `adminPassword`: Admin password
4. `dnsNameForPublicIP`: DNS of the VM. To ensure uniqueness a unique suffix will be added to this value.
5. `VMName`: Name of the VM inside Azure

## Finding the IP and DNS of the VM

To find the IP and FQDN of the VM you can type `az vm list --resource-group <resourcegroup> --output table --show-details` (where resourcegroup is the
name of the resourcegroup where you created the VM). This command will generate output like:

```
Name        ResourceGroup    PowerState    PublicIps      Fqdns                                             Location
----------  ---------------  ------------  -------------  ------------------------------------------------  ----------
MyDockerVM  MyResourceGroup  VM running    xx.xx.xxx.xxx  eshop-srvxxxxxxxxxxxxx.westus.cloudapp.azure.com  westus
```

You can use this information to connect your new VM.

## Deploy services in the VM

We are providing public images of the services in DockerHub (https://hub.docker.com/u/eshop/). To use these images, just create a folder in the VM and copy
following files to it (those files are in the root of the repo):

1. `docker-compose.yml`
2. `docker-compose.prod.yml`

Then log into the VM and run the command `docker-compose -f docker-compose.yml -f docker-compose.prod.yml up --no-build -d` to start all the microservices.



