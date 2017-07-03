Param(
    [parameter(Mandatory=$false)][string]$registry,
    [parameter(Mandatory=$false)][string]$dockerUser,
    [parameter(Mandatory=$false)][string]$dockerPassword,
    [parameter(Mandatory=$false)][string]$execPath,
    [parameter(Mandatory=$false)][string]$kubeconfigPath,
    [parameter(Mandatory=$true)][string]$configFile,
    [parameter(Mandatory=$false)][string]$imageTag,
    [parameter(Mandatory=$false)][string]$externalDns,
    [parameter(Mandatory=$false)][bool]$deployCI=$false,
    [parameter(Mandatory=$false)][bool]$buildImages=$true,
    [parameter(Mandatory=$false)][bool]$buildBits=$false,
    [parameter(Mandatory=$false)][bool]$deployInfrastructure=$true,
    [parameter(Mandatory=$false)][string]$dockerOrg="eshop"
)

function ExecKube($cmd) {    
    if($deployCI) {
        $kubeconfig = $kubeconfigPath + 'config';
        $exp = $execPath + 'kubectl ' + $cmd + ' --kubeconfig=' + $kubeconfig
        Invoke-Expression $exp
    }
    else{
        $exp = $execPath + 'kubectl ' + $cmd
        Invoke-Expression $exp
    }
}

# Initialization
$debugMode = $PSCmdlet.MyInvocation.BoundParameters["Debug"].IsPresent
$useDockerHub = [string]::IsNullOrEmpty($registry)

# Check required commands (only if not in CI environment)
if(-not $deployCI) {
        $requiredCommands = ("docker", "docker-compose", "kubectl")
        foreach ($command in $requiredCommands) {
        if ((Get-Command $command -ErrorAction SilentlyContinue) -eq $null) {
            Write-Host "$command must be on path" -ForegroundColor Red
            exit
        }
    }
}
else {
    $buildBits = false;
    $buildImages = false;       # Never build images through CI, as they previously built
}

# Get tag to use from current branch if no tag is passed
if ([string]::IsNullOrEmpty($imageTag)) {
    $imageTag = $(git rev-parse --abbrev-ref HEAD)
}
Write-Host "Docker image Tag: $imageTag" -ForegroundColor Yellow

# building and publishing docker images if needed
if($buildBits) {
    Write-Host "Building and publishing eShopOnContainers..." -ForegroundColor Yellow
    dotnet restore ../eShopOnContainers-ServicesAndWebApps.sln
    dotnet publish -c Release -o obj/Docker/publish ../eShopOnContainers-ServicesAndWebApps.sln
}
if ($buildImages) {
    Write-Host "Building Docker images tagged with '$imageTag'" -ForegroundColor Yellow
    $env:TAG=$imageTag
    docker-compose -p .. -f ../docker-compose.yml build    

    Write-Host "Pushing images to $registry/$dockerOrg..." -ForegroundColor Yellow
    $services = ("basket.api", "catalog.api", "identity.api", "ordering.api", "marketing.api","payment.api","locations.api", "webmvc", "webspa", "webstatus")

    foreach ($service in $services) {
        $imageFqdn = if ($useDockerHub)  {"$dockerOrg/${service}"} else {"$registry/$dockerOrg/${service}"}
        docker tag eshop/${service}:$imageTag ${imageFqdn}:$imageTag
        docker push ${imageFqdn}:$imageTag            
    }
}

