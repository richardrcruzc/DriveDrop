using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DriveDrop.Bl.Services
{
    public class PictureService : IPictureService
    {
        IOptionsSnapshot<AppSettings> _settings;
        private readonly string _pictureUrl;

        private IHostingEnvironment _hostingEnvironment;
        public PictureService(IOptionsSnapshot<AppSettings> settings, IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            _pictureUrl = $"{settings.Value.DriveDropUrl}/api/v1/Pic/UploadFiles/belong/";
            _settings = settings;
        }

        public async Task<string> UploadImage(IFormFile formFile, string belong)
        {
                Guid extName = Guid.NewGuid();
                //saving files
                //   long size = file.Sum(f => f.Length);

                // full path to file in temp location
                var filePath = Path.GetTempFileName();
                var uploads = Path.Combine(_hostingEnvironment.WebRootPath, string.Format("uploads\\img\\{0}", belong));
                var fileName = "";


                var extension = ".jpg";
                if (formFile.Length > 0)
                {

                    if (formFile.FileName.ToLower().EndsWith(".jpg"))
                        extension = ".jpg";
                    if (formFile.FileName.ToLower().EndsWith(".tif"))
                        extension = ".tif";
                    if (formFile.FileName.ToLower().EndsWith(".png"))
                        extension = ".png";
                    if (formFile.FileName.ToLower().EndsWith(".gif"))
                        extension = ".gif";
                    if (formFile.FileName.ToLower().EndsWith(".pdf"))
                        extension = ".pdf";
                    if (formFile.FileName.ToLower().EndsWith(".jpeg"))
                        extension = ".jpeg";


                    filePath = string.Format("{0}\\{1}{2}", uploads, extName, extension);
                    fileName = string.Format("/uploads/img/{0}/{1}{2}", belong, extName, extension);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }

                }
                fileName = string.Format("{0}/{1}{2}", belong, extName, extension);
                return fileName;
            }
        public  string DisplayImage(string partialUri)
        {
            var fullUri = string.Empty;
            if (string.IsNullOrEmpty(partialUri))
                partialUri = "profile-icon.png";

              fullUri =  _settings.Value.PicBaseUrl.Replace("[0]", System.Net.WebUtility.UrlEncode(partialUri));
            return fullUri;
        }

    }
}
