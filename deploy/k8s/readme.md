# Kubernetes (k8s) deploy information

This folder contains files needed to **create** a ACS with Kubernetes in Azure and to **deploy** eShopServices in a existing Kubernetes:

- `gen-k8s-env.ps1` Script to create a ACS with Kubernetes in Azure
- `deploy.ps1` Script to deploy eShopOnContainers in a existing k8s

Refer to file [README.k8s.md](./README.k8s.md) for detailed information

Refer to file [README.CICD.k8s.md](./README.CICD.k8s.md) for information about how to set a VSTS build for deploying on k8s

Refer to file [conf-files.md](./conf-files.md) for a brief description of every YAML file in this folder