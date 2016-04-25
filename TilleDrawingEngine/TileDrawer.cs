using Windows.Foundation;
using Windows.System;

namespace TilleDrawingEngine
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using Windows.Storage;
    using Windows.Storage.Streams;
    using Windows.UI;
    using Microsoft.Graphics.Canvas;
    using Logging;

    public class TileDrawer
    {
        private readonly Uri _backgroundPhotoUri;
        public TileDrawer(Uri backgroundPhotoUri)
        {
            this._backgroundPhotoUri = backgroundPhotoUri;
        }

        /// <summary>
        /// Draws and save rendered bitmap to application storage
        /// </summary>
        /// <param name="tileId">Marker for tile Id</param>
        /// <param name="counter">Additional parameter used in render process</param>
        /// <returns>Absolute file location on file system</returns>
        public string DrawAndSaveTileBitmap(int tileId, int counter)
        {
            DisplayStatusWithMemoryWarning("1. Started drawing tile with id: {0} and counter {1}", tileId, counter);
            var canvasDriver = CanvasDevice.GetSharedDevice();
            var canvasBitmap = CanvasBitmap.LoadAsync(canvasDriver, this._backgroundPhotoUri).AsTask().Result;
            DisplayStatusWithMemoryWarning("2. Loaded background bitmap for tile with id: {0} and counter {1}", tileId, counter);

            var savedFileUri = DrawTileWithIdnetifier(tileId, counter, canvasBitmap);
            canvasDriver.Dispose();
            canvasBitmap.Dispose();
            return savedFileUri;
        }

        private string DrawTileWithIdnetifier(int tileId, int counter, CanvasBitmap backgroundBitmap)
        {
            var tileSize = new Size(691, 360);
            var drawingDevice = CanvasDevice.GetSharedDevice();
            var renderer = new CanvasRenderTarget(drawingDevice, (float)tileSize.Width, (float)tileSize.Height, 96);
            var drawingSession = renderer.CreateDrawingSession();

            DisplayStatusWithMemoryWarning("3. Created drawing session for tile with id: {0} and counter {1}", tileId, counter);
            if (backgroundBitmap != null)
            {
                drawingSession.DrawImage(backgroundBitmap, new Windows.Foundation.Rect(new Point(0,0), tileSize));
                backgroundBitmap.Dispose();
                DisplayStatusWithMemoryWarning("4. Bitmap drawn for tile with id: {0} and counter {1}", tileId, counter);
            }

            drawingSession.DrawCircle(60, (float) tileSize.Height/2f, 40, Colors.White);
            drawingSession.FillCircle(60, (float) tileSize.Height/2f, 40, Colors.White);
            drawingSession.DrawCircle((float)tileSize.Width - 60, (float)tileSize.Height / 2f, 40, Colors.White);
            drawingSession.FillCircle((float)tileSize.Width - 60, (float)tileSize.Height / 2f, 40, Colors.White);
            DisplayStatusWithMemoryWarning("5. Cirlces drawn for tile with id: {0} and counter {1}", tileId, counter);

            var textFromatter = new Microsoft.Graphics.Canvas.Text.CanvasTextFormat();
            textFromatter.FontFamily = "Segoe UI Bold";
            textFromatter.FontSize = 48;
            textFromatter.HorizontalAlignment = Microsoft.Graphics.Canvas.Text.CanvasHorizontalAlignment.Center;
            textFromatter.VerticalAlignment = Microsoft.Graphics.Canvas.Text.CanvasVerticalAlignment.Center;

            drawingSession.DrawText(tileId.ToString(), 60, (float)tileSize.Height / 2f, Colors.Black, textFromatter);
            drawingSession.DrawText(counter.ToString(), (float)tileSize.Width - 60, (float)tileSize.Height / 2f, Colors.Black, textFromatter);
            drawingSession.DrawText(String.Format("MEM: {0} MB", (float)(MemoryManager.AppMemoryUsage/1024f/1024f)), (float)tileSize.Width/2f, (float)tileSize.Height - 50, Colors.White, textFromatter);

            DisplayStatusWithMemoryWarning("5. Text drawn for tile with id: {0} and counter {1}", tileId, counter);

            drawingSession.Dispose();

            var path = Path.Combine(ApplicationData.Current.LocalFolder.Path, string.Format("LiveTile_ID_{0}_CTR_{1}.png", tileId, counter));
            renderer.SaveAsync(path).AsTask().Wait();
            DisplayStatusWithMemoryWarning("6. Bitmap saved for tile with id: {0} and counter {1}", tileId, counter);

            //convert to bitmapo
            var memoStream = new InMemoryRandomAccessStream();
            renderer.SaveAsync(memoStream, CanvasBitmapFileFormat.Png).AsTask().Wait();
            drawingDevice.Dispose();
            textFromatter.Dispose();
            renderer.Dispose();
            drawingSession.Dispose();
            memoStream.Dispose();
            DisplayStatusWithMemoryWarning("7. Memory Cleanup for tile with id: {0} and counter {1}", tileId, counter);

            return "ms-appdata:///Local/" + string.Format("LiveTile_ID_{0}_CTR_{1}.png", tileId, counter);
        }

        private void DisplayStatusWithMemoryWarning(string format, params object[] paraeters)
        {
            Debug.WriteLine(format, paraeters);
            MemoryInfoNotifier.DisplayCurrentMemoryStatus();
        }
    }
}
