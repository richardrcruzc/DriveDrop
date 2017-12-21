using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using DriveDrop.Core.ViewModels.Base;
using DriveDrop.Core.Helpers;
using DriveDrop.Core.Models.Marketing;
using DriveDrop.Core.Services.Marketing;

namespace DriveDrop.Core.ViewModels
{
    public class CampaignDetailsViewModel : ViewModelBase
    {
        private CampaignItem _campaign;
        private bool _isDetailsSite;
        private readonly ICampaignService _campaignService;

        public CampaignDetailsViewModel(ICampaignService campaignService)
        {
            _campaignService = campaignService;
        }

        public CampaignItem Campaign
        {
            get => _campaign;
            set
            {
                _campaign = value;
                RaisePropertyChanged(() => Campaign);
            }
        }

        public bool IsDetailsSite
        {
            get => _isDetailsSite;
            set
            {
                _isDetailsSite = value;
                RaisePropertyChanged(() => IsDetailsSite);
            }
        }

        public override async Task InitializeAsync(object navigationData)
        {
            if (navigationData is int)
            {
                IsBusy = true;

                // Get campaign by id
                Campaign = await _campaignService.GetCampaignByIdAsync((int)navigationData, Settings.AuthAccessToken);

                IsBusy = false;
            }
        }

        public ICommand EnableDetailsSiteCommand => new Command(EnableDetailsSite);

        private void EnableDetailsSite()
        {
            IsDetailsSite = true;
        }
    }
}