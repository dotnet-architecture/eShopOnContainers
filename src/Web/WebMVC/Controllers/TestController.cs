using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnContainers.BuildingBlocks.Resilience.Http;
using Microsoft.eShopOnContainers.WebMVC.Services;
using Microsoft.eShopOnContainers.WebMVC.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.Controllers
{
    class TestPayload
    {
        public int CatalogItemId { get; set; }
        public string BasketId { get; set; }

        public int Quantity { get; set; }
    }

    [Authorize]
    public class TestController : Controller
    {
        private readonly IHttpClient _client;
        private readonly IIdentityParser<ApplicationUser> _appUserParser;
        public TestController(IHttpClient client, IIdentityParser<ApplicationUser> identityParser)
        {
            _client = client;
            _appUserParser = identityParser;
        }

        public async Task<IActionResult> Ocelot()
        {
            var url = "http://apigw/shopping/api/v1/basket/items";
            var payload = new TestPayload()
            {
                CatalogItemId = 1,
                Quantity = 1,
                BasketId = _appUserParser.Parse(User).Id
            };
            var token = await HttpContext.GetTokenAsync("access_token");
            var response = await _client.PostAsync<TestPayload>(url, payload, token);
            
            if (response.IsSuccessStatusCode)
            {
                var str =  await response.Content.ReadAsStringAsync();
                return Ok(str);
            }
            else
            {
                return Ok(new { response.StatusCode, response.ReasonPhrase });
            }
        }
    }
}
