using System;
using System.Collections.Generic;
using System.Text;

#if IS_WINUI
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.UI
{
	public partial class Card
	{
		private static class CommonStates
		{
			public const string Normal = nameof(Normal);
			public const string PointerOver = nameof(PointerOver);
			public const string Pressed = nameof(Pressed);
			public const string Disabled = nameof(Disabled);
		}
		private static class FocusStates
		{
			public const string Unfocused = nameof(Unfocused);
			public const string Focused = nameof(Focused);
			public const string PointerFocused = nameof(PointerFocused);
		}

		private static readonly Windows.UI.Color DefaultShadowColor
#if __ANDROID__
			= Colors.Black;
#else
			= Windows.UI.Color.FromArgb(64, 0, 0, 0);
#endif
	}

	public partial class Card
	{
		#region DependencyProperty: HeaderContent

		public static DependencyProperty HeaderContentProperty { get; } = DependencyProperty.Register(
			nameof(HeaderContent),
			typeof(object),
			typeof(Card),
			new PropertyMetadata(default(object)));

		public object HeaderContent
		{
			get => (object)GetValue(HeaderContentProperty);
			set => SetValue(HeaderContentProperty, value);
		}

		#endregion
		#region DependencyProperty: HeaderContentTemplate

		public static DependencyProperty HeaderContentTemplateProperty { get; } = DependencyProperty.Register(
			nameof(HeaderContentTemplate),
			typeof(DataTemplate),
			typeof(Card),
			new PropertyMetadata(default(DataTemplate)));

		public DataTemplate HeaderContentTemplate
		{
			get => (DataTemplate)GetValue(HeaderContentTemplateProperty);
			set => SetValue(HeaderContentTemplateProperty, value);
		}

		#endregion
		#region DependencyProperty: SubHeaderContent

		public static DependencyProperty SubHeaderContentProperty { get; } = DependencyProperty.Register(
			nameof(SubHeaderContent),
			typeof(object),
			typeof(Card),
			new PropertyMetadata(default(object)));

		public object SubHeaderContent
		{
			get => (object)GetValue(SubHeaderContentProperty);
			set => SetValue(SubHeaderContentProperty, value);
		}

		#endregion
		#region DependencyProperty: SubHeaderContentTemplate

		public static DependencyProperty SubHeaderContentTemplateProperty { get; } = DependencyProperty.Register(
			nameof(SubHeaderContentTemplate),
			typeof(DataTemplate),
			typeof(Card),
			new PropertyMetadata(default(DataTemplate)));

		public DataTemplate SubHeaderContentTemplate
		{
			get => (DataTemplate)GetValue(SubHeaderContentTemplateProperty);
			set => SetValue(SubHeaderContentTemplateProperty, value);
		}

		#endregion
		#region DependencyProperty: AvatarContent

		public static DependencyProperty AvatarContentProperty { get; } = DependencyProperty.Register(
			nameof(AvatarContent),
			typeof(object),
			typeof(Card),
			new PropertyMetadata(default(object)));

		public object AvatarContent
		{
			get => (object)GetValue(AvatarContentProperty);
			set => SetValue(AvatarContentProperty, value);
		}

		#endregion
		#region DependencyProperty: AvatarContentTemplate

		public static DependencyProperty AvatarContentTemplateProperty { get; } = DependencyProperty.Register(
			nameof(AvatarContentTemplate),
			typeof(DataTemplate),
			typeof(Card),
			new PropertyMetadata(default(DataTemplate)));

		public DataTemplate AvatarContentTemplate
		{
			get => (DataTemplate)GetValue(AvatarContentTemplateProperty);
			set => SetValue(AvatarContentTemplateProperty, value);
		}

		#endregion
		#region DependencyProperty: MediaContent

		public static DependencyProperty MediaContentProperty { get; } = DependencyProperty.Register(
			nameof(MediaContent),
			typeof(object),
			typeof(Card),
			new PropertyMetadata(default(object)));

		public object MediaContent
		{
			get => (object)GetValue(MediaContentProperty);
			set => SetValue(MediaContentProperty, value);
		}

		#endregion
		#region DependencyProperty: MediaContentTemplate

		public static DependencyProperty MediaContentTemplateProperty { get; } = DependencyProperty.Register(
			nameof(MediaContentTemplate),
			typeof(DataTemplate),
			typeof(Card),
			new PropertyMetadata(default(DataTemplate)));

		public DataTemplate MediaContentTemplate
		{
			get => (DataTemplate)GetValue(MediaContentTemplateProperty);
			set => SetValue(MediaContentTemplateProperty, value);
		}

		#endregion
		#region DependencyProperty: SupportingContent

		public static DependencyProperty SupportingContentProperty { get; } = DependencyProperty.Register(
			nameof(SupportingContent),
			typeof(object),
			typeof(Card),
			new PropertyMetadata(default(object)));

		public object SupportingContent
		{
			get => (object)GetValue(SupportingContentProperty);
			set => SetValue(SupportingContentProperty, value);
		}

		#endregion
		#region DependencyProperty: SupportingContentTemplate

		public static DependencyProperty SupportingContentTemplateProperty { get; } = DependencyProperty.Register(
			nameof(SupportingContentTemplate),
			typeof(DataTemplate),
			typeof(Card),
			new PropertyMetadata(default(DataTemplate)));

		public DataTemplate SupportingContentTemplate
		{
			get => (DataTemplate)GetValue(SupportingContentTemplateProperty);
			set => SetValue(SupportingContentTemplateProperty, value);
		}

		#endregion
		#region DependencyProperty: IconsContent

		public static DependencyProperty IconsContentProperty { get; } = DependencyProperty.Register(
			nameof(IconsContent),
			typeof(object),
			typeof(Card),
			new PropertyMetadata(default(object)));

		public object IconsContent
		{
			get => (object)GetValue(IconsContentProperty);
			set => SetValue(IconsContentProperty, value);
		}

		#endregion
		#region DependencyProperty: IconsContentTemplate

		public static DependencyProperty IconsContentTemplateProperty { get; } = DependencyProperty.Register(
			nameof(IconsContentTemplate),
			typeof(DataTemplate),
			typeof(Card),
			new PropertyMetadata(default(DataTemplate)));

		public DataTemplate IconsContentTemplate
		{
			get => (DataTemplate)GetValue(IconsContentTemplateProperty);
			set => SetValue(IconsContentTemplateProperty, value);
		}

		#endregion

		#region DependencyProperty: Elevation

		public static DependencyProperty ElevationProperty { get; } = DependencyProperty.Register(
			nameof(Elevation),
			typeof(double),
			typeof(Card),
			new PropertyMetadata(default(double)));

		public
#if __ANDROID__
			new
#endif
			double Elevation
		{
			get => (double)GetValue(ElevationProperty);
			set => SetValue(ElevationProperty, value);
		}

		#endregion
		#region DependencyProperty: ShadowColor = DefaultShadowColor

		public static DependencyProperty ShadowColorProperty { get; } = DependencyProperty.Register(
			nameof(ShadowColor),
			typeof(Windows.UI.Color),
			typeof(Card),
			new PropertyMetadata(DefaultShadowColor));

		public Windows.UI.Color ShadowColor
		{
			get => (Windows.UI.Color)GetValue(ShadowColorProperty);
			set => SetValue(ShadowColorProperty, value);
		}

		#endregion
		#region DependencyProperty: IsClickable = true

		public static DependencyProperty IsClickableProperty { get; } = DependencyProperty.Register(
			nameof(IsClickable),
			typeof(bool),
			typeof(Card),
			new PropertyMetadata(true));

		/// <summary>
		/// Gets or sets a value indicating whether the control will respond to pointer and focus events.
		/// </summary>
		public bool IsClickable
		{
			get => (bool)GetValue(IsClickableProperty);
			set => SetValue(IsClickableProperty, value);
		}

		#endregion
	}
}
