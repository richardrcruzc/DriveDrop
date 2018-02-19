using System;
using Xamarin.Forms;

namespace GoDriveDrop.Core.Controls
{
    public class GoDriveDropNavigationPage : NavigationPage
    {
        public GoDriveDropNavigationPage(Page root) : base(root)
        {
            Init();
        }

        public GoDriveDropNavigationPage()
        {
            Init();
        }

        void Init()
        {

            BarBackgroundColor = Color.FromHex("#03A9F4");
            BarTextColor = Color.White;
        }
    }
}

