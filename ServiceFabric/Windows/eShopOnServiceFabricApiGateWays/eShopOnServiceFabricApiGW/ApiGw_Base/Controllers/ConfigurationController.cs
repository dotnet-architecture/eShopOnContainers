using ApiGw_Base.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Text;

namespace ApiGw_Base.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        [HttpGet("mobile/marketing")]
        public IActionResult MobileMarketing() => ReadConfigurationFile(ConfigurationType.Mobile, ConfigurationBffType.Marketing);

        [HttpGet("mobile/shopping")]
        public IActionResult MobileShopping() => ReadConfigurationFile(ConfigurationType.Mobile, ConfigurationBffType.Shopping);

        [HttpGet("web/marketing")]
        public IActionResult WebMarketing() => ReadConfigurationFile(ConfigurationType.Web, ConfigurationBffType.Marketing);

        [HttpGet("web/shopping")]
        public IActionResult WebShopping() => ReadConfigurationFile(ConfigurationType.Web, ConfigurationBffType.Shopping);

        private IActionResult ReadConfigurationFile(ConfigurationType configurationType, ConfigurationBffType configurationBffType)
        {
            var path = $"{AppDomain.CurrentDomain.BaseDirectory}/Configurations/configuration.{configurationType}.Bff.{configurationBffType}.json";

            using (var streamReader = new StreamReader(path, Encoding.UTF8))
            {
                var jsonString = streamReader.ReadToEnd();

                if (string.IsNullOrWhiteSpace(jsonString))
                {
                    return BadRequest($"Configuration file 'configuration.{configurationType}.Bff.{configurationBffType}.json' not found");
                }

                return Ok(jsonString);
            }
        }
    }
}