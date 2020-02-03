using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.eShopOnContainers.WebMVC;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace WebMVC.Infrastructure
{
    public class WebContextSeed
    {
        public static void Seed(IApplicationBuilder applicationBuilder, IWebHostEnvironment env)
        {
            var log = Serilog.Log.Logger;

            var settings = (AppSettings)applicationBuilder
                .ApplicationServices.GetRequiredService<IOptions<AppSettings>>().Value;

            var useCustomizationData = settings.UseCustomizationData;
            var contentRootPath = env.ContentRootPath;
            var webroot = env.WebRootPath;

            if (useCustomizationData)
            {
                GetPreconfiguredImages(contentRootPath, webroot, log);

                GetPreconfiguredCSS(contentRootPath, webroot, log);
            }
        }

        static void GetPreconfiguredCSS(string contentRootPath, string webroot, ILogger log)
        {
            try
            { 
                string overrideCssFile = Path.Combine(contentRootPath, "Setup", "override.css");
                if (!File.Exists(overrideCssFile))
                {
                    log.Error("Override css file '{FileName}' does not exists.", overrideCssFile);
                    return;
                }

                string destinationFilename = Path.Combine(webroot, "css", "override.css");
                File.Copy(overrideCssFile, destinationFilename, true );
            }
            catch (Exception ex)
            {
                log.Error(ex, "EXCEPTION ERROR: {Message}", ex.Message);
            }
        }

        static void GetPreconfiguredImages(string contentRootPath, string webroot, ILogger log)
        {
            try
            {
                string imagesZipFile = Path.Combine(contentRootPath, "Setup", "images.zip");
                if (!File.Exists(imagesZipFile))
                {
                    log.Error("Zip file '{ZipFileName}' does not exists.", imagesZipFile);
                    return;
                }

                string imagePath = Path.Combine(webroot, "images");
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
                            log.Warning("Skipped file '{FileName}' in zipfile '{ZipFileName}'", entry.Name, imagesZipFile);
                        }
                    }
                }
            }
            catch ( Exception ex )
            {
                log.Error(ex, "EXCEPTION ERROR: {Message}", ex.Message);
            }
        }

    }

}
