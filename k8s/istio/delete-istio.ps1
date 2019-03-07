helm delete --purge istio
kubectl delete -f install/kubernetes/helm/istio/templates/crds.yaml -n istio-system