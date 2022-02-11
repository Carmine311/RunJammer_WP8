using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Shell;
using RunJammer.WP.Model;

namespace RunJammer.WP.UI.Controls
{
    public partial class RunSessionMapUserControl : UserControl
    {
        private MapLayer _songsLayer;


        public static readonly DependencyProperty CurrentLocationProperty = DependencyProperty.Register(
            "CurrentLocation", typeof(GeoCoordinate), typeof(RunSessionMapUserControl), new PropertyMetadata(default(GeoCoordinate), OnCurrentLocationChanged));

        private static void OnCurrentLocationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (RunSessionMapUserControl)d;
            control.UpdateMapCenter(e.NewValue as GeoCoordinate);
        }

        public static readonly DependencyProperty HeadingProperty = DependencyProperty.Register(
            "Heading", typeof(double), typeof(RunSessionMapUserControl), new PropertyMetadata(default(double), OnHeadingChanged));

        private static void OnHeadingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                var control = (RunSessionMapUserControl)d;
                control.RouteMap.Heading = (double)e.NewValue;
            }
        }

        public double Heading
        {
            get { return (double)GetValue(HeadingProperty); }
            set { SetValue(HeadingProperty, value); }
        }

        public static readonly DependencyProperty RouteLocationsProperty = DependencyProperty.Register(
            "RouteLocations", typeof(GeoCoordinateCollection), typeof(RunSessionMapUserControl), new PropertyMetadata(default(GeoCoordinateCollection), OnRouteLocationsChanged));

        private static void OnRouteLocationsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (RunSessionMapUserControl)d;
            control.SubscribeToRouteCollectionItemsChanged(e.NewValue as GeoCoordinateCollection);
        }

        private void SubscribeToRouteCollectionItemsChanged(GeoCoordinateCollection routeCollection)
        {
            routeCollection.CollectionChanged += HandleRouteCollectionItemsChanged;
        }

        private void HandleRouteCollectionItemsChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        var line = RouteMap.MapElements.FirstOrDefault();
                        var mapLine = line as MapPolyline;
                        if (mapLine != null) mapLine.Path.Add(item as GeoCoordinate);
                    });
                }
            }

        }

        public static readonly DependencyProperty RunSessionWaypointsProperty = DependencyProperty.Register(
            "RunSessionWaypoints", typeof(ObservableCollection<RunSessionWaypoint>), typeof(RunSessionMapUserControl), new PropertyMetadata(default(ObservableCollection<RunSessionWaypoint>), OnWaypointsChanged));

        private static void OnWaypointsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (RunSessionMapUserControl)d;
            control.SubscribeToWaypointsCollectionChanged(e.NewValue as ObservableCollection<RunSessionWaypoint>);
        }

        private void SubscribeToWaypointsCollectionChanged(ObservableCollection<RunSessionWaypoint> list)
        {
            list.CollectionChanged += HandleWaypointsCollectionChanged;
        }

        private void HandleWaypointsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var newWaypoints = e.NewItems.OfType<RunSessionWaypoint>().ToList();
            foreach (var runSessionWaypoint in newWaypoints)
            {
                UpdateMapLayers(runSessionWaypoint);
            }
        }

        private void UpdateMapLayers(RunSessionWaypoint runSessionWaypoint)
        {
            var song = runSessionWaypoint.CurrentSong;
            var lastWaypoint = _songsLayer.LastOrDefault();
            var lastWaypointSong = lastWaypoint != null ? lastWaypoint.Content as RunJammerSong : null;

            if (lastWaypointSong != null && lastWaypointSong == song) return;

            _songsLayer.Add(new MapOverlay { GeoCoordinate = new GeoCoordinate(runSessionWaypoint.Lat, runSessionWaypoint.Lon, runSessionWaypoint.Altitude), Content = runSessionWaypoint.CurrentSong });
        }

        public ICollection<RunSessionWaypoint> RunSessionWaypoints
        {
            get { return (ICollection<RunSessionWaypoint>)GetValue(RunSessionWaypointsProperty); }
            set { SetValue(RunSessionWaypointsProperty, value); }
        }

        public GeoCoordinateCollection RouteLocations
        {
            get { return (GeoCoordinateCollection)GetValue(RouteLocationsProperty); }
            set { SetValue(RouteLocationsProperty, value); }
        }

        private void UpdateMapCenter(GeoCoordinate location)
        {
            RouteMap.SetValue(Map.CenterProperty, location);
        }

        public GeoCoordinate CurrentLocation
        {
            get { return (GeoCoordinate)GetValue(CurrentLocationProperty); }
            set { SetValue(CurrentLocationProperty, value); }
        }

        public RunSessionMapUserControl()
        {
            InitializeComponent();
            InitializeMapLayers();
            if (CurrentLocation != null)
            {
                UpdateMapCenter(CurrentLocation);
            }
        }

        private void InitializeMapLayers()
        {
            _songsLayer = new MapLayer();
        }

        private void HandleTerrainButtonClick(object sender, RoutedEventArgs e)
        {
            var tb = sender as ToggleButton;
            if (tb != null)
            {
                if (tb.IsChecked.HasValue && tb.IsChecked.Value)
                {
                    RouteMap.CartographicMode = MapCartographicMode.Aerial;
                }
                else
                {
                    RouteMap.CartographicMode = MapCartographicMode.Road;
                }
            }
        }

        private void HandleSongsButtonClick(object sender, RoutedEventArgs e)
        {
            var tb = sender as ToggleButton;
            if (tb != null)
            {
                if (tb.IsChecked.HasValue && tb.IsChecked.Value)
                {
                    RouteMap.Layers.Add(_songsLayer);
                }
                else
                {
                    RouteMap.Layers.Remove(_songsLayer);
                }
            }
        }
    }
}
