using DriveDrop.Core.Services.RequestProvider;
using System;
using System.Threading.Tasks;
using DriveDrop.Core.Models.User;
using DriveDrop.Core.Models.Commons;

namespace DriveDrop.Core.Services.User
{
    public class UserService : IUserService
    {
        private readonly IRequestProvider _requestProvider;

        public UserService(IRequestProvider requestProvider)
        {
            _requestProvider = requestProvider;
        }

        public async Task<UserInfo> GetUserInfoAsync(string authToken)
        {
            UriBuilder builder = new UriBuilder(GlobalSetting.Instance.UserInfoEndpoint);

            string uri = builder.ToString();

            var userInfo =
                await _requestProvider.GetAsync<UserInfo>(uri, authToken);

            return userInfo;
        }
        public async Task<string> GetUserInfoAsync(string authToken, string userName, string password)
        {
            var model = new LoginModel { Email =  userName, Password =  password };

            UriBuilder builder = new UriBuilder(GlobalSetting.Instance.UserValidation) ;

            string uri = builder.ToString();
            try
            {
                var response =
                    await _requestProvider.PostAsync(uri, model, authToken);
                model.ReturnUrl = response.ReturnUrl;
            }
            catch(Exception ex)
            {
                model.ReturnUrl +=" "+ ex.Message.ToString();
            }
            return model.ReturnUrl;
            }
    }
}