using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Toolkit.RuntimeTests.Extensions;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.UI;
using Uno.UI.RuntimeTests;
using ChipControl = Uno.Toolkit.UI.Chip; // ios/macos: to avoid collision with `global::Chip` namespace...

#if IS_WINUI
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Automation.Provider;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.RuntimeTests.Tests;

[TestClass]
[RunsOnUIThread]
internal class ChipGroupTests
{
	/*	Test Plan
	 *		- tap triggered selection in various SelectionMode
	 *		- SelectedItem(s) in various SelectionMode
	 *		- pre-assertions & post-assertions when changing SelectionMode
	 */

	#region Selection via Toggle

	[TestMethod]
#if __ANDROID__
	[Ignore("Skip Android due to failures. Tracked by https://github.com/unoplatform/uno.toolkit.ui/issues/1300")]
#endif
	[DataRow(ChipSelectionMode.None, new[] { 1 }, null)]
	[DataRow(ChipSelectionMode.SingleOrNone, new[] { 1 }, 1)]
	[DataRow(ChipSelectionMode.SingleOrNone, new[] { 1, 1 }, null)] // deselection
	[DataRow(ChipSelectionMode.SingleOrNone, new[] { 1, 2 }, 2)] // reselection
	[DataRow(ChipSelectionMode.Single, new int[0], 0)] // selection enforced by 'Single'
	[DataRow(ChipSelectionMode.Single, new[] { 1 }, 1)]
	[DataRow(ChipSelectionMode.Single, new[] { 1, 1 }, 1)] // deselection denied
	[DataRow(ChipSelectionMode.Single, new[] { 1, 2 }, 2)] // reselection
	[DataRow(ChipSelectionMode.Multiple, new[] { 1 }, new object[] { 1 })]
	[DataRow(ChipSelectionMode.Multiple, new[] { 1, 2 }, new object[] { 1, 2 })] // multi-select@1,2
	[DataRow(ChipSelectionMode.Multiple, new[] { 1, 2, 2 }, new object[] { 1 })] // multi-select@1,2, deselection@2
	public async Task VariousMode_TapSelection(ChipSelectionMode mode, int[] selectionSequence, object expectation)
	{
		var source = Enumerable.Range(0, 3).ToArray();
		var SUT = new ChipGroup
		{
			SelectionMode = mode,
			ItemsSource = source,
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);
		Assert.AreEqual(mode is ChipSelectionMode.Single ? source[0] : null, SUT.SelectedItem);
		Assert.IsNull(SUT.SelectedItems);

		foreach (var i in selectionSequence)
		{
			((ChipControl)SUT.ContainerFromIndex(i)).Toggle();
		}
		if (mode is ChipSelectionMode.SingleOrNone or ChipSelectionMode.Single)
		{
			Assert.AreEqual((int?)expectation, SUT.SelectedItem);
			Assert.IsNull(SUT.SelectedItems);
		}
		else
		{
			Assert.IsNull(SUT.SelectedItem);
			CollectionAssert.AreEqual((object[]?)expectation, SUT.SelectedItems);
		}
	}

	[TestMethod]
	public async Task SingleMode_Selection()
	{
		var source = Enumerable.Range(0, 3).ToArray();
		var SUT = new ChipGroup
		{
			SelectionMode = ChipSelectionMode.SingleOrNone,
			ItemsSource = source,
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);
		Assert.IsNull(SUT.SelectedItem);

		((ChipControl)SUT.ContainerFromIndex(1)).Toggle();
		Assert.AreEqual(source[1], SUT.SelectedItem);
	}

#endregion

	#region Selection via SelectedItem & SelectItems

	[TestMethod]
	public async Task None_SetSelection()
	{
		var source = Enumerable.Range(0, 3).ToArray();
		var SUT = new ChipGroup
		{
			SelectionMode = ChipSelectionMode.None,
			ItemsSource = source,
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);
		Assert.IsNull(SUT.SelectedItem);
		Assert.IsNull(SUT.SelectedItems);

		// invalid assignment for current selection mode will reset the selection (clear, and then coerced)
		SUT.SelectedItem = source[1];
		Assert.IsNull(SUT.SelectedItem);
		Assert.IsNull(SUT.SelectedItems);

		// invalid assignment for current selection mode will reset the selection (clear, and then coerced)
		SUT.SelectedItems = source.Take(2).ToArray();
		Assert.IsNull(SUT.SelectedItem);
		Assert.IsNull(SUT.SelectedItems);
	}

