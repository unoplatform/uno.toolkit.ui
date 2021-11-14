#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Controls.Primitives;

#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Controls.Primitives;

#endif
using System;
using Windows.UI.Core;
using Windows.UI.Input;

namespace Uno.Toolkit.UI.Controls
{
	/// <summary>
	/// An item to be contained within a <see cref="TabBar"/>
	/// </summary>
	public partial class TabBarItem : SelectorItem
	{
		private static class TabBarIconPositionStates
		{
			public const string IconOnTop = "IconOnTop";
			public const string IconOnly = "IconOnly";
			public const string ContentOnly = "ContentOnly";
		}

		internal event RoutedEventHandler? IsSelectedChanged;
		public event RoutedEventHandler? Click;

		private bool m_hasPointerCaptured;

		public TabBarItem()
		{
			DefaultStyleKey = typeof(TabBarItem);
			Unloaded += OnUnloaded;
			Tapped += OnTap;
			this.RegisterPropertyChangedCallback(SelectorItem.IsSelectedProperty, OnIsSelectedChanged);
		}

		private void OnTap(object sender, TappedRoutedEventArgs e)
		{
			Click?.Invoke(this, null);
			ExecuteCommand();
			Flyout?.ShowAt(this);
		}

		private void OnIsSelectedChanged(DependencyObject sender, DependencyProperty dp)
		{
			if (dp == SelectorItem.IsSelectedProperty)
			{
				IsSelectedChanged?.Invoke(this, null);
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
	}
}
