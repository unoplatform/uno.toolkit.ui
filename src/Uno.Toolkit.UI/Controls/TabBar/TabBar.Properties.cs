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
		#region DependencyProperty: SelectedItem

		public static DependencyProperty SelectedItemProperty { get; } = DependencyProperty.Register(
			nameof(SelectedItem),
			typeof(object),
			typeof(TabBar),
			new PropertyMetadata(default(object), OnPropertyChanged));

		public object SelectedItem
		{
			get => (object)GetValue(SelectedItemProperty);
			set => SetValue(SelectedItemProperty, value);
		}

		#endregion
		#region DependencyProperty: SelectedIndex

		public static DependencyProperty SelectedIndexProperty { get; } = DependencyProperty.Register(
			nameof(SelectedIndex),
			typeof(int),
			typeof(TabBar),
			new PropertyMetadata(-1, OnPropertyChanged));

		public int SelectedIndex
		{
			get => (int)GetValue(SelectedIndexProperty);
			set => SetValue(SelectedIndexProperty, value);
		}

		#endregion
		#region DependencyProperty: TemplateSettings

		public static DependencyProperty TemplateSettingsProperty { get; } = DependencyProperty.Register(
			nameof(TemplateSettings),
			typeof(TabBarTemplateSettings),
			typeof(TabBar),
			new PropertyMetadata(default(TabBarTemplateSettings)));

		public TabBarTemplateSettings TemplateSettings
		{
			get => (TabBarTemplateSettings)GetValue(TemplateSettingsProperty);
			set => SetValue(TemplateSettingsProperty, value);
		}

		#endregion
		#region DependencyProperty: Orientation

		public static DependencyProperty OrientationProperty { get; } = DependencyProperty.Register(
			nameof(Orientation),
			typeof(Orientation),
			typeof(TabBar),
			new PropertyMetadata(Orientation.Horizontal, OnPropertyChanged));

		public Orientation Orientation
		{
			get => (Orientation)GetValue(OrientationProperty);
			set => SetValue(OrientationProperty, value);
		}

		#endregion
		#region DependencyProperty: SelectionIndicatorContent

		public static DependencyProperty SelectionIndicatorContentProperty { get; } = DependencyProperty.Register(
			nameof(SelectionIndicatorContent),
			typeof(object),
			typeof(TabBar),
			new PropertyMetadata(default(object), OnPropertyChanged));

		public object SelectionIndicatorContent
		{
			get => (object)GetValue(SelectionIndicatorContentProperty);
			set => SetValue(SelectionIndicatorContentProperty, value);
		}

		#endregion
		#region DependencyProperty: SelectionIndicatorContentTemplate

		public static DependencyProperty SelectionIndicatorContentTemplateProperty { get; } = DependencyProperty.Register(
			nameof(SelectionIndicatorContentTemplate),
			typeof(DataTemplate),
			typeof(TabBar),
			new PropertyMetadata(default(DataTemplate), OnPropertyChanged));

		public DataTemplate SelectionIndicatorContentTemplate
		{
			get => (DataTemplate)GetValue(SelectionIndicatorContentTemplateProperty);
			set => SetValue(SelectionIndicatorContentTemplateProperty, value);
		}

		#endregion
		#region DependencyProperty: SelectionIndicatorPresenterStyle

		public static DependencyProperty SelectionIndicatorPresenterStyleProperty { get; } = DependencyProperty.Register(
			nameof(SelectionIndicatorPresenterStyle),
			typeof(Style),
			typeof(TabBar),
			new PropertyMetadata(default(Style), OnPropertyChanged));

		public Style SelectionIndicatorPresenterStyle
		{
			get => (Style)GetValue(SelectionIndicatorPresenterStyleProperty);
			set => SetValue(SelectionIndicatorPresenterStyleProperty, value);
		}

		#endregion
		#region DependencyProperty: SelectionIndicatorTransitionMode

		public static DependencyProperty SelectionIndicatorTransitionModeProperty { get; } = DependencyProperty.Register(
			nameof(SelectionIndicatorTransitionMode),
			typeof(IndicatorTransitionMode),
			typeof(TabBar),
			new PropertyMetadata(IndicatorTransitionMode.Snap, OnPropertyChanged));

		public IndicatorTransitionMode SelectionIndicatorTransitionMode
		{
			get => (IndicatorTransitionMode)GetValue(SelectionIndicatorTransitionModeProperty);
			set => SetValue(SelectionIndicatorTransitionModeProperty, value);
		}

		#endregion

		private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			var owner = (TabBar)sender;
			owner.OnPropertyChanged(args);
		}
	}
}
