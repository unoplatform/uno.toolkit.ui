using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;

#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
#endif

namespace Uno.Toolkit.UI
{
	partial class TabBarItem
	{
		#region Icon
		public IconElement? Icon
		{
			get { return (IconElement)GetValue(IconProperty); }
			set { SetValue(IconProperty, value); }
		}

		public static readonly DependencyProperty IconProperty =
			DependencyProperty.Register(nameof(Icon), typeof(IconElement), typeof(TabBarItem), new PropertyMetadata(null, OnPropertyChanged));
		#endregion

		#region BadgeVisibility
		public Visibility BadgeVisibility
		{
			get { return (Visibility)GetValue(BadgeVisibilityProperty); }
			set { SetValue(BadgeVisibilityProperty, value); }
		}

		public static readonly DependencyProperty BadgeVisibilityProperty =
			DependencyProperty.Register("BadgeVisibility", typeof(Visibility), typeof(TabBarItem), new PropertyMetadata(Visibility.Collapsed, OnPropertyChanged));
		#endregion

		#region BadgeValue
		public string? BadgeValue
		{
			get { return (string)GetValue(BadgeValueProperty); }
			set { SetValue(BadgeValueProperty, value); }
		}

		public static readonly DependencyProperty BadgeValueProperty =
			DependencyProperty.Register("BadgeValue", typeof(string), typeof(TabBarItem), new PropertyMetadata(default(string?), OnPropertyChanged));
		#endregion

		#region IsSelectable
		public bool IsSelectable
		{
			get { return (bool)GetValue(IsSelectableProperty); }
			set { SetValue(IsSelectableProperty, value); }
		}

		public static DependencyProperty IsSelectableProperty { get; } =
			DependencyProperty.Register(nameof(IsSelectable), typeof(bool), typeof(TabBarItem), new PropertyMetadata(true, OnPropertyChanged));
		#endregion

		#region Flyout
		public FlyoutBase Flyout
		{
			get { return (FlyoutBase)GetValue(FlyoutProperty); }
			set { SetValue(FlyoutProperty, value); }
		}

		public static DependencyProperty FlyoutProperty { get; } =
			DependencyProperty.Register(nameof(Flyout), typeof(FlyoutBase), typeof(TabBarItem), new PropertyMetadata(null, OnPropertyChanged));
		#endregion

		#region Command
		public ICommand Command
		{
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}

		public static DependencyProperty CommandProperty { get; } =
			DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(TabBarItem), new PropertyMetadata(null, OnPropertyChanged));
		#endregion

		#region CommandParameter
		public object CommandParameter
		{
			get { return (object)GetValue(CommandParameterProperty); }
			set { SetValue(CommandParameterProperty, value); }
		}

		public static DependencyProperty CommandParameterProperty { get; } =
			DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(TabBarItem), new PropertyMetadata(null, OnPropertyChanged));
		#endregion

		private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			var owner = (TabBarItem)sender;
			owner.OnPropertyChanged(args);
		}
	}
}
