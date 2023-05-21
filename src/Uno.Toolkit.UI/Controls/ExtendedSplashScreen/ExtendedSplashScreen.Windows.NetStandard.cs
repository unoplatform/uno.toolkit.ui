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
		private const string PackageAppxManifestFileName = "package.appxmanifest";
		private const string AppxManifestFilename = "AppxManifest.xml";
		private const string WasmAppManifestFilename = "appmanifest.js";

		public bool SplashIsEnabled =>
#if WINDOWS_UWP || WINDOWS
				(Platforms & SplashScreenPlatform.Windows) != 0;
#else
				(Platforms &
						(IsBrowser ? SplashScreenPlatform.WebAssembly : SplashScreenPlatform.Skia))
						!= 0;
#endif

		private static bool IsBrowser { get; } =
					// Origin of the value : https://github.com/mono/mono/blob/a65055dbdf280004c56036a5d6dde6bec9e42436/mcs/class/corlib/System.Runtime.InteropServices.RuntimeInformation/RuntimeInformation.cs#L115
					RuntimeInformation.IsOSPlatform(OSPlatform.Create("WEBASSEMBLY")) // Legacy Value (Bootstrapper 1.2.0-dev.29 or earlier).
					|| RuntimeInformation.IsOSPlatform(OSPlatform.Create("BROWSER"));

		private static async Task<FrameworkElement?> GetNativeSplashScreen(SplashScreen? splashScreen)
		{
			// Position of image aligns with WASM Bootstrapper style for splash image
			// see https://github.com/unoplatform/Uno.Wasm.Bootstrap/blob/7d82af66c7dc587f6d1f6b6382860051fc2d92a0/src/Uno.Wasm.Bootstrap/WasmCSS/uno-bootstrap.css#L21
			var splashScreenImage = new Image
			{
				MaxHeight = 300,
				MaxWidth = 620,
				Stretch = Stretch.Uniform,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center
			};
			var splashScreenBackground = new SolidColorBrush();

			Grid.SetColumn(splashScreenImage, 1);
			var splashScreenElement = new Grid
			{
				Background = splashScreenBackground,
				ColumnDefinitions = {
					new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star)},
					new ColumnDefinition { Width = new GridLength(18, GridUnitType.Star)},
					new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star)}
				},
				Children = { splashScreenImage },
			};

			BitmapImage? bmp;
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

			return splashScreenElement;
		}

		private static async Task<BitmapImage?> LoadSplashScreenFromWasmManifest(Image splashScreenImage, SolidColorBrush splashScreenBackground)
		{
			try
			{
				string? manifestString = default;
				try
				{
					var storageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///{WasmAppManifestFilename}"));
					manifestString = await FileIO.ReadTextAsync(storageFile);
				}
				catch
				{
					var entry = Assembly.GetEntryAssembly();
					var res = entry?.GetManifestResourceNames()?.FirstOrDefault(x => x.ToLower().Contains(WasmAppManifestFilename));
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
				var entry = Assembly.GetEntryAssembly();
				var doc = await LoadAppManifest(entry, AppxManifestFilename, PackageAppxManifestFileName);

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

		private static async Task<XDocument?> LoadAppManifest(Assembly? entry, params string[] manifestFiles)
		{
			foreach (var file in manifestFiles)
			{
				var doc = await LoadManifestFromPackageFile(entry, file);
				if (doc is not null)
				{
					return doc;
				}
				doc = LoadManifestFromEmbeddedFile(entry, file);
				if (doc is not null)
				{
					return doc;
				}
			}
			return default;
		}

		private static async Task<XDocument?> LoadManifestFromPackageFile(Assembly? entry, string manifestFile)
		{
			try
			{
				typeof(ExtendedSplashScreen).Log().LogTrace($"Attempting to load manifest `{manifestFile}` from file packaged with application.");
				var filePath = await ApplicationPathFromFileName(entry, manifestFile);
				if (!string.IsNullOrWhiteSpace(filePath))
				{
					typeof(ExtendedSplashScreen).Log().LogTrace($"Full path to {manifestFile} is {filePath}.");
					return XDocument.Load(filePath, LoadOptions.None);
				}
			}
			catch (Exception docError)
			{
				typeof(ExtendedSplashScreen).Log().LogDebug($"The manifest `{manifestFile}` exists as a packaged file but could not be loaded to XDocument: {docError}.");
			}

			return default;
		}

		private static XDocument? LoadManifestFromEmbeddedFile(Assembly? entry, string manifestFile)
		{
			typeof(ExtendedSplashScreen).Log().LogTrace($"Attempting to load manifest from embedded resource `{manifestFile}` within the {entry?.GetName().Name} assembly.");
			// Check EndsWith because the file may have a prefix based on the project and folder the file was sourced from
			var res = entry?.GetManifestResourceNames()?.FirstOrDefault(x => x.ToLower().EndsWith(manifestFile));
			if (!string.IsNullOrWhiteSpace(res))
			{
				var packageStream = entry!.GetManifestResourceStream(res!);

				if (packageStream is not null)
				{
					try
					{
						return XDocument.Load(packageStream, LoadOptions.None);
					}
					catch (Exception docError)
					{
						typeof(ExtendedSplashScreen).Log().LogTrace($"The manifest `{manifestFile}` exists as a packaged file but could not be loaded to XDocument: {docError}.");
					}
				}
			}

			return default;
		}

		private static async Task<string> ApplicationPathFromFileName(Assembly? entry, string fileName)
		{
			// Note: We attempt to resolve based on the assembly first because it doesn't throw an exception if the file doesn't exist
			// whereas StorageFile.GetFileFromApplicationUriAsync will throw an exception if the file doesn't exist.
			var filePath = string.Empty;
			var codebase = entry?.GetName().CodeBase;
			if (codebase is not null)
			{
				var manifestPath = Path.Combine(Path.GetDirectoryName(codebase) ?? string.Empty, fileName);
				if (File.Exists(manifestPath))
				{
					filePath = manifestPath;
				}
			}

			if (string.IsNullOrWhiteSpace(filePath))
			{
				try
				{
					var storageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///{fileName}"));
					filePath = storageFile.Path;
				}
				catch
				{
					typeof(ExtendedSplashScreen).Log().LogTrace($"Unable to load application StorageFile for {fileName}.");
				}
			}

			return filePath;
		}
	}
}
#endif
