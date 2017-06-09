namespace eShopOnContainers.Core.Services.Location
{
    using eShopOnContainers.Core.Models.Location;
    using eShopOnContainers.Core.Services.RequestProvider;
    using System;
    using System.Threading.Tasks;

    public class LocationService : ILocationService
    {
        private readonly IRequestProvider _requestProvider;

        public LocationService(IRequestProvider requestProvider)
        {
            _requestProvider = requestProvider;
        }

        public async Task UpdateUserLocation(LocationRequest newLocReq)
        {
            UriBuilder builder = new UriBuilder(GlobalSetting.Instance.LocationEndpoint);

            builder.Path = "api/v1/locations";

            string uri = builder.ToString();

            var result = await _requestProvider.PostAsync(uri, newLocReq);
        }
    }
}
