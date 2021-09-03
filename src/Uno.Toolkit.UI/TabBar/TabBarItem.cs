#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#endif

namespace Uno.UI.ToolkitLib
{
	/// <summary>
	/// An item to be contained within a <see cref="TabBar"/>
	/// </summary>
	public partial class TabBarItem : Button
	{
		private static class CommonStates
		{
			public const string Selected = "Selected";
			public const string Normal = "Normal";
			public const string Over = "PointerOver";
			public const string Pressed = "Pressed";
			public const string OverSelected = "PointerOverSelected";
			public const string PressedSelected = "PressedSelected";
			public const string Unselectable = "Unselectable";
		}

		private static class DisabledStates
		{
			public const string Enabled = "Enabled";
			public const string Disabled = "Disabled";
		}

		private static class TabBarIconPositionStates
		{
			public const string IconOnTop = "IconOnTop";
			public const string IconOnly = "IconOnly";
			public const string ContentOnly = "ContentOnly";
		}

		internal event RoutedEventHandler? IsSelectedChanged;

		public TabBarItem()
		{
			DefaultStyleKey = typeof(TabBarItem);
			Loaded += OnLoaded;
            LostFocus += OnLostFocus;
		}

        private void OnLoaded(object sender, RoutedEventArgs e)
		{
			UpdateCommonStates();
		}

		protected override void OnPointerEntered(PointerRoutedEventArgs args)
		{
			base.OnPointerEntered(args);
			UpdateCommonStates();
		}


		protected override void OnPointerExited(PointerRoutedEventArgs args)
		{
			base.OnPointerExited(args);
			UpdateCommonStates();
		}

		protected override void OnPointerPressed(PointerRoutedEventArgs args)
		{
			base.OnPointerPressed(args);
			UpdateCommonStates();
		}


		protected override void OnPointerReleased(PointerRoutedEventArgs args)
		{
			base.OnPointerReleased(args);
			UpdateCommonStates();
		}

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			UpdateLocalVisualState();
		}

		private void OnPropertyChanged(DependencyPropertyChangedEventArgs args)
		{
			var property = args.Property;
			if (property == IconProperty)
			{
				UpdateLocalVisualState();
			}
			else if (property == IsSelectedProperty)
			{
				IsSelectedChanged?.Invoke(this, null);
				UpdateCommonStates();
			}
			else if (property == IsSelectableProperty)
			{
				UpdateCommonStates();
			}
		}

		private void UpdateLocalVisualState()
		{
			bool shouldShowIcon = ShouldShowIcon();
			bool shouldShowContent = ShouldShowContent();

			UpdateVisualStateForIconAndContent(shouldShowIcon, shouldShowContent);
		}

		private void UpdateVisualStateForIconAndContent(bool showIcon, bool showContent)
		{
			var stateName = showIcon 
				? (showContent ? TabBarIconPositionStates.IconOnTop : TabBarIconPositionStates.IconOnly) 
				: TabBarIconPositionStates.ContentOnly;

			VisualStateManager.GoToState(this, stateName, useTransitions: false);
		}

		private bool ShouldShowIcon()
		{
			return Icon != null;
		}

		private bool ShouldShowContent()
		{
			return Content != null;
		}

		private void UpdateCommonStates()
		{
			var state = GetState(IsEnabled, IsSelected, IsPointerOver, IsPressed);
			VisualStateManager.GoToState(this, state, useTransitions: false);
		}

		private string GetState(bool isEnabled, bool isSelected, bool isOver, bool isPressed)
		{
			var state = CommonStates.Normal;

			if (isEnabled)
			{
				if (isSelected && isPressed)
				{
					state = CommonStates.PressedSelected;
				}
				else if (isSelected && isOver)
				{
					state = CommonStates.OverSelected;
				}
				else if (isSelected)
				{
					state = CommonStates.Selected;
				}
				else if (isPressed)
				{
					state = CommonStates.Pressed;
				}
				else if (isOver)
				{
					state = CommonStates.Over;
				}
			}
			else
			{
				// If item is disabled, we only care about selection state
				if (isSelected)
				{
					state = CommonStates.Selected;
				}
			}

			return state;
		}

		private void OnLostFocus(object sender, RoutedEventArgs e)
		{
			// Prevent VisualState from being reset to Normal upon lost focus
			UpdateCommonStates();
		}
	}
}
