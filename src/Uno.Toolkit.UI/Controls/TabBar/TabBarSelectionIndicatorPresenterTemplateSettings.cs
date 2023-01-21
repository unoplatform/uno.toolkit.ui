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
		internal static readonly DependencyProperty IndicatorTransitionFromProperty =
			DependencyProperty.Register(nameof(IndicatorTransitionFrom),
				typeof(Point),
				typeof(TabBarSelectionIndicatorPresenterTemplateSettings),
				new PropertyMetadata(default));

		public Point IndicatorTransitionFrom
		{
			get => (Point)GetValue(IndicatorTransitionFromProperty);
			internal set => SetValue(IndicatorTransitionFromProperty, value);
		}

		internal static readonly DependencyProperty IndicatorTransitionToProperty =
			DependencyProperty.Register(nameof(IndicatorTransitionTo),
				typeof(Point),
				typeof(TabBarSelectionIndicatorPresenterTemplateSettings),
				new PropertyMetadata(default));

		public Point IndicatorTransitionTo
		{
			get => (Point)GetValue(IndicatorTransitionToProperty);
			internal set => SetValue(IndicatorTransitionToProperty, value);
		}

		internal static readonly DependencyProperty IndicatorMaxSizeProperty =
			DependencyProperty.Register(nameof(IndicatorMaxSize),
				typeof(Size),
				typeof(TabBarSelectionIndicatorPresenterTemplateSettings),
				new PropertyMetadata(default));

		public Size IndicatorMaxSize
		{
			get => (Size)GetValue(IndicatorMaxSizeProperty);
			internal set => SetValue(IndicatorMaxSizeProperty, value);
		}
	}
}