	[TestMethod]
#if __ANDROID__
	[Ignore("Skip Android due to failures. Tracked by https://github.com/unoplatform/uno.toolkit.ui/issues/1300")]
#endif
	public async Task Single_SetSelection()
	{
		var source = Enumerable.Range(0, 3).ToArray();
		var SUT = new ChipGroup
		{
			SelectionMode = ChipSelectionMode.Single,
			ItemsSource = source,
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);
		Assert.AreEqual(source[0], SUT.SelectedItem);
		Assert.IsNull(SUT.SelectedItems);

		SUT.SelectedItem = source[1];
		Assert.AreEqual(source[1], SUT.SelectedItem);
		Assert.IsNull(SUT.SelectedItems);

		// invalid assignment for current selection mode will reset the selection (clear, and then coerced)
		SUT.SelectedItems = source.Skip(1).Take(2).ToArray();
		Assert.AreEqual(source[0], SUT.SelectedItem); // coerced from Single
		Assert.IsNull(SUT.SelectedItems);
	}

	[TestMethod]
	public async Task SingleOrNone_SetSelection()
	{
		var source = Enumerable.Range(0, 3).ToArray();
		var SUT = new ChipGroup
		{
			SelectionMode = ChipSelectionMode.SingleOrNone,
			ItemsSource = source,
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);
		Assert.IsNull(SUT.SelectedItem);
		Assert.IsNull(SUT.SelectedItems);

		SUT.SelectedItem = source[1];
		Assert.AreEqual(source[1], SUT.SelectedItem);
		Assert.IsNull(SUT.SelectedItems);

		// invalid assignment for current selection mode will reset the selection (clear, and then coerced)
		SUT.SelectedItems = source.Take(2).ToArray();
		Assert.IsNull(SUT.SelectedItem);
		Assert.IsNull(SUT.SelectedItems);
	}

	[TestMethod]
	public async Task Multiple_SetSelection()
	{
		var source = Enumerable.Range(0, 3).ToArray();
		var SUT = new ChipGroup
		{
			SelectionMode = ChipSelectionMode.Multiple,
			ItemsSource = source,
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);
		Assert.IsNull(SUT.SelectedItem);
		Assert.IsNull(SUT.SelectedItems);

		// invalid assignment for current selection mode will reset the selection (clear, and then coerced)
		SUT.SelectedItem = source[1];
		Assert.IsNull(SUT.SelectedItem);
		Assert.IsNull(SUT.SelectedItems);

		var newSelection = source.Skip(1).Take(2).ToArray();
		SUT.SelectedItems = newSelection;
		Assert.IsNull(SUT.SelectedItem);
		CollectionAssert.AreEqual(newSelection, SUT.SelectedItems);
	}

	[TestMethod]
	public async Task Multiple_ReassignSelectedItems_ShouldNotStack()
	{
		var source = Enumerable.Range(0, 3).ToArray();
		var SUT = new ChipGroup
		{
			SelectionMode = ChipSelectionMode.Multiple,
			ItemsSource = source,
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);
		Assert.IsNull(SUT.SelectedItem);
		Assert.IsNull(SUT.SelectedItems);

		// Changing SelectedItems should not create union of old & new values
		foreach (var selection in Enumerable.Range(0, 1).Select(x => source.Skip(x).Take(2).ToArray()))
		{
			SUT.SelectedItems = selection;
			Assert.IsNull(SUT.SelectedItem);
			CollectionAssert.AreEqual(selection, SUT.SelectedItems);
		}
	}
	#endregion

	#region Changing SelectionMode

	[TestMethod]
	public async Task SingleOrNoneToSingle_NoneSelected_ShouldAutoSelect()
	{
		var source = Enumerable.Range(0, 3).ToArray();
		var SUT = new ChipGroup
		{
			SelectionMode = ChipSelectionMode.SingleOrNone,
			ItemsSource = source,
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);
		Assert.IsNull(SUT.SelectedItem);

		SUT.SelectionMode = ChipSelectionMode.Single;
		Assert.AreEqual(source[0], SUT.SelectedItem);
	}

	[TestMethod]
	public async Task SingleOrNoneToSingle_Selected_ShouldPreserveSelection()
	{
		var source = Enumerable.Range(0, 3).ToArray();
		var selected = source.Last();
		var SUT = new ChipGroup
		{
			SelectionMode = ChipSelectionMode.SingleOrNone,
			ItemsSource = source,
			SelectedItem = selected,
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);
		Assert.AreEqual(selected, SUT.SelectedItem);

		SUT.SelectionMode = ChipSelectionMode.Single;
		Assert.AreEqual(selected, SUT.SelectedItem);
	}

	[TestMethod]
	public async Task MultiToSingle_Selected_ShouldPreserveFirstSelection()
	{
		var source = Enumerable.Range(0, 3).ToArray();
		var selected = source.Skip(1).Take(2).ToArray();
		var SUT = new ChipGroup
		{
			SelectionMode = ChipSelectionMode.Multiple,
			ItemsSource = source,
			SelectedItems = selected,
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);
		CollectionAssert.AreEqual(selected, SUT.SelectedItems);

		SUT.SelectionMode = ChipSelectionMode.Single;
		Assert.AreEqual(selected.First(), SUT.SelectedItem);
	}

