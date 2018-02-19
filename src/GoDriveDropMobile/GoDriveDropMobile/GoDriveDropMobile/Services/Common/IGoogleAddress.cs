using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoDriveDrop.Core.Services.Common
{
    public interface IGoogleAddress
    {
        Task<IEnumerable<string>> AutoComplete(string addressPart, string token);
        Task<GoDriveDrop.Core.Models.Commons.AddressModel> CompleteAddress(string address, string token);
    }
}
