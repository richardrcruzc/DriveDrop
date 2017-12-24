using DriveDrop.Core.Models.Drivers;
using DriveDrop.Core.Services.RequestProvider;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveDrop.Core.Services.Driver
{
    public class DriverService: IDriverService
    {
        private readonly IRequestProvider _requestProvider;
        private const string ApiUrlBase = "drivers/";
        public DriverService(IRequestProvider requestProvider)
        {
            _requestProvider = requestProvider;
        }
        public async Task<NewDriver> CreateDriverAsync(NewDriver newDriver, string token)
        {
            var builder = new UriBuilder(GlobalSetting.Instance.ApiEndpoint)
            {
                Path = ApiUrlBase + "NewDriver"
            };
            var uri = builder.ToString();

            var result = await _requestProvider.PostAsync(uri, newDriver, token);

            return result;
        }
        public async Task<ObservableCollection<NewDriver>> GetDriversAsync(string token)
        {
            var builder = new UriBuilder(GlobalSetting.Instance.ApiEndpoint)
            {
                Path = ApiUrlBase
            };
            var uri = builder.ToString();

            ObservableCollection<NewDriver> drivers =
               await _requestProvider.GetAsync<ObservableCollection<NewDriver>>(uri, token);

            return drivers;
        }
        public async Task<NewDriver> GetDriverAsync(int driverId,   string token)
        {
            try
            {
                UriBuilder builder = new UriBuilder(GlobalSetting.Instance.ApiEndpoint);

                builder.Path = string.Format("GetbyId/{0}", driverId);

                string uri = builder.ToString();

                NewDriver driver =
                    await _requestProvider.GetAsync<NewDriver>(uri, token);

                return driver;
            }
            catch
            {
                return new NewDriver();
            }

        }
        public async Task<NewDriver> GetDriverAsync( string userName, string token)
        {
            try
            {
                UriBuilder builder = new UriBuilder(GlobalSetting.Instance.ApiEndpoint);

                builder.Path = string.Format("GetByUserName/userName/{0}", userName);

                string uri = builder.ToString();

                NewDriver driver =
                    await _requestProvider.GetAsync<NewDriver>(uri, token);

                return driver;
            }
            catch
            {
                return new NewDriver();
            }

        }
    }
}
