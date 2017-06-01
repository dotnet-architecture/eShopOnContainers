using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnContainers.Services.Locations.API.Infrastructure.Services;
using Microsoft.eShopOnContainers.Services.Locations.API.ViewModel;
using System;
using System.Threading.Tasks;

namespace Locations.API.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize]
    public class LocationsController : ControllerBase
    {
        private readonly ILocationsService _locationsService;
        private readonly IIdentityService _identityService;

        public LocationsController(ILocationsService locationsService, IIdentityService identityService)
        {
            _locationsService = locationsService ?? throw new ArgumentNullException(nameof(locationsService));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
        }

        //POST api/v1/[controller]/
        [Route("")]
        [HttpPost]
        public async Task<IActionResult> UpdateUserLocation([FromBody]LocationRequest newLocReq)
        {
            var userId = _identityService.GetUserIdentity();
            var result = await _locationsService.AddOrUpdateUserLocation(userId, newLocReq);
            return result ? 
                (IActionResult)Ok() : 
                (IActionResult)BadRequest();
        }
    }
}
