using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using TilleDrawingEngine;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace Win2d_LiveTileRenderSample_wp81UA
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            RegisterBackgroundTask();
        }

        private async void CreateAndPinTileWithId(object sender, TappedRoutedEventArgs e)
        {
            var button = sender as Button;
            var id = int.Parse(button.Content.ToString());

            var bitmapPath = await Task.Factory.StartNew(() =>
            {
                var tileGenerator = new TileDrawer(new Uri("ms-appx:///Assets/PoznanDowntown.jpg", UriKind.Absolute));
                return tileGenerator.DrawAndSaveTileBitmap(id, 0);
            });

            PinTile(id.ToString(), new Uri(bitmapPath, UriKind.Absolute));
        }

        private void PinTile(string id, Uri tileBitmap)
        {
            var secondaryTile = new SecondaryTile(id,
                                                                "Title text shown on the tile",
                                                                "!",
                                                                tileBitmap,
                                                                TileSize.Default);
            secondaryTile.VisualElements.ShowNameOnSquare310x310Logo = true;
            secondaryTile.WideLogo = tileBitmap;
            secondaryTile.RequestCreateAsync();
        }

        private async void RegisterBackgroundTask()
        {
            try
            {
                BackgroundAccessStatus status = await BackgroundExecutionManager.RequestAccessAsync();
                if (status == BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity || status == BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity)
                {
                    bool isRegistered = BackgroundTaskRegistration.AllTasks.Any(x => x.Value.Name == "TileUpdater");
                    if (!isRegistered)
                    {
                        BackgroundTaskBuilder builder = new BackgroundTaskBuilder
                        {
                            Name = "TileUpdater",
                            TaskEntryPoint =
                                "Win2d_LiveTileRenderTask.TileDrawingTask"
                        };
                        builder.SetTrigger(new TimeTrigger(30, false));
                        BackgroundTaskRegistration task = builder.Register();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("The access has already been granted");
            }
        }
    }
}
