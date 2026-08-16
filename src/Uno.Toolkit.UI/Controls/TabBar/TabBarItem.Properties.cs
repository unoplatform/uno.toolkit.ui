using System;
using System.Windows.Input;
using MUXC = Microsoft.UI.Xaml.Controls;

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

		// UNO TODO: Deprecate and remove BadgeVisibility and BadgeValue properties and use InfoBadge instead
		#region BadgeVisibility
		// UNO TODO: Obsolete attribute is currently not working with generators, for more details see https://github.com/unoplatform/uno.csharpmarkup/issues/741
		// [Obsolete("This property is deprecated. Use InfoBadge instead.", true)]
		public Visibility BadgeVisibility
		{
			get => (Visibility)GetValue(BadgeVisibilityProperty);
			set => SetValue(BadgeVisibilityProperty, value);
		}

		// UNO TODO: Obsolete attribute is currently not working with generators, for more details see https://github.com/unoplatform/uno.csharpmarkup/issues/741
		// [Obsolete("This property is deprecated. Use InfoBadge instead.", true)]
		public static readonly DependencyProperty BadgeVisibilityProperty =
			DependencyProperty.Register(nameof(BadgeVisibility), typeof(Visibility), typeof(TabBarItem), new PropertyMetadata(Visibility.Collapsed, OnBadgeVisibilityChanged));
		#endregion

		#region BadgeValue
		// UNO TODO: Obsolete attribute is currently not working with generators, for more details see https://github.com/unoplatform/uno.csharpmarkup/issues/741
		// [Obsolete("This property is deprecated. Use InfoBadge instead.", true)]
		public string? BadgeValue
		{
			get => (string?)GetValue(BadgeValueProperty);
			set => SetValue(BadgeValueProperty, value);
		}
		// UNO TODO: Obsolete attribute is currently not working with generators, for more details see https://github.com/unoplatform/uno.csharpmarkup/issues/741
		// [Obsolete("This property is deprecated. Use InfoBadge instead.", true)]
		public static readonly DependencyProperty BadgeValueProperty =
			DependencyProperty.Register(nameof(BadgeValue), typeof(string), typeof(TabBarItem), new PropertyMetadata(default(string?), OnBadgeValueChanged));
		#endregion

		#region InfoBadge
		public Control InfoBadge
		{
			get => (Control)GetValue(InfoBadgeProperty);
			set => SetValue(InfoBadgeProperty, value);
		}

		public static readonly DependencyProperty InfoBadgeProperty =
			DependencyProperty.Register(nameof(InfoBadge), typeof(Control), typeof(TabBarItem), new PropertyMetadata(null, OnPropertyChanged));
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

		internal bool IsStyleSetFromTabBar { get; set; }

		private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			var owner = (TabBarItem)sender;
			owner.OnPropertyChanged(args);
		}

		private static void OnBadgeVisibilityChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			var owner = (TabBarItem)sender;
			owner.InfoBadge ??= new MUXC.InfoBadge();
			owner.InfoBadge.Visibility = (Visibility)args.NewValue;
		}

		private static void OnBadgeValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			var owner = (TabBarItem)sender;
			owner.InfoBadge ??= new MUXC.InfoBadge
			{
				Visibility = owner.BadgeVisibility
			};

			if (owner.InfoBadge is not MUXC.InfoBadge infoBadge)
			{
				return;
			}

			var value = (string?)args.NewValue;
			if (int.TryParse(value, out int intValue))
			{
				infoBadge.IconSource = null;
				infoBadge.Value = intValue;
			}
			else
			{
				infoBadge.Value = -1;
				infoBadge.IconSource = string.IsNullOrEmpty(value)
					? null
					: new MUXC.FontIconSource { Glyph = value };
			}
		}
	}
}
