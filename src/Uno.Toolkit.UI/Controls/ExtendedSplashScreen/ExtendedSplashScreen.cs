using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI;
using Uno.Extensions;

#if IS_WINUI
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
#endif

namespace Uno.Toolkit.UI;

/// <summary>
/// Displays a view that replicates the look and behavior of the native splash screen
/// </summary>
public partial class ExtendedSplashScreen : LoadingView
{
	// This file is generated and included by Resizetizer.
	private const string DefinitionFileName = "UnoSplash.def";

	protected static ExtendedSplashScreen? Instance { get; private set; }

	#region DependencyProperty: SplashScreenContent
	internal static DependencyProperty SplashScreenContentProperty { get; } = DependencyProperty.Register(
		nameof(SplashScreenContent),
		typeof(object),
		typeof(ExtendedSplashScreen),
		new PropertyMetadata(default(object)));

	/// <summary>
	/// Gets or sets the native splash screen content to be displayed during loading/waiting.
	/// </summary>
	internal object SplashScreenContent
	{
		get => (object)GetValue(SplashScreenContentProperty);
		set => SetValue(SplashScreenContentProperty, value);
	}
	#endregion

#if !(__IOS__ || __MACOS__ && !HAS_UNO_WINUI)
	public Window? Window { get; set; }
#else
	public new Window? Window { get; set; } // hides UIView.Window and NSView.Window
#endif

	public ExtendedSplashScreen()
	{
		Instance = this;
	}

	protected override void OnApplyTemplate()
	{
		base.OnApplyTemplate();

		SplashScreenContent = new Border();

		_ = LoadSplashScreen();
	}

	private async Task LoadSplashScreen()
	{
		var content = await GetSplashScreen();

		if (content is { })
		{
			SplashScreenContent = content;
		}
	}

	internal static async Task<FrameworkElement?> GetSplashScreen()
	{
		var text = await LoadUnoSplashDefinitionText();
		var def = UnoSplashDef.Parse(text);

		if (def is { })
		{
			return BuildUI(def);
		}

		return null;
	}

	private static readonly int[] SkiaLayoutingColumns = [1, 18, 1];
	internal static FrameworkElement BuildUI(UnoSplashDef def)
	{
		var source = Regex.Replace($"ms-appx:///{Regex.Replace(def.File, @"^Assets\\Splash\\", "")}", "\\.svg$", ".png");
#if NETCOREAPP
		if (OperatingSystem.IsBrowser())
		{
			// wasm requires the full name with scale qualifier
			// which is hardcoded to be scale-200 (see: [resizetizer]GenerateWasmSplashAssets.ProcessAppManifestFile)
			source = Regex.Replace(source, "(?=\\.png$)", ".scale-200");
		}
		var layout = (LogoLayout)((OperatingSystem.IsAndroid(), OperatingSystem.IsIOS()) switch
		{
			(true, _) => new(true, 192, 192, default, default),
			(_, true) => new(true, 300, 300, default, default),
			_ => new(false, default, default, 300, 620),
		});
#else
		var layout = new LogoLayout(false, default, default, 300, 620);
#endif
		var background = TryParseColor(def.Color) ?? Colors.Transparent;

		var image = new Image
		{
			Source = new BitmapImage(new Uri(source)),
			Width = layout.Width ?? double.NaN,
			Height = layout.Height ?? double.NaN,
			MaxWidth = layout.MaxWidth ?? double.PositiveInfinity,
			MaxHeight = layout.MaxHeight ?? double.PositiveInfinity,
			Stretch = Stretch.Uniform,
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
		};
		var wrapper = new Grid
		{
			Background = new SolidColorBrush(background),
			Children = { image },
		};

		if (!layout.IsMobile)
		{
			Grid.SetColumn(image, 1);
			wrapper.ColumnDefinitions.AddRange(SkiaLayoutingColumns.Select(x => new ColumnDefinition { Width = new(1, GridUnitType.Star) }));
		}

		return wrapper;
	}

	private static async Task<string?> LoadUnoSplashDefinitionText()
	{
		var assembly = // workaround for https://github.com/unoplatform/uno/issues/21195
			Application.Current?.GetType().Assembly;
			//Assembly.GetEntryAssembly();

		using var stream = assembly?.GetManifestResourceStream(DefinitionFileName);
		if (stream is { })
		{
			using var reader = new StreamReader(stream);
			return await reader.ReadLineAsync();
		}

		return null;
	}

	private static Color? TryParseColor(string? value)
	{
		try
		{
			if (value is null) return null;

			return (Color)XamlBindingHelper.ConvertValue(typeof(Color), value);
		}
		catch (Exception)
		{
			return null;
		}
	}

	private record LogoLayout(bool IsMobile, double? Width, double? Height, double? MaxWidth, double? MaxHeight);

	public record UnoSplashDef(string File, string? Link, string? BaseSize, string? Resize, string? TintColor, string? Color, string? ForegroundScale)
	{
		public static UnoSplashDef? Parse(string? value)
		{
			if (value is null) return null;

			var map = value
#if NETCOREAPP
				.Split(';' , StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
#else
				.Split(';', StringSplitOptions.RemoveEmptyEntries).Where(x => x?.Trim().Length > 0)
#endif
				.Select(pair => pair.Split('=', 2))
				.Where(pair => !string.IsNullOrEmpty(pair[0]))
				.ToDictionary(x => x[0], x => x[1]);

			if (!map.TryGetValue(nameof(File), out var file)) return null;

			return new UnoSplashDef(
				file,
				map.GetValueOrDefault(nameof(Link)),
				map.GetValueOrDefault(nameof(BaseSize)),
				map.GetValueOrDefault(nameof(Resize)),
				map.GetValueOrDefault(nameof(TintColor)),
				map.GetValueOrDefault(nameof(Color)),
				map.GetValueOrDefault(nameof(ForegroundScale))
			);
		}
	}
}
