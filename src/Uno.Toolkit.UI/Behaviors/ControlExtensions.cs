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

		public static DependencyProperty CommandProperty { get; } = DependencyProperty.RegisterAttached(
			"Command",
			typeof(ICommand),
			typeof(ControlExtensions),
			new PropertyMetadata(default(ICommand), OnCommandChanged));

		public static ICommand GetCommand(DependencyObject obj) => (ICommand)obj.GetValue(CommandProperty);
		public static void SetCommand(DependencyObject obj, ICommand value) => obj.SetValue(CommandProperty, value);

		#endregion
		#region DependencyProperty: CommandParameter

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
