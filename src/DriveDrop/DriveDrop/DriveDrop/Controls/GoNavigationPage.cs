
using Xamarin.Forms;

namespace DriveDrop.Core.Controls
{ 
    public class GoNavigationPage : NavigationPage
    {
        public GoNavigationPage(Page root) : base(root)
        {
            Init();
        }

        public GoNavigationPage()
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
