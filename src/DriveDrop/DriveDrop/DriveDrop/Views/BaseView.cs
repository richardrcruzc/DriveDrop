using DriveDrop.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DriveDrop.Core.Views
{
    public class BaseView : ContentPage
    {
        public BaseView()
        {
            SetBinding(Page.TitleProperty, new Binding(BaseViewModel.TitlePropertyName));
            SetBinding(Page.IconProperty, new Binding(BaseViewModel.IconPropertyName));
        }
    }
}
