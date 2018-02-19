using GoDriveDrop.Core.Identity;
using GoDriveDrop.Core.Services.OpenUrl;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GoDriveDrop.Core.ViewModels
{

    public class SplashViewModel : BaseViewModel
    {
        private IOpenUrlService _openUrlService;
        private IIdentityService _identityService;

        public SplashViewModel(IOpenUrlService openUrlService, IIdentityService identityService)
        {
            _openUrlService = openUrlService;
            _identityService = identityService; 
        }

        public override   Task InitializeAsync(object navigationData)
        {
               Task.Delay(1000);
              NavigationService.NavigateToAsync<LoginViewModel>();
              NavigationService.RemoveLastFromBackStackAsync();

            return base.InitializeAsync(navigationData);
        }
    }
}
