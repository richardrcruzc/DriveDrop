using GoDriveDrop.Core.Models;
using GoDriveDrop.Core.Models.Commons;
using GoDriveDrop.Core.Services.RequestProvider;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GoDriveDrop.Core.Services.User
{
    public class UserService : IUserService
    {
        private readonly IRequestProvider _requestProvider;

        public UserService(IRequestProvider requestProvider)
        {
            _requestProvider = requestProvider;
        }
        public async Task<string> GetLoginTokenAsync(string username,string password)
        {
            LoginToken token = new LoginToken { Access_token = "", Expires_in = "", Token_type = "", Refresh_token = "" };
            UriBuilder builder = new UriBuilder(GlobalSetting.Instance.UserInfoEndpoint);
            string uri = builder.ToString();

            string postBody = @"client_id=" + GlobalSetting.Instance.ClientId + 
                "&client_secret=" + GlobalSetting.Instance.ClientSecret + 
                "&grant_type=password&username=" + username +
                "&password=" + password + 
                "&scope="+
                " offline_access";

            var postString = new StringContent(postBody, Encoding.UTF8, "application/x-www-form-urlencoded");


            //StringContent token = await _requestProvider.PostAsync<StringContent>(uri, postString);

            using (HttpClient client = new HttpClient())
            {
                var accept = "application/json";
                client.DefaultRequestHeaders.Add("Accept", accept);

                var response = client.PostAsync("http://10.0.0.53:5205", postString).Result;
                if (response.IsSuccessStatusCode)
                {

                    string responseStream =await response.Content.ReadAsStringAsync();
                    token = JsonConvert.DeserializeObject<LoginToken>(responseStream);
                }

            }




            return token.ToString();
            //if (response.IsSuccessStatusCode)
            //{

            //    string responseStream = response.Content.ReadAsStringAsync().Result;
            //    token = JsonConvert.DeserializeObject<LoginToken>(responseStream);
            //}
        }

        public async Task<UserInfo> GetUserInfoAsync(string authToken)
        {
            UriBuilder builder = new UriBuilder(GlobalSetting.Instance.UserInfoEndpoint);

            string uri = builder.ToString();

            var userInfo =
                await _requestProvider.GetAsync<UserInfo>(uri, authToken);

            return userInfo;
        }

        public async Task<CustomerModel> MyAccount(string authToken)
        {
            try
            {
                UriBuilder builder = new UriBuilder(GlobalSetting.Instance.UserInfoEndpoint);

                string uri = builder.ToString();

                var userInfo =
                    await _requestProvider.GetAsync<UserInfo>(uri, authToken);


                //get the current customer info
                var username = System.Net.WebUtility.UrlEncode(userInfo.Email);
                builder = new UriBuilder(GlobalSetting.Instance.ApiEndpoint)
                {
                    Path = $"/api/v1/CurrentUser/{username}"
                };
                uri = builder.ToString();

                var customerString =
                   await _requestProvider.GetAsync<string>(uri, authToken);

                var currentCustomer = JsonConvert.DeserializeObject<CustomerModel>(customerString );

                return currentCustomer;
            }
            catch (Exception ex)
            {
                var tt = ex.Message;

            }
            return new CustomerModel();
        }
    }
}
