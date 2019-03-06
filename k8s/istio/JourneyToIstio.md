# ISTIO on Local
## Prerequisites on local
You need the eshopsOnContainers configured on your local, with this  
in a powershell console, we need to enter in /k8s/istio and execute
```
>kubectl get pods
NAME                                             READY     STATUS             RESTARTS   AGE
eshop-apigwmm-54ccc6c589-557fn                   0/1       Running            26         3h
eshop-apigwms-7d5f86cf7c-2j2zp                   0/1       CrashLoopBackOff   30         3h
eshop-apigwwm-7794b6d879-7j4mt                   0/1       CrashLoopBackOff   39         3h
eshop-apigwws-8585f6899f-7kkg2                   0/1       Running            11         3h
eshop-basket-api-8bfc5c5f6-8xxcv                 0/1       Running            41         3h
eshop-basket-data-66fbc788cc-dmkgb               1/1       Running            0          3h
eshop-catalog-api-c77747b76-4gp6c                0/1       CrashLoopBackOff   40         3h
eshop-identity-api-7574f6b458-4rbp6              0/1       CrashLoopBackOff   44         3h
eshop-keystore-data-5c9c85cb99-s5qz7             1/1       Running            0          3h
eshop-locations-api-64847646d-5wv52              0/1       CrashLoopBackOff   36         3h
eshop-marketing-api-745f9546b8-krjqq             0/1       Running            33         3h
eshop-mobileshoppingagg-7d467f86bd-bw9c7         0/1       Running            22         3h
eshop-nosql-data-579c9d89f8-x4z2k                1/1       Running            0          3h
eshop-ordering-api-5c55bd5464-7hnjx              0/1       CrashLoopBackOff   38         3h
eshop-ordering-backgroundtasks-f6dcb7db4-xq7gr   1/1       Running            22         3h
eshop-ordering-signalrhub-6664868779-dphxm       1/1       Running            0          3h
eshop-payment-api-7988db5f76-z76tc               1/1       Running            17         3h
eshop-rabbitmq-6b68647bc4-qjjrb                  1/1       Running            0          3h
eshop-sql-data-5c4fdcccf4-2z5dm                  1/1       Running            0          3h
eshop-webhooks-api-588b58bb66-lmx5c              1/1       Running            0          3h
eshop-webhooks-web-565c68b59c-dk8hp              1/1       Running            0          3h
eshop-webmvc-55c596544b-9fqsj                    1/1       Running            0          3h
eshop-webshoppingagg-f8547f45b-4mjvp             0/1       CrashLoopBackOff   16         3h
eshop-webspa-84fd54466d-hzrlb                    1/1       Running            0          3h
eshop-webstatus-775b487d4d-tbfbn                 1/1       Running            0          3h
```

```ps1
> ./install-istio-local.ps1

```
This will install the cli utility and register to the path, you can test this phase launching

```ps1
> istioctl
```

Afterthat you can install Istio on your cluster executing
```ps1
> ./deploy-istio-helm.ps1
```

the result should be like:
```
NAME                                      READY     STATUS             RESTARTS   AGE
grafana-774bf8cb47-clqkp                  1/1       Running            0          2h
istio-citadel-548f4cdd9-dbrbn             1/1       Running            0          2h
istio-egressgateway-5f77f6c979-8922g      1/1       Running            0          2h
istio-galley-8f6585898-7c7wq              1/1       Running            0          2h
istio-ingressgateway-8484579cdb-7tw8n     1/1       Running            0          2h
istio-pilot-7c5c5778fb-r987v              2/2       Running            0          2h
istio-policy-7d67d47c65-rdqwj             2/2       Running            15         2h
istio-sidecar-injector-6fb6845cdd-nnhks   1/1       Running            0          2h
istio-telemetry-8b9fc7769-pwx5m           2/2       Running            24         2h
istio-tracing-ff94688bb-xnhnd             1/1       Running            4          2h
kiali-8644dbcdbc-pb627                    0/1       CrashLoopBackOff   7          2h
prometheus-f556886b8-mr6wb                1/1       Running            13         2h
```

Is a common error that kiali-pod have errors, because it needs a credentials for working.
enter in k8s/istio/kiali and execute:
```
> ./set-kiali-credentials.ps1
```
this script will prompt for a valid account/password and setups the secret in kubernetes
(at the moment account/password will be admin/admin we need to modify the yml)