# if we have login/pwd add the secret to k8s
if (-not [string]::IsNullOrEmpty($dockerUser)) {
    $registryFDQN =  if (-not $useDockerHub) {$registry} else {"index.docker.io/v1/"}

    Write-Host "Logging in to $registryFDQN as user $dockerUser" -ForegroundColor Yellow
    if ($useDockerHub) {
        docker login -u $dockerUser -p $dockerPassword
    }
    else {
        docker login -u $dockerUser -p $dockerPassword $registryFDQN
    }
    
    if (-not $LastExitCode -eq 0) {
        Write-Host "Login failed" -ForegroundColor Red
        exit
    }

    # create registry key secret
    ExecKube -cmd 'create secret docker-registry registry-key `
    --docker-server=$registryFDQN `
    --docker-username=$dockerUser `
    --docker-password=$dockerPassword `
    --docker-email=not@used.com'
}

# Removing previous services & deployments
Write-Host "Removing existing services & deployments.." -ForegroundColor Yellow
ExecKube -cmd 'delete deployments --all'
ExecKube -cmd 'delete services --all'
ExecKube -cmd 'delete configmap config-files'
ExecKube -cmd 'delete configmap urls'
ExecKube -cmd 'delete configmap externalcfg'

# start sql, rabbitmq, frontend deploymentsExecKube -cmd 'delete configmap config-files'
ExecKube -cmd 'create configmap config-files --from-file=nginx-conf=nginx.conf'
ExecKube -cmd 'label configmap config-files app=eshop'

if ($deployInfrastructure) {
    Write-Host 'Deploying infrastructure deployments (databases, redis, ...)' -ForegroundColor Yellow
    ExecKube -cmd 'create -f sql-data.yaml -f basket-data.yaml -f keystore-data.yaml -f rabbitmq.yaml -f nosql-data.yaml'
}

Write-Host 'Deploying code deployments (databases, redis, ...)' -ForegroundColor Yellow
ExecKube -cmd 'create -f services.yaml -f frontend.yaml'

if ([string]::IsNullOrEmpty($externalDns)) {
        Write-Host "Waiting for frontend's external ip..." -ForegroundColor Yellow
        while ($true) {
            $frontendUrl = & ExecKube -cmd 'get svc frontend -o=jsonpath="{.status.loadBalancer.ingress[0].ip}"'
            if ([bool]($frontendUrl -as [ipaddress])) {
                break
            }
            Start-Sleep -s 15
        }
        $externalDns = $frontendUrl
}

Write-Host "Using $externalDns as the external DNS/IP of the k8s cluster"

ExecKube -cmd 'create configmap urls `
    --from-literal=BasketUrl=http://basket `
    --from-literal=BasketHealthCheckUrl=http://basket/hc `
    --from-literal=CatalogUrl=http://$($frontendUrl)/catalog-api `
    --from-literal=CatalogHealthCheckUrl=http://catalog/hc `
    --from-literal=IdentityUrl=http://$($frontendUrl)/identity `
    --from-literal=IdentityHealthCheckUrl=http://identity/hc `
    --from-literal=OrderingUrl=http://ordering `
    --from-literal=OrderingHealthCheckUrl=http://ordering/hc `
    --from-literal=MvcClientExternalUrl=http://$($frontendUrl)/webmvc `
    --from-literal=WebMvcHealthCheckUrl=http://webmvc/hc `
    --from-literal=MvcClientOrderingUrl=http://ordering `
    --from-literal=MvcClientCatalogUrl=http://catalog `
    --from-literal=MvcClientBasketUrl=http://basket `
    --from-literal=WebSpaHealthCheckUrl=http://webspa/hc `
    --from-literal=SpaClientOrderingExternalUrl=http://$($externalDns)/ordering-api `
    --from-literal=SpaClientCatalogExternalUrl=http://$($externalDns)/catalog-api `
    --from-literal=SpaClientBasketExternalUrl=http://$($externalDns)/basket-api `
    --from-literal=SpaClientIdentityExternalUrl=http://$($externalDns)/identity `
    --from-literal=SpaClientExternalUrl=http://$($externalDns)'
    
ExecKube -cmd 'label configmap urls app=eshop'

Write-Host "Deploying configuration from $configFile" -ForegroundColor Yellow

ExecKube -cmd "create -f $configFile"

Write-Host "Creating deployments..." -ForegroundColor Yellow
ExecKube -cmd 'create -f deployments.yaml'

# update deployments with the correct image (with tag and/or registry)
$registryPath = ""
if (-not [string]::IsNullOrEmpty($registry)) {
    $registryPath = "$registry/"
}

Write-Host "Update Image containers to use prefix '$registry$dockerOrg' and tag '$imageTag'" -ForegroundColor Yellow

ExecKube -cmd 'set image deployments/basket basket=${registryPath}${dockerOrg}/basket.api:$imageTag'
ExecKube -cmd 'set image deployments/catalog catalog=${registryPath}${dockerOrg}/catalog.api:$imageTag'
ExecKube -cmd 'set image deployments/identity identity=${registryPath}${dockerOrg}/identity.api:$imageTag'
ExecKube -cmd 'set image deployments/ordering ordering=${registryPath}${dockerOrg}/ordering.api:$imageTag'
ExecKube -cmd 'set image deployments/marketing marketing=${registryPath}${dockerOrg}/marketing.api:$imageTag'
ExecKube -cmd 'set image deployments/locations locations=${registryPath}${dockerOrg}/locations.api:$imageTag'
ExecKube -cmd 'set image deployments/payment payment=${registryPath}${dockerOrg}/payment.api:$imageTag'
ExecKube -cmd 'set image deployments/webmvc webmvc=${registryPath}${dockerOrg}/webmvc:$imageTag'
ExecKube -cmd 'set image deployments/webstatus webstatus=${registryPath}${dockerOrg}/webstatus:$imageTag'
ExecKube -cmd 'set image deployments/webspa webspa=${registryPath}${dockerOrg}/webspa:$imageTag'

Write-Host "Execute rollout..." -ForegroundColor Yellow
ExecKube -cmd 'rollout resume deployments/basket'
ExecKube -cmd 'rollout resume deployments/catalog'
ExecKube -cmd 'rollout resume deployments/identity'
ExecKube -cmd 'rollout resume deployments/ordering'
ExecKube -cmd 'rollout resume deployments/marketing'
ExecKube -cmd 'rollout resume deployments/locations'
ExecKube -cmd 'rollout resume deployments/payment'
ExecKube -cmd 'rollout resume deployments/webmvc'
ExecKube -cmd 'rollout resume deployments/webstatus'
ExecKube -cmd 'rollout resume deployments/webspa'

Write-Host "WebSPA is exposed at http://$frontendUrl, WebMVC at http://$frontendUrl/webmvc, WebStatus at http://$frontendUrl/webstatus" -ForegroundColor Yellow

