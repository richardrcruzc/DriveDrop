using GoDriveDrop.Core.Identity;
using GoDriveDrop.Core.Models;
using GoDriveDrop.Core.Models.Commons;
using GoDriveDrop.Core.Models.Shippments;
using GoDriveDrop.Core.Services.RequestProvider;
using GoDriveDrop.Core.ViewModels;
using IdentityModel.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GoDriveDrop.Core.Services.Driver
{
    public class SenderService : ISenderService
    {
        private IIdentityService _identityService;
        private readonly IRequestProvider _requestProvider;
        private const string ApiUrlBase = "drivers/";
        public SenderService(IIdentityService identityService, IRequestProvider requestProvider)
        {
            _identityService = identityService;
            _requestProvider = requestProvider;
        }

        public async Task<string> CreatePackageAsync(NewPackageModel p, string token)
        {
            var builder = new UriBuilder(GlobalSetting.Instance.BaseEndpoint)
            { 
                Path = "/sender/SaveNewShipment"
            };
            var uri = builder.ToString();
            try
            {
               var response= await _requestProvider.PostAsync<NewPackageModel>(uri, p, token);
                return "Info updated";
            }
            catch (WebException wex)
            {
                if (wex.Response != null)
                {
                    using (var errorResponse = (HttpWebResponse)wex.Response)
                    {
                        using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                        {
                            string error = reader.ReadToEnd();

                            var errorList = JsonConvert.DeserializeObject<List<string>>(error,
                   new JsonSerializerSettings
                   {
                       NullValueHandling = NullValueHandling.Ignore
                   });


                            //TODO: use JSON.net to parse this string and look at the error message
                            //newDriver.ErrorMsg = error;
                        }
                    }
                }

                return "Something wrong !";
            }

        }

        public async Task<string> CreateSenderAsync(NewSenderModel c)
        {
            c.FromXamarin = true;

            var builder = new UriBuilder(GlobalSetting.Instance.BaseEndpoint)
            {
                //Path = $"/account/RegisterUser?userName={userName}&password={password}"
                Path = "/sender/NewSenderFromBody"
            };
            c.UserEmail = System.Net.WebUtility.UrlEncode(c.UserEmail);
            c.Password = System.Net.WebUtility.UrlEncode(c.Password);
            var uri = builder.ToString(); 
             
            try
            {
                 await _requestProvider.PostAsync<NewSenderModel>(uri, c);
                return "Info updated";
            }
            catch (WebException wex)
            {
                if (wex.Response != null)
                {
                    using (var errorResponse = (HttpWebResponse)wex.Response)
                    {
                        using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                        {
                            string error = reader.ReadToEnd();

                            var errorList = JsonConvert.DeserializeObject<List<string>>(error,
                   new JsonSerializerSettings
                   {
                       NullValueHandling = NullValueHandling.Ignore
                   });


                            //TODO: use JSON.net to parse this string and look at the error message
                            //newDriver.ErrorMsg = error;
                        }
                    }
                }

                return "Something wrong !";
            }
        }
     
        public async Task<ObservableCollection<NewSenderModel>> GetDriversAsync(string token)
        {
            var builder = new UriBuilder(GlobalSetting.Instance.ApiEndpoint)
            {
                Path = ApiUrlBase
            };
            var uri = builder.ToString();

            ObservableCollection<NewSenderModel> drivers =
               await _requestProvider.GetAsync<ObservableCollection<NewSenderModel>>(uri, token);

            return drivers;
        }
        public async Task<NewSenderModel> GetSenderAsync(int driverId, string token)
        {
            try
            {
                UriBuilder builder = new UriBuilder(GlobalSetting.Instance.ApiEndpoint);

                builder.Path = string.Format("GetbyId/{0}", driverId);

                string uri = builder.ToString();

                NewSenderModel driver =
                    await _requestProvider.GetAsync<NewSenderModel>(uri, token);

                return driver;
            }
            catch
            {
                return new NewSenderModel();
            }

        }
        public async Task<NewSenderModel> GetSenderAsync(string userName, string token)
        {
            try
            {
                UriBuilder builder = new UriBuilder(GlobalSetting.Instance.ApiEndpoint);

                builder.Path = string.Format("GetByUserName/userName/{0}", userName);

                string uri = builder.ToString();

                NewSenderModel driver =
                    await _requestProvider.GetAsync<NewSenderModel>(uri, token);

                return driver;
            }
            catch
            {
                return new NewSenderModel();
            }

        }

        async  Task<string> GetUserTokenAsync()
        {
            var LoginUrl = _identityService.CreateAuthorizationRequest();
            var unescapedUrl = System.Net.WebUtility.UrlDecode(LoginUrl);
            var authResponse = new AuthorizeResponse(LoginUrl);
            var userToken = await _identityService.GetTokenAsync(authResponse.Code);
            string accessToken = userToken.AccessToken;
            return accessToken;
        }

        public async Task<string> UpdateSenderAsync(CustomerModel c)
        { 
            var builder = new UriBuilder(GlobalSetting.Instance.BaseEndpoint)
            {
                //Path = $"/account/RegisterUser?userName={userName}&password={password}"
                Path = "/sender/UpdateInfoFromBody"
            };
          
            var uri = builder.ToString();

            try
            {
                await _requestProvider.PostAsync<CustomerModel>(uri, c);
                return "Info updated";
            }
            catch (WebException wex)
            {
                if (wex.Response != null)
                {
                    using (var errorResponse = (HttpWebResponse)wex.Response)
                    {
                        using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                        {
                            string error = reader.ReadToEnd();

                            var errorList = JsonConvert.DeserializeObject<List<string>>(error,
                   new JsonSerializerSettings
                   {
                       NullValueHandling = NullValueHandling.Ignore
                   });


                            //TODO: use JSON.net to parse this string and look at the error message
                            //newDriver.ErrorMsg = error;
                        }
                    }
                }

                return "Something wrong !";
            }
        }
    }
}
