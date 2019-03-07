$ISTIO_VERSION="1.0.6"
cd istio-$ISTIO_VERSION
<<<<<<< HEAD
helm install install/kubernetes/helm/istio --wait --name istio --namespace istio-system --set global.controlPlaneSecurityEnabled=true --set grafana.enabled=true --set tracing.enabled=true --set kiali.enabled=true --set ingress.enabled=false --set gateways.istio-ingressgateway.enabled=false
cd ..
=======
helm install install/kubernetes/helm/istio --name istio --namespace istio-system --set global.controlPlaneSecurityEnabled=true --set grafana.enabled=true --set tracing.enabled=true --set kiali.enabled=true
>>>>>>> abb108e03dd0ddf0b43d7ea4b2f5b307452ac88e
