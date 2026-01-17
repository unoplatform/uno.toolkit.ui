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
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
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
		/// <see cref="ListViewBase.ItemClick" />, <see cref="Selector.SelectionChanged" />, <see cref="NavigationView.ItemInvoked" />, <see cref="ItemsRepeater" /> item tapped, <see cref="ToggleSwitch.Toggled"/> or <see cref="UIElement.Tapped" />.
		/// </summary>
		/// <remarks>
		/// For Command, the relevant parameter is also provided for the <see cref="ICommand.CanExecute(object)"/> and <see cref="ICommand.Execute(object)"/> call:
		/// <list type="bullet">
		///   <item><see cref="TextBox.Text"/></item>
		///   <item><see cref="PasswordBox.Password"/></item>
		///   <item><see cref="ItemClickEventArgs.ClickedItem"/> from <see cref="ListViewBase.ItemClick"/></item>
		///   <item><see cref="Selector.SelectedItem"/></item>
		///   <item><see cref="NavigationViewItemInvokedEventArgs.InvokedItem"/> from <see cref="NavigationView.ItemInvoked"/></item>
		///   <item><see cref="ItemsRepeater"/>'s item root's DataContext</item>
		///   <item><see cref="ToggleSwitch.IsOn"/></item>
		///   <item><see cref="UIElement"/> itself</item>
		/// </list>
		/// <see cref="CommandParameterProperty"/> can be set, on the item-container or item-template's root for collection-type controls, or control itself for other controls, to replace the above.
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
		#region DependencyProperty: EventCommands

		/// <summary>
		/// Backing property for a collection of <see cref="EventToCommand"/> that maps events to commands.
		/// This allows binding multiple events to multiple commands on a single element.
		/// </summary>
		/// <remarks>
		/// Each <see cref="EventToCommand"/> in the collection specifies an event name and a command to execute when that event is raised.
		/// Optional properties include CommandParameter, EventArgsConverter, and PassEventArgsToCommand.
		/// </remarks>
		public static DependencyProperty EventCommandsProperty { [DynamicDependency(nameof(GetEventCommands))] get; } = DependencyProperty.RegisterAttached(
			"EventCommands",
			typeof(EventToCommandCollection),
			typeof(CommandExtensions),
			new PropertyMetadata(default(EventToCommandCollection), OnEventCommandsChanged));

		[DynamicDependency(nameof(SetEventCommands))]
		public static EventToCommandCollection? GetEventCommands(DependencyObject obj) => (EventToCommandCollection?)obj.GetValue(EventCommandsProperty);

		[DynamicDependency(nameof(GetEventCommands))]
		public static void SetEventCommands(DependencyObject obj, EventToCommandCollection? value) => obj.SetValue(EventCommandsProperty, value);

		#endregion

		private static void OnEventCommandsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			// Detach old collection
			if (e.OldValue is EventToCommandCollection oldCollection)
			{
				oldCollection.Detach();
			}

			// Attach new collection
			if (e.NewValue is EventToCommandCollection newCollection)
			{
				newCollection.Attach(sender);
			}
		}

		private static void OnCommandChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (InputExtensions.IsEnterCommandSupportedFor(sender))
			{
				// for input controls, this will be implemented in InputExtensions.
				InputExtensions.OnEnterCommandChanged(sender, e);
			}
			else if (sender is ItemsRepeater ir)
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
			else if (sender is Selector s)
			{
				s.SelectionChanged -= OnSelectorSelectionChanged;
				if (GetCommand(sender) is { } command)
				{
					s.SelectionChanged += OnSelectorSelectionChanged;
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
			else if (sender is ToggleSwitch ts)
			{
				ts.Toggled -= OnToggleSwitchToggled;
				if (GetCommand(sender) is { } command)
				{
					ts.Toggled += OnToggleSwitchToggled;
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
		internal static object? TryGetItemCommandParameter(DependencyObject? container)
		{
			if (container is ListViewItem)
			{
				// fixme: it doesn't work here, because we came from ListView::ItemClick,
				// and when that happens the SelectedIndex not yet set.
				return null;
			}
			if (container is { })
			{
				// we can have two scenarios here: // where the CommandParameter property can be assigned to
				// 1. direct (IsItemItsOwnContainerOverride=true) container as items
				if (GetCommandParameter(container) is { } parameter1)
				{
					return parameter1;
				}

				// 2. root element of item-template
				if (container is ContentControl && // typically Selector's item-container are all of ContentControl descents: LVI, CBI, LBI...
					container.GetFirstDescendant<ContentPresenter>(IsTemplateBoundToContent) is { } presenter &&
					presenter.GetTemplateRoot() is { } root &&
					GetCommandParameter(root) is { } parameter2)
				{
					return parameter2;
				}

				bool IsTemplateBoundToContent(ContentPresenter presenter) =>
					presenter.GetBindingExpression(ContentPresenter.ContentProperty) is { ParentBinding.Path.Path: "Content" };
			}

			return null;
		}

		private static void OnListViewItemClick(object sender, ItemClickEventArgs e)
		{
			if (sender is not ListViewBase host) return;

			TryInvokeCommand(host, /*TryGetItemCommandParameter(host.ContainerFromIndex(host.SelectedIndex)) ??*/ e.ClickedItem);
		}
		private static void OnSelectorSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (sender is not Selector host) return;

			TryInvokeCommand(host, TryGetItemCommandParameter(host.ContainerFromIndex(host.SelectedIndex)) ?? host.SelectedItem);
		}
		private static void OnNavigationViewItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs e)
		{
			TryInvokeCommand(sender, TryGetItemCommandParameter(e.InvokedItemContainer) ?? e.InvokedItem);
		}
		private static void OnToggleSwitchToggled(object sender, RoutedEventArgs e)
		{
			if (sender is not ToggleSwitch host) return;

			TryInvokeCommand(host, host.IsOn);
		}
		private static void OnUIElementTapped(object sender, TappedRoutedEventArgs e)
		{
			if (sender is not UIElement host) return;

			TryInvokeCommand(host, GetCommandParameter(host) ?? host);
		}
	}
}
