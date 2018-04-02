using GoDriveDrop.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoDriveDrop.Core.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NewPackageView : ContentPage
	{
		public NewPackageView ()
		{
			InitializeComponent ();
            Task.Run(async () => await (BindingContext as BaseViewModel).InitializeAsync(null));
        }
        private async Task AttachEventhandlers()
        {

            await (BindingContext as BaseViewModel).InitializeAsync(null);
 
            //Disappearing += (sender, e) =>
            //{
            //    if (BindingContext is BaseViewModel baseViewModel)
            //    {
            //        baseViewModel.OnDisappearing();
            //    }
            //};
            //LayoutChanged += (sender, e) =>
            //{
            //    if (BindingContext is BaseViewModel baseViewModel)
            //    {
            //        baseViewModel.OnLayoutChanged();
            //    }
            //};
        }
    }
}