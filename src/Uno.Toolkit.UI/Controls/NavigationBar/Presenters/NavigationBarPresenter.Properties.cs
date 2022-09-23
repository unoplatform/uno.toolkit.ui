using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Uno.Toolkit.UI
{
	partial class NavigationBarPresenter
	{

		#region IsSticky
		/// <summary>
		/// Gets or sets a value that indicates whether the AppBar does not close on light dismiss.
		/// </summary>
		public bool IsSticky
		{
			get => (bool)GetValue(IsStickyProperty);
			set => SetValue(IsStickyProperty, value);
		}

		public static DependencyProperty IsStickyProperty { get; } =
			DependencyProperty.Register(
				nameof(IsSticky),
				typeof(bool),
				typeof(NavigationBarPresenter),
				new PropertyMetadata(default(bool))
			);

		#endregion

		#region IsOpen
		/// <summary>
		/// Gets or sets a value that indicates whether the AppBar is open.
		/// </summary>
		public bool IsOpen
		{
			get => (bool)GetValue(IsOpenProperty);
			set => SetValue(IsOpenProperty, value);
		}

		public static DependencyProperty IsOpenProperty { get; } =
		DependencyProperty.Register(
			nameof(IsOpen),
			typeof(bool),
			typeof(NavigationBarPresenter),
			new PropertyMetadata(default(bool))
		);

		#endregion

		#region LightDismissOverlayMode
		/// <summary>
		/// Gets or sets a value that specifies whether the area outside of a light-dismiss UI is darkened.
		/// </summary>
		public LightDismissOverlayMode LightDismissOverlayMode
		{
			get => (LightDismissOverlayMode)GetValue(LightDismissOverlayModeProperty);
			set => SetValue(LightDismissOverlayModeProperty, value);
		}

		public static DependencyProperty LightDismissOverlayModeProperty { get; } =
			DependencyProperty.Register(
				nameof(LightDismissOverlayMode),
				typeof(LightDismissOverlayMode),
				typeof(NavigationBarPresenter),
				new PropertyMetadata(default(LightDismissOverlayMode))
			);

		#endregion

		#region IsDynamicOverflowEnabled
		/// <summary>
		/// Gets or sets a value that indicates whether primary commands automatically move to the overflow menu when space is limited.
		/// </summary>
		public bool IsDynamicOverflowEnabled
		{
			get => (bool)this.GetValue(IsDynamicOverflowEnabledProperty);
			set => SetValue(IsDynamicOverflowEnabledProperty, value);
		}

		public static DependencyProperty IsDynamicOverflowEnabledProperty { get; } =
			DependencyProperty.Register(
				nameof(IsDynamicOverflowEnabled),
				typeof(bool),
				typeof(NavigationBarPresenter),
				new PropertyMetadata(default(bool))
			);

		#endregion

		#region NavigationBarContent
		/// <summary>
		/// Gets or sets a value that indicates whether primary commands automatically move to the overflow menu when space is limited.
		/// </summary>
		public object NavigationBarContent
		{
			get => (object)this.GetValue(NavigationBarContentProperty);
			set => SetValue(NavigationBarContentProperty, value);
		}

		public static DependencyProperty NavigationBarContentProperty { get; } =
			DependencyProperty.Register(
				nameof(NavigationBarContent),
				typeof(object),
				typeof(NavigationBarPresenter),
				new PropertyMetadata(default(object))
			);

		#endregion

		#region MainCommand
		/// <summary>
		/// Gets or sets an AppBarButton to be used for back-navigation or invoking a custom command.
		/// </summary>
		public AppBarButton MainCommand
		{
			get { return (AppBarButton)GetValue(MainCommandProperty); }
			set { SetValue(MainCommandProperty, value); }
		}

		public static DependencyProperty MainCommandProperty { get; } =
			DependencyProperty.Register(
				nameof(MainCommand),
				typeof(AppBarButton),
				typeof(NavigationBarPresenter),
				new PropertyMetadata(default(AppBarButton), OnPropertyChanged));
		#endregion

		#region MainCommandStyle
		/// <summary>
		/// Gets or sets the Style for the MainCommand
		/// </summary>
		public Style MainCommandStyle
		{
			get { return (Style)GetValue(MainCommandStyleProperty); }
			set { SetValue(MainCommandStyleProperty, value); }
		}

		public static DependencyProperty MainCommandStyleProperty { get; } =
			DependencyProperty.Register(
				nameof(MainCommandStyle),
				typeof(Style),
				typeof(NavigationBarPresenter),
				new PropertyMetadata(default(Style), OnPropertyChanged));
		#endregion

		#region CommandBarStyle
		/// <summary>
		/// Gets or sets the Style for the CommandBar
		/// </summary>
		public Style CommandBarStyle
		{
			get { return (Style)GetValue(CommandBarStyleProperty); }
			set { SetValue(CommandBarStyleProperty, value); }
		}

		public static DependencyProperty CommandBarStyleProperty { get; } =
			DependencyProperty.Register(
				nameof(CommandBarStyle),
				typeof(Style),
				typeof(NavigationBarPresenter),
				new PropertyMetadata(default(Style)));
		#endregion

		private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			var owner = (NavigationBarPresenter)sender;
			owner.OnPropertyChanged(args);
		}
	}
}
