using System;

#if IS_WINUI
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml;
#endif

/// <summary>
/// Material Colors resources
/// </summary>
namespace Uno.Toolkit.UI.Material
{
	public sealed partial class MaterialToolkitColors : ResourceDictionary
	{
		private static string? ColorPaletteOverrideSource;

		private const string PackageName =
#if IS_WINUI
			"Uno.Toolkit.WinUI.Material";
#else
			"Uno.Toolkit.UI.Material";
#endif

		public string OverrideSource
		{
			get => (string)GetValue(OverrideSourceProperty);
			set => SetValue(OverrideSourceProperty, value);
		}

		public static DependencyProperty OverrideSourceProperty { get; } =
			DependencyProperty.Register(
				nameof(OverrideSource),
				typeof(string),
				typeof(MaterialToolkitColors),
				new PropertyMetadata(null, OnColorPaletteOverrideSourceChanged));

		private static void OnColorPaletteOverrideSourceChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
		{
			ColorPaletteOverrideSource = args.NewValue as string;
		}

		public MaterialToolkitColors()
		{
			MergedDictionaries.Add(new ResourceDictionary { Source = new Uri($"ms-appx:///{PackageName}/Styles/Application/ColorPalette.xaml") });
			if (!string.IsNullOrWhiteSpace(ColorPaletteOverrideSource))
			{
				MergedDictionaries.Add(new ResourceDictionary { Source = new Uri(ColorPaletteOverrideSource) });
			}

			this.InitializeComponent();
		}
	}
}
