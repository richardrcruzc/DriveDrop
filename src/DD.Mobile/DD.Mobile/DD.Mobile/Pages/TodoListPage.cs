using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DD.Mobile.Pages
{
    public class TodoListPage : ContentPage
    {
        public TodoListPage()
        {
            Title = "TodoList Page";
            Content = new StackLayout
            {
                Children = {
                    new Label {
                        Text = "Todo list data goes here",
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.CenterAndExpand
                    }
                }
            };
        }
    }
}
