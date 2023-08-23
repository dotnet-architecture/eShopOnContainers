namespace WebSPA.Infrastructure;

using Microsoft.Extensions.Logging;

public class WebContextSeed
{
    public static void Seed(IApplicationBuilder applicationBuilder, IWebHostEnvironment env, ILogger logger)
    {
        var settings = applicationBuilder
            .ApplicationServices.GetRequiredService<IOptions<AppSettings>>().Value;

        var useCustomizationData = settings.UseCustomizationData;
        var contentRootPath = env.ContentRootPath;
        var webroot = env.WebRootPath;

        if (useCustomizationData)
        {
            GetPreconfiguredImages(contentRootPath, webroot, logger);
        }
    }

    static void GetPreconfiguredImages(string contentRootPath, string webroot, ILogger logger)
    {
        try
        {
            string imagesZipFile = Path.Combine(contentRootPath, "Setup", "images.zip");
            if (!File.Exists(imagesZipFile))
            {
                logger.LogError("Zip file '{ZipFileName}' does not exists.", imagesZipFile);
                return;
            }

            string imagePath = Path.Combine(webroot, "assets", "images");
            if (!Directory.Exists(imagePath))
            {
                Directory.CreateDirectory(imagePath);
            }
            string[] imageFiles = Directory.GetFiles(imagePath).Select(file => Path.GetFileName(file)).ToArray();

            using ZipArchive zip = ZipFile.Open(imagesZipFile, ZipArchiveMode.Read);
            foreach (ZipArchiveEntry entry in zip.Entries)
            {
                if (!imageFiles.Contains(entry.Name))
                {
                    string destinationFilename = Path.Combine(imagePath, entry.Name);
                    if (File.Exists(destinationFilename))
                    {
                        File.Delete(destinationFilename);
                    }
                    entry.ExtractToFile(destinationFilename);
                }
                else
                {
                    logger.LogWarning("Skipped file '{FileName}' in zip file '{ZipFileName}'", entry.Name, imagesZipFile);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting preconfigured images");
        }
    }
}
