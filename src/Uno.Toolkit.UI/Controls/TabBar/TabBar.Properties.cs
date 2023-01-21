using System;
using System.Collections.Generic;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.UI
{
	public partial class TabBar
	{
		#region SelectedItem
		public object? SelectedItem
		{
			get { return (object)GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}

		public static DependencyProperty SelectedItemProperty { get; } =
			DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(TabBar), new PropertyMetadata(null, OnPropertyChanged));
		#endregion

		#region SelectedIndex
		public int SelectedIndex
		{
			get { return (int)GetValue(SelectedIndexProperty); }
			set { SetValue(SelectedIndexProperty, value); }
		}

		public static DependencyProperty SelectedIndexProperty { get; } =
			DependencyProperty.Register(nameof(SelectedIndex), typeof(int), typeof(TabBar), new PropertyMetadata(-1, OnPropertyChanged));
		#endregion

		#region TemplateSettings
		public TabBarTemplateSettings TemplateSettings
		{
			get => (TabBarTemplateSettings)GetValue(TemplateSettingsProperty);
			private set => SetValue(TemplateSettingsProperty, value);
		}
		public static DependencyProperty TemplateSettingsProperty { get; } =
			DependencyProperty.Register(nameof(TemplateSettings), typeof(TabBarTemplateSettings), typeof(TabBar), new PropertyMetadata(null));
		#endregion

		#region Orientation
		public Orientation Orientation
		{
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}

		public static DependencyProperty OrientationProperty { get; } =
			DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(TabBar), new PropertyMetadata(Orientation.Horizontal, OnPropertyChanged));
		#endregion

		#region SelectionIndicatorContent
		public object? SelectionIndicatorContent
		{
			get { return (object)GetValue(SelectionIndicatorContentProperty); }
			set { SetValue(SelectionIndicatorContentProperty, value); }
		}

		public static DependencyProperty SelectionIndicatorContentProperty { get; } =
			DependencyProperty.Register(nameof(SelectionIndicatorContent), typeof(object), typeof(TabBar), new PropertyMetadata(null, OnPropertyChanged));
		#endregion

		#region SelectionIndicatorContentTemplate
		public DataTemplate SelectionIndicatorContentTemplate
		{
			get { return (DataTemplate)GetValue(SelectionIndicatorContentTemplateProperty); }
			set { SetValue(SelectionIndicatorContentTemplateProperty, value); }
		}

		public static DependencyProperty SelectionIndicatorContentTemplateProperty { get; } =
			DependencyProperty.Register(nameof(SelectionIndicatorContentTemplate), typeof(DataTemplate), typeof(TabBar), new PropertyMetadata(null, OnPropertyChanged));
		#endregion

		#region SelectionIndicatorPresenterStyle
		public Style SelectionIndicatorPresenterStyle
		{
			get { return (Style)GetValue(SelectionIndicatorPresenterStyleProperty); }
			set { SetValue(SelectionIndicatorPresenterStyleProperty, value); }
		}

		public static DependencyProperty SelectionIndicatorPresenterStyleProperty { get; } =
			DependencyProperty.Register(nameof(SelectionIndicatorPresenterStyle), typeof(Style), typeof(TabBar), new PropertyMetadata(null, OnPropertyChanged));
		#endregion

		#region SelectionIndicatorTransitionMode
		public IndicatorTransitionMode SelectionIndicatorTransitionMode
		{
			get { return (IndicatorTransitionMode)GetValue(SelectionIndicatorTransitionModeProperty); }
			set { SetValue(SelectionIndicatorTransitionModeProperty, value); }
		}

		public static DependencyProperty SelectionIndicatorTransitionModeProperty { get; } =
			DependencyProperty.Register(nameof(SelectionIndicatorTransitionMode), typeof(IndicatorTransitionMode), typeof(TabBar), new PropertyMetadata(IndicatorTransitionMode.Snap, OnPropertyChanged));
		#endregion

		private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			var owner = (TabBar)sender;
			owner.OnPropertyChanged(args);
		}
	}
}
