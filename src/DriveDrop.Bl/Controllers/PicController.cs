using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DriveDrop.Bl.Controllers
{
    [Route("[controller]")]
    [Consumes("application/json", "application/json-patch+json", "multipart/form-data")]
    public class PicController : Controller
    {
        private readonly IHostingEnvironment _env;
        public PicController(IHostingEnvironment env)
        {
            _env = env;
        }

        [HttpPost]
        [Route("UploadFiles/belong/{belong}")]
        public async Task<IActionResult> Post(IFormFile formFile, string belong)
        {
            Guid extName = Guid.NewGuid();
            //saving files
         //   long size = file.Sum(f => f.Length);

            // full path to file in temp location
            var filePath = Path.GetTempFileName();
            var uploads = Path.Combine(_env.WebRootPath, string.Format("uploads\\img\\{0}", belong));
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
            return Ok(fileName);
        }

        [HttpGet]
        [Route("[action]/fileName/{fileName}/pic")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        // GET: /<controller>/
        public   IActionResult GetImage(string fileName)
        {
            if (fileName == null)
            {
                return NotFound();
            }

            fileName = System.Net.WebUtility.UrlDecode(fileName);
            fileName= fileName.Replace("/", "\\");
            var webRoot = _env.WebRootPath;
            var path = Path.Combine(webRoot, fileName);
            path = string.Format("{0}\\uploads\\img\\{1}", webRoot, fileName);
            string imageFileExtension = Path.GetExtension(fileName);
            string mimetype = GetImageMimeTypeFromImageFileExtension(imageFileExtension);

            if(!System.IO.File.Exists(path))
            return NotFound();

            var buffer = System.IO.File.ReadAllBytes(path);

            return  File(buffer, mimetype);
        }

        private string GetImageMimeTypeFromImageFileExtension(string extension)
        {
            string mimetype;

            switch (extension)
            {
                case ".png":
                    mimetype = "image/png";
                    break;
                case ".gif":
                    mimetype = "image/gif";
                    break;
                case ".jpg":
                case ".jpeg":
                    mimetype = "image/jpeg";
                    break;
                case ".bmp":
                    mimetype = "image/bmp";
                    break;
                case ".tiff":
                    mimetype = "image/tiff";
                    break;
                case ".wmf":
                    mimetype = "image/wmf";
                    break;
                case ".jp2":
                    mimetype = "image/jp2";
                    break;
                case ".svg":
                    mimetype = "image/svg+xml";
                    break;
                default:
                    mimetype = "application/octet-stream";
                    break;
            }

            return mimetype;
        }
    }
}
