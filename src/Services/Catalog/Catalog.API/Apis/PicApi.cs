namespace Catalog.API.Apis;

public static class PicApi
{
    public static IEndpointConventionBuilder MapPicApi(this IEndpointRouteBuilder routes)
    {
        return routes.MapGet("api/v1/catalog/items/{catalogItemId:int}/pic",
            async (int catalogItemId, CatalogContext db, IWebHostEnvironment environment) =>
        {
            var item = await db.CatalogItems.FindAsync(catalogItemId);

            if (item is null)
            {
                return Results.NotFound();
            }

            var path = Path.Combine(environment.ContentRootPath, "Pics", item.PictureFileName);

            string imageFileExtension = Path.GetExtension(item.PictureFileName);
            string mimetype = GetImageMimeTypeFromImageFileExtension(imageFileExtension);

            return Results.File(path, mimetype);
        })
        .WithTags("Pic")
        .Produces(404);

        static string GetImageMimeTypeFromImageFileExtension(string extension) => extension switch
        {
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".bmp" => "image/bmp",
            ".tiff" => "image/tiff",
            ".wmf" => "image/wmf",
            ".jp2" => "image/jp2",
            ".svg" => "image/svg+xml",
            _ => "application/octet-stream",
        };
    }
}
