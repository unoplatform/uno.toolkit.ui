using System;
using System.Collections.Generic;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.UI.Controls
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

		private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			var owner = (TabBar)sender;
			owner.OnPropertyChanged(args);
		}
	}
}
