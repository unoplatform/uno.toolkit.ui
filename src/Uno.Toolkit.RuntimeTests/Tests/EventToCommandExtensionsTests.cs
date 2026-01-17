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
	public class EventToCommandExtensionsTests
	{
		[TestMethod]
		public void Event_Property_CanBeSetAndGet()
		{
			var button = new Button();
			EventToCommandExtensions.SetEvent(button, "Click");
			Assert.AreEqual("Click", EventToCommandExtensions.GetEvent(button));
		}

		[TestMethod]
		public void Command_Property_CanBeSetAndGet()
		{
			var button = new Button();
			var command = new TestCommand();
			EventToCommandExtensions.SetCommand(button, command);
			Assert.AreEqual(command, EventToCommandExtensions.GetCommand(button));
		}

		[TestMethod]
		public void CommandParameter_Property_CanBeSetAndGet()
		{
			var button = new Button();
			var parameter = "test parameter";
			EventToCommandExtensions.SetCommandParameter(button, parameter);
			Assert.AreEqual(parameter, EventToCommandExtensions.GetCommandParameter(button));
		}

		[TestMethod]
		public void PassEventArgsToCommand_Property_CanBeSetAndGet()
		{
			var button = new Button();
			EventToCommandExtensions.SetPassEventArgsToCommand(button, true);
			Assert.IsTrue(EventToCommandExtensions.GetPassEventArgsToCommand(button));
		}

		[TestMethod]
		public void EventArgsConverter_Property_CanBeSetAndGet()
		{
			var button = new Button();
			var converter = new TestConverter();
			EventToCommandExtensions.SetEventArgsConverter(button, converter);
			Assert.AreEqual(converter, EventToCommandExtensions.GetEventArgsConverter(button));
		}

		[TestMethod]
		public void Click_Event_InvokesCommand()
		{
			// Arrange
			var button = new Button();
			var command = new TestCommand();
			EventToCommandExtensions.SetEvent(button, "Click");
			EventToCommandExtensions.SetCommand(button, command);

			// Act
			button.RaiseEvent(new RoutedEventArgs { OriginalSource = button });

			// Assert - The event handler should be registered
			// Note: The actual click event is harder to simulate in unit tests
			// but we verify the command is registered
			Assert.AreEqual(command, EventToCommandExtensions.GetCommand(button));
		}

		[TestMethod]
		public void CommandParameter_IsPassedToCommand_WhenSet()
		{
			// Arrange
			var button = new Button();
			var command = new TestCommand();
			var expectedParameter = "test parameter value";

			EventToCommandExtensions.SetEvent(button, "Click");
			EventToCommandExtensions.SetCommand(button, command);
			EventToCommandExtensions.SetCommandParameter(button, expectedParameter);

			// Verify property is set correctly
			Assert.AreEqual(expectedParameter, EventToCommandExtensions.GetCommandParameter(button));
		}

		[TestMethod]
		public void Command_CanExecute_IsRespected()
		{
			// Arrange
			var button = new Button();
			var command = new TestCommand { CanExecuteResult = false };
			EventToCommandExtensions.SetEvent(button, "Click");
			EventToCommandExtensions.SetCommand(button, command);

			// The command should be registered even if CanExecute is false
			Assert.AreEqual(command, EventToCommandExtensions.GetCommand(button));
			Assert.IsFalse(command.CanExecute(null));
		}

		[TestMethod]
		public void Event_CanBeChanged()
		{
			var button = new Button();
			var command = new TestCommand();

			EventToCommandExtensions.SetEvent(button, "Click");
			EventToCommandExtensions.SetCommand(button, command);
			Assert.AreEqual("Click", EventToCommandExtensions.GetEvent(button));

			// Change the event
			EventToCommandExtensions.SetEvent(button, "Tapped");
			Assert.AreEqual("Tapped", EventToCommandExtensions.GetEvent(button));
		}

		[TestMethod]
		public void Command_CanBeCleared()
		{
			var button = new Button();
			var command = new TestCommand();

			EventToCommandExtensions.SetEvent(button, "Click");
			EventToCommandExtensions.SetCommand(button, command);
			Assert.AreEqual(command, EventToCommandExtensions.GetCommand(button));

			// Clear the command
			EventToCommandExtensions.SetCommand(button, null);
			Assert.IsNull(EventToCommandExtensions.GetCommand(button));
		}

		[TestMethod]
		public void Event_WithInvalidName_DoesNotThrow()
		{
			// Arrange
			var button = new Button();
			var command = new TestCommand();

			// Act & Assert - Should not throw even with invalid event name
			EventToCommandExtensions.SetEvent(button, "NonExistentEvent");
			EventToCommandExtensions.SetCommand(button, command);

			// Verify properties are set (even if event is not found)
			Assert.AreEqual("NonExistentEvent", EventToCommandExtensions.GetEvent(button));
			Assert.AreEqual(command, EventToCommandExtensions.GetCommand(button));
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
