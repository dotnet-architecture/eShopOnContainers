using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnContainers.Services.Identity.API.Models;
using Microsoft.eShopOnContainers.Services.Identity.API.Services;
using Microsoft.Extensions.Logging;

namespace Identity.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserIdController : ControllerBase

    {
        private readonly ILoginService<ApplicationUser> _loginService;
        private readonly ILogger<UserIdController> _logger;

        public UserIdController(ILoginService<ApplicationUser> loginService, ILogger<UserIdController> logger)
        {
            _loginService = loginService;
            _logger = logger;
        }

        // GET: api/UserId
        [HttpGet]
        public async Task<int> Get(String userName)
        {
            if (String.IsNullOrEmpty(userName))
            {
                return 0;
            }

            var user = await _loginService.FindByUsername(userName);

            if(user == null)
            {
                return 0;
            }

            return user.TenantId;
        }
    }
}
