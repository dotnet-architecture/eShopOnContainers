# Azure Red Hat OpenShift (ARO) build definitions

This folder contains the Azure DevOps build definitions in YAML format targeting container builds in OpenShift on ARO. Each folder contains one `azure-pipelines.yml` file that contains the build definition for one microservice.  The container images are built inside of the Azure Red Hat OpenShift cluster from the latest release of the RHEL .NET Core base image (currently 3.1).  There is also a folder named `openshift-templates` that contains a parameterized OpenShift YAML template file that provisions BuildConfig and ImageStream objects in OpenShift for the microservice containers.

Each build definition calls out to two YAML Pipelines templates, one to build the artifacts (`webapp-build-artifacts.yml`), and one to create and upload the container image (`webapp-container-build.yml`) to the target OpenShift project.  The containers are built using OpenShift's source-to-image (s2i) binary build technology.  The `webapp-container-build.yml` YAML pipeline template calls out to the `app-build-template.yml` YAML file to provision the BuildConfig and ImageStream objects for the container.

When connecting to OpenShift from Azure DevOps, there is a dependency on an OpenShift service connection named 'OpenShift on ARO' that must point to an ARO cluster.  In order to create this connection, you must add the [OpenShift Extension for Azure DevOps](https://marketplace.visualstudio.com/items?itemName=redhat.openshift-vsts).

For more information on ARO, please read the [ARO documentation](https://docs.openshift.com/aro/welcome/index.html).

For more information about YAML pipelines read the [Azure DevOps documentation](https://docs.microsoft.com/azure/devops/pipelines/get-started-yaml?view=azure-devops).