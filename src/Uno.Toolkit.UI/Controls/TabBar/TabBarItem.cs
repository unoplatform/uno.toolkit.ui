#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;

#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

#endif
using System;
using Windows.UI.Core;
using Windows.UI.Input;

namespace Uno.Toolkit.UI
{
	/// <summary>
	/// An item to be contained within a <see cref="TabBar"/>
	/// </summary>
	public partial class TabBarItem : SelectorItem
	{
		private const string ContentPresenterName = "ContentPresenter";

		private static class CommonStates
		{
			public const string Selected = "Selected";
			public const string Normal = "Normal";
			public const string Over = "PointerOver";
			public const string Pressed = "Pressed";
			public const string OverSelected = "PointerOverSelected"; // "SelectedPointerOver" for ListBoxItem, ComboBoxItem and PivotHeaderItem
			public const string PressedSelected = "PressedSelected"; // "SelectedPressed" for ListBoxItem, ComboBoxItem and PivotHeaderItem
		}

		private static class TabBarIconPositionStates
		{
			public const string IconOnTop = "IconOnTop";
			public const string IconOnly = "IconOnly";
			public const string ContentOnly = "ContentOnly";
		}

		internal event RoutedEventHandler? IsSelectedChanged;
		public
#if __ANDROID__
			new
#endif
			event RoutedEventHandler? Click;

		private bool _isPointerOver;
		private bool _isPointerPressed;
		private bool _wasAutomationSelected;

		public TabBarItem()
		{
			DefaultStyleKey = typeof(TabBarItem);
			Unloaded += OnUnloaded;
			Tapped += OnTap;
			this.RegisterPropertyChangedCallback(SelectorItem.IsSelectedProperty, OnIsSelectedChanged);
		}

		/// <inheritdoc />
		protected override AutomationPeer OnCreateAutomationPeer() => new TabBarItemAutomationPeer(this);

		internal UIElement? GetContentTemplateRoot()
		{
			// ContentControl.ContentTemplateRoot is typed as the native view on
			// some platforms (UIView on iOS/macOS, View on Android), so it needs
			// a cast rather than an implicit conversion. On those platforms
			// UIElement derives from the native type, so the cast succeeds for
			// XAML content.
			if (ContentTemplateRoot as UIElement is { } root)
			{
				return root;
			}

			// ContentPresenter.ContentTemplateRoot is Uno-specific and does not
			// exist on WinUI, so walk the visual tree instead.
			if (GetTemplateChild(ContentPresenterName) is ContentPresenter presenter &&
				VisualTreeHelper.GetChildrenCount(presenter) > 0)
			{
				return VisualTreeHelper.GetChild(presenter, 0) as UIElement;
			}

			return null;
		}

		private void OnTap(object sender, TappedRoutedEventArgs e)
		{
			ExecuteTap();
		}

		internal void ExecuteTap()
		{
			Click?.Invoke(this, null);
			ExecuteCommand();
			Flyout?.ShowAt(this);
		}

		private void OnIsSelectedChanged(DependencyObject sender, DependencyProperty dp)
		{
			if (dp == SelectorItem.IsSelectedProperty)
			{
				var isSelected = IsSelected;

				IsSelectedChanged?.Invoke(this, null);
				UpdateCommonStates();

				if (isSelected != _wasAutomationSelected)
				{
					var oldValue = _wasAutomationSelected;
					_wasAutomationSelected = isSelected;

					// Screen readers rely on this to announce selection changes (WinUI parity).
					if (AutomationPeer.ListenerExists(AutomationEvents.PropertyChanged) &&
						FrameworkElementAutomationPeer.CreatePeerForElement(this) is TabBarItemAutomationPeer peer)
					{
						peer.RaisePropertyChangedEvent(SelectionItemPatternIdentifiers.IsSelectedProperty, oldValue, isSelected);
					}
				}
			}
		}

		private void OnUnloaded(object sender, RoutedEventArgs e)
		{
			Flyout?.Hide();
		}

