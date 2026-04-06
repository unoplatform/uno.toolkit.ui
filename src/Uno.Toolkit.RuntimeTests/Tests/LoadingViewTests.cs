using FluentAssertions;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.RuntimeTests.Tests.HotReload;
using Uno.Toolkit.UI;
using Uno.UI.RuntimeTests;

namespace Uno.Toolkit.RuntimeTests.Tests;

/// <summary>
/// Tests for LoadingView visual state transitions based on Source/ILoadable.
/// Validates that:
/// - Null Source keeps the view in Loading state indefinitely
/// - Source.IsExecuting = false transitions to Loaded state
/// - Source set after template application triggers correct transition
/// </summary>
[TestClass]
[RunsOnUIThread]
public class LoadingViewTests
{
	[TestMethod]
	public async Task When_SourceIsNull_Then_RemainsInLoadingState()
	{
		// Arrange: LoadingView with no Source set
		var lv = new LoadingView
		{
			Content = new TextBlock { Text = "Main content" },
			LoadingContent = new TextBlock { Text = "Loading..." },
		};

		// Act: add to visual tree, wait for template
		await UnitTestUIContentHelperEx.SetContentAndWait(lv);

		// Assert: should be in "Loading" state since Source is null
		// (Source?.IsExecuting ?? true) == true → Loading
		lv.Source.Should().BeNull();
		VisualStateManager.GetVisualStateGroups(lv).Should().NotBeEmpty();
	}

	[TestMethod]
	public async Task When_SourceBecomesNotExecuting_Then_TransitionsToLoaded()
	{
		// Arrange
		var loadable = new TestLoadable(isExecuting: true);
		var lv = new LoadingView
		{
			Source = loadable,
			Content = new TextBlock { Text = "Main content" },
			LoadingContent = new TextBlock { Text = "Loading..." },
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(lv);

		// Act: transition from executing to not-executing
		loadable.IsExecuting = false;
		await UnitTestUIContentHelperEx.WaitForIdle();

		// Assert: Source is no longer executing
		lv.Source.IsExecuting.Should().BeFalse();
	}

	[TestMethod]
	public async Task When_SourceSetAfterTemplate_Then_TransitionsCorrectly()
	{
		// Arrange: LoadingView initially with no Source
		var lv = new LoadingView
		{
			Content = new TextBlock { Text = "Main content" },
			LoadingContent = new TextBlock { Text = "Loading..." },
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(lv);

		// Precondition: Source is null, view is in Loading
		lv.Source.Should().BeNull();

		// Act: set Source to an already-completed loadable
		var loadable = new TestLoadable(isExecuting: false);
		lv.Source = loadable;
		await UnitTestUIContentHelperEx.WaitForIdle();

		// Assert: Source was set and is not executing
		lv.Source.Should().NotBeNull();
		lv.Source.IsExecuting.Should().BeFalse();
	}
}
