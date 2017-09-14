using DD.Mobile.Models.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
 


namespace DD.Mobile.Pages.Drivers
{
   public class DriverTravelPage : ContentPage
    {
        public DriverTravelPage(DriverEntry driverItem)
        {
            Title = "Distance Travelled";


            ////initial our map object
            //var trailMap = new Map();
            ////place a pin on the map for the chosen walk type

            //trailMap.Pins.Add(new Pin
            //    {
            //    Type=PinType.Place,
            //    Label = driverItem.FirstName,
            //    Position = new Position(driverItem.Latitude, driverItem.longitude)
            //});

            //// center the map arround the list of driver's location
            //trailMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(driverItem.Latitude, driverItem.longitude), Distance.FromKilometers(1.0)));

            var trailNameLabel = new Label()
            {
                FontSize=18,
                FontAttributes=  FontAttributes.Bold,
                TextColor =Color.Black,
                Text=driverItem.FirstName,
            };
            var trailDistanceTraveledLabel = new Label()
            {
                FontSize = 20,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.Black,
                Text = "Distance Traveled",
                HorizontalTextAlignment= TextAlignment.Center
            };
            var totalDistanceTaken = new Label()
            {
                FontSize = 20,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.Black,
                Text = $"{driverItem.MaxPackage}Km",                
                HorizontalTextAlignment = TextAlignment.Center
            };
            var totalTimeTakenLabel = new Label()
            {
                FontSize = 20,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.Black,
                Text = "Time Taken",
                HorizontalTextAlignment = TextAlignment.Center
            };

            var totalTimeTaken = new Label()
            {
                FontSize = 20,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.Black,
                Text = "oh om os",
                HorizontalTextAlignment = TextAlignment.Center
            };

            var walksHomeButtom = new Button
            {
                BackgroundColor =Color.FromHex("#008080"),
                TextColor=Color.White,
                Text="End this Trail"
            };
            // setup event handler 
            walksHomeButtom.Clicked += (sender, e) =>
              {
                  if (driverItem == null) return;
                  Navigation.PopToRootAsync(true);
                  //walkeItem = null;
              };

            this.Content = new ScrollView
            {
                Padding = 10,
//                Content = new StackLayout
//                {
//                    Orientation = StackOrientation.Vertical,
//                    HorizontalOptions=LayoutOptions.FillAndExpand,
//                    Children = {
//                              trailDistanceTraveledLabel, totalDistanceTaken,totalTimeTakenLabel, totalTimeTaken, walksHomeButtom

//                    }
//                }

            };
        }
    }
}
