kubectl patch deployment -n ingress-nginx nginx-ingress-controller --type=json --patch="$(cat nginx-ingress$([IO.Path]::DirectorySeparatorChar)publish-service-patch.yaml)"
kubectl apply -f nginx-ingress$([IO.Path]::DirectorySeparatorChar)azure$([IO.Path]::DirectorySeparatorChar)service.yaml
kubectl apply -f nginx-ingress$([IO.Path]::DirectorySeparatorChar)patch-service-without-rbac.yaml
