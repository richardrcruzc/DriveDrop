using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveDrop.Core.Services.Common
{
    public interface IGoogleAddress
    {
        Task<IEnumerable<string>> AutoComplete(string addressPart, string token);
        Task<DriveDrop.Core.Models.Commons.AddressModel> CompleteAddress(string address, string token);
    }
}
