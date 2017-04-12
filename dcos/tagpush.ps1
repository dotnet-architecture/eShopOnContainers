Param(
    [Parameter(Mandatory=$true)][string]$registry
)

$images = ("eshop/basket.api", "eshop/catalog.api", "eshop/identity.api", "eshop/ordering.api", "eshop/webmvc", "eshop/webspa", "eshop/webstatus")
foreach ($image in $images) {
    $newTag = "$registry/$image"
    docker tag $image $newTag
    docker push $newTag
}
