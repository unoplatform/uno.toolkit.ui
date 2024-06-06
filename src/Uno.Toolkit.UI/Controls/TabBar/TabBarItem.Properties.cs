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
		[Obsolete("This property is deprecated. Use InfoBadge instead.", false)]
		public Visibility BadgeVisibility
		{
			get => InfoBadge?.Visibility ?? (Visibility)GetValue(BadgeVisibilityProperty);
			set
			{
				SetValue(BadgeVisibilityProperty, value);

				InfoBadge ??= new MUXC.InfoBadge();
				InfoBadge.Visibility = value;
			}
		}

		[Obsolete("This property is deprecated. Use InfoBadge instead.", false)]
		public static readonly DependencyProperty BadgeVisibilityProperty =
			DependencyProperty.Register(nameof(BadgeVisibility), typeof(Visibility), typeof(TabBarItem), new PropertyMetadata(Visibility.Collapsed, OnPropertyChanged));
		#endregion

		#region BadgeValue
		[Obsolete("This property is deprecated. Use InfoBadge instead.", false)]
		public string? BadgeValue
		{
			get => (InfoBadge as MUXC.InfoBadge)?.Value.ToString() ?? (string)GetValue(BadgeValueProperty);
			set
			{
				SetValue(BadgeValueProperty, value);

				InfoBadge ??= new MUXC.InfoBadge();

				if (InfoBadge is MUXC.InfoBadge infoBadge)
				{
					if (int.TryParse(value, out int intValue))
					{
						infoBadge.Value = intValue;
					}
					else
					{
						infoBadge.IconSource = new MUXC.FontIconSource { Glyph = value };
					}
				}
			}
		}
		[Obsolete("This property is deprecated. Use InfoBadge instead.", false)]
		public static readonly DependencyProperty BadgeValueProperty =
			DependencyProperty.Register(nameof(BadgeValue), typeof(string), typeof(TabBarItem), new PropertyMetadata(default(string?), OnPropertyChanged));
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

		private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			var owner = (TabBarItem)sender;
			owner.OnPropertyChanged(args);
		}
	}
}
