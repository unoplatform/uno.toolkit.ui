using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using Uno.Extensions;
using Uno.Logging;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
#endif

namespace Uno.Toolkit.UI
{
	/// <summary>
	/// Provides attached properties to bind events to commands in XAML.
	/// This allows you to invoke a command when an event is raised, similar to .NET MAUI's EventToCommandBehavior.
	/// </summary>
	public static class EventToCommandExtensions
	{
		private static readonly ILogger _logger = typeof(EventToCommandExtensions).Log();

		#region DependencyProperty: Event

		/// <summary>
		/// Identifies the Event attached property.
		/// Specifies the name of the event to subscribe to.
		/// </summary>
		public static DependencyProperty EventProperty { [DynamicDependency(nameof(GetEvent))] get; } = DependencyProperty.RegisterAttached(
			"Event",
			typeof(string),
			typeof(EventToCommandExtensions),
			new PropertyMetadata(default(string), OnEventChanged));

		[DynamicDependency(nameof(SetEvent))]
		public static string? GetEvent(DependencyObject obj) => (string?)obj.GetValue(EventProperty);

		[DynamicDependency(nameof(GetEvent))]
		public static void SetEvent(DependencyObject obj, string? value) => obj.SetValue(EventProperty, value);

		#endregion

		#region DependencyProperty: Command

		/// <summary>
		/// Identifies the Command attached property.
		/// Specifies the command to execute when the event is raised.
		/// </summary>
		public static DependencyProperty CommandProperty { [DynamicDependency(nameof(GetCommand))] get; } = DependencyProperty.RegisterAttached(
			"Command",
			typeof(ICommand),
			typeof(EventToCommandExtensions),
			new PropertyMetadata(default(ICommand), OnCommandChanged));

		[DynamicDependency(nameof(SetCommand))]
		public static ICommand? GetCommand(DependencyObject obj) => (ICommand?)obj.GetValue(CommandProperty);

		[DynamicDependency(nameof(GetCommand))]
		public static void SetCommand(DependencyObject obj, ICommand? value) => obj.SetValue(CommandProperty, value);

		#endregion

		#region DependencyProperty: CommandParameter

		/// <summary>
		/// Identifies the CommandParameter attached property.
		/// Specifies an optional parameter to pass to the Command.
		/// </summary>
		public static DependencyProperty CommandParameterProperty { [DynamicDependency(nameof(GetCommandParameter))] get; } = DependencyProperty.RegisterAttached(
			"CommandParameter",
			typeof(object),
			typeof(EventToCommandExtensions),
			new PropertyMetadata(default(object)));

		[DynamicDependency(nameof(SetCommandParameter))]
		public static object? GetCommandParameter(DependencyObject obj) => obj.GetValue(CommandParameterProperty);

		[DynamicDependency(nameof(GetCommandParameter))]
		public static void SetCommandParameter(DependencyObject obj, object? value) => obj.SetValue(CommandParameterProperty, value);

		#endregion

		#region DependencyProperty: EventArgsConverter

		/// <summary>
		/// Identifies the EventArgsConverter attached property.
		/// Specifies an optional converter to transform the event arguments before passing them to the Command.
		/// </summary>
		public static DependencyProperty EventArgsConverterProperty { [DynamicDependency(nameof(GetEventArgsConverter))] get; } = DependencyProperty.RegisterAttached(
			"EventArgsConverter",
			typeof(IValueConverter),
			typeof(EventToCommandExtensions),
			new PropertyMetadata(default(IValueConverter)));

		[DynamicDependency(nameof(SetEventArgsConverter))]
		public static IValueConverter? GetEventArgsConverter(DependencyObject obj) => (IValueConverter?)obj.GetValue(EventArgsConverterProperty);

		[DynamicDependency(nameof(GetEventArgsConverter))]
		public static void SetEventArgsConverter(DependencyObject obj, IValueConverter? value) => obj.SetValue(EventArgsConverterProperty, value);

		#endregion

		#region DependencyProperty: PassEventArgsToCommand

		/// <summary>
		/// Identifies the PassEventArgsToCommand attached property.
		/// When set to true, the event arguments are passed to the Command as the parameter.
		/// If EventArgsConverter is also set, the converter is applied to the event arguments first.
		/// </summary>
		public static DependencyProperty PassEventArgsToCommandProperty { [DynamicDependency(nameof(GetPassEventArgsToCommand))] get; } = DependencyProperty.RegisterAttached(
			"PassEventArgsToCommand",
			typeof(bool),
			typeof(EventToCommandExtensions),
			new PropertyMetadata(false));

		[DynamicDependency(nameof(SetPassEventArgsToCommand))]
		public static bool GetPassEventArgsToCommand(DependencyObject obj) => (bool)obj.GetValue(PassEventArgsToCommandProperty);

		[DynamicDependency(nameof(GetPassEventArgsToCommand))]
		public static void SetPassEventArgsToCommand(DependencyObject obj, bool value) => obj.SetValue(PassEventArgsToCommandProperty, value);

		#endregion

