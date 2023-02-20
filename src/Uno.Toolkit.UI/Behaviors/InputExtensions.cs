﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using Uno.Extensions;
using Uno.Logging;
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
		private static readonly ILogger _logger = typeof(InputExtensions).Log();

		#region DependencyProperty: KeyUpCommand

		/// <summary>
		/// Backing property to trigger a command when a key is released.
		/// </summary>
		public static DependencyProperty KeyUpCommandProperty { get; } = DependencyProperty.RegisterAttached(
			"KeyUpCommand", typeof(ICommand), typeof(InputExtensions), new PropertyMetadata(default(ICommand), OnKeyUpCommandChanged));

		public static ICommand GetKeyUpCommand(UIElement element)
			=> (ICommand)element.GetValue(KeyUpCommandProperty);

		public static void SetKeyUpCommand(UIElement element, ICommand command)
			=> element.SetValue(KeyUpCommandProperty, command);

		private static void OnKeyUpCommandChanged(DependencyObject snd, DependencyPropertyChangedEventArgs args)
		{
			if (snd is UIElement elt)
			{
				elt.KeyUp -= OnKeyUp;
				if (args.NewValue is ICommand)
				{
					elt.KeyUp += OnKeyUp;
				}
			}
		}
		private static void OnKeyUp(object snd, KeyRoutedEventArgs e)
		{
			if (snd is UIElement elt && GetKeyUpCommand(elt) is { } command
				&& command.CanExecute(e.Key))
			{
				command.Execute(e.Key);
			}
		}
		#endregion
		#region DependencyProperty: AutoDismiss

		/// <summary>
		/// Backing property for whether the soft-keyboard will be dismissed when the enter key is pressed.
		/// </summary>
		public static DependencyProperty AutoDismissProperty { [DynamicDependency(nameof(GetAutoDismiss))] get; } = DependencyProperty.RegisterAttached(
			"AutoDismiss",
			typeof(bool),
			typeof(InputExtensions),
			new PropertyMetadata(default(bool), OnAutoDismissChanged));

		[DynamicDependency(nameof(SetAutoDismiss))]
		public static bool GetAutoDismiss(DependencyObject obj) => (bool)obj.GetValue(AutoDismissProperty);
		[DynamicDependency(nameof(GetAutoDismiss))]
		public static void SetAutoDismiss(DependencyObject obj, bool value) => obj.SetValue(AutoDismissProperty, value);

		#endregion
		#region DependencyProperty: AutoFocusNext

		/// <summary>
		/// Backing property for whether the focus will move to the next focusable element when the enter key is pressed.
		/// </summary>
		/// <remarks>
		/// Having either or both of the <see cref="AutoFocusNextProperty"/> and <see cref="AutoFocusNextElementProperty"/> set will enable the focus next behavior.
		/// AutoFocusNextElement will take precedences over AutoFocusNext when both are set.
		/// </remarks>
		public static DependencyProperty AutoFocusNextProperty { [DynamicDependency(nameof(GetAutoFocusNext))] get; } = DependencyProperty.RegisterAttached(
			"AutoFocusNext",
			typeof(bool),
			typeof(InputExtensions),
			new PropertyMetadata(default(bool), OnAutoFocusNextChanged));

		[DynamicDependency(nameof(SetAutoFocusNext))]
		public static bool GetAutoFocusNext(DependencyObject obj) => (bool)obj.GetValue(AutoFocusNextProperty);
		[DynamicDependency(nameof(GetAutoFocusNext))]
		public static void SetAutoFocusNext(DependencyObject obj, bool value) => obj.SetValue(AutoFocusNextProperty, value);

		#endregion
		#region DependencyProperty: AutoFocusNextElement

		/// <summary>
		/// Sets the next control to focus when the enter key is pressed.
		/// </summary>
		/// <remarks>
		/// Having either or both of the <see cref="AutoFocusNextProperty"/> and <see cref="AutoFocusNextElementProperty"/> set will enable the focus next behavior.
		/// AutoFocusNextElement will take precedences over AutoFocusNext when both are set.
		/// </remarks>
		public static DependencyProperty AutoFocusNextElementProperty { [DynamicDependency(nameof(GetAutoFocusNextElement))] get; } = DependencyProperty.RegisterAttached(
			"AutoFocusNextElement",
			typeof(DependencyObject),
			typeof(InputExtensions),
			new PropertyMetadata(default(Control), OnAutoFocusNextElementChanged));

		[DynamicDependency(nameof(SetAutoFocusNextElement))]
		public static Control GetAutoFocusNextElement(DependencyObject obj) => (Control)obj.GetValue(AutoFocusNextElementProperty);
		[DynamicDependency(nameof(GetAutoFocusNextElement))]
		public static void SetAutoFocusNextElement(DependencyObject obj, Control value) => obj.SetValue(AutoFocusNextElementProperty, value);

		#endregion
