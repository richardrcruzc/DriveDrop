using GoDriveDrop.Core.Models.Commons;
using GoDriveDrop.Core.Services.RequestProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoDriveDrop.Core.Services.Common
{
    public class GoogleAddress: IGoogleAddress
    {

        private readonly IRequestProvider _requestProvider;
        private const string ApiUrlBase = "api/v1/common";

        public GoogleAddress(IRequestProvider requestProvider)
        {
            _requestProvider = requestProvider;
        }


        public async Task<IEnumerable<string>> AutoComplete(string addressPart, string token)
        { 
            var builder = new UriBuilder(GlobalSetting.Instance.ApiEndpoint)
            {
                Path = $"{ApiUrlBase}/Autocomplete/addresspart/{addressPart}"
            };
            var uri = builder.ToString();

            IEnumerable<string> predictions =
                  await _requestProvider.GetAsync<IEnumerable<string>>(uri, token); 
             
            return predictions;
        }
        public async Task<AddressModel> CompleteAddress(string address, string token)
        {
            var builder = new UriBuilder(GlobalSetting.Instance.BasketEndpoint)
            {
                Path = $"{ApiUrlBase}/GetCompleAddress/address/{address}"
            };
            var uri = builder.ToString();

            AddressModel complete =
                  await _requestProvider.GetAsync<AddressModel>(uri, token);
            return complete;
        }
    }
}
