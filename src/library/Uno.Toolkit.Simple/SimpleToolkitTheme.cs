using System;
using Uno.Simple;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace Uno.Toolkit.UI.Simple
{
	/// <summary>
	/// Simple Design System styles for the controls in the Uno.Toolkit.UI library.
	/// </summary>
	public class SimpleToolkitTheme : ResourceDictionary
	{
		private const string ToolkitPackageName = "Uno.Toolkit.WinUI";
		private const string ToolkitSimplePackageName = "Uno.Toolkit.WinUI.Simple";

		private bool _isOverridingColor;
		private bool _isOverridingFont;

		#region DependencyProperty: FontOverrideSource
		/// <summary>
		/// (Optional) Gets or sets a Uniform Resource Identifier (<see cref="Uri"/>) that provides the source location
		/// of a <see cref="ResourceDictionary"/> containing overrides for the default Simple <see cref="FontFamily"/> resources
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
				typeof(SimpleToolkitTheme),
				new PropertyMetadata(null, OnFontOverrideSourceChanged));
		#endregion

		#region DependencyProperty: ColorOverrideSource
		public string ColorOverrideSource
		{
			get => (string)GetValue(ColorOverrideSourceProperty);
			set => SetValue(ColorOverrideSourceProperty, value);
		}

		public static DependencyProperty ColorOverrideSourceProperty { get; } =
			DependencyProperty.Register(
				nameof(ColorOverrideSource),
				typeof(string),
				typeof(SimpleToolkitTheme),
				new PropertyMetadata(null, OnColorOverrideSourceChanged));
		#endregion

		#region DependencyProperty: FontOverrideDictionary
		public ResourceDictionary? FontOverrideDictionary
		{
			get => (ResourceDictionary?)GetValue(FontOverrideDictionaryProperty);
			set => SetValue(FontOverrideDictionaryProperty, value);
		}

		public static DependencyProperty FontOverrideDictionaryProperty { get; } =
			DependencyProperty.Register(
				nameof(FontOverrideDictionary),
				typeof(ResourceDictionary),
				typeof(SimpleToolkitTheme),
				new PropertyMetadata(null, OnFontOverrideChanged));
		#endregion

		#region DependencyProperty: DefaultSize
		/// <summary>
		/// Gets or sets the default size variant for control styles.
		/// The default is <see cref="SimpleControlSize.Small"/>.
		/// The value is forwarded to the underlying <see cref="SimpleTheme"/>.
		/// </summary>
		public SimpleControlSize DefaultSize
		{
			get => (SimpleControlSize)GetValue(DefaultSizeProperty);
			set => SetValue(DefaultSizeProperty, value);
		}

		public static DependencyProperty DefaultSizeProperty { get; } =
			DependencyProperty.Register(
				nameof(DefaultSize),
				typeof(SimpleControlSize),
				typeof(SimpleToolkitTheme),
				new PropertyMetadata(SimpleControlSize.Small, OnDefaultSizeChanged));

		private static void OnDefaultSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is SimpleToolkitTheme toolkitTheme)
			{
				toolkitTheme.UpdateSource();
			}
		}
		#endregion

		#region DependencyProperty: ColorOverrideDictionary
		public ResourceDictionary? ColorOverrideDictionary
		{
			get => (ResourceDictionary?)GetValue(ColorOverrideDictionaryProperty);
			set => SetValue(ColorOverrideDictionaryProperty, value);
		}

		public static DependencyProperty ColorOverrideDictionaryProperty { get; } =
			DependencyProperty.Register(
				nameof(ColorOverrideDictionary),
				typeof(ResourceDictionary),
				typeof(SimpleToolkitTheme),
				new PropertyMetadata(null, OnColorOverrideChanged));
		#endregion

		private static void OnFontOverrideSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is SimpleToolkitTheme theme && e.NewValue is string sourceUri)
			{
				theme.FontOverrideDictionary = new ResourceDictionary() { Source = new Uri(sourceUri) };
			}
		}

		private static void OnColorOverrideSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is SimpleToolkitTheme theme && e.NewValue is string sourceUri)
			{
				theme.ColorOverrideDictionary = new ResourceDictionary() { Source = new Uri(sourceUri) };
			}
		}

		private static void OnFontOverrideChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is SimpleToolkitTheme { _isOverridingFont: false } toolkitTheme)
			{
				toolkitTheme.UpdateSource();
			}
		}

		private static void OnColorOverrideChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is SimpleToolkitTheme { _isOverridingColor: false } toolkitTheme)
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

		public SimpleToolkitTheme() : this(colorOverride: null, fontOverride: null)
		{
		}

		public SimpleToolkitTheme(ResourceDictionary? colorOverride = null, ResourceDictionary? fontOverride = null)
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

			MergedDictionaries.Add(new SimpleTheme(ColorOverrideDictionary, FontOverrideDictionary) { DefaultSize = DefaultSize });
			MergedDictionaries.Add(new ResourceDictionary { Source = new Uri($"ms-appx:///{ToolkitPackageName}/Generated/mergedpages.xaml") });
			MergedDictionaries.Add(new ResourceDictionary { Source = new Uri($"ms-appx:///{ToolkitSimplePackageName}/Generated/mergedpages.xaml") });
		}
	}
}
