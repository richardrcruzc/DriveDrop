using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using DriverDrop.Droid.Renderers;
using Xamarin.Forms.Platform.Android.AppCompat;

[assembly: ExportRenderer(typeof(NavigationPage), typeof(CustomNavigationPageRenderer))]
namespace DriverDrop.Droid.Renderers
{
    public class CustomNavigationPageRenderer : NavigationPageRenderer
    {
        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);

            if (Element.CurrentPage == null)
            {
                return;
            }

            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);

            if (toolbar != null)
            {
                var image = toolbar.FindViewById<ImageView>(Resource.Id.toolbar_image);

                if (!string.IsNullOrEmpty(Element.CurrentPage.Title))
                    image.Visibility = Android.Views.ViewStates.Invisible;
                else
                    image.Visibility = Android.Views.ViewStates.Visible;
            }
        }
    }
}