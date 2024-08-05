#if WINDOWS || WINDOWS_UWP || __SKIA_OR_WASM__
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.Extensions.Logging;
using Uno.Extensions;
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
		private const string PackageAppxManifestFileName = "package.appxmanifest";
		private const string AppxManifestFilename = "AppxManifest.xml";
		private const string WasmAppManifestFilename = "appmanifest.js";

		public bool SplashIsEnabled =>
#if WINDOWS_UWP || WINDOWS
			Platforms.HasFlag(SplashScreenPlatform.Windows);
#else
			Platforms.HasFlag(IsBrowser ? SplashScreenPlatform.WebAssembly : SplashScreenPlatform.Skia);
#endif

		private static bool IsBrowser { get; } =
			// Origin of the value : https://github.com/mono/mono/blob/a65055dbdf280004c56036a5d6dde6bec9e42436/mcs/class/corlib/System.Runtime.InteropServices.RuntimeInformation/RuntimeInformation.cs#L115
			RuntimeInformation.IsOSPlatform(OSPlatform.Create("WEBASSEMBLY")) || // Legacy Value (Bootstrapper 1.2.0-dev.29 or earlier).
			RuntimeInformation.IsOSPlatform(OSPlatform.Create("BROWSER"));

		internal static async Task<FrameworkElement?> GetNativeSplashScreen()
		{
			var (source, background) = IsBrowser
				? await LoadSplashScreenFromWasmManifest()
				: await LoadSplashScreenFromPackageManifest();

			// Position of image aligns with WASM Bootstrapper style for splash image
			// see https://github.com/unoplatform/Uno.Wasm.Bootstrap/blob/7d82af66c7dc587f6d1f6b6382860051fc2d92a0/src/Uno.Wasm.Bootstrap/WasmCSS/uno-bootstrap.css#L21
			var image = new Image
			{
				Source = source,
				MaxHeight = 300,
				MaxWidth = 620,
				Stretch = Stretch.Uniform,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center
			};
			Grid.SetColumn(image, 1);
			var wrapper = new Grid
			{
				Background = new SolidColorBrush { Color = background },
				ColumnDefinitions = {
					new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star)},
					new ColumnDefinition { Width = new GridLength(18, GridUnitType.Star)},
					new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star)}
				},
				Children = { image },
			};

			return wrapper;
		}

		private static async Task<(ImageSource? Source, Color Background)> LoadSplashScreenFromWasmManifest()
		{
			var result = (Source: default(ImageSource), Background: Colors.Transparent);

			try
			{
				var js = await LoadWasmManifestFile(Assembly.GetEntryAssembly(), WasmAppManifestFilename);
				if (string.IsNullOrWhiteSpace(js))
				{
					return result;
				}

				// note: naive js parsing with string manipulation
				// this may just fail, if we start to use nested object or escaped string containing '}'...
				var startIdx = js!.IndexOf('{') + 1;
				var endIdx = js.LastIndexOf('}') - 1;
				var manifestProps = js.Substring(startIdx, endIdx - startIdx); // Trim "var UnoAppManifest = " from the start of the file so we're left with just the inner of JSON
#if !WINDOWS_UWP
				var manifest = manifestProps.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
					.Select(x => x.Split(':', 2, StringSplitOptions.TrimEntries))
					.ToDictionarySafe(x => x[0], x => x[1].Trim('\"'));
#else
				var manifest = manifestProps.Trim().Split(',', StringSplitOptions.RemoveEmptyEntries)
					.Select(x => x.Split(':', 2))
					.ToDictionary(x => x[0].Trim(), x => x[1].Trim().Trim('\"'));
#endif

				if (manifest.TryGetValue("splashScreenImage", out var image))
				{
					result.Source = new BitmapImage(new Uri("ms-appx:///" + image));
				}
				result.Background = manifest.TryGetValue("splashScreenColor", out var color)
					? TryParseColor(color) ?? Colors.Transparent
					: Colors.White;

				return result;
			}

			catch (Exception e)
			{
				typeof(ExtendedSplashScreen).Log().LogError(0, e, "Error while getting native splash screen.");
			}

			return result;
		}

		private static async Task<(ImageSource? Source, Color Background)> LoadSplashScreenFromPackageManifest()
		{
			var result = (Source: default(ImageSource), Background: Colors.Transparent);

			try
			{
				var manifest = await LoadAppManifest(Assembly.GetEntryAssembly(), AppxManifestFilename, PackageAppxManifestFileName);
				if (manifest is null)
				{
					return result;
				}

				// https://learn.microsoft.com/en-us/uwp/schemas/appxpackage/uapmanifestschema/element-uap-visualelements
				var resolver = new XmlNamespaceManager(new NameTable());
				resolver.AddNamespace("win", "http://schemas.microsoft.com/appx/manifest/foundation/windows10"); // cant use "" as default in xpath-select
				resolver.AddNamespace("uap", "http://schemas.microsoft.com/appx/manifest/uap/windows10");
				if (manifest.XPathSelectElement(@"//uap:VisualElements/uap:SplashScreen", resolver) is { } splash)
				{
					if (splash.Attribute("Image") is { } image)
					{
						result.Source = new BitmapImage(new Uri("ms-appx:///" + image.Value));
					}
					result.Background = splash.Attribute("BackgroundColor") is { } color
						? TryParseColor(color.Value) ?? Colors.Transparent
						: Colors.White;
				}
			}
			catch (Exception e)
			{
				typeof(ExtendedSplashScreen).Log().LogError(0, e, "Error while getting native splash screen.");
			}

			return result;
		}

		private static async Task<string?> LoadWasmManifestFile(Assembly? entry, string manifestFile)
		{
			try
			{
				var storageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///{manifestFile}"));
				return await FileIO.ReadTextAsync(storageFile);
			}
			catch
			{
				var res = entry?.GetManifestResourceNames()?.FirstOrDefault(x => x.ToLower().Contains(manifestFile, StringComparison.InvariantCultureIgnoreCase));
				if (string.IsNullOrWhiteSpace(res))
				{
					return null;
				}

				var packageStream = entry?.GetManifestResourceStream(res);
				if (packageStream is not null)
				{
					using var streamReader = new StreamReader(packageStream);
					return await streamReader.ReadToEndAsync();
				}
			}

			return null;
		}

		private static async Task<XDocument?> LoadAppManifest(Assembly? entry, params string[] manifestFiles)
		{
			foreach (var file in manifestFiles)
			{
				if ((await LoadManifestFromPackageFile(entry, file) ?? LoadManifestFromEmbeddedFile(entry, file)) is { } doc)
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
#pragma warning disable SYSLIB0044 // Type or member is obsolete
			var codebase = entry?.GetName().CodeBase;
#pragma warning restore SYSLIB0044 // Type or member is obsolete
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

		private static Color? TryParseColor(string value)
		{
			try
			{
				return (Color)XamlBindingHelper.ConvertValue(typeof(Color), value);
			}
			catch (Exception colorError)
			{
				typeof(ExtendedSplashScreen).Log().LogError(0, colorError, $"Error while converting color: {value}.");
				return null;
			}
		}
	}
}
#endif
