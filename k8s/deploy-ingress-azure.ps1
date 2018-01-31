kubectl patch deployment -n ingress-nginx nginx-ingress-controller --type=json --patch="$(cat nginx-ingress\publish-service-patch.yaml)"
kubectl apply -f nginx-ingress\azure\service.yaml
kubectl apply -f nginx-ingress\patch-service-without-rbac.yaml