Param(
    [parameter(Mandatory=$true)][string]$registry,
    [parameter(Mandatory=$true)][string]$dockerUser,
    [parameter(Mandatory=$true)][string]$dockerPassword
)

$requiredCommands = ("docker", "docker-compose", "kubectl")
foreach ($command in $requiredCommands) {
    if ((Get-Command $command -ErrorAction SilentlyContinue) -eq $null) {
        Write-Host "$command must be on path" -ForegroundColor Red
        exit
    }
}

Write-Host "Logging in to $registry" -ForegroundColor Yellow
docker login -u $dockerUser -p $dockerPassword $registry
if (-not $LastExitCode -eq 0) {
    Write-Host "Login failed" -ForegroundColor Red
    exit
}

# create registry key secret
kubectl create secret docker-registry registry-key `
    --docker-server=$registry `
    --docker-username=$dockerUser `
    --docker-password=$dockerPassword `
    --docker-email=not@used.com

# start sql and frontend deployments
kubectl create configmap config-files --from-file=nginx-conf=nginx.conf
kubectl label configmap config-files app=eshop
kubectl create -f sql-data.yaml -f services.yaml -f frontend.yaml

Write-Host "Building solution..." -ForegroundColor Yellow
../cli-windows/build-bits-simple.ps1

Write-Host "Building Docker images..." -ForegroundColor Yellow
docker-compose -p .. -f ../docker-compose.yml build

Write-Host "Pushing images to $registry..." -ForegroundColor Yellow
$services = ("basket.api", "catalog.api", "identity.api", "ordering.api", "webmvc", "webspa")
foreach ($service in $services) {
    docker tag eshop/$service $registry/$service
    docker push $registry/$service
}

Write-Host "Waiting for frontend's external ip..." -ForegroundColor Yellow
while ($true) {
    $frontendUrl = kubectl get svc frontend -o=jsonpath="{.status.loadBalancer.ingress[0].ip}" 2> $_
    if ([bool]($frontendUrl -as [ipaddress])) {
        break
    }
    Start-Sleep -s 15
}

kubectl create configmap urls `
    --from-literal=BasketUrl=http://$($frontendUrl)/basket-api `
    --from-literal=CatalogUrl=http://$($frontendUrl)/catalog-api `
    --from-literal=IdentityUrl=http://$($frontendUrl)/identity `
    --from-literal=OrderingUrl=http://$($frontendUrl)/ordering-api `
    --from-literal=MvcClient=http://$($frontendUrl)/webmvc `
    --from-literal=SpaClient=http://$($frontendUrl)
kubectl label configmap urls app=eshop

# TODO verify database readiness?
Write-Host "Creating deployments..."
kubectl apply -f deployments.yaml

# update deployments with the private registry
# (deployment templating, or Helm, would obviate this)
kubectl set image -f deployments.yaml `
    basket=$registry/basket.api `
    catalog=$registry/catalog.api `
    identity=$registry/identity.api `
    ordering=$registry/ordering.api `
    webmvc=$registry/webmvc `
    webspa=$registry/webspa
kubectl rollout resume -f deployments.yaml

Write-Host "WebSPA is exposed at http://$frontendUrl, WebMVC at http://$frontendUrl/webmvc" -ForegroundColor Yellow
