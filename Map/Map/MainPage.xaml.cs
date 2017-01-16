using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Maps;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

namespace Map
{
    public sealed partial class MainPage : Page
    {
        Geolocator geolocator;
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            MyMap.MapServiceToken = "abcdef-abcdefghijklmno";

            //var httpsource = new HttpMapTileDataSource("http://a.tile.openstreetmap.org/{zoomlevel}/{x}/{y}.png");
            //var ts = new MapTileSource(httpsource);
            //MyMap.TileSources.Add(ts);

            geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 50;
            try
            {
                Geoposition geoposition = await geolocator.GetGeopositionAsync(
                    maximumAge: TimeSpan.FromMinutes(5),
                    timeout: TimeSpan.FromSeconds(10));

                MapIcon mapIcon = new MapIcon();
                mapIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/my-position.png"));
                mapIcon.Title = "Current Location";
                mapIcon.Location = new Geopoint(new BasicGeoposition()
                {
                    //Latitude = geoposition.Coordinate.Latitude,
                    //Longitude = geoposition.Coordinate.Longitude
                    Latitude = geoposition.Coordinate.Point.Position.Latitude,
                    Longitude = geoposition.Coordinate.Point.Position.Longitude
                });
                mapIcon.NormalizedAnchorPoint = new Point(0.5, 0.5);
                MyMap.MapElements.Add(mapIcon);
                await MyMap.TrySetViewAsync(mapIcon.Location, 18D, 0, 0, MapAnimationKind.Bow);

                /*var pushpin = CreatePushPin();
                MyMap.Children.Add(pushpin);
                var location = new Geopoint(new BasicGeoposition()
                {
                    Latitude = geoposition.Coordinate.Latitude,
                    Longitude = geoposition.Coordinate.Longitude
                });
                MapControl.SetLocation(pushpin, location);
                MapControl.SetNormalizedAnchorPoint(pushpin, new Point(0.0, 1.0));
                await MyMap.TrySetViewAsync(location, 18D, 0, 0, MapAnimationKind.Bow);*/
                progressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                mySlider.Value = MyMap.ZoomLevel;
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox("Location service is turned off!");
            }
            base.OnNavigatedTo(e);
        }

        private DependencyObject CreatePushPin()
        {
            // Creating a Grid element.
            //var myGrid = new Grid();
            //myGrid.RowDefinitions.Add(new RowDefinition());
            //myGrid.RowDefinitions.Add(new RowDefinition());
            //myGrid.Background = new SolidColorBrush(Colors.Transparent);

            // Creating a Polygon Marker
            Polygon polygon = new Polygon();
            polygon.Points.Add(new Point(0, 0));
            polygon.Points.Add(new Point(0, 50));
            polygon.Points.Add(new Point(25, 0));
            polygon.Fill = new SolidColorBrush(Colors.Red);

            // Adding the Polygon to the Grid
            //myGrid.Children.Add(polygon);
            return polygon;
        }

        private async void MyMap_MapTapped(MapControl sender, MapInputEventArgs args)
        {
            Geopoint pointToReverseGeocode = new Geopoint(args.Location.Position);

            // Reverse geocode the specified geographic location.
            MapLocationFinderResult result =
                await MapLocationFinder.FindLocationsAtAsync(pointToReverseGeocode);

            var resultText = new StringBuilder();

            if (result.Status == MapLocationFinderStatus.Success)
            {
                resultText.AppendLine(result.Locations[0].Address.District + ", " + result.Locations[0].Address.Town + ", " + result.Locations[0].Address.Country);
            }

            MessageBox(resultText.ToString());
        }

        private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (MyMap != null)
                MyMap.ZoomLevel = e.NewValue;
        }

        private async void LocateMe_Click(object sender, RoutedEventArgs e)
        {
            progressBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
            geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 50;

            try
            {
                Geoposition geoposition = await geolocator.GetGeopositionAsync(
                    maximumAge: TimeSpan.FromMinutes(5),
                    timeout: TimeSpan.FromSeconds(10));
                await MyMap.TrySetViewAsync(geoposition.Coordinate.Point, 18D);
                mySlider.Value = MyMap.ZoomLevel;
                progressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox("Location service is turned off!");
            }
        }

        private async void MessageBox(string message)
        {
            var dialog = new MessageDialog(message.ToString());
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => await dialog.ShowAsync());
        }
    }
}
