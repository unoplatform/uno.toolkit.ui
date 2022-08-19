using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content.PM;
using Android.Views;
using Java.Interop;
using System.IO;
using System;
using System.Threading;

namespace Uno.Toolkit.Samples.Droid
{
	[Activity(
			MainLauncher = true,
			ConfigurationChanges = global::Uno.UI.ActivityHelper.AllConfigChanges,
			WindowSoftInputMode = SoftInput.AdjustPan | SoftInput.StateHidden
		)]
	public class MainActivity : Windows.UI.Xaml.ApplicationActivity
	{
#if USE_UITESTS
		private HandlerThread _pixelCopyHandlerThread;

		[Export("NavBackFromNestedPage")]
		public void NavBackFromNestedPage() => App.NavBackFromNestedPage();

		[Export("ForceNavigation")]
		public void ForceNavigation(string sampleName) => App.ForceNavigation(sampleName);

		[Export("ExitNestedSample")]
		public void ExitNestedSampleBackdoor() => App.ExitNestedSample();

		[Export("NavigateToNestedSample")]
		public void NavigateToNestedSample(string pageName) => App.NavigateToNestedSample(pageName);

		/// <summary>
		/// Returns a base64 encoded PNG file
		/// </summary>
		[Export("GetScreenshot")]
		public string GetScreenshot(string displayId)
		{
			var rootView = Windows.UI.Xaml.Window.Current.Content as View;

			var bitmap = Android.Graphics.Bitmap.CreateBitmap(rootView.Width, rootView.Height, Android.Graphics.Bitmap.Config.Argb8888);
			var locationOfViewInWindow = new int[2];
			rootView.GetLocationInWindow(locationOfViewInWindow);

			var xCoordinate = locationOfViewInWindow[0];
			var yCoordinate = locationOfViewInWindow[1];

			var scope = new Android.Graphics.Rect(
				xCoordinate,
				yCoordinate,
				xCoordinate + rootView.Width,
				yCoordinate + rootView.Height
			);

			if (_pixelCopyHandlerThread == null)
			{
				_pixelCopyHandlerThread = new Android.OS.HandlerThread("ScreenshotHelper");
				_pixelCopyHandlerThread.Start();
			}

			var listener = new PixelCopyListener();

			// PixelCopy.Request returns the actual rendering of the screen location
			// for the app, incliing OpenGL content.
			PixelCopy.Request(Window, scope, bitmap, listener, new Android.OS.Handler(_pixelCopyHandlerThread.Looper));

			listener.WaitOne();

			using var memoryStream = new MemoryStream();
			bitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Png, 100, memoryStream);

			return Convert.ToBase64String(memoryStream.ToArray());
		}

		class PixelCopyListener : Java.Lang.Object, PixelCopy.IOnPixelCopyFinishedListener
		{
			private ManualResetEvent _event = new ManualResetEvent(false);

			public void WaitOne()
			{
				_event.WaitOne();
			}

			public void OnPixelCopyFinished(int copyResult)
			{
				_event.Set();
			}
		}
#endif
	}
}

