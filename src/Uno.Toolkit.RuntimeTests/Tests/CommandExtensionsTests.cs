using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.UI;
using Uno.UI.RuntimeTests;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Automation.Provider;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.RuntimeTests.Tests;

[TestClass]
[RunsOnUIThread]
public class CommandExtensionsTests
{
	[TestMethod]
	public async Task ListView_ItemClick_Without_CommandParameter_Uses_ClickedItem()
	{
		var command = new RecordingCommand();
		var SUT = new ListView
		{
			ItemsSource = new[] { "a", "b", "c" },
			IsItemClickEnabled = true,
		};
		CommandExtensions.SetCommand(SUT, command);

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);
		InvokeItem(SUT, 1);

		CollectionAssert.AreEqual(new object[] { "b" }, command.Parameters);
	}

	[TestMethod]
	[DataRow(ListViewSelectionMode.None)]
	[DataRow(ListViewSelectionMode.Single)]
	public async Task ListView_ItemClick_With_CommandParameter_On_ItemTemplateRoot(ListViewSelectionMode selectionMode)
	{
		var command = new RecordingCommand();
		var SUT = new ListView
		{
			ItemsSource = new[] { new TestItem(1, "Item 1"), new TestItem(2, "Item 2"), new TestItem(3, "Item 3") },
			IsItemClickEnabled = true,
			SelectionMode = selectionMode,
			ItemTemplate = XamlHelper.LoadXaml<DataTemplate>("""
				<DataTemplate>
					<TextBlock Text="{Binding Name}" utu:CommandExtensions.CommandParameter="{Binding Name}" />
				</DataTemplate>
				"""),
		};
		CommandExtensions.SetCommand(SUT, command);

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);
		InvokeItem(SUT, 1);
		InvokeItem(SUT, 2);

		// the CommandParameter on the template root takes precedence over the clicked item
		CollectionAssert.AreEqual(new object[] { "Item 2", "Item 3" }, command.Parameters);
	}

	[TestMethod]
	public async Task ListView_ItemClick_With_CommandParameter_On_ItemContainer()
	{
		var command = new RecordingCommand();
		var SUT = new ListView { IsItemClickEnabled = true };
		for (int i = 0; i < 3; i++)
		{
			var item = new ListViewItem { Content = $"Item {i}" };
			CommandExtensions.SetCommandParameter(item, i);
			SUT.Items.Add(item);
		}
		CommandExtensions.SetCommand(SUT, command);

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);
		InvokeItem(SUT, 2);

		CollectionAssert.AreEqual(new object[] { 2 }, command.Parameters);
	}

	private static void InvokeItem(ListViewBase lvb, int index)
	{
		var container = lvb.ContainerFromIndex(index) as ListViewItem ?? throw new AssertFailedException($"Container at index {index} not found.");
		var peer = FrameworkElementAutomationPeer.CreatePeerForElement(container);
		var invoker = peer?.GetPattern(PatternInterface.Invoke) as IInvokeProvider ?? throw new AssertFailedException("ListViewItem should support the Invoke pattern.");

		invoker.Invoke();
	}

	public sealed record TestItem(int Id, string Name);

	private sealed class RecordingCommand : ICommand
	{
#pragma warning disable CS0067 // never raised: the command is always executable
		public event EventHandler? CanExecuteChanged;
#pragma warning restore CS0067

		public List<object?> Parameters { get; } = new();

		public bool CanExecute(object? parameter) => true;

		public void Execute(object? parameter) => Parameters.Add(parameter);
	}
}
