Param(
    [parameter(Mandatory=$true)][string]$registry
)

if ([String]::IsNullOrEmpty($registry)) {
    Write-Host "Registry must be set to docker registry to use" -ForegroundColor Red
    exit 1 
}

Write-Host "This script creates the local manifests, for pushing the multi-arch manifests" -ForegroundColor Yellow
Write-Host "Tags used are linux-master, win-master, linux-dev, win-dev, linux-latest, win-latest" -ForegroundColor Yellow
Write-Host "Multiarch images tags will be master, dev, latest" -ForegroundColor Yellow


$services = "identity.api", "basket.api", "catalog.api", "ordering.api", "ordering.backgroundtasks", "marketing.api", "payment.api", "locations.api", "webhooks.api", "ocelotapigw", "mobileshoppingagg", "webshoppingagg", "ordering.signalrhub", "webstatus", "webspa", "webmvc", "webhooks.client"

foreach ($svc in $services) {
    Write-Host "Creating manifest for $svc and tags :latest, :master, and :dev"
    docker manifest create $registry/${svc}:master $registry/${svc}:linux-master $registry/${svc}:win-master
    docker manifest create $registry/${svc}:dev $registry/${svc}:linux-dev $registry/${svc}:win-dev
    docker manifest create $registry/${svc}:latest $registry/${svc}:linux-latest $registry/${svc}:win-latest
    Write-Host "Pushing manifest for $svc and tags :latest, :master, and :dev"
    docker manifest push $registry/${svc}:latest
    docker manifest push $registry/${svc}:dev
    docker manifest push $registry/${svc}:master
}