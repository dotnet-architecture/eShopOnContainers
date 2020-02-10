# OpenShift on ARO build definitions

This folder contains the Azure DevOps build definitions in YAML format targeting container builds in OpenShift on ARO. Each folder contains one `azure-pipelines.yml` that contains the build definition for one microservice.  The container images are built inside of the OpenShift cluster from the latest release of the RHEL .NET Core base image.

For more information about YAML builds read the [Azure DevOps documentation](https://docs.microsoft.com/azure/devops/pipelines/get-started-yaml?view=azure-devops).