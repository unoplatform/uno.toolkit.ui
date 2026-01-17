using System;
using System.Windows.Input;
using Uno.Toolkit.UI;
using Uno.UI.RuntimeTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
#endif

namespace Uno.Toolkit.RuntimeTests.Tests
{
	[TestClass]
	[RunsOnUIThread]
	public class EventToCommandTests
	{
		[TestMethod]
		public void EventToCommand_Event_Property_CanBeSetAndGet()
		{
			var eventToCommand = new EventToCommand();
			eventToCommand.Event = "Click";
			Assert.AreEqual("Click", eventToCommand.Event);
		}

		[TestMethod]
		public void EventToCommand_Command_Property_CanBeSetAndGet()
		{
			var eventToCommand = new EventToCommand();
			var command = new TestCommand();
			eventToCommand.Command = command;
			Assert.AreEqual(command, eventToCommand.Command);
		}

		[TestMethod]
		public void EventToCommand_CommandParameter_Property_CanBeSetAndGet()
		{
			var eventToCommand = new EventToCommand();
			var parameter = "test parameter";
			eventToCommand.CommandParameter = parameter;
			Assert.AreEqual(parameter, eventToCommand.CommandParameter);
		}

		[TestMethod]
		public void EventToCommand_PassEventArgsToCommand_Property_CanBeSetAndGet()
		{
			var eventToCommand = new EventToCommand();
			eventToCommand.PassEventArgsToCommand = true;
			Assert.IsTrue(eventToCommand.PassEventArgsToCommand);
		}

		[TestMethod]
		public void EventToCommand_EventArgsConverter_Property_CanBeSetAndGet()
		{
			var eventToCommand = new EventToCommand();
			var converter = new TestConverter();
			eventToCommand.EventArgsConverter = converter;
			Assert.AreEqual(converter, eventToCommand.EventArgsConverter);
		}

		[TestMethod]
		public void EventCommands_CanBeSetOnElement()
		{
			// Arrange
			var button = new Button();
			var collection = new EventToCommandCollection
			{
				new EventToCommand { Event = "Click", Command = new TestCommand() }
			};

			// Act
			CommandExtensions.SetEventCommands(button, collection);

			// Assert
			var retrievedCollection = CommandExtensions.GetEventCommands(button);
			Assert.IsNotNull(retrievedCollection);
			Assert.AreEqual(1, retrievedCollection.Count);
		}

		[TestMethod]
		public void EventCommands_MultipleEventsOnSameElement()
		{
			// Arrange
			var button = new Button();
			var clickCommand = new TestCommand();
			var tappedCommand = new TestCommand();

			var collection = new EventToCommandCollection
			{
				new EventToCommand { Event = "Click", Command = clickCommand },
				new EventToCommand { Event = "Tapped", Command = tappedCommand }
			};

			// Act
			CommandExtensions.SetEventCommands(button, collection);

			// Assert
			var retrievedCollection = CommandExtensions.GetEventCommands(button);
			Assert.IsNotNull(retrievedCollection);
			Assert.AreEqual(2, retrievedCollection.Count);
		}

		[TestMethod]
		public void EventCommands_CanBeCleared()
		{
			// Arrange
			var button = new Button();
			var collection = new EventToCommandCollection
			{
				new EventToCommand { Event = "Click", Command = new TestCommand() }
			};
			CommandExtensions.SetEventCommands(button, collection);

			// Act
			CommandExtensions.SetEventCommands(button, null);

			// Assert
			Assert.IsNull(CommandExtensions.GetEventCommands(button));
		}

		[TestMethod]
		public void EventCommands_WithInvalidEventName_DoesNotThrow()
		{
			// Arrange
			var button = new Button();
			var command = new TestCommand();
			var collection = new EventToCommandCollection
			{
				new EventToCommand { Event = "NonExistentEvent", Command = command }
			};

			// Act & Assert - Should not throw even with invalid event name
			CommandExtensions.SetEventCommands(button, collection);

			// Verify collection is set (event won't be subscribed but that's expected)
			var retrievedCollection = CommandExtensions.GetEventCommands(button);
			Assert.IsNotNull(retrievedCollection);
		}

		[TestMethod]
		public void EventCommands_AddingItemAfterAttach()
		{
			// Arrange
			var button = new Button();
			var collection = new EventToCommandCollection();
			CommandExtensions.SetEventCommands(button, collection);

			// Act - Add item after collection is attached
			var command = new TestCommand();
			collection.Add(new EventToCommand { Event = "Click", Command = command });

			// Assert
			Assert.AreEqual(1, collection.Count);
		}

		/// <summary>
		/// Test command implementation for unit testing.
		/// </summary>
		private class TestCommand : ICommand
		{
			public bool CanExecuteResult { get; set; } = true;
			public int ExecuteCount { get; private set; }
			public object? LastParameter { get; private set; }

			public event EventHandler? CanExecuteChanged;

			public bool CanExecute(object? parameter)
			{
				return CanExecuteResult;
			}

			public void Execute(object? parameter)
			{
				LastParameter = parameter;
				ExecuteCount++;
			}

			public void RaiseCanExecuteChanged()
			{
				CanExecuteChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Test value converter implementation for unit testing.
		/// </summary>
		private class TestConverter : IValueConverter
		{
			public object? ConvertResult { get; set; } = "converted";

			public object? Convert(object? value, Type targetType, object? parameter, string? language)
			{
				return ConvertResult;
			}

			public object? ConvertBack(object? value, Type targetType, object? parameter, string? language)
			{
				throw new NotImplementedException();
			}
		}
	}
}
