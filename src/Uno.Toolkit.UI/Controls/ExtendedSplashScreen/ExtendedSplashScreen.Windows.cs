#if WINDOWS || WINDOWS_UWP || NET472_OR_GREATER
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using Uno.Extensions;
using Windows.ApplicationModel.Activation;
using Windows.Graphics.Display;
using System.Reflection;
using System.IO;
using Windows.Storage;
#if IS_WINUI
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.UI;
#else
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
#endif

namespace Uno.Toolkit.UI
{
	public partial class ExtendedSplashScreen
	{
		private async Task<FrameworkElement?> GetNativeSplashScreen(SplashScreen? splashScreen)
		{
			var splashScreenImage = new Image();
			var splashScreenBackground = new SolidColorBrush();
			var scaleFactor =
#if WINDOWS_UWP
				DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
#else
				1.0; // WinUI will throw exception calling GetForCurrentView
#endif

			var splashScreenElement = new Canvas
			{
				Background = splashScreenBackground,
				Children = { splashScreenImage },
			};

			try
			{
				var manifestFilename = "AppxManifest.xml";

				var storageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///{manifestFilename}"));

				var doc = XDocument.Load(storageFile.Path, LoadOptions.None);
				var xnamespace = XNamespace.Get("http://schemas.microsoft.com/appx/manifest/uap/windows10");

				var visualElementsNode = doc.Descendants(xnamespace + "VisualElements").First();
				var splashScreenNode = visualElementsNode.Descendants(xnamespace + "SplashScreen").First();
				var splashScreenImagePath = splashScreenNode.Attribute("Image").Value;
				var splashScreenBackgroundColor = splashScreenNode.Attribute("BackgroundColor")?.Value;

				var bmp = new BitmapImage(new Uri("ms-appx:///" + splashScreenImagePath));
				splashScreenImage.Source = bmp;
				splashScreenBackground.Color = splashScreenBackgroundColor != null
					? (Color)XamlBindingHelper.ConvertValue(typeof(Color), splashScreenBackgroundColor)
					: Colors.White;

#if WINDOWS
				splashScreenElement.Loaded += (s, e) => scaleFactor = splashScreenElement.XamlRoot.RasterizationScale;
#endif

				void PositionImage()
				{
					try
					{
						if(Window is null)
						{
							return;
						}

						var imageWidth = (splashScreen?.ImageLocation.Width ?? bmp.PixelWidth
		) / scaleFactor;
						var imageHeight= (splashScreen?.ImageLocation.Height ?? bmp.PixelHeight
		) / scaleFactor;

						var posLeft = splashScreen?.ImageLocation.Left ?? ((Window.Bounds.Width - imageWidth)/2);
						var posTop = splashScreen?.ImageLocation.Top ?? ((Window.Bounds.Height - imageHeight) / 2);

						splashScreenImage.SetValue(Canvas.LeftProperty, posLeft);
						splashScreenImage.SetValue(Canvas.TopProperty, posTop);
						
							splashScreenImage.Height = imageHeight;
							splashScreenImage.Width = imageWidth;
					}
					catch (Exception e)
					{
						typeof(ExtendedSplashScreen).Log().LogError(0, e, "Error while adjusting image position and size");
					}
				}

				if (Window is not null)
				{
					Window.SizeChanged += (s, e) => PositionImage();
				}

				bmp.ImageOpened+=(s,e) => PositionImage();

				PositionImage();
			}
			catch (Exception e)
			{
				typeof(ExtendedSplashScreen).Log().LogError(0, e, "Error while getting native splash screen.");
			}

			return splashScreenElement;
		}
	}
}
#endif