#if false // The property is now forwarded from CommandExtensions.Command
		#region DependencyProperty: EnterCommand

		public static DependencyProperty EnterCommandProperty { get; } = DependencyProperty.RegisterAttached(
			"EnterCommand",
			typeof(ICommand),
			typeof(InputExtensions),
			new PropertyMetadata(default(ICommand), OnEnterCommandChanged));

		public static ICommand GetEnterCommand(DependencyObject obj) => (ICommand)obj.GetValue(EnterCommandProperty);
		public static void SetEnterCommand(DependencyObject obj, ICommand value) => obj.SetValue(EnterCommandProperty, value);

		#endregion
#endif

		/// <summary>
		/// Check if InputExtensions contains the <see cref="CommandExtensions.CommandProperty" /> implementations for <paramref name="host"/>.
		/// </summary>
		internal static bool IsEnterCommandSupportedFor(DependencyObject host)
		{
			return host is TextBox || host is PasswordBox;
		}

		private static void OnAutoDismissChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => UpdateSubscription(sender);
		private static void OnAutoFocusNextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => UpdateSubscription(sender);
		private static void OnAutoFocusNextElementChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => UpdateSubscription(sender);
		internal static void OnEnterCommandChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => UpdateSubscription(sender);

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
			else
			{
				if (_logger.IsEnabled(LogLevel.Warning))
				{
					_logger.Warn($"This property is not supported on '{sender.GetType().FullName}'.");
				}
			}

			bool GetIsBehaviorActive() =>
				GetAutoDismiss(sender) ||
				GetAutoFocusNext(sender) ||
				GetAutoFocusNextElement(sender) != null ||
				CommandExtensions.GetCommand(sender) != null;
		}

		private static void OnUIElementKeyUp(object sender, KeyRoutedEventArgs e)
		{
			if (sender is not DependencyObject host) return;
			if (e.Key != VirtualKey.Enter) return;

			// handle enter command
			CommandExtensions.TryInvokeCommand(host, CommandExtensions.GetCommandParameter(host) ?? GetInputParameter());

#if HAS_UNO
			// dismiss keyboard
			if (GetAutoDismiss(host) ||
				CommandExtensions.GetCommand(host) != null) // we should also dismiss keyboard if a command has been executed (even if CanExecute failed)
			{

				InputPane.GetForCurrentView().TryHide();
			}
#endif

			// change focus
			var target = GetAutoFocusNextElement(host);
			if (GetAutoFocusNext(host) || target != null) // either property can be used to enable this feature
			{
				target ??= FocusManager.FindNextFocusableElement(FocusNavigationDirection.Next) as Control;

				target?.Focus(FocusState.Keyboard);
			}

			object? GetInputParameter() => sender switch
			{
				TextBox tb => tb.Text,
#if !HAS_UNO // note: on uno, PasswordBox inherits from TextBox which isnt the case on uwp...
				PasswordBox pb => pb.Password,
#endif

				_ => default,
			};
		}
	}
}
