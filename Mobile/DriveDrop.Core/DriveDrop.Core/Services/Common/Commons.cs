using DriveDrop.Core.Models.Commons;
using DriveDrop.Core.Services.RequestProvider;
using PCLStorage;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DriveDrop.Core.Services.Common
{
    public class Commons:ICommons
    {
        private readonly IRequestProvider _requestProvider;
        private const string ApiUrlBase = "api/v1/common";

        public Commons(IRequestProvider requestProvider)
        {
            _requestProvider = requestProvider;
        }

        public async Task<IEnumerable<Generic>> VehicleTypes( string token)
        {
            var builder = new UriBuilder(GlobalSetting.Instance.ApiEndpoint)
            {
                Path = $"{ApiUrlBase}/TransportTypes"
            };
            var uri = builder.ToString();

            IEnumerable<Generic> vehicleTypes =
                  await _requestProvider.GetAsync<IEnumerable<Generic>>(uri, token);

            return vehicleTypes;
        }

        public async Task<string> UploadImage(Stream input, string belingTo )
        {
            var returnedFileName = "";
            if (input == null)
                return returnedFileName;

            var builder = new UriBuilder(GlobalSetting.Instance.ApiEndpoint)
            {
                Path = $"api/v1/Pic/UploadFiles/belong/{belingTo}"
            };
            var uri = builder.ToString();
            
            try { 
            using (var content = new MultipartFormDataContent())
            {
                using (var client = new HttpClient())
                {
                    content.Add(new StreamContent(input)
                    {
                        Headers =
                {
                    ContentLength = input.Length,
                    ContentType = new MediaTypeHeaderValue("image/jpeg")
                }
                    }, "formFile", "formFile");

                    var response = await client.PostAsync(uri, content);
                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            returnedFileName = await response.Content.ReadAsStringAsync();
                        }
                        if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                        {
                        }
                            
                }
            }
        }
            catch (Exception e)
            {
                //debug
                Debug.WriteLine("Exception Caught: " + e.ToString());
 
            }
            return returnedFileName;
        }


        public async Task<string> ValidateUserName(string userName )
        {
            var builder = new UriBuilder(GlobalSetting.Instance.ApiEndpoint)
            {
                Path = $"{ApiUrlBase}/ValidateUserName/{userName}"
            };
            var uri = builder.ToString();


            string result =
                 await _requestProvider.GetAsync<string>(uri );


            return result;
        }



        //This for converting media file stream to byte[]
        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }



    }
}
