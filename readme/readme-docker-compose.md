# Docker-compose yaml files

In the root folder of the repo are all docker-compose files (`docker-compose*.yml`). Here is a list of all of them and what is their purpose:

## Files needed to run eShopOnContainers locally 

* `docker-compose.yml`: This file contains **the definition of all images needed for running eShopOnContainers**.
* `docker-compose.override.yml`: This file contains the base configuration for all images of the previous file

Usually these two files are using together. The standard way to start eShopOnContainers from CLI is:

```
docker-compose -f docker-compose.yml -f docker-compose.override.yml
```

This will start eShopOnContainers with all containers running locally, and it is the default development environment.

## Files needed to run eShopOnContainers on a remote docker host

* `docker-compose.prod.yml`: This file is a replacement of the `docker-compose.override.yml` but contains some configurations more suitable for a "production" environment or when you need to run the services using an external docker host.

```
docker-compose -f docker-compose.yml -f docker-compose.prod.yml
```

When using this file the following environments variables must be set:

* `ESHOP_PROD_EXTERNAL_DNS_NAME_OR_IP` with the IP or DNS name of the docker host that runs the services (can use `localhost` if needed). 
* `ESHOP_AZURE_STORAGE_CATALOG` with the URL of the Azure Storage that will host the catalog images
* `ESHOP_AZURE_STORAGE_MARKETING` with the URL of the Azure Storage that will host the marketing campaign images

You might wonder why an external image resource (storage) is needed when using `docker-compose.prod.yml` instead of `docker-compose.override.yml`. Answer to this is related to a limitation of Docker Compose file format. This is how we set the environment configuration of Catalog microservice in `docker-compose.override.yml`:

```
PicBaseUrl=${ESHOP_AZURE_STORAGE_CATALOG:-http://localhost:5101/api/v1/catalog/items/[0]/pic/}
```

The `PicBaseUrl` variable is set to the value of `ESHOP_AZURE_STORAGE_CATALOG` if this variable is set to any value other than blank string. If not, the value is set to `http://localhost:5101/api/v1/catalog/items/[0]/pic/`. That works perfectly in a local environment where you run all your services in `localhost` and setting `ESHOP_AZURE_STORAGE_CATALOG` you can use or not Azure Storage for the images (if you don't use Azure Storage images are served locally by catalog servide). But when you run the services in a external docker host, specified in `ESHOP_PROD_EXTERNAL_DNS_NAME_OR_IP`, the configuration should be as follows:

```
PicBaseUrl=${ESHOP_AZURE_STORAGE_CATALOG:-http://${ESHOP_PROD_EXTERNAL_DNS_NAME_OR_IP}:5101/api/v1/catalog/items/[0]/pic/}
```

So, use `ESHOP_AZURE_STORAGE_CATALOG` if set, and if not use `http://${ESHOP_PROD_EXTERNAL_DNS_NAME_OR_IP}:5101/api/v1/catalog/items/[0]/pic/}`. Unfortunately seems that docker-compose do not substitute variables inside variables, so the value that `PicBaseUrl` gets if `ESHOP_AZURE_STORAGE_CATALOG` is not set is literally `http://${ESHOP_PROD_EXTERNAL_DNS_NAME_OR_IP}:5101/api/v1/catalog/items/[0]/pic/}` without any substitution.

## Build container (DEPRECATED)

NOTE that since we support Docker MULTI-STAGE builds (support in VS 2017 since December 2017), the build container is no loger needed in CI/CD pipelines as a similar process is done by Docker itself under the covers with the multi-stage builds.
For more info on Docker Multi-Stage, read: 

https://docs.docker.com/develop/develop-images/multistage-build/

https://blogs.msdn.microsoft.com/stevelasker/2017/09/11/net-and-multistage-dockerfiles/

* `docker-compose.ci.build.yml`: This file is for starting the build container to build the project using a container that has all needed prerequisites. Refer to [corresponding wiki section](https://github.com/dotnet-architecture/eShopOnContainers/wiki/03.-Setting-the-eShopOnContainers-solution-up-in-a-Windows-CLI-environment-(dotnet-CLI,-Docker-CLI-and-VS-Code)#build-the-bits-through-the-build-container-image) for more information.

**For more information** about docker-compose variable substitution read the [compose docs](https://docs.docker.com/compose/compose-file/#variable-substitution).

## Other files

* `docker-compose.nobuild.yml`: This file contains the definition of all images needed to run the eShopOnContainers. Contains **the same images that `docker-compose.yml`** but without any `build` instruction. If you use this file instead of `docker-compose.yml` when launching the project and you don't have the images built locally, **the images will be pulled from dockerhub**. This file is not intended for development usage, but for some CI/CD scenarios.
* `docker-compose.vs.debug.yml`: This file is used by Docker Tools of VS2017, and should not be used directly.
* `docker-compose.vs.release.yml`: This file is used by Docker Tools of VS2017, and should not be used directly.

**Note**: The reason why we need the `docker-compose.nobuild.yml` is that [docker-compose issue #3391](https://github.com/docker/compose/issues/3391). Once solved, parameter `--no-build` of docker-compose could be used safely in a CI/CD environments and the need for this file will disappear.


## Windows container files

All `docker-compose-windows*.yml` files have a 1:1 relationship with the same file without the `-windows` in its name. Those files are used to run Windows Containers instead of Linux Containers.

* `docker-compose-windows.yml`: Contains the definitions of all containers that are needed to run eShopOnContainers using windows containers (equivalent to `docker-compose.yml`).
* `docker-compose-windows.override.yml`: Contains the base configuration for all windows containers

**Note** We plan **to remove** the `docker-compose-windows.override.yml` file, because it is **exactly the same** as the `docker-compose.override.yml`. The reason of its existence is historical, but is no longer needed. You can use `docker-compose.override.yml` instead.

* `docker-compose-windows.prod.yml` is the equivalent of `docker-compose.prod.yml` for containers windows. As happens with `docker-compose-windows.override.yml` this file will be deleted in a near future, so you should use `docker-compose.prod.yml` instead.

## "External container" files

These files were intended to provide a fast way to start only "infrastructure" containers (SQL Server, Redis, etc). *This files are deprecated and will be deleted in a near future**:

* `docker-compose-external.override.yml`
* `docker-compose-external.yml`

If you want to start only certain containers use `docker-compose -f ... -f ... up container1 contaner2 containerN` as specified in [compose doc](https://docs.docker.com/compose/reference/up/)
