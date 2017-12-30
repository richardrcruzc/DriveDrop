using DriveDrop.Core.Models.Commons;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveDrop.Core.Services.Common
{
    
    public interface ICommons
    {
        Task<IEnumerable<Generic>> VehicleTypes(string token);
        Task<string> UploadImage(Stream input, string belingTo, string token);
        Task<string> ValidateUserName(string userName, string token);
        
    }
}
