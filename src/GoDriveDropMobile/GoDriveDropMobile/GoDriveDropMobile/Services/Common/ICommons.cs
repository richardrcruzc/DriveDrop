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
        Task<CalculatedChargeModel> CalcTotal(string token, double distance, decimal weight, int priority, int packageSizeId, string promoCode = null,
                                                        decimal extraCharge = 0, string extraNote = null, string state = null, string city = null);
        Task<List<Generic>> PriorityTypes(string token);
        Task<List<Generic>> PackageSizes(string token);
        Task<List<Generic>> VehicleTypes(string token);
        Task<string> UploadImage(Stream input, string belingTo );
        Task<string> ValidateUserName(string userName);
        
    }
}
