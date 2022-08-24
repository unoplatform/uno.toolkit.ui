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
	public static class ControlExtensions
	{
		private static ILogger _logger = typeof(ControlExtensions).Log();

		#region DependencyProperty: Command

		/// <summary>
		/// Backing property for the command to execute when <see cref="TextBox"/>/<see cref="PasswordBox"/> enter key is pressed, <see cref="ListViewBase.ItemClick" /> and <see cref="NavigationView.ItemInvoked"/>.
		/// </summary>
		/// <remarks>
		/// For Command, the relevant parameter is also provided for the <see cref="ICommand.CanExecute(object)"/> and <see cref="ICommand.Execute(object)"/> call:
		/// <list type="bullet">
		///   <item><see cref="TextBox.Text"/></item>
		///   <item><see cref="PasswordBox.Password"/></item>
		///   <item><see cref="ItemClickEventArgs.ClickedItem"/> from <see cref="ListViewBase.ItemClick"/></item>
		///   <item><see cref="NavigationViewItemInvokedEventArgs.InvokedItem"/> from <see cref="NavigationView.ItemInvoked"/> </item>
		/// </list>
		/// Unless <see cref="CommandParameterProperty"/> is set, which replaces the above.
		/// </remarks>
		public static DependencyProperty CommandProperty { get; } = DependencyProperty.RegisterAttached(
			"Command",
			typeof(ICommand),
			typeof(ControlExtensions),
			new PropertyMetadata(default(ICommand), OnCommandChanged));

		public static ICommand GetCommand(DependencyObject obj) => (ICommand)obj.GetValue(CommandProperty);
		public static void SetCommand(DependencyObject obj, ICommand value) => obj.SetValue(CommandProperty, value);

		#endregion
		#region DependencyProperty: CommandParameter

		/// <summary>
		/// Backing property for the parameter to pass to the <see cref="CommandProperty"/>.
		/// </summary>
		public static DependencyProperty CommandParameterProperty { get; } = DependencyProperty.RegisterAttached(
			"CommandParameter",
			typeof(object),
			typeof(ControlExtensions),
			new PropertyMetadata(default(object)));

		public static object GetCommandParameter(DependencyObject obj) => (object)obj.GetValue(CommandParameterProperty);
		public static void SetCommandParameter(DependencyObject obj, object value) => obj.SetValue(CommandParameterProperty, value);

		#endregion

		private static void OnCommandChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (InputExtensions.IsEnterCommandSupportedFor(sender))
			{
				// for input controls, this will be implemented in InputExtensions.
				InputExtensions.OnEnterCommandChanged(sender, e);
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
			else
			{
				if (_logger.IsEnabled(LogLevel.Warning))
				{
					_logger.Warn($"ControlExtensions.Command is not supported on '{sender.GetType().FullName}'.");
				}
			}
		}

		private static void OnListViewItemClick(object sender, ItemClickEventArgs e)
		{
			if (sender is not ListViewBase host) return;

			if (GetCommand(host) is { } command &&
				GetCommandParameter(host) is var parameter &&
				command.CanExecute(parameter ?? e.ClickedItem))
			{
				command.Execute(parameter ?? e.ClickedItem);
			}
		}

		private static void OnNavigationViewItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs e)
		{
			if (GetCommand(sender) is { } command &&
				GetCommandParameter(sender) is var parameter &&
				command.CanExecute(parameter ?? e.InvokedItem))
			{
				command.Execute(parameter ?? e.InvokedItem);
			}
		}
	}
}
