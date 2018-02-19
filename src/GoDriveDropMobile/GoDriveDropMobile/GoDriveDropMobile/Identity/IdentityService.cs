using GoDriveDrop.Core.Helpers;
using GoDriveDrop.Core.Models;
using GoDriveDrop.Core.Services.RequestProvider;
 
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModel.Client;
//using PCLCrypto;
//using static PCLCrypto.WinRTCrypto;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using GoDriveDrop.Core.Models.Commons;

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
            string postBody = @"client_id=" + GlobalSetting.Instance.ClientId + 
                "&client_secret=" + GlobalSetting.Instance.ClientSecret + 
                "&grant_type=password&username=" + username + "&password=" + password + "&scope=openid profile drivedrop offline_access";
           

                   var response = await  _requestProvider.PostAsync(GlobalSetting.Instance.TokenEndpoint, new StringContent(postBody, Encoding.UTF8, "application/x-www-form-urlencoded"));
             
               var  token = JsonConvert.DeserializeObject<LoginToken>(response.ToString());
            return token;
        }

        public string CreateAuthorizationRequest()
        {
            // Create URI to authorization endpoint
           

           // var authorizeRequest = new AuthorizeRequest(GlobalSetting.Instance.IdentityEndpoint);

            // Dictionary with values for the authorize request
            var dic = new Dictionary<string, string>();
            dic.Add("client_id", GlobalSetting.Instance.ClientId);
            dic.Add("client_secret", GlobalSetting.Instance.ClientSecret);
            dic.Add("response_type", "code id_token");
            //dic.Add("scope", "openid profile drivedrop basket orders locations marketing offline_access");
            dic.Add("scope", "openid profile drivedrop offline_access");
            dic.Add("redirect_uri", GlobalSetting.Instance.IdentityCallback);
            dic.Add("nonce", Guid.NewGuid().ToString("N"));
         //   dic.Add("code_challenge", CreateCodeChallenge());
            dic.Add("code_challenge_method", "S256");

            // Add CSRF token to protect against cross-site request forgery attacks.
            var currentCSRFToken = Guid.NewGuid().ToString("N");
            dic.Add("state", currentCSRFToken);


            var request = new RequestUrl(GlobalSetting.Instance.IdentityEndpoint);
            var authorizeUri = request.Create(dic);

            return authorizeUri;



            //var authorizeUri = authorizeRequest.Create(dic);
            //return authorizeUri;
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
        public async Task<UserToken> GetTokenAsync(string code, string userName, string password)
        {
            string data = string.Format("grant_type=authorization_code&code={0}&redirect_uri={1}&code_verifier={2}&username=admin@driveDrop.com&password=Pass@word1", code, WebUtility.UrlEncode(GlobalSetting.Instance.IdentityCallback), _codeVerifier);
            var token = await _requestProvider.PostAsync<UserToken>(GlobalSetting.Instance.TokenEndpoint, data, GlobalSetting.Instance.ClientId, GlobalSetting.Instance.ClientSecret);
            return token;

        }

        //private string CreateCodeChallenge()
        //{
        //    _codeVerifier = RandomNumberGenerator.CreateUniqueId();
        //    var sha256 = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithm.Sha256);
        //    var challengeBuffer = sha256.HashData(CryptographicBuffer.CreateFromByteArray(Encoding.UTF8.GetBytes(_codeVerifier)));
        //    byte[] challengeBytes;
        //    CryptographicBuffer.CopyToByteArray(challengeBuffer, out challengeBytes);
        //    return Base64Url.Encode(challengeBytes);
        //}

    }
}
