using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Web.Services
{
    public interface IPictureService
    {
        Task<string> UploadImage(Stream input, string belingTo );
    }
}
