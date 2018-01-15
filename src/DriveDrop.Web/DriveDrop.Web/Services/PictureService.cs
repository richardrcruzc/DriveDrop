using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DriveDrop.Web.Services
{
    public class PictureService: IPictureService
    {

        private readonly string _pictureUrl;     
               

        public PictureService(IOptionsSnapshot<AppSettings> settings)
        {

            _pictureUrl = $"{settings.Value.DriveDropUrl}/api/v1/Pic/UploadFiles/belong/";
        }

        public async Task<string> UploadImage(Stream input, string belingTo )
        {
            var returnedFileName = "";
            if (input == null)
                return returnedFileName;

            var uri = _pictureUrl + belingTo;

            try
            {
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
                //Debug.WriteLine("Exception Caught: " + e.ToString());

            }
            return returnedFileName;
        }


    }
}
