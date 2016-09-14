using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.Controllers
{
    [Route("api/[controller]")]
    public class EnvironmentInfoController : Controller
    {
        // GET api/environmentInfo/machinename
        [HttpGet("machinename")]
        public dynamic GetMachineName()
        {
            return new
            {
                Node = Environment.MachineName
            };
        }

    }
}