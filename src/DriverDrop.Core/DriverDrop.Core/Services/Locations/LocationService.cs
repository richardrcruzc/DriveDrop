namespace eShopOnContainers.Core.Services.Locations
{
    using System;
    using System.Threading.Tasks;
    using DriverDrop.Core.Services.Locations;
    using DriverDrop.Core.Services.RequestProvider;
    using DriverDrop.Core.Models.Locations;
    using DriverDrop.Core;

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