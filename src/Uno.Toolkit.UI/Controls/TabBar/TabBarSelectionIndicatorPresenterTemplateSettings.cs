using System;
using System.Collections.Generic;
using System.Text;
using Windows.Foundation;

#if IS_WINUI
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml;
#endif

namespace Uno.Toolkit.UI
{
	public partial class TabBarSelectionIndicatorPresenterTemplateSettings : DependencyObject
	{
		#region DependencyProperty: IndicatorTransitionFrom

		internal static DependencyProperty IndicatorTransitionFromProperty { get; } = DependencyProperty.Register(
			nameof(IndicatorTransitionFrom),
			typeof(Point),
			typeof(TabBarSelectionIndicatorPresenterTemplateSettings),
			new PropertyMetadata(default(Point)));

		public Point IndicatorTransitionFrom
		{
			get => (Point)GetValue(IndicatorTransitionFromProperty);
			set => SetValue(IndicatorTransitionFromProperty, value);
		}

		#endregion
		#region DependencyProperty: IndicatorTransitionTo

		internal static DependencyProperty IndicatorTransitionToProperty { get; } = DependencyProperty.Register(
			nameof(IndicatorTransitionTo),
			typeof(Point),
			typeof(TabBarSelectionIndicatorPresenterTemplateSettings),
			new PropertyMetadata(default(Point)));

		public Point IndicatorTransitionTo
		{
			get => (Point)GetValue(IndicatorTransitionToProperty);
			set => SetValue(IndicatorTransitionToProperty, value);
		}

		#endregion
		#region DependencyProperty: IndicatorMaxSize

		internal static DependencyProperty IndicatorMaxSizeProperty { get; } = DependencyProperty.Register(
			nameof(IndicatorMaxSize),
			typeof(Size),
			typeof(TabBarSelectionIndicatorPresenterTemplateSettings),
			new PropertyMetadata(default(Size)));

		public Size IndicatorMaxSize
		{
			get => (Size)GetValue(IndicatorMaxSizeProperty);
			set => SetValue(IndicatorMaxSizeProperty, value);
		}

		#endregion
	}
}