		private void ExecuteCommand()
		{
			var command = Command;
			var parameter = CommandParameter;

			if (command != null && command.CanExecute(parameter))
			{
				command.Execute(parameter);
			}
		}

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			UpdateLocalVisualState();
		}

		/// <inheritdoc />
		protected override void OnPointerEntered(PointerRoutedEventArgs args)
		{
			base.OnPointerEntered(args);
			_isPointerOver = true;
			UpdateLocalVisualState();
		}

		/// <inheritdoc />
		protected override void OnPointerPressed(PointerRoutedEventArgs args)
		{
			base.OnPointerPressed(args);
			_isPointerPressed = true;
			UpdateLocalVisualState();
		}

		/// <inheritdoc />
		protected override void OnPointerReleased(PointerRoutedEventArgs args)
		{
			base.OnPointerReleased(args);
			_isPointerPressed = false;
			UpdateLocalVisualState();
		}

		/// <inheritdoc />
		protected override void OnPointerExited(PointerRoutedEventArgs args)
		{
			base.OnPointerExited(args);
			_isPointerOver = false;
			UpdateLocalVisualState();
		}

		protected override void OnPointerCanceled(PointerRoutedEventArgs e)
		{
			base.OnPointerCanceled(e);
			_isPointerPressed = false;
			_isPointerOver = false;
			UpdateLocalVisualState();
		}

		private void OnPropertyChanged(DependencyPropertyChangedEventArgs args)
		{
			var property = args.Property;
			if (property == IconProperty)
			{
				UpdateLocalVisualState();
			}
		}

		private void UpdateLocalVisualState()
		{
			bool shouldShowIcon = ShouldShowIcon();
			bool shouldShowContent = ShouldShowContent();

			UpdateVisualStateForIconAndContent(shouldShowIcon, shouldShowContent);

			UpdateCommonStates();
		}

		private void UpdateCommonStates(bool useTransitions = true)
		{
			var isEnabled = IsEnabled;
			var isPressed = _isPointerPressed;
			var isPointerOver = _isPointerOver;
			var focusState = FocusState;
			var isSelected = IsSelected;

			// Update the Interaction state group
			if (!isEnabled)
			{
				VisualStateManager.GoToState(this, "Disabled", useTransitions);
			}
			else if (isPressed)
			{
				VisualStateManager.GoToState(this, isSelected ? "PressedSelected" : "Pressed", useTransitions);
			}
			else if (isPointerOver)
			{
				VisualStateManager.GoToState(this, isSelected ? "PointerOverSelected" : "PointerOver", useTransitions);
			}
			else
			{
				VisualStateManager.GoToState(this, isSelected ? "Selected" : "Normal", useTransitions);
			}

			// Update the Focus group
			if (focusState != FocusState.Unfocused && isEnabled)
			{
				if (focusState == FocusState.Pointer)
				{
					VisualStateManager.GoToState(this, "PointerFocused", useTransitions);
				}
				else
				{
					VisualStateManager.GoToState(this, "Focused", useTransitions);
				}
			}
			else
			{
				VisualStateManager.GoToState(this, "Unfocused", useTransitions);
			}
		}

		private void UpdateVisualStateForIconAndContent(bool showIcon, bool showContent)
		{
			var stateName = showIcon
				? (showContent ? TabBarIconPositionStates.IconOnTop : TabBarIconPositionStates.IconOnly)
				: TabBarIconPositionStates.ContentOnly;

			VisualStateManager.GoToState(this, stateName, useTransitions: true);
		}

		private bool ShouldShowIcon()
		{
			return Icon != null;
		}

		private bool ShouldShowContent()
		{
			return Content != null;
		}

		protected override void OnKeyDown(KeyRoutedEventArgs e)
		{
			base.OnKeyDown(e);
			if (!e.Handled)
			{
				if (e.OriginalKey == Windows.System.VirtualKey.Enter)
				{
					ExecuteTap();
				}
			}
		}
	}
}
