using System;

#if IS_WINUI
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml;
#endif

/// <summary>
/// Material Colors resources
/// </summary>
namespace Uno.Toolkit.Material
{
	public sealed partial class MaterialToolkitColors : ResourceDictionary
	{
		private static string OverrideSource;
		private const string PackageName =
#if IS_WINUI
			"Uno.WinUI.Toolkit.Material";
#else
			"Uno.UI.Toolkit.Material";
#endif

		public string ColorPaletteOverrideSource
		{
			get => (string)GetValue(ColorPaletteOverrideSourceProperty);
			set => SetValue(ColorPaletteOverrideSourceProperty, value);
		}

		public static DependencyProperty ColorPaletteOverrideSourceProperty { get; } =
			DependencyProperty.Register(
				nameof(ColorPaletteOverrideSource),
				typeof(string),
				typeof(MaterialToolkitColors),
				new PropertyMetadata(null, OnColorPaletteOverrideSourceChanged));

		private static void OnColorPaletteOverrideSourceChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
		{
			OverrideSource = args.NewValue as string;
		}

		public MaterialToolkitColors()
		{
			MergedDictionaries.Add(new ResourceDictionary { Source = new Uri($"ms-appx:///{PackageName}/Styles/Application/ColorPalette.xaml") });
			if (!string.IsNullOrWhiteSpace(OverrideSource))
			{
				MergedDictionaries.Add(new ResourceDictionary { Source = new Uri(OverrideSource) });
			}

			this.InitializeComponent();
		}
	}
}
