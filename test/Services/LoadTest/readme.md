# Load Testing settings

This folder contains files needed to run load tests locally or on a Kubernetes / Service Fabric cluster.

<p>
<img src="../../../img/loadtests/loadtestproj_dir.png">
<p>

## Set a local environment

Modify the **app.config** file in the LoadTest project directory and set the following service urls.

```
<Servers>
    <MvcWebServer url="http://localhost:5100" />
    <CatalogApiServer url="http://localhost:5101" />
    <OrderingApiServer url="http://localhost:5102" />
    <BasketApiServer url="http://localhost:5103" />
    <IdentityApiServer url="http://localhost:5105" />
    <LocationsApiServer url="http://localhost:5109" />
    <MarketingApiServer url="http://localhost:5110" />
  </Servers>
```

Modify the **.env** file and set the following config property as shown bellow.

```
USE_LOADTEST=True
```
## Set a Service Fabric environment

Modify the **app.config** file in the LoadTest project directory and set the following service urls.

```
<Servers>
    <MvcWebServer url="http://<target_sf_dns>:5100" />
    <CatalogApiServer url="http://<target_sf_dns>:5101" />
    <OrderingApiServer url="http://<target_sf_dns>:5102" />
    <BasketApiServer url="http://<target_sf_dns>:5103" />
    <IdentityApiServer url="http://<target_sf_dns>:5105" />
    <LocationsApiServer url="http://<target_sf_dns>:5109" />
    <MarketingApiServer url="http://<target_sf_dns>:5110" />
  </Servers>
```

Modify the **ServiceManifest.xml** files of the eShop SF Services and set the **UseLoadTest** environment variable to True. This setting enables the load tests to bypass authorization in api services.

<p>
<img src="../../../img/loadtests/sfmanifestsettings.png">
<p>

Deploy the SF services. **PLEASE** Read our [SF deployment guide for Linux](./../../../deploy/az/servicefabric/LinuxContainers/readme.md) And [SF deployment guide for Windows](./../../../deploy/az/servicefabric/WindowsContainers/readme.md) to know about how to deploy eshop on SF.

## Set a Kubernetes environment

Modify the **app.config** file in the LoadTest project directory and set the following service urls.

```
<Servers>
    <MvcWebServer url="http://<public_ip_k8s>/webmvc" />
    <CatalogApiServer url="http://<public_ip_k8s>/catalog-api" />
    <OrderingApiServer url="http://<public_ip_k8s>/ordering-api" />
    <BasketApiServer url="http://<public_ip_k8s>/basket-api" />
    <IdentityApiServer url="http://<public_ip_k8s>/identity" />
    <LocationsApiServer url="http://<public_ip_k8s>/locations-api" />
    <MarketingApiServer url="http://<public_ip_k8s>/marketing-api" />
  </Servers>
```

Modify the **conf_local.yml** file in the K8s directory and set the **EnableLoadTest** environment variable to True. This setting enables the load tests to bypass authorization in api services.

<p>
<img src="../../../img/loadtests/k8ssettings.png">
<p>

Deploy the kubernetes services. **PLEASE** Read our [k8s deployment guide](./../../../k8s/README.k8s.md) to know about how to deploy eshop on Kubernetes.

## Run Load Tests

Open the load test you want to perform ***.loadtest** files and click the Run Load test button.

<p>
<img src="./../../../img/loadtests/runloadtest.png">
<p>