using GoDriveDrop.Core.Identity;
using GoDriveDrop.Core.Models;
using GoDriveDrop.Core.Models.Commons;
using GoDriveDrop.Core.Services.RequestProvider;
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
    public class DriverService : IDriverService
    {
        private IIdentityService _identityService;
        private readonly IRequestProvider _requestProvider;
        private const string ApiUrlBase = "api/v1/drivers/";
        public DriverService(
            IIdentityService identityService,
            IRequestProvider requestProvider)
        {
            _identityService = identityService;
            _requestProvider = requestProvider;
        }
        public async Task<string> CreateDriverAsync(NewDriverModel newDriver)
        {
            var dataString = string.Empty;
            //****************
            var result = "Info updated";
            var eroorMsg = string.Empty;

            //create user name first

            var userName = System.Net.WebUtility.UrlEncode(newDriver.UserEmail);
            var password = System.Net.WebUtility.UrlEncode(newDriver.Password);
            var addNewUserUri = new UriBuilder(GlobalSetting.Instance.BaseEndpoint)
            {
                //Path = $"/account/RegisterUser?userName={userName}&password={password}"
                Path = "/account/RegisterUser"
            };


            var userUri = ($"{addNewUserUri}?userName={userName}&password={password}").ToString();
            try
            {
                dataString = await _requestProvider.GetAsync<string>(userUri);
                if (dataString == null)
                {

                    return "Unable to register user: " + newDriver.UserEmail;
                }
                if (!dataString.Contains("IsAuthenticated") && !dataString.Contains("IsNotAuthenticated"))
                {
                    return "Unable to register user: " + newDriver.UserEmail;
                }

            }
            catch (WebException wex)
            {
                result = wex.Message;
                return result;
            }
            //create customer as driver

            var builder = new UriBuilder(GlobalSetting.Instance.ApiEndpoint)
            {
                Path = ApiUrlBase + "NewDriver"
            };
            var uri = builder.ToString();
            try
            {
                //   var response = await _requestProvider.PostAsyncString<NewDriver>(uri, newDriver);                 

                //return result;
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


            var callbackUrl = string.Empty;
            var modfyMsg = string.Empty;
            if (dataString.Contains("IsAuthenticated"))
            {

                dataString = dataString.Replace("IsAuthenticated", "");
                callbackUrl = string.Format("{0}/{1}", GlobalSetting.Instance.BaseEndpoint, dataString.Trim());
                modfyMsg = string.Format("Hi {0} ! You have been sent this email because you created an account on our website. Please click on <a href =\"{1}\">this link</a> to confirm your email address is correct. ", newDriver.UserEmail, callbackUrl);



                var message = new SendEmailModel
                {
                    Subject = "Confirm Email Address for New Account",
                    UserName = newDriver.UserEmail,
                    Message = modfyMsg
                };


                var emailUri = new UriBuilder(GlobalSetting.Instance.ApiEndpoint)
                {
                    Path = ApiUrlBase + "SendEmail"
                };
                var sendEmailUri = emailUri.ToString();
                //var token =await  GetUserTokenAsync();
                var token = string.Empty;
                var x = 0;
                while (x < 4)
                {
                    var testUri = _identityService.CreateAuthorizationRequest();
                    var unescapedUrl = System.Net.WebUtility.UrlDecode(testUri);

                    if (unescapedUrl.Contains(GlobalSetting.Instance.IdentityCallback))
                    {
                        var authResponse = new AuthorizeResponse(testUri);
                        if (!string.IsNullOrWhiteSpace(authResponse.Code))
                        {

                            var userToken = await _identityService.GetTokenAsync(authResponse.Code);
                            string accessToken = userToken.AccessToken;

                        }
                    }
                    x++;
                }

                //ar sendEmailUri = API.Common.SendEmail(_remoteServiceCommonUrl);
                var emailResponse = await _requestProvider.PostAsync(sendEmailUri, message, token);

            }





            return result;
        }
        public async Task<ObservableCollection<NewDriverModel>> GetDriversAsync(string token)
        {
            var builder = new UriBuilder(GlobalSetting.Instance.ApiEndpoint)
            {
                Path = ApiUrlBase
            };
            var uri = builder.ToString();

            ObservableCollection<NewDriverModel> drivers =
               await _requestProvider.GetAsync<ObservableCollection<NewDriverModel>>(uri, token);

            return drivers;
        }
        public async Task<NewDriverModel> GetDriverAsync(int driverId, string token)
        {
            try
            {
                UriBuilder builder = new UriBuilder(GlobalSetting.Instance.ApiEndpoint);

                builder.Path = string.Format("GetbyId/{0}", driverId);

                string uri = builder.ToString();

                NewDriverModel driver =
                    await _requestProvider.GetAsync<NewDriverModel>(uri, token);

                return driver;
            }
            catch
            {
                return new NewDriverModel();
            }

        }
        public async Task<NewDriverModel> GetDriverAsync(string userName, string token)
        {
            try
            {
                UriBuilder builder = new UriBuilder(GlobalSetting.Instance.ApiEndpoint);

                builder.Path = string.Format("GetByUserName/userName/{0}", userName);

                string uri = builder.ToString();

                NewDriverModel driver =
                    await _requestProvider.GetAsync<NewDriverModel>(uri, token);

                return driver;
            }
            catch
            {
                return new NewDriverModel();
            }

        }

        async Task<string> GetUserTokenAsync()
        {
            var LoginUrl = _identityService.CreateAuthorizationRequest();
            var unescapedUrl = System.Net.WebUtility.UrlDecode(LoginUrl);
            var authResponse = new AuthorizeResponse(LoginUrl);
            var userToken = await _identityService.GetTokenAsync(authResponse.Code);
            string accessToken = userToken.AccessToken;
            return accessToken;
        }

    }
}
