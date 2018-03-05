﻿namespace GoDriveDrop.Core.Services.Location
{
    using System;
    using System.Threading.Tasks;
    using GoDriveDrop.Core.Models.Commons;
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

            builder.Path = "locations";

            string uri = builder.ToString();

            await _requestProvider.PostAsync(uri, newLocReq, token);
        }
    }
}