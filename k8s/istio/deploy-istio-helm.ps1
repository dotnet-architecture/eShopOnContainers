$ISTIO_VERSION="1.0.6"
cd istio-$ISTIO_VERSION
helm install install/kubernetes/helm/istio --wait --name istio --namespace istio-system --set global.controlPlaneSecurityEnabled=true --set grafana.enabled=true --set tracing.enabled=true --set kiali.enabled=true --set ingress.enabled=false --set gateways.istio-ingressgateway.enabled=false
cd ..