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
	public partial class LoginView : ContentPage
	{

      //  LoginViewModel loginvm;
        
		public LoginView()
		{
			
           	//var vm = BindingContext as LoginViewModel;

            //vm.DisplayInvalidLoginPrompt += () => DisplayAlert("Error", "Invalid Login, try again", "OK");
            //vm.DisplayInvalidActionPrompt += () => DisplayAlert("Note", "Option not yet available", "OK");
            
             InitializeComponent();

            //Email.Completed += (object sender, EventArgs e) =>
            //{
            //    Password.Focus();
            //};

            //Password.Completed += (object sender, EventArgs e) =>
            //{
            //    vm.SubmitCommand.Execute(null);
            //};
        }
	}
}