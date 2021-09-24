using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using AppBarButton = Microsoft.UI.Xaml.Controls.AppBarButton;
using ICommandBarElement = Microsoft.UI.Xaml.Controls.ICommandBarElement;
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
				typeof(NavigationBar),
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

		public IObservableVector<ICommandBarElement> PrimaryCommands
		{
			get => (IObservableVector<ICommandBarElement>)this.GetValue(PrimaryCommandsProperty);
			private set => this.SetValue(PrimaryCommandsProperty, value);
		}

		public static DependencyProperty PrimaryCommandsProperty { get; } =
			DependencyProperty.Register(
				nameof(PrimaryCommands),
				typeof(IObservableVector<ICommandBarElement>),
				typeof(NavigationBar),
				new PropertyMetadata(
					default(IObservableVector<ICommandBarElement>), OnPropertyChanged)
			);

		#endregion

		#region SecondaryCommands

		public IObservableVector<ICommandBarElement> SecondaryCommands
		{
			get => (IObservableVector<ICommandBarElement>)this.GetValue(SecondaryCommandsProperty);
			private set => this.SetValue(SecondaryCommandsProperty, value);
		}

		public static DependencyProperty SecondaryCommandsProperty { get; } =
			DependencyProperty.Register(
				nameof(SecondaryCommands),
				typeof(IObservableVector<ICommandBarElement>),
				typeof(NavigationBar),
				new PropertyMetadata(
					default(IObservableVector<ICommandBarElement>), OnPropertyChanged)
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
				nameof(OverflowButtonVisibility),
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
				nameof(IsDynamicOverflowEnabled),
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
				nameof(DefaultLabelPosition),
				typeof(NavigationBarDefaultLabelPosition),
				typeof(NavigationBar),
				new PropertyMetadata(default(NavigationBarDefaultLabelPosition), OnPropertyChanged)
			);

		#endregion

		#region LeftCommand
		public AppBarButton LeftCommand
		{
			get { return (AppBarButton)GetValue(LeftCommandProperty); }
			set { SetValue(LeftCommandProperty, value); }
		}

		public static DependencyProperty LeftCommandProperty { get; } =
			DependencyProperty.Register(nameof(LeftCommand), typeof(AppBarButton), typeof(NavigationBar), new PropertyMetadata(default(AppBarButton), OnPropertyChanged));
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

		#region LeftCommandStyle
		public Style LeftCommandStyle
		{
			get { return (Style)GetValue(LeftCommandStyleProperty); }
			set { SetValue(LeftCommandStyleProperty, value); }
		}

		public static DependencyProperty LeftCommandStyleProperty { get; } =
			DependencyProperty.Register(nameof(LeftCommandStyle), typeof(Style), typeof(NavigationBar), new PropertyMetadata(default(Style), OnPropertyChanged));
		#endregion

		private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			var owner = (NavigationBar)sender;
			owner.OnPropertyChanged(args);
		}
	}
}
