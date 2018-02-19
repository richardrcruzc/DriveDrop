using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using ImageCircle.Forms.Plugin.Droid;

namespace DriveDrop.Droid
{
    [Activity(Label = "DriveDrop", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {

            FormsAppCompatActivity.ToolbarResource = Resource.Layout.Toolbar;
            FormsAppCompatActivity.TabLayoutResource = Resource.Layout.Tabbar;
            base.OnCreate(bundle);
            Forms.Init(this, bundle);
            Plugin.MediaManager.Forms.Android.VideoViewRenderer.Init();
            ImageCircleRenderer.Init();
            LoadApplication(new DriveDrop.Core.App());

            //TabLayoutResource = Resource.Layout.Tabbar;
            //ToolbarResource = Resource.Layout.Toolbar;

            //base.OnCreate(bundle);

            //global::Xamarin.Forms.Forms.Init(this, bundle);
            //LoadApplication(new DriveDrop.Core.App());
        }
    }
}

