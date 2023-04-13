#if WINDOWS || WINDOWS_UWP || NET472_OR_GREATER || NETSTANDARD2_0
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
using System.Runtime.InteropServices;
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
		private static bool IsBrowser { get; } =
				// Origin of the value : https://github.com/mono/mono/blob/a65055dbdf280004c56036a5d6dde6bec9e42436/mcs/class/corlib/System.Runtime.InteropServices.RuntimeInformation/RuntimeInformation.cs#L115
				RuntimeInformation.IsOSPlatform(OSPlatform.Create("WEBASSEMBLY")) // Legacy Value (Bootstrapper 1.2.0-dev.29 or earlier).
				|| RuntimeInformation.IsOSPlatform(OSPlatform.Create("BROWSER"));

		private async Task<FrameworkElement?> GetNativeSplashScreen(SplashScreen? splashScreen)
		{
#if WINDOWS_UWP || WINDOWS
			if((Platforms & SplashScreenPlatform.Windows) == 0)
			{
				return null;
			}
#else
			if (IsBrowser)
			{
				if ((Platforms & SplashScreenPlatform.WebAssembly) == 0)
				{
					return null;
				}

			}
			else
			{
				if ((Platforms & SplashScreenPlatform.Skia) == 0)
				{
					return null;
				}
			}
#endif

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

			BitmapImage? bmp = default;
			if (!IsBrowser)
			{
				bmp = await LoadSplashScreenFromPackageManifest(splashScreenImage, splashScreenBackground);
			}
			else
			{
				bmp = await LoadSplashScreenFromWasmManifest(splashScreenImage, splashScreenBackground);
			}

			if (bmp is null)
			{
				return splashScreenElement;
			}

#if WINDOWS
				splashScreenElement.Loaded += (s, e) => scaleFactor = splashScreenElement.XamlRoot.RasterizationScale;
#endif

			// Only use image location (Left/Top) if there's a width/height specified
			var imageLocationLeft = splashScreen?.ImageLocation.Width > 0 ? splashScreen?.ImageLocation.Left : default;
			var imageLocationTop = splashScreen?.ImageLocation.Height > 0 ? splashScreen?.ImageLocation.Top : default;
			var imageLocationWidth = splashScreen?.ImageLocation.Width > 0 ? splashScreen?.ImageLocation.Width : default;
			var imageLocationHeight = splashScreen?.ImageLocation.Height > 0 ? splashScreen?.ImageLocation.Height : default;

			typeof(ExtendedSplashScreen).Log().LogTrace($"Image location ({imageLocationLeft},{imageLocationTop},{imageLocationWidth},{imageLocationHeight}");


			void PositionImage(Image imageElement)
			{
				try
				{
					if (Window is null || bmp is null)
					{
						return;
					}


					var calcImageWidth = (int)((imageLocationWidth ?? bmp.PixelWidth) / scaleFactor);
					var calcImageHeight = (int)((imageLocationHeight ?? bmp.PixelHeight) / scaleFactor);

					var imageWidth = calcImageWidth == 0 ? imageElement.ActualWidth : calcImageWidth;
					var imageHeight = calcImageHeight == 0 ? imageElement.ActualHeight : calcImageHeight;
					if (imageWidth == 0 || imageHeight == 0)
					{
						return;
					}

					if (IsBrowser)
					{
						typeof(ExtendedSplashScreen).Log().LogTrace("Fixing image size in WASM to 300x620");
						imageHeight = 300;
						imageWidth = 620;
					}

					var posLeft = imageLocationLeft ?? ((Window.Bounds.Width - imageWidth) / 2);
					var posTop = imageLocationTop ?? ((Window.Bounds.Height - imageHeight) / 2);

					splashScreenImage.SetValue(Canvas.LeftProperty, posLeft);
					splashScreenImage.SetValue(Canvas.TopProperty, posTop);
					splashScreenImage.HorizontalAlignment = HorizontalAlignment.Left;
					splashScreenImage.VerticalAlignment = VerticalAlignment.Top;

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
				Window.SizeChanged += (s, e) => PositionImage(splashScreenImage);
			}

			// This works on Windows and has bmp.PixelWidth/Height available
			bmp.ImageOpened += (s, e) => PositionImage(splashScreenImage);

			// This doesn't get raised on GTK
			//splashScreenImage.ImageOpened += (s, e) => PositionImage((s as Image)!);
			// This gets raised on GTK, but bmp.PixelWidth/Height is not available, nor is ActualHeight/Width on splashScreenImage
			//splashScreenImage.LayoutUpdated += (s, e) => PositionImage((s as Image)!);

			// This works on GTK and has bmp.PixelWidth/Height available (bmp.ImageOpened does not)
			splashScreenImage.SizeChanged += (s, e) => PositionImage((s as Image)!);
			PositionImage(splashScreenImage);

			return splashScreenElement;
		}

		private static async Task<BitmapImage?> LoadSplashScreenFromWasmManifest(Image splashScreenImage, SolidColorBrush splashScreenBackground)
		{
			try
			{
				var manifestFilename = "appmanifest.js";
				string? manifestString = default;
				try
				{
					var storageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///{manifestFilename}"));
					manifestString = await FileIO.ReadTextAsync(storageFile);
				}
				catch
				{
					var entry = Assembly.GetEntryAssembly();
					var res = entry?.GetManifestResourceNames()?.FirstOrDefault(x => x.ToLower().Contains(manifestFilename));
					if (string.IsNullOrWhiteSpace(res))
					{
						return null;
					}
					var packageStream = entry?.GetManifestResourceStream(res);
					if (packageStream is not null)
					{
						using var streamReader = new StreamReader(packageStream);
						manifestString = await streamReader.ReadToEndAsync();
					}
				}
				if (manifestString is null ||
					string.IsNullOrWhiteSpace(manifestString))
				{
					return null;
				}
				var startIdx = manifestString.IndexOf("{") + 1;
				var endIdx = manifestString.LastIndexOf("}") - 1;
				manifestString = manifestString.Substring(startIdx, endIdx - startIdx); // Trim "var UnoAppManifest = " from the start of the file so we're left with just the JSON
				var pairs = (from pair in manifestString.Trim().Split(',')
							 let bits = pair.Split(':')
							 where bits.Length == 2
							 select (Key: bits[0].Trim(), Value: bits[1].Trim().Trim('\"'))).ToArray();
				var splashScreenImagePath = pairs.FirstOrDefault(x => x.Key == "splashScreenImage").Value;
				var splashScreenBackgroundColor = pairs.FirstOrDefault(x => x.Key == "splashScreenColor").Value;

				var bmp = new BitmapImage(new Uri("ms-appx:///" + splashScreenImagePath));
				splashScreenImage.Source = bmp;
				try
				{
					splashScreenBackground.Color = splashScreenBackgroundColor != null
						? (Color)XamlBindingHelper.ConvertValue(typeof(Color), splashScreenBackgroundColor)
						: Colors.White;
				}
				catch (Exception colorError)
				{
					typeof(ExtendedSplashScreen).Log().LogError(0, colorError, $"Error while converting background color {splashScreenBackgroundColor}.");
				}

				return bmp;
			}

			catch (Exception e)
			{
				typeof(ExtendedSplashScreen).Log().LogError(0, e, "Error while getting native splash screen.");
				return null;
			}

		}

		private static async Task<BitmapImage?> LoadSplashScreenFromPackageManifest(Image splashScreenImage, SolidColorBrush splashScreenBackground)
		{
			try
			{
				var manifestFilename = "AppxManifest.xml";
				XDocument? doc = default;
				try
				{
					var storageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///{manifestFilename}"));
					doc = XDocument.Load(storageFile.Path, LoadOptions.None);
				}
				catch
				{
					var entry = Assembly.GetEntryAssembly();
					var packageStream = entry?.GetManifestResourceStream($"{entry?.GetName().Name}.Package.appxmanifest");
					if (packageStream is not null)
					{
						doc = XDocument.Load(packageStream, LoadOptions.None);
					}
				}
				if (doc is null)
				{
					return null;
				}
				var xnamespace = XNamespace.Get("http://schemas.microsoft.com/appx/manifest/uap/windows10");

				var visualElementsNode = doc.Descendants(xnamespace + "VisualElements").First();
				var splashScreenNode = visualElementsNode.Descendants(xnamespace + "SplashScreen").First();
				var splashScreenImagePath = splashScreenNode.Attribute("Image")!.Value;
				var splashScreenBackgroundColor = splashScreenNode.Attribute("BackgroundColor")?.Value;

				var bmp = new BitmapImage(new Uri("ms-appx:///" + splashScreenImagePath));
				splashScreenImage.Source = bmp;
				try
				{
					splashScreenBackground.Color = splashScreenBackgroundColor != null
						? (Color)XamlBindingHelper.ConvertValue(typeof(Color), splashScreenBackgroundColor)
						: Colors.White;
				}
				catch (Exception colorError)
				{
					typeof(ExtendedSplashScreen).Log().LogError(0, colorError, $"Error while converting background color {splashScreenBackgroundColor}.");
				}
				return bmp;
			}
			catch (Exception e)
			{
				typeof(ExtendedSplashScreen).Log().LogError(0, e, "Error while getting native splash screen.");
			}

			return null;
		}
	}
}
#endif
