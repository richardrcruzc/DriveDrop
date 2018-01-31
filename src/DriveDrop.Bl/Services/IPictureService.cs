using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Bl.Services
{
    public interface IPictureService
    {
        Task<string> UploadImage(IFormFile formFile, string belong);
       string  DisplayImage(string partialUri);
    }
}
