using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoDriveDrop.Core.Extensions
{
    [ContentProperty("SourceImage")]
    public class PlatformImageExtension : IMarkupExtension<string>
    {

        public string SourceImage { get; set; }

        public string ProvideValue(IServiceProvider serviceProvider)
        {
            if (SourceImage == null)
                return null;

            string imagePath;
            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    imagePath = SourceImage;
                    break;
                case Device.iOS:
                    imagePath = SourceImage + ".png";
                    break;
                case Device.UWP:
                    imagePath = "Images/" + SourceImage + ".png";
                    break;
                case Device.WinPhone:
                    imagePath = "Images/" + SourceImage + ".png";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return imagePath;
        }

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
        {
            return ProvideValue(serviceProvider);
        }
    }
}


//Usage
/*
<ContentPage.ToolbarItems>
        <ToolbarItem Order="Primary" Priority="1" Command="{Binding CloseCommand}"
                     Icon="{markupExtensions:PlatformImage SourceImage='refresh'}" />
    </ContentPage.ToolbarItems>

*/
