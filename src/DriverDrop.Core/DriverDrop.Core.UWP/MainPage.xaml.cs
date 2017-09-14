namespace DriverDrop.Core.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();
            LoadApplication(new DriverDrop.Core.App());
        }
    }
}