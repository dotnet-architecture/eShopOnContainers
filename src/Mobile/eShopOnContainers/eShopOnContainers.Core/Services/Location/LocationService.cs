namespace eShopOnContainers.Core.Services.Location
{
    using System;
    using System.Threading.Tasks;
    using Models.Location;
    using RequestProvider;

    public class LocationService : ILocationService
    {
        private readonly IRequestProvider _requestProvider;

        public LocationService(IRequestProvider requestProvider)
        {
            _requestProvider = requestProvider;
        }

        public async Task UpdateUserLocation(Location newLocReq, string token)
        {
            UriBuilder builder = new UriBuilder(GlobalSetting.Instance.LocationEndpoint);

            builder.Path = "api/v1/locations";

            string uri = builder.ToString();

            await _requestProvider.PostAsync(uri, newLocReq, token);
        }
    }
}