using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DriveDrop.Core.Views
{
    public class WebsiteView : BaseView
    {
        public WebsiteView(string site, string title)
        {
            this.Title = title;
            var webView = new WebView();
            webView.Source = new UrlWebViewSource
            {
                Url = site
            };
            Content = webView;
        }
    }
}
