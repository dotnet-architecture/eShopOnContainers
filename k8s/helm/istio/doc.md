# Using Helm Charts to deploy eShopOnContainers to AKS with ISTIO

It is possible to deploy eShopOnContainers on a AKS using [Helm](https://helm.sh/) instead of custom scripts (that will be deprecated soon).

## Create Kubernetes cluster in AKS
You can create the AKS cluster by using two ways:

- A. Use Azure CLI: Follow a procedure suing [Azure CLI like here](https://docs.microsoft.com/en-us/azure/aks/kubernetes-walkthrough), but make sure you **enable RBAC** with `--enable-rbac` in `az aks create` command.

- B. Use Azure's portal

The following steps are using the Azure portal to create the AKS cluster:

- Start the process by providing the general data, like in the following screenshot:

![image](https://user-images.githubusercontent.com/1712635/45787360-c59ecd80-bc29-11e8-9565-c989ad6ad57b.png)

- Then, very important, in the next step, enable RBAC:

![image](https://user-images.githubusercontent.com/1712635/45780917-8bc2cc80-bc13-11e8-87ac-2942b3c7496d.png)

    You can use **basic network** settings since for a test you don't need integration into any existing VNET.

![image](https://user-images.githubusercontent.com/1712635/45780991-b745b700-bc13-11e8-926b-afac57229d0a.png)

- You can also enable monitoring:

![image](https://user-images.githubusercontent.com/1712635/45781148-1277a980-bc14-11e8-8614-f7a239731bec.png)

- Finally, create the cluster. It'll take a few minutes for it to be ready.

### Configure RBAC security for K8s dashboard service-account

In order NOT to get errors in the Kubernetes dashboard, you'll need to set the following service-account steps.

Here you can see the errors you might see:
![image](https://user-images.githubusercontent.com/1712635/45784384-5622e100-bc1d-11e8-8d33-e22fd955150a.png)

Now, just run the Azure CLI command to browse the Kubernetes Dashboard:

`az aks browse --resource-group pro-eshop-aks-helm-linux-resgrp --name pro-eshop-aks-helm-linux`

![image](https://user-images.githubusercontent.com/1712635/45786406-2d9ee500-bc25-11e8-83e9-bdfc302e80f1.png)


## Additional pre-requisites

In addition to having an AKS cluster created in Azure and having kubectl and Azure CLI installed in your local machine and configured to use your Azure subscription, you also need the following pre-requisites:

### Install Helm 

You need to have helm installed on your machine, and Tiller must be installed on the AKS. Follow these instructions on how to ['Install applications with Helm in Azure Kubernetes Service (AKS)'](https://docs.microsoft.com/en-us/azure/aks/kubernetes-helm) to setup Helm and Tiller for AKS.

**Note**: If your ASK cluster is not RBAC-enabled (default option in portal) you may receive following error when running a helm command:

```
Error: Get http://localhost:8080/api/v1/namespaces/kube-system/configmaps?labelSelector=OWNER%!D(MISSING)TILLER: dial tcp [::1]:8080: connect: connection refused
```

If so, type:

```
kubectl --namespace=kube-system edit deployment/tiller-deploy
```

Your default text editor will popup with the YAML definition of the tiller deploy. Search for:

```
automountServiceAccountToken: false
```

And change it to:

```
automountServiceAccountToken: true
```

Save the file and close the editor. This should reapply the deployment in the cluster. Now Helm commands should work.

## Install eShopOnContainers with Istio using Helm

All steps need to be performed on `/k8s/helm` folder. The easiest way is to use the `deploy-all-istio.ps1` script from a Powershell window:

```
.\deploy-all-istio.ps1 -dnsname eshoptestistio -externalDns aks -aksName eshoptest -aksRg eshoptest -imageTag dev
```

This will install all the [eShopOnContainers public images](https://hub.docker.com/u/eshop/) with tag `dev` on the AKS named `eshoptest` in the resource group `eshoptest` and with the dns url: http://**eshoptestistio**.westus.cloudapp.azure.com/ . By default all infrastructure (sql, mongo, rabbit and redis) is installed also in the cluster.

Once the script is run, you should see following output when using `kubectl get deployment`:

```
NAME                             DESIRED   CURRENT   UP-TO-DATE   AVAILABLE   AGE
eshop-apigwmm                    1         1         1            1           4d
eshop-apigwms                    1         1         1            1           4d
eshop-apigwwm                    1         1         1            1           4d
eshop-apigwws                    1         1         1            1           4d
eshop-basket-api                 1         1         1            1           4d
eshop-basket-data                1         1         1            1           4d
eshop-catalog-api                1         1         1            1           4d
eshop-identity-api               1         1         1            1           4d
eshop-keystore-data              1         1         1            1           4d
eshop-locations-api              1         1         1            1           4d
eshop-marketing-api              1         1         1            1           4d
eshop-mobileshoppingagg          1         1         1            1           4d
eshop-nosql-data                 1         1         1            1           4d
eshop-ordering-api               1         1         1            1           4d
eshop-ordering-backgroundtasks   1         1         1            1           4d
eshop-ordering-signalrhub        1         1         1            1           4d
eshop-payment-api                1         1         1            1           4d
eshop-rabbitmq                   1         1         1            1           4d
eshop-sql-data                   1         1         1            1           4d
eshop-webmvc                     1         1         1            1           4d
eshop-webshoppingagg             1         1         1            1           4d
eshop-webspa                     1         1         1            1           4d
eshop-webstatus                  1         1         1            1           4d
```

Every public service is exposed through the istio ingress gateway.
Yo can see the ingress gateway public ip doing `kubectl get services -n istio-system`
```
grafana                  ClusterIP      10.0.204.87    <none>           3000/TCP                                                                                                                  1h
istio-citadel            ClusterIP      10.0.23.86     <none>           8060/TCP,9093/TCP                                                                                                         1h
istio-egressgateway      ClusterIP      10.0.136.169   <none>           80/TCP,443/TCP                                                                                                            1h
istio-galley             ClusterIP      10.0.113.51    <none>           443/TCP,9093/TCP                                                                                                          1h
istio-ingressgateway     LoadBalancer   10.0.76.80     40.118.189.161   80:31380/TCP,443:31390/TCP,31400:31400/TCP,15011:31276/TCP,8060:30519/TCP,853:31698/TCP,15030:31453/TCP,15031:32362/TCP   1h
istio-pilot              ClusterIP      10.0.164.253   <none>           15010/TCP,15011/TCP,8080/TCP,9093/TCP                                                                                     1h
istio-policy             ClusterIP      10.0.170.49    <none>           9091/TCP,15004/TCP,9093/TCP                                                                                               1h
istio-sidecar-injector   ClusterIP      10.0.251.12    <none>           443/TCP                                                                                                                   1h
istio-telemetry          ClusterIP      10.0.195.112   <none>           9091/TCP,15004/TCP,9093/TCP,42422/TCP                                                                                     1h
jaeger-agent             ClusterIP      None           <none>           5775/UDP,6831/UDP,6832/UDP                                                                                                1h
jaeger-collector         ClusterIP      10.0.123.98    <none>           14267/TCP,14268/TCP                                                                                                       1h
jaeger-query             ClusterIP      10.0.244.146   <none>           16686/TCP                                                                                                                 1h
kiali                    ClusterIP      10.0.182.12    <none>           20001/TCP                                                                                                                 1h
prometheus               ClusterIP      10.0.136.223   <none>           9090/TCP                                                                                                                  1h
tracing                  ClusterIP      10.0.57.236    <none>           80/TCP                                                                                                                    1h
zipkin                   ClusterIP      10.0.30.57     <none>           9411/TCP                                                                                                                  1h
```

You can view the MVC client at http://[dns]/

## Customizing the deployment

### Using your own images

To use your own images instead of the public ones, you have to pass following additional parameters to the `deploy-all-istio.ps1` script:

* `registry`: Login server for the Docker registry
* `dockerUser`: User login for the Docker registry
* `dockerPassword`: User password for the Docker registry

This will deploy a secret on the cluster to connect to the specified server, and all image names deployed will be prepended with `registry/` value.

### Not deploying infrastructure containers

If you want to use external resources, use `-deployInfrastructure $false` to not deploy infrastructure containers. However **you still have to manually update the scripts to provide your own configuration** (see next section).

### Providing your own configuration

The file `inf.yaml` contains the description of the infrastructure used. File is docummented so take a look on it to understand all of its entries. If using external resources you need to edit this file according to your needs. You'll need to edit:

* `inf.sql.host` with the host name of the SQL Server
* `inf.sql.common` entries to provide your SQL user, password. `Pid` is not used when using external resources (it is used to set specific product id for the SQL Server container).
* `inf.sql.catalog`, `inf.sql.ordering`, `inf.sql.identity`: To provide the database names for catalog, ordering and identity services
* `mongo.host`: With the host name of the Mongo DB
* `mongo.locations`, `mongo.marketing` with the database names for locations and marketing services
* `redis.basket.constr` with the connection string to Redis for Basket Service. Note that `redis.basket.svc` is not used when using external services
* `redis.keystore.constr` with the connection string to Redis for Keystore Service. Note that `redis.keystore.svc` is not used when using external services
* `eventbus.constr` with the connection string to Azure Service Bus and `eventbus.useAzure` to `true` to use Azure service bus. Note that `eventbus.svc` is not used when using external services

### Using Azure storage for Catalog Photos

Using Azure storage for catalog (and marketing) photos is not directly supported, but you can accomplish it by editing the file `k8s/helm/catalog-api/templates/configmap.yaml`. Search for lines:

```
catalog__PicBaseUrl: http://{{ $webshoppingapigw }}/api/v1/c/catalog/items/[0]/pic/
```

And replace it for:

```
catalog__PicBaseUrl: http://<url-of-the-storage>/
```

In the same way, to use Azure storage for the marketing service, have to edit the file `k8s/helm/marketing-api/templates/configmap.yaml` and replacing the line:

```
marketing__PicBaseUrl: http://{{ $webshoppingapigw }}/api/v1/c/catalog/items/[0]/pic/
```

by:

```
marketing__PicBaseUrl: http://<url-of-the-storage>/
```

# Using Helm Charts to deploy eShopOnContainers to a local Kubernetes in Windows with 'Docker for Windows'

## Additional pre-requisites

In addition to having Docker for Windows/Mac with Kubernetes enabled and having kubectl ayou also need the following pre-requisites:

### Install Helm 

You need to have helm installed on your machine, and Tiller must be installed on the local Docker Kubernetes cluster. Once you have [Helm downloaded](https://helm.sh/) and installed on your machine you must:

1. Create the tiller service account, by running `kubectl apply -f helm-rbac.yaml` from `/k8s` folder
2. Install tiller and configure it to use the tiller service account by typing `helm init --service-account tiller`

## Install eShopOnContainers with Istio using Helm 

All steps need to be performed on `/k8s/helm` folder. The easiest way is to use the `deploy-all-istio.ps1` script from a Powershell window:

```
.\deploy-all-istio.ps1 -imageTag dev -useLocalk8s $true 
```

The parameter `useLocalk8s` to $true, forces the script to use `localhost` as the DNS for all Helm charts.

This will install all the [eShopOnContainers public images](https://hub.docker.com/u/eshop/) with tag `dev` on the Docker local Kubernetes cluster. By default all infrastructure (sql, mongo, rabbit and redis) is installed also in the cluster.

Once the script is run, you should see following output when using `kubectl get deployment`:

```
NAME                             DESIRED   CURRENT   UP-TO-DATE   AVAILABLE   AGE
eshop-apigwmm                    1         1         1            1           2h
eshop-apigwms                    1         1         1            1           2h
eshop-apigwwm                    1         1         1            1           2h
eshop-apigwws                    1         1         1            1           2h
eshop-basket-api                 1         1         1            1           2h
eshop-basket-data                1         1         1            1           2h
eshop-catalog-api                1         1         1            1           2h
eshop-identity-api               1         1         1            1           2h
eshop-keystore-data              1         1         1            1           2h
eshop-locations-api              1         1         1            1           2h
eshop-marketing-api              1         1         1            1           2h
eshop-mobileshoppingagg          1         1         1            1           2h
eshop-nosql-data                 1         1         1            1           2h
eshop-ordering-api               1         1         1            1           2h
eshop-ordering-backgroundtasks   1         1         1            1           2h
eshop-ordering-signalrhub        1         1         1            1           2h
eshop-payment-api                1         1         1            1           2h
eshop-rabbitmq                   1         1         1            1           2h
eshop-sql-data                   1         1         1            1           2h
eshop-webmvc                     1         1         1            1           2h
eshop-webshoppingagg             1         1         1            1           2h
eshop-webspa                     1         1         1            1           2h
eshop-webstatus                  1         1         1            1           2h
```

Note that istio ingress gateway is bound to DNS localhost and the host is also "localhost". So, you can access the webspa by typing `http://localhost` and the MVC by typing `http://localhost/`

As this is the Docker local K8s cluster, you can see also the containers running on your machine. If you type `docker ps` you'll see all them:

```
CONTAINER ID        IMAGE                            COMMAND                  CREATED             STATUS              PORTS               NAMES
fec1e3499416        a3f21ec4bd11                     "/entrypoint.sh /ngi…"   9 minutes ago       Up 9 minutes                            k8s_nginx-ingress-controller_nginx-ingress-controller-f88c75bc6-5xs2n_ingress-nginx_f1cc7094-e68f-11e8-b4b6-00155d016146_0
76485867f032        eshop/payment.api                "dotnet Payment.API.…"   2 hours ago         Up 2 hours                              k8s_payment-api_eshop-payment-api-75d5f9bdf6-6zx2v_default_4a3cdab4-e67f-11e8-b4b6-00155d016146_1
c2c4640ed610        eshop/marketing.api              "dotnet Marketing.AP…"   2 hours ago         Up 2 hours                              k8s_marketing-api_eshop-marketing-api-6b8c5989fd-jpxqv_default_45780626-e67f-11e8-b4b6-00155d016146_1
85301d538574        eshop/ordering.signalrhub        "dotnet Ordering.Sig…"   2 hours ago         Up 2 hours                              k8s_ordering-signalrhub_eshop-ordering-signalrhub-58cf5ff6-cnlm8_default_4932c344-e67f-11e8-b4b6-00155d016146_1
7a408a98000e        eshop/ordering.backgroundtasks   "dotnet Ordering.Bac…"   2 hours ago         Up 2 hours                              k8s_ordering-backgroundtasks_eshop-ordering-backgroundtasks-cc8f6d4d8-ztfk7_default_47f9cf10-e67f-11e8-b4b6-00155d016146_1
12c64b3a13e0        eshop/basket.api                 "dotnet Basket.API.d…"   2 hours ago         Up 2 hours                              k8s_basket-api_eshop-basket-api-658546684d-6hlvd_default_4262d022-e67f-11e8-b4b6-00155d016146_1
133fccfeeff3        eshop/webstatus                  "dotnet WebStatus.dll"   2 hours ago         Up 2 hours                              k8s_webstatus_eshop-webstatus-7f46479dc4-bqnq7_default_4dc13eb2-e67f-11e8-b4b6-00155d016146_0
00c6e4c52135        eshop/webspa                     "dotnet WebSPA.dll"      2 hours ago         Up 2 hours                              k8s_webspa_eshop-webspa-64cb8df9cb-dcbwg_default_4cd47376-e67f-11e8-b4b6-00155d016146_0
d4507f1f6b1a        eshop/webshoppingagg             "dotnet Web.Shopping…"   2 hours ago         Up 2 hours                              k8s_webshoppingagg_eshop-webshoppingagg-cc94fc86-sxd2v_default_4be6cdb9-e67f-11e8-b4b6-00155d016146_0
9178e26703da        eshop/webmvc                     "dotnet WebMVC.dll"      2 hours ago         Up 2 hours                              k8s_webmvc_eshop-webmvc-985779684-4br5z_default_4addd4d6-e67f-11e8-b4b6-00155d016146_0
1088c281c710        eshop/ordering.api               "dotnet Ordering.API…"   2 hours ago         Up 2 hours                              k8s_ordering-api_eshop-ordering-api-fb8c548cb-k68x9_default_4740958a-e67f-11e8-b4b6-00155d016146_0
12424156d5c9        eshop/mobileshoppingagg          "dotnet Mobile.Shopp…"   2 hours ago         Up 2 hours                              k8s_mobileshoppingagg_eshop-mobileshoppingagg-b54645d7b-rlrgh_default_46c00017-e67f-11e8-b4b6-00155d016146_0
65463ffd437d        eshop/locations.api              "dotnet Locations.AP…"   2 hours ago         Up 2 hours                              k8s_locations-api_eshop-locations-api-577fc94696-dfhq8_default_44929c4b-e67f-11e8-b4b6-00155d016146_0
5b3431873763        eshop/identity.api               "dotnet Identity.API…"   2 hours ago         Up 2 hours                              k8s_identity-api_eshop-identity-api-85d9b79f4-s5ks7_default_43d6eb7c-e67f-11e8-b4b6-00155d016146_0
7c8e77252459        eshop/catalog.api                "dotnet Catalog.API.…"   2 hours ago         Up 2 hours                              k8s_catalog-api_eshop-catalog-api-59fd444fb-ztvhz_default_4356705a-e67f-11e8-b4b6-00155d016146_0
94d95d0d3653        eshop/ocelotapigw                "dotnet OcelotApiGw.…"   2 hours ago         Up 2 hours                              k8s_apigwws_eshop-apigwws-65474b979d-n99jw_default_41395473-e67f-11e8-b4b6-00155d016146_0
bc4bbce71d5f        eshop/ocelotapigw                "dotnet OcelotApiGw.…"   2 hours ago         Up 2 hours                              k8s_apigwwm_eshop-apigwwm-857c549dd8-8w5gv_default_4098d770-e67f-11e8-b4b6-00155d016146_0
840aabcceaa9        eshop/ocelotapigw                "dotnet OcelotApiGw.…"   2 hours ago         Up 2 hours                              k8s_apigwms_eshop-apigwms-5b94dfb54b-dnmr9_default_401fc611-e67f-11e8-b4b6-00155d016146_0
aabed7646f5b        eshop/ocelotapigw                "dotnet OcelotApiGw.…"   2 hours ago         Up 2 hours                              k8s_apigwmm_eshop-apigwmm-85f96cbdb4-dhfwr_default_3ed7967a-e67f-11e8-b4b6-00155d016146_0
49c5700def5a        f06a5773f01e                     "docker-entrypoint.s…"   2 hours ago         Up 2 hours                              k8s_basket-data_eshop-basket-data-66fbc788cc-csnlw_default_3e0c45fe-e67f-11e8-b4b6-00155d016146_0
a5db4c521807        f06a5773f01e                     "docker-entrypoint.s…"   2 hours ago         Up 2 hours                              k8s_keystore-data_eshop-keystore-data-5c9c85cb99-8k56s_default_3ce1a273-e67f-11e8-b4b6-00155d016146_0
aae88fd2d810        d69a5113ceae                     "docker-entrypoint.s…"   2 hours ago         Up 2 hours                              k8s_rabbitmq_eshop-rabbitmq-6b68647bc4-gr565_default_3c37ee6a-e67f-11e8-b4b6-00155d016146_0
65d49ca9589d        bbed8d0e01c1                     "docker-entrypoint.s…"   2 hours ago         Up 2 hours                              k8s_nosql-data_eshop-nosql-data-579c9d89f8-mtt95_default_3b9c1f89-e67f-11e8-b4b6-00155d016146_0
090e0dde2ec4        bbe2822dfe38                     "/opt/mssql/bin/sqls…"   2 hours ago         Up 2 hours                              k8s_sql-data_eshop-sql-data-5c4fdcccf4-bscdb_default_3afd29b8-e67f-11e8-b4b6-00155d016146_0
```

## Known issues

Login from the webmvc results in following error: HttpRequestException: Response status code does not indicate success: 404 (Not Found).

The reason is because MVC needs to access the Identity Server from both outside the container (browser) and inside the container (C# code). Thus, the configuration uses always the *external url* of the Identity Server, which in this case is just `http://localhost/identity-api`. But this external url is incorrect when used from C# code, and the web mvc can't access the identity api. This is the only case when this issue happens (and is the reason why we use 10.0.75.1 for local address in web mvc in local development mode)

Solving this requires some manual steps:

Update the configmap of Web MVC by typing (**line breaks are mandatory**) and your cluster dns name has to be the same of your environment:

```
kubectl patch cm cfg-eshop-webmvc --type strategic --patch @'
data:
  urls__IdentityUrl: http://**eshoptest**.westus.cloudapp.azure.com/identity
  urls__mvc: http://**eshoptest**.westus.cloudapp.azure.com/webmvc
'@
```

Update the configmap of Identity API by typing (**line breaks are mandatory**):

```
kubectl patch cm cfg-eshop-identity-api --type strategic --patch @'
data:
  mvc_e: http://**eshoptest**.westus.cloudapp.azure.com/webmvc
'@
```

Restart the SQL Server pod to ensure the database is recreated again:

```
kubectl delete pod --selector app=sql-data
``` 

Wait until SQL Server pod is ready to accept connections and then restart all other pods:

```
kubectl delete pod --selector="app!=sql-data"
```

**Note:** Pods are deleted to ensure the databases are recreated again, as identity api stores its client names and urls in the database.

Now, you can access the MVC app using: `http://**eshoptest**.westus.cloudapp.azure.com/`.

