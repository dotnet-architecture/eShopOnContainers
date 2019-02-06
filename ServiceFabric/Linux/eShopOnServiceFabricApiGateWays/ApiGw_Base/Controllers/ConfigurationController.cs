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
        public IActionResult MobileMarketing() => ReadConfigurationFile(Channel.Mobile, ChannelType.Marketing);

        [HttpGet("mobile/shopping")]
        public IActionResult MobileShopping() => ReadConfigurationFile(Channel.Mobile, ChannelType.Shopping);

        [HttpGet("web/marketing")]
        public IActionResult WebMarketing() => ReadConfigurationFile(Channel.Web, ChannelType.Marketing);

        [HttpGet("web/shopping")]
        public IActionResult WebShopping() => ReadConfigurationFile(Channel.Web, ChannelType.Shopping);

        private IActionResult ReadConfigurationFile(Channel channel, ChannelType channelType)
        {
            var path = $"{AppDomain.CurrentDomain.BaseDirectory}/Configurations/configuration.{channel}.Bff.{channelType}.json";

            using (var streamReader = new StreamReader(path, Encoding.UTF8))
            {
                var jsonString = streamReader.ReadToEnd();

                if (string.IsNullOrWhiteSpace(jsonString))
                {
                    return BadRequest($"Configuration file 'configuration.{channel}.Bff.{channelType}.json' not found");
                }

                return Ok(jsonString);
            }
        }
    }
}