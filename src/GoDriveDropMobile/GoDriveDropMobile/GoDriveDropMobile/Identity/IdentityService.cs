using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GoDriveDrop.Core.Services.RequestProvider;
using GoDriveDrop.Core.Models;
using GoDriveDrop.Core.Models.Commons;
using IdentityModel;
using PCLCrypto;
using static PCLCrypto.WinRTCrypto;
using System.Net.Http;
using IdentityModel.Client;
using Newtonsoft.Json;
using GoDriveDrop.Core.Helpers;

namespace GoDriveDrop.Core.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly IRequestProvider _requestProvider;
        private string _codeVerifier;

        public IdentityService(IRequestProvider requestProvider)
        {
            _requestProvider = requestProvider;
        }

        public async Task<LoginToken> GetLoginTokenAsync(string password, string username)
        {
            LoginToken token = new LoginToken { Access_token = "", Expires_in = "", Token_type = "", Refresh_token = "" };
            UriBuilder builder = new UriBuilder("http://169.254.80.80:5205/connect/authorize");
            string uri = builder.ToString();

            string postBody = @"client_id=" + GlobalSetting.Instance.ClientId +
                "&client_secret=" + GlobalSetting.Instance.ClientSecret +
                "&grant_type=password&username=" + username +
                "&password=" + password +
                "&scope=openid profile drivedrop offline_access" +
                " offline_access";

             var postString = new StringContent(postBody, Encoding.UTF8, "application/x-www-form-urlencoded");

            using (HttpClient client = new HttpClient())
            {
                var accept = "application/json";
                client.DefaultRequestHeaders.Add("Accept", accept);

                var response0 = client.PostAsync("http://169.254.80.80:5205/connect/authorize", postString).Result;
                if (response0.IsSuccessStatusCode)
                {

                    string responseStream = await response0.Content.ReadAsStringAsync();
                    token = JsonConvert.DeserializeObject<LoginToken>(responseStream);
                }

            }




            var client1 = new  DiscoveryClient("http://169.254.80.80:5205");
            client1.Policy.RequireHttps = false;
          
            var disco = await client1.GetAsync();


            var tokenEndpoint = disco.TokenEndpoint;
            var keys = disco.KeySet.Keys;



            // Get the token
            //
            var tokenClient = new TokenClient(
                disco.TokenEndpoint,
                GlobalSetting.Instance.ClientId,
                GlobalSetting.Instance.ClientSecret);
            var gt = await tokenClient.RequestCustomGrantAsync("authorization_code", "openid profile drivedrop offline_access");
           var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync(username, password, "openid profile drivedrop offline_access");
           // var tokenResponse = await tokenClient.RequestClientCredentialsAsync("drivedrop");
            if (tokenResponse.IsError)
            {
                var token3 = tokenResponse.Error;

            }

            var request = new RequestUrl(disco.AuthorizeEndpoint);
            var url = request.CreateAuthorizeUrl(
    clientId: GlobalSetting.Instance.ClientId,
    scope: "openid profile drivedrop offline_access",    
    responseType: OidcConstants.ResponseTypes.CodeIdToken,
    responseMode: OidcConstants.ResponseModes.FormPost,
    redirectUri: "http://169.254.80.80:5205/xamarincallback",
    state: CryptoRandom.CreateUniqueId(),
    nonce: CryptoRandom.CreateUniqueId()    
    );

            var response = new AuthorizeResponse(url);
            
            var accessToken = response.AccessToken;
            var idToken = response.IdentityToken;
            var state = response.State;

            var tmp = CreateAuthorizationRequest();





            ////StringContent token = await _requestProvider.PostAsync<StringContent>(uri, postString);

            //using (HttpClient client = new HttpClient())
            //{
            //    var accept = "application/json";
            //    client.DefaultRequestHeaders.Add("Accept", accept);

            //    var response = client.PostAsync("http://169.254.80.80:5205/connect/token", postString).Result;
            //    if (response.IsSuccessStatusCode)
            //    {

            //        string responseStream = await response.Content.ReadAsStringAsync();
            //        token = JsonConvert.DeserializeObject<LoginToken>(responseStream);
            //    }

            //}
            return token;
        }

        public string CreateAuthorizationRequest()
        {
            // Create URI to authorization endpoint
            var authorizeRequest = new AuthorizeRequest(GlobalSetting.Instance.IdentityEndpoint);

            // Dictionary with values for the authorize request
            var dic = new Dictionary<string, string>();
            dic.Add("client_id", GlobalSetting.Instance.ClientId);
            dic.Add("client_secret", GlobalSetting.Instance.ClientSecret);
            dic.Add("response_type", "code id_token");
            dic.Add("scope", "openid profile drivedrop locations offline_access");
            dic.Add("redirect_uri",   GlobalSetting.Instance.IdentityCallback);
            dic.Add("nonce", Guid.NewGuid().ToString("N"));
            dic.Add("code_challenge", CreateCodeChallenge());
            dic.Add("code_challenge_method", "S256");

            // Add CSRF token to protect against cross-site request forgery attacks.
            var currentCSRFToken = Guid.NewGuid().ToString("N");
            dic.Add("state", currentCSRFToken);

            var authorizeUri = authorizeRequest.Create(dic);
            return authorizeUri;
             
        }

        public string CreateLogoutRequest(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return string.Empty;
            }

            return string.Format("{0}?id_token_hint={1}&post_logout_redirect_uri={2}",
                GlobalSetting.Instance.LogoutEndpoint,
                token,
                GlobalSetting.Instance.LogoutCallback);
        }

        public async Task<UserToken> GetTokenAsync(string code)
        {
            string data = string.Format("grant_type=authorization_code&code={0}&redirect_uri={1}&code_verifier={2}", code, WebUtility.UrlEncode(GlobalSetting.Instance.IdentityCallback), _codeVerifier);
            var token = await _requestProvider.PostAsync<UserToken>(GlobalSetting.Instance.TokenEndpoint, data, GlobalSetting.Instance.ClientId, GlobalSetting.Instance.ClientSecret);
            return token;
        }

        private string CreateCodeChallenge()
        {
            _codeVerifier = RandomNumberGenerator.CreateUniqueId();
            var sha256 = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithm.Sha256);
            var challengeBuffer = sha256.HashData(CryptographicBuffer.CreateFromByteArray(Encoding.UTF8.GetBytes(_codeVerifier)));
            byte[] challengeBytes;
            CryptographicBuffer.CopyToByteArray(challengeBuffer, out challengeBytes);
            return Base64Url.Encode(challengeBytes);
        }

    }
}
