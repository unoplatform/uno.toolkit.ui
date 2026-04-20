using System;
using System.Collections.Generic;
using System.Text;
using Uno.Material;
using Uno.Themes;
using Windows.UI;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
#endif

namespace Uno.Toolkit.UI.Material
{
	/// <summary>
	/// Material (Material Design 3) styles for the controls in the Uno.Toolkit.UI library.
	/// </summary>
	public class MaterialToolkitTheme : ResourceDictionary
	{
		private const string ToolkitPackageName = "Uno.Toolkit.WinUI";
		private const string ToolkitMaterialPackageName = "Uno.Toolkit.WinUI.Material";

		private bool _isOverridingColor;
		private bool _isOverridingFont;

		#region DependencyProperty: FontOverrideSource
		/// <summary>
		/// (Optional) Gets or sets a Uniform Resource Identifier (<see cref="Uri"/>) that provides the source location
		/// of a <see cref="ResourceDictionary"/> containing overrides for the default Uno.Material <see cref="FontFamily"/> resources
		/// </summary>
		public string FontOverrideSource
		{
			get => (string)GetValue(FontOverrideSourceProperty);
			set => SetValue(FontOverrideSourceProperty, value);
		}

		public static DependencyProperty FontOverrideSourceProperty { get; } =
			DependencyProperty.Register(
				nameof(FontOverrideSource),
				typeof(string),
				typeof(MaterialToolkitTheme),
				new PropertyMetadata(null, OnFontOverrideSourceChanged));
		#endregion

		#region DependencyProperty: ColorOverrideSource
		/// <summary>
		/// (Optional) Gets or sets a Uniform Resource Identifier (<see cref="Uri"/>) that provides the source location
		/// of a <see cref="ResourceDictionary"/> containing overrides for the default Uno.Material <see cref="Color"/> resources
		/// </summary>
		/// <remarks>The overrides set here should be re-defining the <see cref="Color"/> resources used by Uno.Material, not the <see cref="SolidColorBrush"/> resources</remarks>
		public string ColorOverrideSource
		{
			get => (string)GetValue(ColorOverrideSourceProperty);
			set => SetValue(ColorOverrideSourceProperty, value);
		}

		public static DependencyProperty ColorOverrideSourceProperty { get; } =
			DependencyProperty.Register(
				nameof(ColorOverrideSource),
				typeof(string),
				typeof(MaterialToolkitTheme),
				new PropertyMetadata(null, OnColorOverrideSourceChanged));
		#endregion

		#region DependencyProperty: FontOverrideDictionary
		/// <summary>
		/// (Optional) Gets or sets a <see cref="ResourceDictionary"/> containing overrides for the default Uno.Material <see cref="FontFamily"/> resources
		/// </summary>
		public ResourceDictionary? FontOverrideDictionary
		{
			get => (ResourceDictionary?)GetValue(FontOverrideDictionaryProperty);
			set => SetValue(FontOverrideDictionaryProperty, value);
		}

		public static DependencyProperty FontOverrideDictionaryProperty { get; } =
			DependencyProperty.Register(
				nameof(FontOverrideDictionary),
				typeof(ResourceDictionary),
				typeof(MaterialToolkitTheme),
				new PropertyMetadata(null, OnFontOverrideChanged));
		#endregion

		#region DependencyProperty: ColorOverrideDictionary
		/// <summary>
		/// (Optional) Gets or sets a <see cref="ResourceDictionary"/> containing overrides for the default Uno.Material <see cref="Color"/> resources
		/// </summary>
		/// <remarks>The overrides set here should be re-defining the <see cref="Color"/> resources used by Uno.Material, not the <see cref="SolidColorBrush"/> resources</remarks>
		public ResourceDictionary? ColorOverrideDictionary
		{
			get => (ResourceDictionary?)GetValue(ColorOverrideDictionaryProperty);
			set => SetValue(ColorOverrideDictionaryProperty, value);
		}

		public static DependencyProperty ColorOverrideDictionaryProperty { get; } =
			DependencyProperty.Register(
				nameof(ColorOverrideDictionary),
				typeof(ResourceDictionary),
				typeof(MaterialToolkitTheme),
				new PropertyMetadata(null, OnColorOverrideChanged));
		#endregion

		#region DependencyProperty: Colors
		/// <summary>
		/// (Optional) Gets or sets a <see cref="ThemeColors"/> object that groups all color-related
		/// configuration including seed colors and overrides.
		/// This is the recommended way to configure theme colors, including seed-based palette generation.
		/// </summary>
		public ThemeColors? Colors
		{
			get => (ThemeColors?)GetValue(ColorsProperty);
			set => SetValue(ColorsProperty, value);
		}

		public static DependencyProperty ColorsProperty { get; } =
			DependencyProperty.Register(
				nameof(Colors),
				typeof(ThemeColors),
				typeof(MaterialToolkitTheme),
				new PropertyMetadata(null, OnColorsChanged));

		private static void OnColorsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is MaterialToolkitTheme toolkitTheme)
			{
				toolkitTheme.UpdateSource();
			}
		}
		#endregion

		private static void OnFontOverrideSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is MaterialToolkitTheme theme && e.NewValue is string sourceUri)
			{
				theme.FontOverrideDictionary = new ResourceDictionary() { Source = new Uri(sourceUri) };
			}
		}

		private static void OnColorOverrideSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is MaterialToolkitTheme theme && e.NewValue is string sourceUri)
			{
				theme.ColorOverrideDictionary = new ResourceDictionary() { Source = new Uri(sourceUri) };
			}
		}

		private static void OnFontOverrideChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is MaterialToolkitTheme { _isOverridingFont: false } toolkitTheme)
			{
				toolkitTheme.UpdateSource();
			}
		}

		private static void OnColorOverrideChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is MaterialToolkitTheme { _isOverridingColor: false } toolkitTheme)
			{
				toolkitTheme.UpdateSource();
			}
		}

		private void SetColorOverrideSilently(ResourceDictionary? colorOverride)
		{
			try
			{
				_isOverridingColor = true;
				ColorOverrideDictionary = colorOverride;
			}
			finally
			{
				_isOverridingColor = false;
			}
		}

		private void SetFontOverrideSilently(ResourceDictionary? fontOverride)
		{
			try
			{
				_isOverridingFont = true;
				FontOverrideDictionary = fontOverride;
			}
			finally
			{
				_isOverridingFont = false;
			}
		}

		public MaterialToolkitTheme() : this(colorOverride: null, fontOverride: null)
		{
			
		}

		public MaterialToolkitTheme(ResourceDictionary? colorOverride = null, ResourceDictionary? fontOverride = null)
		{
			SetColorOverrideSilently(colorOverride);
			SetFontOverrideSilently(fontOverride);

			UpdateSource();
		}

		private void UpdateSource()
		{
#if !HAS_UNO
			Source = null;
#endif
			ThemeDictionaries.Clear();
			MergedDictionaries.Clear();
			this.Clear();

			var materialTheme = new MaterialTheme(ColorOverrideDictionary, FontOverrideDictionary);
			if (Colors is { } colors)
			{
				materialTheme.Colors = colors;
			}

			MergedDictionaries.Add(materialTheme);
			MergedDictionaries.Add(new ResourceDictionary { Source = new Uri($"ms-appx:///{ToolkitPackageName}/Generated/mergedpages.xaml") });
			MergedDictionaries.Add(new ResourceDictionary { Source = new Uri($"ms-appx:///{ToolkitMaterialPackageName}/Generated/mergedpages.v2.xaml") });
		}
	}
}
