using eShopOnContainers.WebSPA;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace WebSPA.Infrastructure
{
    public class WebContextSeed
    {
        public static void Seed(IApplicationBuilder applicationBuilder, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var log = loggerFactory.CreateLogger("WebSPA seed");

            var settings = (AppSettings)applicationBuilder
                .ApplicationServices.GetRequiredService<IOptions<AppSettings>>().Value;

            var useCustomizationData = settings.UseCustomizationData;
            var contentRootPath = env.ContentRootPath;
            var webroot = env.WebRootPath;

            if (useCustomizationData)
            {
                GetPreconfiguredImages(contentRootPath, webroot, log);
            }
        }

        static void GetPreconfiguredImages(string contentRootPath, string webroot, ILogger log)
        {
            try
            {
                string imagesZipFile = Path.Combine(contentRootPath, "Setup", "images.zip");
                if (!File.Exists(imagesZipFile))
                {
                    log.LogError($" zip file '{imagesZipFile}' does not exists.");
                    return;
                }

                string imagePath = Path.Combine(webroot, "assets", "images");
                string[] imageFiles = Directory.GetFiles(imagePath).Select(file => Path.GetFileName(file)).ToArray();

                using (ZipArchive zip = ZipFile.Open(imagesZipFile, ZipArchiveMode.Read))
                {
                    foreach (ZipArchiveEntry entry in zip.Entries)
                    {
                        if (imageFiles.Contains(entry.Name))
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
                            log.LogWarning($"Skip file '{entry.Name}' in zipfile '{imagesZipFile}'");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogError($"Exception in method GetPreconfiguredImages WebSPA. Exception Message={ex.Message}");
            }
        }
    }
}
