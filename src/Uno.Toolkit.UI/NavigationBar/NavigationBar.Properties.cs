using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using AppBarButton = Microsoft.UI.Xaml.Controls.AppBarButton;
using LightDismissOverlayMode = Microsoft.UI.Xaml.Controls.LightDismissOverlayMode;
#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
#endif


namespace Uno.UI.ToolkitLib
{
	partial class NavigationBar
	{
		#region IsSticky

		public bool IsSticky
		{
			get => (bool)GetValue(IsStickyProperty);
			set => SetValue(IsStickyProperty, value);
		}

		public static DependencyProperty IsStickyProperty { get; } =
			DependencyProperty.Register(
				nameof(IsSticky),
				typeof(bool),
				typeof(NavigationBar),
				new PropertyMetadata(default(bool))
			);

		#endregion

		#region IsOpen

		public bool IsOpen
		{
			get => (bool)GetValue(IsOpenProperty);
			set => SetValue(IsOpenProperty, value);
		}

		public static DependencyProperty IsOpenProperty { get; } =
		DependencyProperty.Register(
			nameof(IsOpen),
			typeof(bool),
			typeof(NavigationBar),
			new PropertyMetadata(default(bool))
		);

		#endregion

		#region ClosedDisplayMode

		public NavigationBarClosedDisplayMode ClosedDisplayMode
		{
			get => (NavigationBarClosedDisplayMode)GetValue(ClosedDisplayModeProperty);
			set => SetValue(ClosedDisplayModeProperty, value);
		}

		public static DependencyProperty ClosedDisplayModeProperty { get; } =
			DependencyProperty.Register(
				nameof(ClosedDisplayMode),
				typeof(NavigationBarClosedDisplayMode),
				typeof(NavigationBar),
				new PropertyMetadata(NavigationBarClosedDisplayMode.Compact)
			);

		#endregion

		#region LightDismissOverlayMode

		public LightDismissOverlayMode LightDismissOverlayMode
		{
			get => (LightDismissOverlayMode)GetValue(LightDismissOverlayModeProperty);
			set => SetValue(LightDismissOverlayModeProperty, value);
		}

		public static DependencyProperty LightDismissOverlayModeProperty { get; } =
			DependencyProperty.Register(
				nameof(LightDismissOverlayMode),
				typeof(LightDismissOverlayMode),
				typeof(AppBar),
				new PropertyMetadata(default(LightDismissOverlayMode))
			);

		#endregion

		#region TemplateSettings
		public NavigationBarTemplateSettings TemplateSettings
		{
			get { return (NavigationBarTemplateSettings)this.GetValue(TemplateSettingsProperty); }
			set { this.SetValue(TemplateSettingsProperty, value); }
		}
		public static DependencyProperty TemplateSettingsProperty { get; } =
			DependencyProperty.Register(nameof(TemplateSettings), typeof(NavigationBarTemplateSettings), typeof(AppBar), new PropertyMetadata(null));
		#endregion

		#region PrimaryCommands

		public IObservableVector<AppBarButton> PrimaryCommands
		{
			get => (IObservableVector<AppBarButton>)this.GetValue(PrimaryCommandsProperty);
			private set => this.SetValue(PrimaryCommandsProperty, value);
		}

		public static DependencyProperty PrimaryCommandsProperty { get; } =
			DependencyProperty.Register(
				"PrimaryCommands",
				typeof(IObservableVector<AppBarButton>),
				typeof(NavigationBar),
				new PropertyMetadata(
					default(IObservableVector<AppBarButton>), OnPropertyChanged)
			);

		#endregion

		#region SecondaryCommands

		public IObservableVector<AppBarButton> SecondaryCommands
		{
			get => (IObservableVector<AppBarButton>)this.GetValue(SecondaryCommandsProperty);
			private set => this.SetValue(SecondaryCommandsProperty, value);
		}

		public static DependencyProperty SecondaryCommandsProperty { get; } =
			DependencyProperty.Register(
				"SecondaryCommands",
				typeof(IObservableVector<AppBarButton>),
				typeof(NavigationBar),
				new PropertyMetadata(
					default(IObservableVector<AppBarButton>), OnPropertyChanged)
			);

		#endregion

		#region OverflowButtonVisibility

		public NavigationBarOverflowButtonVisibility OverflowButtonVisibility
		{
			get => (NavigationBarOverflowButtonVisibility)this.GetValue(OverflowButtonVisibilityProperty);
			set => SetValue(OverflowButtonVisibilityProperty, value);
		}

		public static DependencyProperty OverflowButtonVisibilityProperty { get; } =
			DependencyProperty.Register(
				"OverflowButtonVisibility",
				typeof(NavigationBarOverflowButtonVisibility),
				typeof(NavigationBar),
				new PropertyMetadata(default(NavigationBarOverflowButtonVisibility), OnPropertyChanged)
			);

		#endregion

		#region IsDynamicOverflowEnabled

		public bool IsDynamicOverflowEnabled
		{
			get => (bool)this.GetValue(IsDynamicOverflowEnabledProperty);
			set => SetValue(IsDynamicOverflowEnabledProperty, value);
		}

		public static DependencyProperty IsDynamicOverflowEnabledProperty { get; } =
			DependencyProperty.Register(
				"IsDynamicOverflowEnabled",
				typeof(bool),
				typeof(NavigationBar),
				new PropertyMetadata(default(bool), OnPropertyChanged)
			);

		#endregion

		#region DefaultLabelPosition

		public NavigationBarDefaultLabelPosition DefaultLabelPosition
		{
			get => (NavigationBarDefaultLabelPosition)this.GetValue(DefaultLabelPositionProperty);
			set => SetValue(DefaultLabelPositionProperty, value);
		}

		public static DependencyProperty DefaultLabelPositionProperty { get; } =
			DependencyProperty.Register(
				"DefaultLabelPosition",
				typeof(NavigationBarDefaultLabelPosition),
				typeof(NavigationBar),
				new PropertyMetadata(default(NavigationBarDefaultLabelPosition), OnPropertyChanged)
			);

		#endregion

		#region MainCommand
		public AppBarButton MainCommand
		{
			get { return (AppBarButton)GetValue(MainCommandProperty); }
			set { SetValue(MainCommandProperty, value); }
		}

		public static DependencyProperty MainCommandProperty { get; } =
			DependencyProperty.Register(nameof(MainCommand), typeof(AppBarButton), typeof(NavigationBar), new PropertyMetadata(default(AppBarButton), OnPropertyChanged));
		#endregion

		#region Subtitle
		public string? Subtitle
		{
			get { return (string)GetValue(SubtitleProperty); }
			set { SetValue(SubtitleProperty, value); }
		}

		public static DependencyProperty SubtitleProperty { get; } =
			DependencyProperty.Register(nameof(Subtitle), typeof(string), typeof(NavigationBar), new PropertyMetadata(default(string?), OnPropertyChanged));
		#endregion

		private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			var owner = (NavigationBar)sender;
			owner.OnPropertyChanged(args);
		}
	}
}