	#endregion

	#region Automation

	[TestMethod]
	[DataRow(ChipSelectionMode.None, false, false, false)]
	[DataRow(ChipSelectionMode.SingleOrNone, true, false, false)]
	[DataRow(ChipSelectionMode.Single, true, false, true)]
	[DataRow(ChipSelectionMode.Multiple, true, true, false)]
	public async Task AutomationPeer_SelectionPattern_MatchesSelectionMode(
		ChipSelectionMode mode,
		bool supportsSelection,
		bool canSelectMultiple,
		bool isSelectionRequired)
	{
		var SUT = new ChipGroup
		{
			SelectionMode = mode,
			ItemsSource = new[] { "One", "Two" },
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

		var peer = FrameworkElementAutomationPeer.CreatePeerForElement(SUT) as ChipGroupAutomationPeer;
		Assert.IsNotNull(peer, "ChipGroup should expose a ChipGroupAutomationPeer.");
		Assert.AreEqual(AutomationControlType.List, peer!.GetAutomationControlType());

		var selectionProvider = peer.GetPattern(PatternInterface.Selection) as ISelectionProvider;
		if (!supportsSelection)
		{
			Assert.IsNull(selectionProvider, "Selection mode None should not expose the Selection pattern.");
			return;
		}

		Assert.IsNotNull(selectionProvider);
		Assert.AreEqual(canSelectMultiple, selectionProvider!.CanSelectMultiple);
		Assert.AreEqual(isSelectionRequired, selectionProvider.IsSelectionRequired);
		Assert.AreEqual(mode == ChipSelectionMode.Single ? 1 : 0, selectionProvider.GetSelection().Length);
	}

	[TestMethod]
	[DataRow(ChipSelectionMode.None, false)]
	[DataRow(ChipSelectionMode.SingleOrNone, true)]
	[DataRow(ChipSelectionMode.Single, true)]
	[DataRow(ChipSelectionMode.Multiple, true)]
	public async Task ChipAutomationPeer_PatternsAndState_AreModeAware(
		ChipSelectionMode mode,
		bool supportsSelectionItem)
	{
		var SUT = new ChipGroup
		{
			SelectionMode = mode,
			ItemsSource = new[] { "One", "Two" },
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

		var chip = (ChipControl)SUT.ContainerFromIndex(0);
		var peer = FrameworkElementAutomationPeer.CreatePeerForElement(chip) as ChipAutomationPeer;
		Assert.IsNotNull(peer, "Chip should expose a ChipAutomationPeer.");
		Assert.IsNull(peer!.GetPattern(PatternInterface.Toggle), "A ChipGroup item should not expose Toggle.");

		var invokeProvider = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
		var selectionItem = peer.GetPattern(PatternInterface.SelectionItem) as ISelectionItemProvider;
		if (!supportsSelectionItem)
		{
			Assert.IsNotNull(invokeProvider, "Selection mode None should expose Invoke.");
			Assert.IsNull(selectionItem, "Selection mode None should not expose SelectionItem.");
			Assert.AreEqual(AutomationControlType.Button, peer.GetAutomationControlType());

			var clickCount = 0;
			chip.Click += (_, _) => clickCount++;
			invokeProvider!.Invoke();
			Assert.AreEqual(false, chip.IsChecked);
			Assert.AreEqual(1, clickCount);
			return;
		}

		Assert.IsNull(invokeProvider, "Selectable ChipGroup items should expose SelectionItem instead of Invoke.");
		Assert.IsNotNull(selectionItem);
		Assert.AreEqual(AutomationControlType.ListItem, peer.GetAutomationControlType());
		Assert.AreEqual(mode == ChipSelectionMode.Single, selectionItem!.IsSelected);
		Assert.IsNotNull(selectionItem.SelectionContainer);
	}

	[TestMethod]
	public async Task StandaloneChipAutomationPeer_PreservesTogglePattern()
	{
		var SUT = new ChipControl { Content = "Standalone" };
		var clickCount = 0;
		SUT.Click += (_, _) => clickCount++;

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

		var peer = FrameworkElementAutomationPeer.CreatePeerForElement(SUT) as ChipAutomationPeer;
		Assert.IsNotNull(peer);
		Assert.AreEqual(AutomationControlType.Button, peer!.GetAutomationControlType());
		Assert.IsNull(peer.GetPattern(PatternInterface.SelectionItem));

		var toggleProvider = peer.GetPattern(PatternInterface.Toggle) as IToggleProvider;
		Assert.IsNotNull(toggleProvider);
		Assert.AreEqual(ToggleState.Off, toggleProvider!.ToggleState);

		toggleProvider.Toggle();

		Assert.AreEqual(ToggleState.On, toggleProvider.ToggleState);
		Assert.AreEqual(1, clickCount, "Automation Toggle should preserve ToggleButton click semantics.");
	}

	[TestMethod]
	public async Task SelectionItemProvider_UpdatesMultipleSelection()
	{
		var SUT = new ChipGroup
		{
			SelectionMode = ChipSelectionMode.Multiple,
			ItemsSource = new[] { "One", "Two" },
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

		var groupPeer = FrameworkElementAutomationPeer.CreatePeerForElement(SUT) as ChipGroupAutomationPeer ??
			throw new InvalidOperationException("ChipGroup does not expose its automation peer.");
		var groupProvider = groupPeer.GetPattern(PatternInterface.Selection) as ISelectionProvider ??
			throw new InvalidOperationException("ChipGroup does not expose Selection.");
		var first = GetSelectionItemProvider(SUT, 0);
		var second = GetSelectionItemProvider(SUT, 1);

		first.AddToSelection();
		Assert.IsTrue(first.IsSelected);
		Assert.AreEqual(1, groupProvider.GetSelection().Length);

		second.AddToSelection();
		Assert.IsTrue(first.IsSelected);
		Assert.IsTrue(second.IsSelected);
		Assert.AreEqual(2, groupProvider.GetSelection().Length);

		second.Select();
		Assert.IsFalse(first.IsSelected);
		Assert.IsTrue(second.IsSelected);
		Assert.AreEqual(1, groupProvider.GetSelection().Length);

		second.RemoveFromSelection();
		Assert.IsFalse(second.IsSelected);
		Assert.AreEqual(0, groupProvider.GetSelection().Length);
	}

	[TestMethod]
	public async Task SelectionItemProvider_CannotRemoveRequiredSelection()
	{
		var SUT = new ChipGroup
		{
			SelectionMode = ChipSelectionMode.Single,
			ItemsSource = new[] { "One", "Two" },
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

		var selectionItem = GetSelectionItemProvider(SUT, 0);
		Assert.IsTrue(selectionItem.IsSelected);
		Assert.ThrowsException<InvalidOperationException>(selectionItem.RemoveFromSelection);
	}

	[TestMethod]
	public async Task RemoveButtonAutomationName_UsesRenderedChipName()
	{
		var SUT = new ChipControl
		{
			CanRemove = true,
			Content = new object(),
			ContentTemplate = XamlHelper.LoadXaml<DataTemplate>("""
				<DataTemplate>
					<TextBlock Text="Project" />
				</DataTemplate>
				"""),
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

		var chipPeer = FrameworkElementAutomationPeer.CreatePeerForElement(SUT) as ChipAutomationPeer;
		Assert.IsNotNull(chipPeer);
		Assert.AreEqual("Project", chipPeer!.GetName());

		var removeButton = SUT.GetFirstDescendantOrThrow<Button>("PART_RemoveButton");
		var removeButtonPeer = FrameworkElementAutomationPeer.CreatePeerForElement(removeButton);
		Assert.IsNotNull(removeButtonPeer);
		Assert.AreEqual("Remove Project", removeButtonPeer!.GetName());
	}

	private static ISelectionItemProvider GetSelectionItemProvider(ChipGroup chipGroup, int index)
	{
		var chip = (ChipControl)chipGroup.ContainerFromIndex(index);
		var peer = FrameworkElementAutomationPeer.CreatePeerForElement(chip) as ChipAutomationPeer;
		return peer?.GetPattern(PatternInterface.SelectionItem) as ISelectionItemProvider ??
			throw new InvalidOperationException($"Chip at index {index} does not expose SelectionItem.");
	}

	#endregion

	#region Misc

	[TestMethod]
#if __ANDROID__
	[Ignore("Skip Android due to failures. Tracked by https://github.com/unoplatform/uno.toolkit.ui/issues/1300")]
#endif
	public async Task Initial_Selection()
	{
		var setup = XamlHelper.LoadXaml<ChipGroup>("""
			<utu:ChipGroup>
				<utu:Chip Content="Uno" IsChecked="True"/>
				<utu:Chip Content="Deux" />
				<utu:Chip Content="Three" />
			</utu:ChipGroup>
		""");
		await UnitTestUIContentHelperEx.SetContentAndWait(setup);

		var selected = setup.ContainerFromIndex(0) as Chip ?? throw new Exception("Container#0 not found");

		Assert.AreEqual(selected, setup.SelectedItem, "SelectedItem is expected to be container#0");
		Assert.AreEqual(true, selected.IsChecked, "Container#0 should be selected");
	}

	#endregion
}
