﻿namespace Microsoft.eShopOnContainers.Services.Marketing.API.Controllers
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using System.IO;

    [ApiController]
    public class PicController : ControllerBase
    {
        private readonly IHostingEnvironment _env;
        public PicController(IHostingEnvironment env)
        {
            _env = env;
        }

        [HttpGet]
        [Route("api/v1/campaigns/{campaignId:int}/pic")]
        public ActionResult GetImage(int campaignId)
        {
            var webRoot = _env.WebRootPath;
            var path = Path.Combine(webRoot, campaignId + ".png");

            var buffer = System.IO.File.ReadAllBytes(path); 
            
            return File(buffer, "image/png");
        }
    }
}
