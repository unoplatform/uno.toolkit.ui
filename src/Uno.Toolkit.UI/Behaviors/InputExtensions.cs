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

#if __ANDROID__
using Android.Views.InputMethods;
#elif __IOS__
using UIKit;
#endif

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

		public enum ReturnType {
			Default,
			Done,
			Go,
			Next,
			Search,
			Send
		}

		#region DependencyProperty: ReturnType

		/// <summary>
		/// Backing property for what type of return the soft keyboard will show.
		/// </summary>
		public static DependencyProperty ReturnTypeProperty { [DynamicDependency(nameof(GetReturnType))] get; } = DependencyProperty.RegisterAttached(
			"ReturnType",
			typeof(ReturnType),
			typeof(InputExtensions),
			new PropertyMetadata(ReturnType.Default, OnReturnTypeChanged));

		[DynamicDependency(nameof(SetReturnType))]
		public static ReturnType GetReturnType(DependencyObject obj) => (ReturnType)obj.GetValue(ReturnTypeProperty);
		[DynamicDependency(nameof(GetReturnType))]
		public static void SetReturnType(DependencyObject obj, ReturnType value) => obj.SetValue(ReturnTypeProperty, value);

		#endregion
		#region DependencyProperty: AutoDismiss

		/// <summary>
		/// Backing property for whether the soft keyboard will be dismissed when the enter key is pressed.
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

		private static void OnReturnTypeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is TextBox || sender is PasswordBox)
			{
				if (e.NewValue is not ReturnType returnType)
				{
					returnType = ReturnType.Default;
				}
#if __ANDROID__
				ImeAction imeAction = GetImeActionFromReturnType(returnType);

				if (sender is TextBox textBox)
				{
					textBox.ImeOptions = imeAction;
				}
				else if (sender is PasswordBox passwordBox)
				{
					passwordBox.ImeOptions = imeAction;
				}
#elif __IOS__
				UIReturnKeyType returnKeyType = GetReturnKeyTypeFromReturnType(returnType);

				if (sender is TextBox textBox)
				{
					textBox.ReturnKeyType = returnKeyType;
				}
				else if (sender is PasswordBox passwordBox)
				{
					passwordBox.ReturnKeyType = returnKeyType;
				}
#endif
			}
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
				target ??= FocusManager.FindNextElement(FocusNavigationDirection.Next, new FindNextElementOptions { SearchRoot = host }) as Control;

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

#if __ANDROID__
		private static ImeAction GetImeActionFromReturnType(ReturnType returnType) => returnType switch
		{
			ReturnType.Next => ImeAction.Next,
			ReturnType.Go => ImeAction.Go,
			ReturnType.Search => ImeAction.Search,
			ReturnType.Send => ImeAction.Send,
			ReturnType.Done => ImeAction.Done,
			ReturnType.Default or _ => ImeAction.Unspecified
		};
#endif

#if __IOS__
		private static UIReturnKeyType GetReturnKeyTypeFromReturnType(ReturnType returnType) => returnType switch
		{
			ReturnType.Next => UIReturnKeyType.Next,
			ReturnType.Go => UIReturnKeyType.Go,
			ReturnType.Search => UIReturnKeyType.Search,
			ReturnType.Send => UIReturnKeyType.Send,
			ReturnType.Done => UIReturnKeyType.Done,
			ReturnType.Default or _ => UIReturnKeyType.Default
		};
#endif
	}
}
