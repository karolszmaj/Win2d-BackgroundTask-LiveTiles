using System;
using System.Diagnostics;
using Windows.ApplicationModel.Background;
using Windows.System;
using Windows.UI.StartScreen;
using TilleDrawingEngine;

namespace Win2d_LiveTileRenderTask
{
    public sealed class TileDrawingTask: IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();
            Debug.WriteLine("Started with memory usage: {0} MB", (float)(MemoryManager.AppMemoryUsage/2014f/2014f));
            //we launch an async operation using the async / await pattern    
            var tileGenerator = new TileDrawer(new Uri("ms-appx:///Assets/PoznanDowntown.jpg", UriKind.Absolute));;
            for (int i = 1; i <= 5; i++)
            {
                var savedFileLocation = tileGenerator.DrawAndSaveTileBitmap(i, 1);
                var secondaryTile = new SecondaryTile(i.ToString());
                secondaryTile.WideLogo = new Uri(savedFileLocation, UriKind.Absolute);
                var updated = secondaryTile.UpdateAsync().AsTask().Result;

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }

            Debug.WriteLine("End with memory usage: {0} MB", (float)(MemoryManager.AppMemoryUsage / 2014f / 2014f));
            deferral.Complete();
        }
    }
}
