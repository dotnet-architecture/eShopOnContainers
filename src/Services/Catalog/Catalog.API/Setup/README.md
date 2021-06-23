# Catalog set up

The catalog images have been updated to the new SPA looks.

If you want to use the classical images:

1. Drop the `Microsoft.eShopOnContainers.Services.CatalogDb` database from the `sqldata` container.
2. Rename `CatalogItems-MVC.zip` as `CatalogItems.zip`
3. Rebuild the `catalog-api` service with `docker-compose build catalog-api`
4. Restart the application as usual
