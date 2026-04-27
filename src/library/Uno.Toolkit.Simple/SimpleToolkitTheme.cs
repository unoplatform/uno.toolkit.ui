using System;
using Uno.Simple;
using Uno.Themes;
using Microsoft.UI.Xaml;

namespace Uno.Toolkit.UI.Simple
{
	/// <summary>
	/// Simple Design System styles for the controls in the Uno.Toolkit.UI library.
	/// </summary>
	public class SimpleToolkitTheme : BaseToolkitTheme
	{
		private const string ToolkitPackageName = "Uno.Toolkit.WinUI";
		private const string ToolkitSimplePackageName = "Uno.Toolkit.WinUI.Simple";

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

		public SimpleToolkitTheme() : this(colorOverride: null, fontOverride: null)
		{
		}

		public SimpleToolkitTheme(ResourceDictionary? colorOverride = null, ResourceDictionary? fontOverride = null)
			: base(colorOverride, fontOverride)
		{
		}

		protected override void UpdateSource()
		{
#if !HAS_UNO
			Source = null;
#endif
			ThemeDictionaries.Clear();
			MergedDictionaries.Clear();
			this.Clear();

			var simpleTheme = new SimpleTheme(ColorOverrideDictionary, FontOverrideDictionary) { DefaultSize = DefaultSize };
			if (Colors is { } colors)
			{
				simpleTheme.Colors = colors;
			}

			MergedDictionaries.Add(simpleTheme);
			MergedDictionaries.Add(new ResourceDictionary { Source = new Uri($"ms-appx:///{ToolkitPackageName}/Generated/mergedpages.xaml") });
			MergedDictionaries.Add(new ResourceDictionary { Source = new Uri($"ms-appx:///{ToolkitSimplePackageName}/Generated/mergedpages.xaml") });
		}
	}
}
