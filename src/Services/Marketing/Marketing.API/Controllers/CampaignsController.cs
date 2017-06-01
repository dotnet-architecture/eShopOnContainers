namespace Microsoft.eShopOnContainers.Services.Marketing.API.Controllers
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    public class CampaignsController : Controller
    {
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("{id:int}")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        [HttpPut("{id:int}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        [HttpDelete("{id:int}")]
        public void Delete(int id)
        {
        }
    }
}
