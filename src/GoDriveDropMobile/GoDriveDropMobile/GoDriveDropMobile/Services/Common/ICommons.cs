using GoDriveDrop.Core.Models.Commons;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoDriveDrop.Core.Services.Common
{
    
    public interface ICommons
    {
        Task<IEnumerable<Generic>> VehicleTypes(string token);
        Task<string> UploadImage(Stream input, string belingTo );
        Task<string> ValidateUserName(string userName);
        
    }
}
