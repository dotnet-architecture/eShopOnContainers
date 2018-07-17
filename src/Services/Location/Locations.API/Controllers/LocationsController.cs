using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnContainers.Services.Locations.API.Infrastructure.Services;
using Microsoft.eShopOnContainers.Services.Locations.API.Model;
using Microsoft.eShopOnContainers.Services.Locations.API.ViewModel;
using System;
using System.Collections.Generic;
using System.Net;
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

        //GET api/v1/[controller]/user/1
        [Route("user/{userId:guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(UserLocation), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetUserLocation(Guid userId)
        {
            var userLocation = await _locationsService.GetUserLocation(userId.ToString());
            return Ok(userLocation);
        }

        //GET api/v1/[controller]/
        [Route("")]
        [HttpGet]
        //[ProducesResponseType(typeof(List<Locations>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllLocations()
        {
            var locations = await _locationsService.GetAllLocation();
            return Ok(locations);
        }

        //GET api/v1/[controller]/1
        [Route("{locationId}")]
        [HttpGet]
        //[ProducesResponseType(typeof(List<Locations>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetLocation(int locationId)
        {
            var location = await _locationsService.GetLocation(locationId);
            return Ok(location);
        }
         
        //POST api/v1/[controller]/
        [Route("")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateOrUpdateUserLocation([FromBody]LocationRequest newLocReq)
        {
            var userId = _identityService.GetUserIdentity();
            var result = await _locationsService.AddOrUpdateUserLocation(userId, newLocReq);
           
            return result ? 
                (IActionResult)Ok() : 
                (IActionResult)BadRequest();
        }
    }
}
