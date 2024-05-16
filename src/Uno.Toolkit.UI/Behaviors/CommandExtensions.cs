using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.System;
using Microsoft.Extensions.Logging;
using Uno.Extensions;
using Uno.Logging;
using System.Diagnostics.CodeAnalysis;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using ItemsRepeater = Microsoft.UI.Xaml.Controls.ItemsRepeater;
#endif

namespace Uno.Toolkit.UI
{
	public static class CommandExtensions
	{
		private static ILogger _logger = typeof(CommandExtensions).Log();

		#region DependencyProperty: Command

		/// <summary>
		/// Backing property for the command to execute when <see cref="TextBox"/>/<see cref="PasswordBox"/> enter key is pressed,
		/// <see cref="ListViewBase.ItemClick" />, <see cref="NavigationView.ItemInvoked"/>, or <see cref="ItemsRepeater"/> item tapped.
		/// </summary>
		/// <remarks>
		/// For Command, the relevant parameter is also provided for the <see cref="ICommand.CanExecute(object)"/> and <see cref="ICommand.Execute(object)"/> call:
		/// <list type="bullet">
		///   <item><see cref="TextBox.Text"/></item>
		///   <item><see cref="PasswordBox.Password"/></item>
		///   <item><see cref="ItemClickEventArgs.ClickedItem"/> from <see cref="ListViewBase.ItemClick"/></item>
		///   <item><see cref="NavigationViewItemInvokedEventArgs.InvokedItem"/> from <see cref="NavigationView.ItemInvoked"/></item>
		///   <item><see cref="ItemsRepeater"/>'s item root's DataContext</item>
		/// </list>
		/// Unless <see cref="CommandParameterProperty"/> is set, which replaces the above.
		/// </remarks>
		public static DependencyProperty CommandProperty { [DynamicDependency(nameof(GetCommand))] get; } = DependencyProperty.RegisterAttached(
			"Command",
			typeof(ICommand),
			typeof(CommandExtensions),
			new PropertyMetadata(default(ICommand), OnCommandChanged));

		[DynamicDependency(nameof(SetCommand))]
		public static ICommand GetCommand(DependencyObject obj) => (ICommand)obj.GetValue(CommandProperty);

		[DynamicDependency(nameof(GetCommand))]
		public static void SetCommand(DependencyObject obj, ICommand value) => obj.SetValue(CommandProperty, value);

		#endregion
		#region DependencyProperty: CommandParameter

		/// <summary>
		/// Backing property for the parameter to pass to the <see cref="CommandProperty"/>.
		/// </summary>
		public static DependencyProperty CommandParameterProperty { [DynamicDependency(nameof(GetCommandParameter))] get; } = DependencyProperty.RegisterAttached(
			"CommandParameter",
			typeof(object),
			typeof(CommandExtensions),
			new PropertyMetadata(default(object?)));

		[DynamicDependency(nameof(SetCommandParameter))]
		public static object? GetCommandParameter(DependencyObject obj) => obj.GetValue(CommandParameterProperty);

		[DynamicDependency(nameof(GetCommandParameter))]
		public static void SetCommandParameter(DependencyObject obj, object? value) => obj.SetValue(CommandParameterProperty, value);

		#endregion

		private static void OnCommandChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (InputExtensions.IsEnterCommandSupportedFor(sender))
			{
				// for input controls, this will be implemented in InputExtensions.
				InputExtensions.OnEnterCommandChanged(sender, e);
			}
			if (sender is ItemsRepeater ir)
			{
				ItemsRepeaterExtensions.OnItemCommandChanged(ir, e);
			}
			else if (sender is ListViewBase lvb)
			{
				if (!lvb.IsItemClickEnabled && _logger.IsEnabled(LogLevel.Warning))
				{
					_logger.Warn("IsItemClickEnabled is not enabled on the associated list. This must be enabled to make this behavior work");
				}

				lvb.ItemClick -= OnListViewItemClick;
				if (GetCommand(sender) is { } command)
				{
					lvb.ItemClick += OnListViewItemClick;
				}
			}
			else if (sender is NavigationView nv)
			{
				nv.ItemInvoked -= OnNavigationViewItemInvoked;
				if (GetCommand(sender) is { } command)
				{
					nv.ItemInvoked += OnNavigationViewItemInvoked;
				}
			}
			else if (sender is UIElement uie)
			{
				uie.Tapped -= OnUIElementTapped;
				if (GetCommand(sender) is { } command)
				{
					uie.Tapped += OnUIElementTapped;
				}
			}
			else
			{
				if (_logger.IsEnabled(LogLevel.Warning))
				{
					_logger.Warn($"CommandExtensions.Command is not supported on '{sender.GetType().FullName}'.");
				}
			}
		}

		internal static bool TryInvokeCommand(DependencyObject owner) => TryInvokeCommand(owner, GetCommandParameter(owner));
		internal static bool TryInvokeCommand(DependencyObject owner, object? parameter)
		{
			if (GetCommand(owner) is { } command &&
				command.CanExecute(parameter))
			{
				command.Execute(parameter);
				return true;
			}

			return false;
		}

		private static void OnListViewItemClick(object sender, ItemClickEventArgs e)
		{
			if (sender is not ListViewBase host) return;

			TryInvokeCommand(host, GetCommandParameter(host) ?? e.ClickedItem);
		}
		private static void OnNavigationViewItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs e)
		{
			TryInvokeCommand(sender, GetCommandParameter(sender) ?? e.InvokedItem);
		}
		private static void OnUIElementTapped(object sender, TappedRoutedEventArgs e)
		{
			if (sender is not  UIElement host) return;

			TryInvokeCommand(host, GetCommandParameter(host) ?? host);
		}
	}
}
