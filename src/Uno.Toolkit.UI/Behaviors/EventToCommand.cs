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
	/// Represents a mapping between an event and a command.
	/// When the specified event is raised on the associated element, the command is executed.
	/// </summary>
	public class EventToCommand : DependencyObject
	{
		private static readonly ILogger _logger = typeof(EventToCommand).Log();
		private DependencyObject? _associatedObject;
		private EventSubscription? _subscription;

		#region DependencyProperty: Event

		/// <summary>
		/// Identifies the Event dependency property.
		/// Specifies the name of the event to subscribe to.
		/// </summary>
		public static DependencyProperty EventProperty { [DynamicDependency(nameof(GetEvent))] get; } = DependencyProperty.Register(
			nameof(Event),
			typeof(string),
			typeof(EventToCommand),
			new PropertyMetadata(default(string), OnEventChanged));

		/// <summary>
		/// Gets or sets the name of the event to subscribe to.
		/// </summary>
		public string? Event
		{
			get => (string?)GetValue(EventProperty);
			set => SetValue(EventProperty, value);
		}

		[DynamicDependency(nameof(Event))]
		private static string? GetEvent(DependencyObject obj) => ((EventToCommand)obj).Event;

		#endregion

		#region DependencyProperty: Command

		/// <summary>
		/// Identifies the Command dependency property.
		/// Specifies the command to execute when the event is raised.
		/// </summary>
		public static DependencyProperty CommandProperty { [DynamicDependency(nameof(GetCommand))] get; } = DependencyProperty.Register(
			nameof(Command),
			typeof(ICommand),
			typeof(EventToCommand),
			new PropertyMetadata(default(ICommand), OnCommandChanged));

		/// <summary>
		/// Gets or sets the command to execute when the event is raised.
		/// </summary>
		public ICommand? Command
		{
			get => (ICommand?)GetValue(CommandProperty);
			set => SetValue(CommandProperty, value);
		}

		[DynamicDependency(nameof(Command))]
		private static ICommand? GetCommand(DependencyObject obj) => ((EventToCommand)obj).Command;

		#endregion

		#region DependencyProperty: CommandParameter

		/// <summary>
		/// Identifies the CommandParameter dependency property.
		/// Specifies an optional parameter to pass to the Command.
		/// </summary>
		public static DependencyProperty CommandParameterProperty { [DynamicDependency(nameof(GetCommandParameter))] get; } = DependencyProperty.Register(
			nameof(CommandParameter),
			typeof(object),
			typeof(EventToCommand),
			new PropertyMetadata(default(object)));

		/// <summary>
		/// Gets or sets the optional parameter to pass to the Command.
		/// </summary>
		public object? CommandParameter
		{
			get => GetValue(CommandParameterProperty);
			set => SetValue(CommandParameterProperty, value);
		}

		[DynamicDependency(nameof(CommandParameter))]
		private static object? GetCommandParameter(DependencyObject obj) => ((EventToCommand)obj).CommandParameter;

		#endregion

		#region DependencyProperty: EventArgsConverter

		/// <summary>
		/// Identifies the EventArgsConverter dependency property.
		/// Specifies an optional converter to transform the event arguments before passing them to the Command.
		/// </summary>
		public static DependencyProperty EventArgsConverterProperty { [DynamicDependency(nameof(GetEventArgsConverter))] get; } = DependencyProperty.Register(
			nameof(EventArgsConverter),
			typeof(IValueConverter),
			typeof(EventToCommand),
			new PropertyMetadata(default(IValueConverter)));

		/// <summary>
		/// Gets or sets an optional converter to transform the event arguments.
		/// </summary>
		public IValueConverter? EventArgsConverter
		{
			get => (IValueConverter?)GetValue(EventArgsConverterProperty);
			set => SetValue(EventArgsConverterProperty, value);
		}

		[DynamicDependency(nameof(EventArgsConverter))]
		private static IValueConverter? GetEventArgsConverter(DependencyObject obj) => ((EventToCommand)obj).EventArgsConverter;

		#endregion

		#region DependencyProperty: PassEventArgsToCommand

		/// <summary>
		/// Identifies the PassEventArgsToCommand dependency property.
		/// When set to true, the event arguments are passed to the Command as the parameter.
		/// </summary>
		public static DependencyProperty PassEventArgsToCommandProperty { [DynamicDependency(nameof(GetPassEventArgsToCommand))] get; } = DependencyProperty.Register(
			nameof(PassEventArgsToCommand),
			typeof(bool),
			typeof(EventToCommand),
			new PropertyMetadata(false));

		/// <summary>
		/// Gets or sets whether the event arguments should be passed to the Command.
		/// </summary>
		public bool PassEventArgsToCommand
		{
			get => (bool)GetValue(PassEventArgsToCommandProperty);
			set => SetValue(PassEventArgsToCommandProperty, value);
		}

		[DynamicDependency(nameof(PassEventArgsToCommand))]
		private static bool GetPassEventArgsToCommand(DependencyObject obj) => ((EventToCommand)obj).PassEventArgsToCommand;

		#endregion

		private static void OnEventChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is EventToCommand etc)
			{
				etc.UpdateSubscription();
			}
		}

		private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is EventToCommand etc)
			{
				etc.UpdateSubscription();
			}
		}

		/// <summary>
		/// Associates this EventToCommand with a target element.
		/// </summary>
		internal void Attach(DependencyObject associatedObject)
		{
			if (_associatedObject == associatedObject)
			{
				return;
			}

			Detach();
			_associatedObject = associatedObject;
			UpdateSubscription();
		}

		/// <summary>
		/// Detaches this EventToCommand from its associated element.
		/// </summary>
		internal void Detach()
		{
			_subscription?.Unsubscribe();
			_subscription = null;
			_associatedObject = null;
		}

		private void UpdateSubscription()
		{
			// Unsubscribe from previous event
			_subscription?.Unsubscribe();
			_subscription = null;

			// If not attached or missing required properties, don't subscribe
			if (_associatedObject is null)
			{
				return;
			}

			var eventName = Event;
			var command = Command;

			if (string.IsNullOrEmpty(eventName) || command is null)
			{
				return;
			}

			// Get the event from the object's type
			var eventInfo = _associatedObject.GetType().GetEvent(eventName, BindingFlags.Public | BindingFlags.Instance);
			if (eventInfo is null)
			{
				if (_logger.IsEnabled(LogLevel.Warning))
				{
					_logger.Warn($"Event '{eventName}' not found on type '{_associatedObject.GetType().FullName}'.");
				}
				return;
			}

			// Create and store the subscription
			_subscription = new EventSubscription(_associatedObject, eventInfo, OnEventRaised);
		}

		private void OnEventRaised(DependencyObject sender, object? eventArgs)
		{
			var command = Command;
			if (command is null)
			{
				return;
			}

			// Determine the command parameter
			// Priority: CommandParameter > EventArgs (if PassEventArgsToCommand is true)
			object? parameter;

			var commandParameter = CommandParameter;
			if (commandParameter is not null)
			{
				// Explicit CommandParameter takes precedence
				parameter = commandParameter;
			}
			else if (PassEventArgsToCommand)
			{
				// Use event args as parameter
				parameter = eventArgs;

				// Apply converter if specified
				var converter = EventArgsConverter;
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
				_target = target ?? throw new ArgumentNullException(nameof(target));
				_eventInfo = eventInfo ?? throw new ArgumentNullException(nameof(eventInfo));
				_callback = callback ?? throw new ArgumentNullException(nameof(callback));

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
					throw new InvalidOperationException($"Unsupported event signature with {parameters.Length} parameters. Only standard .NET event patterns with 2 parameters (object sender, TEventArgs e) are supported.");
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
