# Azure Red Hat OpenShift (ARO) deployment

This folder contains all of the the files required to deploy eShopOnContainers to an Azure Red Hat OpenShift (ARO) cluster.  The strategy being used here is a GitOps strategy that takes advantage of OpenShift deployment triggers to deploy the containers whenever any of the container images or deployment configuration changes, such as Deployment Config, Service or Route objects.

Each subfolder in this folder corresponds to one of the eShopOnContainers microservices containers.  Each microservice folder contains a YAML pipeline deployment file and an OpenShift template YAML file that will create the necessary OpenShift objects for the respective microservice.  The following folders do not correspond to any of the eShopOnContainers microservices:

- apigw-templates - this contains an OpenShift template file (`apigw-deploy-template.yml`) used to deploy the API gateway services
- azure-devops-templates - this contains a centralized variables template file (`variables.yml`) that is shared among all of the deploy pipelines, as well as an Azure DevOps pipeline job template (`api-gw-deploy.yml`) that is used for deploying all of the gateway API services.  It calls the `apigw-deploy-template.yml` template file to configure the gateway API services.
- infrastructure - this contains a YAML pipeline that calls several OpenShift templates to deploy all of the infrastructure services, such as MongoDB, SQL Server, RabbitMQ, etc.

Some of the microservices folders contain extra files, such as for exposing public routes.  These are not included in the OpenShift templates because they need to be tokenized in order to preserve the integrity of the certificate data used for the routes.

In order to get secrets passed in to the pipelines, I have chosen to use an Azure Key Vault instance in conjunction with the [Azure DevOps Azure Key Vault task](https://docs.microsoft.com/en-us/azure/devops/pipelines/tasks/deploy/azure-key-vault?view=azure-devops) in order to bring in any secrets needed during the build as ephemeral environment variables.  The only secrets that I have chosen to store in Azure Key Vault are the SQL username/password and the certificate values required for the public routes.  The following are the envrionment variable names of the secrets:

- SqlUsername
- SqlPassword
- CaCertificate3
- Certificate3
- PrivateKey3

These secrets are then pushed into OpenShift secret objects during the infrastructure deployment so that they can be consumed from the pods in the OpenShift cluster.

When connecting to OpenShift from Azure DevOps, there is a dependency on an OpenShift service connection named 'OpenShift on ARO' that must point to an ARO cluster.  In order to create this connection, you must add the [OpenShift Extension for Azure DevOps](https://marketplace.visualstudio.com/items?itemName=redhat.openshift-vsts).

For more information on ARO, please read the [ARO documentation](https://docs.openshift.com/aro/welcome/index.html).

For more information about YAML pipelines read the [Azure DevOps documentation](https://docs.microsoft.com/azure/devops/pipelines/get-started-yaml?view=azure-devops.