		#region DependencyProperty: EventHandler (private, used for cleanup)

		private static DependencyProperty EventHandlerProperty { get; } = DependencyProperty.RegisterAttached(
			"EventHandler",
			typeof(EventSubscription),
			typeof(EventToCommandExtensions),
			new PropertyMetadata(default(EventSubscription)));

		private static EventSubscription? GetEventHandler(DependencyObject obj) => (EventSubscription?)obj.GetValue(EventHandlerProperty);
		private static void SetEventHandler(DependencyObject obj, EventSubscription? value) => obj.SetValue(EventHandlerProperty, value);

		#endregion

		private static void OnEventChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			UpdateEventSubscription(sender);
		}

		private static void OnCommandChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			UpdateEventSubscription(sender);
		}

		private static void UpdateEventSubscription(DependencyObject sender)
		{
			// Unsubscribe from previous event
			var existingHandler = GetEventHandler(sender);
			existingHandler?.Unsubscribe();
			SetEventHandler(sender, null);

			// Get current values
			var eventName = GetEvent(sender);
			var command = GetCommand(sender);

			// If either event name or command is not set, don't subscribe
			if (string.IsNullOrEmpty(eventName) || command is null)
			{
				return;
			}

			// Get the event from the object's type
			var eventInfo = sender.GetType().GetEvent(eventName, BindingFlags.Public | BindingFlags.Instance);
			if (eventInfo is null)
			{
				if (_logger.IsEnabled(LogLevel.Warning))
				{
					_logger.Warn($"Event '{eventName}' not found on type '{sender.GetType().FullName}'.");
				}
				return;
			}

			// Create and store the subscription
			var subscription = new EventSubscription(sender, eventInfo, OnEventRaised);
			SetEventHandler(sender, subscription);
		}

		private static void OnEventRaised(DependencyObject sender, object? eventArgs)
		{
			var command = GetCommand(sender);
			if (command is null)
			{
				return;
			}

			// Determine the command parameter
			// Priority: CommandParameter > EventArgs (if PassEventArgsToCommand is true)
			object? parameter;

			var commandParameter = GetCommandParameter(sender);
			if (commandParameter is not null)
			{
				// Explicit CommandParameter takes precedence
				parameter = commandParameter;
			}
			else if (GetPassEventArgsToCommand(sender))
			{
				// Use event args as parameter
				parameter = eventArgs;

				// Apply converter if specified
				var converter = GetEventArgsConverter(sender);
				if (converter is not null)
				{
					parameter = converter.Convert(eventArgs, typeof(object), null, null);
				}
			}
			else
			{
				// No parameter specified
				parameter = null;
			}

			// Execute the command if possible
			if (command.CanExecute(parameter))
			{
				command.Execute(parameter);
			}
		}

		/// <summary>
		/// Represents a subscription to an event that can be unsubscribed.
		/// </summary>
		private sealed class EventSubscription
		{
			private readonly DependencyObject _target;
			private readonly EventInfo _eventInfo;
			private readonly Delegate _handler;
			private readonly Action<DependencyObject, object?> _callback;

			public EventSubscription(DependencyObject target, EventInfo eventInfo, Action<DependencyObject, object?> callback)
			{
				_target = target;
				_eventInfo = eventInfo;
				_callback = callback;

				// Create a delegate that matches the event's handler type
				var handlerType = eventInfo.EventHandlerType;
				if (handlerType is null)
				{
					throw new InvalidOperationException($"Cannot determine handler type for event '{eventInfo.Name}'.");
				}

				// Create handler that invokes our callback
				_handler = CreateHandler(handlerType);

				// Subscribe to the event
				eventInfo.AddEventHandler(target, _handler);
			}

			private Delegate CreateHandler(Type handlerType)
			{
				// Get the method signature from the handler type
				var invokeMethod = handlerType.GetMethod("Invoke");
				if (invokeMethod is null)
				{
					throw new InvalidOperationException($"Cannot determine Invoke method for handler type '{handlerType.FullName}'.");
				}

				var parameters = invokeMethod.GetParameters();

				// For standard event patterns (sender, EventArgs), create appropriate handler
				if (parameters.Length == 2)
				{
					// Create a strongly-typed handler using the OnEvent method
					var onEventMethod = typeof(EventSubscription).GetMethod(nameof(OnEvent), BindingFlags.NonPublic | BindingFlags.Instance);
					if (onEventMethod is null)
					{
						throw new InvalidOperationException("Cannot find OnEvent method.");
					}

					// Create a delegate for the generic event handler
					return Delegate.CreateDelegate(handlerType, this, onEventMethod);
				}
				else
				{
					throw new InvalidOperationException($"Unsupported event signature with {parameters.Length} parameters.");
				}
			}

			// This method is called via reflection/delegate for any event with (object sender, TEventArgs e) signature
			private void OnEvent(object? sender, object? e)
			{
				_callback.Invoke(_target, e);
			}

			public void Unsubscribe()
			{
				_eventInfo.RemoveEventHandler(_target, _handler);
			}
		}
	}
}
