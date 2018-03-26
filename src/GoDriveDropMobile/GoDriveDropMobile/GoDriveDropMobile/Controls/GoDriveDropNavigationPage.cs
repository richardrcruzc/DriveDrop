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

         BarBackgroundColor = Color.FromHex("#43b249");
            BarTextColor = Color.White;
        }
    }
}

