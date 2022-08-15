using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.System;
using Windows.UI.ViewManagement;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#endif

namespace Uno.Toolkit.UI
{
	public static class InputExtensions
	{
		#region DependencyProperty: AutoDismiss

		public static DependencyProperty AutoDismissProperty { get; } = DependencyProperty.RegisterAttached(
			"AutoDismiss",
			typeof(bool),
			typeof(InputExtensions),
			new PropertyMetadata(default(bool), OnAutoDismissChanged));

		public static bool GetAutoDismiss(DependencyObject obj) => (bool)obj.GetValue(AutoDismissProperty);
		public static void SetAutoDismiss(DependencyObject obj, bool value) => obj.SetValue(AutoDismissProperty, value);

		#endregion
		#region DependencyProperty: AutoFocusNext

		public static DependencyProperty AutoFocusNextProperty { get; } = DependencyProperty.RegisterAttached(
			"AutoFocusNext",
			typeof(bool),
			typeof(InputExtensions),
			new PropertyMetadata(default(bool), OnAutoFocusNextChanged));

		public static bool GetAutoFocusNext(DependencyObject obj) => (bool)obj.GetValue(AutoFocusNextProperty);
		public static void SetAutoFocusNext(DependencyObject obj, bool value) => obj.SetValue(AutoFocusNextProperty, value);

		#endregion
		#region DependencyProperty: AutoFocusNextElement

		public static DependencyProperty AutoFocusNextElementProperty { get; } = DependencyProperty.RegisterAttached(
			"AutoFocusNextElement",
			typeof(DependencyObject),
			typeof(InputExtensions),
			new PropertyMetadata(default(Control), OnAutoFocusNextElementChanged));

		public static Control GetAutoFocusNextElement(DependencyObject obj) => (Control)obj.GetValue(AutoFocusNextElementProperty);
		public static void SetAutoFocusNextElement(DependencyObject obj, Control value) => obj.SetValue(AutoFocusNextElementProperty, value);

		#endregion
		#region DependencyProperty: EnterCommand

		public static DependencyProperty EnterCommandProperty { get; } = DependencyProperty.RegisterAttached(
			"EnterCommand",
			typeof(ICommand),
			typeof(InputExtensions),
			new PropertyMetadata(default(ICommand), OnEnterCommandChanged));

		public static ICommand GetEnterCommand(DependencyObject obj) => (ICommand)obj.GetValue(EnterCommandProperty);
		public static void SetEnterCommand(DependencyObject obj, ICommand value) => obj.SetValue(EnterCommandProperty, value);

		#endregion

		private static void OnAutoDismissChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => UpdateSubscription(sender);
		private static void OnAutoFocusNextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => UpdateSubscription(sender);
		private static void OnAutoFocusNextElementChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => UpdateSubscription(sender);
		private static void OnEnterCommandChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => UpdateSubscription(sender);

		private static void UpdateSubscription(DependencyObject sender)
		{
			if (sender is Control control && (sender is TextBox || sender is PasswordBox))
			{
				// note: on android, UIElement.KeyUp and Control.KeyUp are not the same event, and the UIElement one doesnt work.
				control.KeyUp -= OnUIElementKeyUp;
				if (GetIsBehaviorActive())
				{
					control.KeyUp += OnUIElementKeyUp;
				}
			}

			bool GetIsBehaviorActive() =>
				GetAutoDismiss(sender) ||
				GetAutoFocusNext(sender) ||
				GetAutoFocusNextElement(sender) != null ||
				GetEnterCommand(sender) != null;
		}

		private static void OnUIElementKeyUp(object sender, KeyRoutedEventArgs e)
		{
			if (sender is not DependencyObject owner) return;
			if (e.Key != VirtualKey.Enter) return;

			// handle enter command
			var command = GetEnterCommand(owner);
			if (command != null &&
				GetCommandParameter() is var parameter && // this is not really needed, but we throw it in just for good measure
				command.CanExecute(parameter))
			{
				command.Execute(parameter);
			}

			object? GetCommandParameter() => sender switch
			{
				TextBox tb => tb.Text,
#if !HAS_UNO // note: on uno, PasswordBox inherits from TextBox which isnt the case on uwp...
				PasswordBox pb => pb.Password,
#endif

				_ => default,
			};

			// dismiss keyboard
			if (GetAutoDismiss(owner) ||
				command != null) // we should also dismiss keyboard if a command has been executed (even if CanExecute failed)
			{
				InputPane.GetForCurrentView().TryHide();
			}

			// change focus
			var target = GetAutoFocusNextElement(owner);
			if (GetAutoFocusNext(owner) || target != null) // either property can be used to enable this feature
			{
				target ??= FocusManager.FindNextFocusableElement(FocusNavigationDirection.Next) as Control;

				target?.Focus(FocusState.Keyboard);
			}
		}
	}
}
