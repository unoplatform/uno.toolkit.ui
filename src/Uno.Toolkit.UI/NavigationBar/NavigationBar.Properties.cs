using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
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
				typeof(NavigationBar),
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
			typeof(NavigationBar),
			new PropertyMetadata(default(bool))
		);

		#endregion

		#region ClosedDisplayMode
		/// <summary>
		/// Gets or sets a value that indicates whether icon buttons are displayed when the app bar is not completely open.
		/// </summary>
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
				typeof(NavigationBar),
				new PropertyMetadata(default(LightDismissOverlayMode))
			);

		#endregion

		#region PrimaryCommands
		/// <summary>
		/// Gets the collection of primary command elements for the CommandBar.
		/// </summary>
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
		/// <summary>
		/// Gets the collection of secondary command elements for the CommandBar.
		/// </summary>
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
		/// <summary>
		/// Gets or sets a value that indicates when a command bar's overflow button is shown.
		/// </summary>
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
				typeof(NavigationBar),
				new PropertyMetadata(default(bool), OnPropertyChanged)
			);

		#endregion

		#region DefaultLabelPosition
		/// <summary>
		/// Gets or sets a value that indicates the placement and visibility of the labels on the command bar's buttons.
		/// </summary>
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
		/// <summary>
		/// Gets or sets an AppBarButton to be used for back-navigation or invoking a custom command.
		/// </summary>
		public AppBarButton LeftCommand
		{
			get { return (AppBarButton)GetValue(LeftCommandProperty); }
			set { SetValue(LeftCommandProperty, value); }
		}

		public static DependencyProperty LeftCommandProperty { get; } =
			DependencyProperty.Register(nameof(LeftCommand), typeof(AppBarButton), typeof(NavigationBar), new PropertyMetadata(default(AppBarButton), OnPropertyChanged));
		#endregion

		#region LeftCommandMode
		/// <summary>
		/// Gets or sets a value to indicate whether or not the LeftCommand will be used for back-navigation or custom logic.
		/// </summary>
		public LeftCommandMode LeftCommandMode
		{
			get { return (LeftCommandMode)GetValue(LeftCommandModeProperty); }
			set { SetValue(LeftCommandModeProperty, value); }
		}

		public static DependencyProperty LeftCommandModeProperty { get; } =
			DependencyProperty.Register(nameof(LeftCommandMode), typeof(LeftCommandMode), typeof(NavigationBar), new PropertyMetadata(LeftCommandMode.Back, OnPropertyChanged));
		#endregion

		#region Subtitle
		/// <summary>
		/// Gets or sets the subtitle for the CommandBar. Android only.
		/// </summary>
		public string? Subtitle
		{
			get { return (string)GetValue(SubtitleProperty); }
			set { SetValue(SubtitleProperty, value); }
		}

		public static DependencyProperty SubtitleProperty { get; } =
			DependencyProperty.Register(nameof(Subtitle), typeof(string), typeof(NavigationBar), new PropertyMetadata(default(string?), OnPropertyChanged));
		#endregion

		#region LeftCommandStyle
		/// <summary>
		/// Gets or sets the Style for the LeftCommand
		/// </summary>
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
