using GoDriveDrop.Core.Models.Commons; 
using System.Collections.ObjectModel; 
using System.Threading.Tasks;

namespace GoDriveDrop.Core.Services.Address
{
    public interface IAddressService
    {
        Task<string> CreateAsync(AddressModel address);
        Task<ObservableCollection<AddressModel>> GetAsync(string token);
        Task<AddressModel> GetAsync(int driverId, string token);
        Task<AddressModel> GetAsync(string userName, string token);
    }
}
