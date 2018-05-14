Param(
    [parameter(Mandatory=$false)][string]$acrName,
    [parameter(Mandatory=$false)][string]$gitUser,
    [parameter(Mandatory=$false)][string]$repoName="eShopOnContainers",
    [parameter(Mandatory=$false)][string]$gitBranch="dev",
    [parameter(Mandatory=$true)][string]$patToken
)

$gitContext = "https://github.com/$gitUser/$repoName"

$services = @( 
    @{ Name="eshopbasket"; Image="eshop/basket.api"; File="src/Services/Basket/Basket.API/Dockerfile" },
    @{ Name="eshopcatalog"; Image="eshop/catalog.api"; File="src/Services/Catalog/Catalog.API/Dockerfile" },
    @{ Name="eshopidentity"; Image="eshop/identity.api"; File="src/Services/Identity/Identity.API/Dockerfile" },
    @{ Name="eshopordering"; Image="eshop/ordering.api"; File="src/Services/Ordering/Ordering.API/Dockerfile" },
    @{ Name="eshoporderingbg"; Image="eshop/ordering.backgroundtasks"; File="src/Services/Ordering/Ordering.BackgroundTasks/Dockerfile" },
    @{ Name="eshopmarketing"; Image="eshop/marketing.api"; File="src/Services/Marketing/Marketing.API/Dockerfile" },
    @{ Name="eshopwebspa"; Image="eshop/webspa"; File="src/Web/WebSPA/Dockerfile" },
    @{ Name="eshopwebmvc"; Image="eshop/webmvc"; File="src/Web/WebMVC/Dockerfile" },
    @{ Name="eshopwebstatus"; Image="eshop/webstatus"; File="src/Web/WebStatus/Dockerfile" },
    @{ Name="eshoppayment"; Image="eshop/payment.api"; File="src/Services/Payment/Payment.API/Dockerfile" },
    @{ Name="eshoplocations"; Image="eshop/locations.api"; File="src/Services/Location/Locations.API/Dockerfile" },
    @{ Name="eshopocelotapigw"; Image="eshop/ocelotapigw"; File="src/ApiGateways/ApiGw-Base/Dockerfile" },
    @{ Name="eshopmobileshoppingagg"; Image="eshop/mobileshoppingagg"; File="src/ApiGateways/Mobile.Bff.Shopping/aggregator/Dockerfile" },
    @{ Name="eshopwebshoppingagg"; Image="eshop/webshoppingagg"; File="src/ApiGateways/Web.Bff.Shopping/aggregator/Dockerfile" },
    @{ Name="eshoporderingsignalrhub"; Image="eshop/ordering.signalrhub"; File="src/Services/Ordering/Ordering.SignalrHub/Dockerfile" }
)

$services |% {
    $bname = $_.Name
    $bimg = $_.Image
    $bfile = $_.File
    Write-Host "Setting ACR build $bname ($bimg)"    
    az acr build-task create --registry $acrName --name $bname --image ${bimg}:$gitBranch --context $gitContext --branch $gitBranch --git-access-token $patToken --file $bfile
}

# Basket.API
