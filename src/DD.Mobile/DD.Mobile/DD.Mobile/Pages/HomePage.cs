﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace DD.Mobile.Pages
{
	public class HomePage : ContentPage
	{
		public HomePage ()
		{
			Content = new StackLayout {
				Children = {
					new Label { Text = "Welcome to Xamarin Forms!" }
				}
			};
		}
	}
}