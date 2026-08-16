using FluentAssertions;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
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
/// - Null Source keeps the view in Loading state indefinitely (content hidden)
/// - Source.IsExecuting = false transitions to Loaded state (content visible)
/// - Source set after template application triggers correct transition
/// </summary>
[TestClass]
[RunsOnUIThread]
public class LoadingViewTests
{
	private static ContentPresenter? FindContentPresenter(LoadingView lv)
	{
		// The template's ContentPresenter named "ContentPresenter" is inside a Grid.
		// Walk the visual tree to find it.
		if (VisualTreeHelper.GetChildrenCount(lv) == 0)
		{
			return null;
		}

		var root = VisualTreeHelper.GetChild(lv, 0); // Grid "RootPanel"
		for (var i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
		{
			if (VisualTreeHelper.GetChild(root, i) is ContentPresenter cp && cp.Name == "ContentPresenter")
			{
				return cp;
			}
		}

		return null;
	}

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

		// Assert: Source is null so view stays in "Loading" state.
		// ContentPresenter starts at Opacity=0 and stays there.
		lv.Source.Should().BeNull();

		var cp = FindContentPresenter(lv);
		cp.Should().NotBeNull("template should have been applied");
		cp!.Opacity.Should().Be(0, "ContentPresenter should remain hidden (opacity 0) when Source is null (Loading state)");
	}

	[TestMethod]
	public async Task When_SourceBecomesNotExecuting_Then_TransitionsToLoaded()
	{
		// Arrange: start with Source.IsExecuting = true
		var loadable = new TestLoadable(isExecuting: true);
		var lv = new LoadingView
		{
			Source = loadable,
			Content = new TextBlock { Text = "Main content" },
			LoadingContent = new TextBlock { Text = "Loading..." },
			UseTransitions = false, // skip animation for deterministic assertion
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(lv);

		var cp = FindContentPresenter(lv);
		cp.Should().NotBeNull();
		cp!.Opacity.Should().Be(0, "content should be hidden while Source is executing");

		// Act: transition to not-executing
		loadable.IsExecuting = false;
		await UnitTestUIContentHelperEx.WaitForIdle();

		// Assert: "Loaded" visual state sets ContentPresenter.Opacity = 1
		cp.Opacity.Should().Be(1,
			"ContentPresenter should become visible (opacity 1) after Source stops executing");
	}

	[TestMethod]
	public async Task When_SourceSetAfterTemplate_Then_TransitionsCorrectly()
	{
		// Arrange: LoadingView initially with no Source
		var lv = new LoadingView
		{
			Content = new TextBlock { Text = "Main content" },
			LoadingContent = new TextBlock { Text = "Loading..." },
			UseTransitions = false,
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(lv);

		var cp = FindContentPresenter(lv);
		cp.Should().NotBeNull();

		// Precondition: Source is null → Loading state → content hidden
		lv.Source.Should().BeNull();
		cp!.Opacity.Should().Be(0);

		// Act: set Source to an already-completed loadable
		var loadable = new TestLoadable(isExecuting: false);
		lv.Source = loadable;
		await UnitTestUIContentHelperEx.WaitForIdle();

		// Assert: transitions to Loaded → content visible
		cp.Opacity.Should().Be(1,
			"ContentPresenter should become visible after Source is set with IsExecuting=false");
	}
}
