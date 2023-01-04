#!/bin/bash

# Color theming
. <(cat ../../../../infrastructure/scripts/theme.sh)

## Add the discount coupon field in the checkout view.
echo "Uncommenting HTML in src/Web/WebSPA/Client/src/modules/orders/orders-new/orders-new.component.html..."
sed -i -E "/DISCOUNT-COUPON-COMMENT/s/<!--DISCOUNT-COUPON-COMMENT\*\*(.*)-->/\1/" ../../src/Web/WebSPA/Client/src/modules/orders/orders-new/orders-new.component.html

## Show the discount amount in the order details view.
echo "Uncommenting HTML in src/Web/WebSPA/Client/src/modules/orders/orders-detail/orders-detail.component.html..."
sed -i -E "/DISCOUNT-COUPON-COMMENT/s/<!--DISCOUNT-COUPON-COMMENT\*\*(.*)-->/\1/" ../../src/Web/WebSPA/Client/src/modules/orders/orders-detail/orders-detail.component.html

echo "Creating a Helm chart (with templates) for the coupon service in deploy/k8s/helm-simple/coupon..."
## Add the Helm chart to deploy the coupon service to AKS.
mkdir helm-simple/coupon
mkdir helm-simple/coupon/templates

# helm-simple/coupon/Chart.yaml
cat >helm-simple/coupon/Chart.yaml <<EOL
apiVersion: v2
name: coupon
description: A Helm chart for Kubernetes

# A chart can be either an 'application' or a 'library' chart.
#
# Application charts are a collection of templates that can be packaged into versioned archives
# to be deployed.
#
# Library charts provide useful utilities or functions for the chart developer. They're included as
# a dependency of application charts to inject those utilities and functions into the rendering
# pipeline. Library charts do not define any templates and therefore cannot be deployed.
type: application

# This is the chart version. This version number should be incremented each time you make changes
# to the chart and its templates, including the app version.
version: 0.1.0

# This is the version number of the application being deployed. This version number should be
# incremented each time you make changes to the application.
appVersion: 1.0.0
EOL

# helm-simple/coupon/templates/deployment.yaml
cat >helm-simple/coupon/templates/deployment.yaml <<EOL
kind: Deployment
apiVersion: apps/v1
metadata:
  name: coupon
  labels:
    app: eshop
    service: coupon
spec:
  replicas: 1
  selector:
    matchLabels:
      service: coupon
  template:
    metadata:
      labels:
        app: eshop
        service: coupon
    spec:
      containers:
        - name: coupon-api
          image: {{ .Values.registry }}/coupon.api:linux-net6-initial
          imagePullPolicy: Always
          ports:
            - containerPort: 80
              protocol: TCP
            - containerPort: 81
              protocol: TCP
          livenessProbe:
            httpGet:
              port: 80
              path: /liveness
            initialDelaySeconds: 10
            periodSeconds: 15
          readinessProbe:
            httpGet:
              port: 80
              path: /hc
            initialDelaySeconds: 90
            periodSeconds: 60
            timeoutSeconds: 5
          envFrom:
            - configMapRef:
                name: coupon-cm
EOL

# helm-simple/coupon/templates/service.yaml
cat >helm-simple/coupon/templates/service.yaml <<EOL
kind: Service
apiVersion: v1
metadata:
  name: coupon-api
  labels:
    app: eshop
    service: coupon
spec:
  ports:
    - port: 80
      protocol: TCP
      name: http
  selector:
    service: coupon
EOL

# helm-simple/coupon/templates/configmap.yaml
cat >helm-simple/coupon/templates/configmap.yaml <<EOL
kind: ConfigMap
apiVersion: v1
metadata:
  name: coupon-cm
  labels:
    app: eshop
    service: coupon
data:
  ASPNETCORE_ENVIRONMENT: Development
  ASPNETCORE_URLS: http://0.0.0.0:80
  AzureServiceBusEnabled: "False"
  CheckUpdateTime: "30000"
  ConnectionString: mongodb://nosqldata
  EventBusConnection: rabbitmq
  identityUrl: http://identity-api
  OrchestratorType: K8S
  PATH_BASE: /coupon-api
  Serilog__MinimumLevel__Override__coupon-api: Verbose
  Serilog__MinimumLevel__Override__Microsoft.eShopOnContainers.BuildingBlocks.EventBusRabbitMQ: Verbose
EOL

# helm-simple/coupon/templates/ingress.yaml
cat >helm-simple/coupon/templates/ingress.yaml <<EOL
kind: Ingress
apiVersion: networking.k8s.io/v1
metadata:
  name: coupon
  labels:
    app: eshop
    service: coupon
  annotations:
    kubernetes.io/ingress.class: "nginx"
spec:
  rules:
    - http:
        paths:
        - path: /coupon-api
          pathType: Prefix
          backend:
            service:
              name: coupon-api
              port:
                number: 80
EOL


## Add the coupon service endpoints in the aggregator
echo "Adding coupon service endpoints to the API gateway in deploy/k8s/helm-simple/webshoppingagg/templates/configmap.yaml..."
sed -i -E "/DISCOUNT-COUPON-COMMENT/s/#DISCOUNT-COUPON-COMMENT\*\*//" helm-simple/webshoppingagg/templates/configmap.yaml

## Add the coupon service as a health check item in the webstatus application
echo "Adding coupon service as a WebStatus health check item in deploy/k8s/helm-simple/webstatus/templates/configmap.yaml..."
sed -i -E "/DISCOUNT-COUPON-COMMENT/s/#DISCOUNT-COUPON-COMMENT\*\*//" helm-simple/webstatus/templates/configmap.yaml

echo " "
echo "Implementation script done!"
