# Azure Red Hat OpenShift (ARO) build definitions

This folder contains the Azure DevOps build definitions in YAML format targeting container builds in OpenShift on ARO. Each folder contains one `azure-pipelines.yml` that contains the build definition for one microservice.  The container images are built inside of the Azure Red Hat OpenShift cluster from the latest release of the RHEL .NET Core base image (currently 3.1).

Each build definition calls out to two YAML Pipelines templates, one to build the artifacts (webapp-build-artifacts.yml), and one to create the container image (webapp-container-build.yml) in the target OpenShift project.  The containers are built using OpenShift's source-to-image (s2i) binary build technology.

When connecting to OpenShift, there is a dependency on an OpenShift service connection named 'OpenShift on ARO'.

For more information about YAML builds read the [Azure DevOps documentation](https://docs.microsoft.com/azure/devops/pipelines/get-started-yaml?view=azure-devops).