using System;
using Uno.Themes;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#endif

namespace Uno.Toolkit.UI
{
	/// <summary>
	/// Base class for toolkit theme wrappers (e.g. MaterialToolkitTheme,
	/// SimpleToolkitTheme). Contains the shared override dependency properties
	/// and plumbing that each concrete theme inherits.
	/// </summary>
	public abstract class BaseToolkitTheme : ResourceDictionary
	{
		private bool _isOverridingColor;
		private bool _isOverridingFont;

		#region DependencyProperty: FontOverrideSource
		/// <summary>
		/// (Optional) Gets or sets a Uniform Resource Identifier (<see cref="Uri"/>) that provides the source location
		/// of a <see cref="ResourceDictionary"/> containing overrides for the default <see cref="FontFamily"/> resources
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
				typeof(BaseToolkitTheme),
				new PropertyMetadata(null, OnFontOverrideSourceChanged));
		#endregion

		#region DependencyProperty: ColorOverrideSource
		/// <summary>
		/// (Optional) Gets or sets a Uniform Resource Identifier (<see cref="Uri"/>) that provides the source location
		/// of a <see cref="ResourceDictionary"/> containing overrides for the default Color resources
		/// </summary>
		public string ColorOverrideSource
		{
			get => (string)GetValue(ColorOverrideSourceProperty);
			set => SetValue(ColorOverrideSourceProperty, value);
		}

		public static DependencyProperty ColorOverrideSourceProperty { get; } =
			DependencyProperty.Register(
				nameof(ColorOverrideSource),
				typeof(string),
				typeof(BaseToolkitTheme),
				new PropertyMetadata(null, OnColorOverrideSourceChanged));
		#endregion

		#region DependencyProperty: FontOverrideDictionary
		/// <summary>
		/// (Optional) Gets or sets a <see cref="ResourceDictionary"/> containing overrides for the default <see cref="FontFamily"/> resources
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
				typeof(BaseToolkitTheme),
				new PropertyMetadata(null, OnFontOverrideChanged));
		#endregion

		#region DependencyProperty: ColorOverrideDictionary
		/// <summary>
		/// (Optional) Gets or sets a <see cref="ResourceDictionary"/> containing overrides for the default Color resources
		/// </summary>
		public ResourceDictionary? ColorOverrideDictionary
		{
			get => (ResourceDictionary?)GetValue(ColorOverrideDictionaryProperty);
			set => SetValue(ColorOverrideDictionaryProperty, value);
		}

		public static DependencyProperty ColorOverrideDictionaryProperty { get; } =
			DependencyProperty.Register(
				nameof(ColorOverrideDictionary),
				typeof(ResourceDictionary),
				typeof(BaseToolkitTheme),
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
				typeof(BaseToolkitTheme),
				new PropertyMetadata(null, OnColorsChanged));
		#endregion

		private static void OnFontOverrideSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is BaseToolkitTheme theme && e.NewValue is string sourceUri)
			{
				theme.FontOverrideDictionary = new ResourceDictionary() { Source = new Uri(sourceUri) };
			}
		}

		private static void OnColorOverrideSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is BaseToolkitTheme theme && e.NewValue is string sourceUri)
			{
				theme.ColorOverrideDictionary = new ResourceDictionary() { Source = new Uri(sourceUri) };
			}
		}

		private static void OnFontOverrideChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is BaseToolkitTheme { _isOverridingFont: false } theme)
			{
				theme.UpdateSource();
			}
		}

		private static void OnColorOverrideChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is BaseToolkitTheme { _isOverridingColor: false } theme)
			{
				theme.UpdateSource();
			}
		}

		private static void OnColorsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is BaseToolkitTheme theme)
			{
				theme.UpdateSource();
			}
		}

		protected void SetColorOverrideSilently(ResourceDictionary? colorOverride)
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

		protected void SetFontOverrideSilently(ResourceDictionary? fontOverride)
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

		protected BaseToolkitTheme(ResourceDictionary? colorOverride, ResourceDictionary? fontOverride)
		{
			SetColorOverrideSilently(colorOverride);
			SetFontOverrideSilently(fontOverride);

			UpdateSource();
		}

		protected abstract void UpdateSource();
	}
}
