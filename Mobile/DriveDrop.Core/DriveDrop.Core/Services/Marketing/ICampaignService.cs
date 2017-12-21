using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DriveDrop.Core.Models.Marketing;

namespace DriveDrop.Core.Services.Marketing
{
    public interface ICampaignService
    {
        Task<ObservableCollection<CampaignItem>> GetAllCampaignsAsync(string token);
        Task<CampaignItem> GetCampaignByIdAsync(int id, string token);
    }
}