using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.RuntimeTests.Tests.HotReload;
using Uno.Toolkit.UI;
using Uno.UI.RuntimeTests;
using Windows.Foundation;

namespace Uno.Toolkit.RuntimeTests.Tests;

/// <summary>
/// Tests for SkeletonView auto-generated placeholders.
/// Validates that placeholders are derived from the content's leaf elements,
/// honor the Ignore/Shape attached properties, and follow IsLoading/Source.
/// </summary>
[TestClass]
[RunsOnUIThread]
public class SkeletonViewTests
{
	private const double Tolerance = 1.5;

	private static Canvas GetOverlay(SkeletonView sut) =>
		sut.GetFirstDescendant<Canvas>(x => x.Name == "PART_SkeletonOverlay") ?? throw new Exception("Failed to find PART_SkeletonOverlay");

	private static ContentPresenter GetPresenter(SkeletonView sut) =>
		sut.GetFirstDescendant<ContentPresenter>(x => x.Name == "PART_ContentPresenter") ?? throw new Exception("Failed to find PART_ContentPresenter");

	[TestMethod]
	public async Task When_Loading_Placeholders_Match_Leaves()
	{
		var text = new TextBlock { Text = "Hello World" };
		var ellipse = new Ellipse { Width = 48, Height = 48, Fill = new SolidColorBrush(Microsoft.UI.Colors.Red), HorizontalAlignment = HorizontalAlignment.Left };
		var border = new Border(); // container without content: not a leaf, no placeholder
		var sut = new SkeletonView
		{
			EnableShimmer = false,
			Content = new StackPanel { Width = 200, Spacing = 8, Children = { text, ellipse, border } },
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(sut);

		var overlay = GetOverlay(sut);
		overlay.Visibility.Should().Be(Visibility.Visible);
		overlay.Children.Count.Should().Be(2, "one placeholder per leaf (TextBlock, Ellipse)");

		var textBounds = text.TransformToVisual(overlay).TransformBounds(new Rect(0, 0, text.ActualWidth, text.ActualHeight));
		var textPlaceholder = (FrameworkElement)overlay.Children[0];
		Canvas.GetLeft(textPlaceholder).Should().BeApproximately(textBounds.X, Tolerance);
		Canvas.GetTop(textPlaceholder).Should().BeApproximately(textBounds.Y, Tolerance);
		textPlaceholder.Width.Should().BeApproximately(textBounds.Width, Tolerance);
		textPlaceholder.Height.Should().BeApproximately(textBounds.Height, Tolerance);

		var ellipsePlaceholder = (Grid)overlay.Children[1];
		ellipsePlaceholder.Width.Should().BeApproximately(48, Tolerance);
		ellipsePlaceholder.Height.Should().BeApproximately(48, Tolerance);
		ellipsePlaceholder.CornerRadius.TopLeft.Should().BeApproximately(24, Tolerance, "an Ellipse leaf should produce a circle");
	}

	[TestMethod]
	public async Task When_Empty_TextBlock_Uses_Slot_Heuristic()
	{
		var text = new TextBlock(); // no value present yet
		var sut = new SkeletonView
		{
			EnableShimmer = false,
			Content = new StackPanel { Width = 300, Children = { text } },
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(sut);

		var overlay = GetOverlay(sut);
		overlay.Children.Count.Should().Be(1);

		var placeholder = (FrameworkElement)overlay.Children[0];
		placeholder.Width.Should().BeApproximately(300, Tolerance, "an empty stretched TextBlock should produce a line filling its layout slot");
		placeholder.Height.Should().BeGreaterThan(8, "the line height should be synthesized even though the TextBlock measured empty");
	}

	[TestMethod]
	public async Task When_IsLoading_Toggles()
	{
		var sut = new SkeletonView
		{
			EnableShimmer = false,
			Content = new StackPanel { Width = 200, Children = { new TextBlock { Text = "content" } } },
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(sut);

		var overlay = GetOverlay(sut);
		var presenter = GetPresenter(sut);
		overlay.Children.Count.Should().Be(1);
		presenter.Opacity.Should().Be(0, "content should be hidden while loading");

		sut.IsLoading = false;
		await UnitTestUIContentHelperEx.WaitForIdle();

		overlay.Children.Count.Should().Be(0, "placeholders should be released once loaded");
		overlay.Visibility.Should().Be(Visibility.Collapsed);
		presenter.Opacity.Should().Be(1, "content should be visible once loaded");
		presenter.IsHitTestVisible.Should().BeTrue();

		sut.IsLoading = true;
		await UnitTestUIContentHelperEx.WaitForIdle();

		overlay.Children.Count.Should().Be(1, "placeholders should be regenerated when loading restarts");
		presenter.Opacity.Should().Be(0);
	}

	[TestMethod]
	public async Task When_Ignore_Set_Subtree_Skipped()
	{
		var ignored = new StackPanel { Children = { new TextBlock { Text = "ignored" }, new TextBlock { Text = "also ignored" } } };
		SkeletonView.SetIgnore(ignored, true);
		var sut = new SkeletonView
		{
			EnableShimmer = false,
			Content = new StackPanel { Width = 200, Children = { new TextBlock { Text = "visible" }, ignored } },
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(sut);

		GetOverlay(sut).Children.Count.Should().Be(1, "the ignored subtree should not contribute placeholders");
	}

	[TestMethod]
	public async Task When_Shape_Circle_Forced()
	{
		var square = new Border { Width = 64, Height = 64, Background = new SolidColorBrush(Microsoft.UI.Colors.Blue) };
		SkeletonView.SetShape(square, SkeletonShape.Circle);
		var sut = new SkeletonView
		{
			EnableShimmer = false,
			Content = new StackPanel { Width = 200, Children = { square } },
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(sut);

		var overlay = GetOverlay(sut);
		overlay.Children.Count.Should().Be(1, "a forced shape makes the container a leaf");

		var placeholder = (Grid)overlay.Children[0];
		placeholder.Width.Should().BeApproximately(64, Tolerance);
		placeholder.Height.Should().BeApproximately(64, Tolerance);
		placeholder.CornerRadius.TopLeft.Should().BeApproximately(32, Tolerance);
	}

	[TestMethod]
	public async Task When_Shimmer_Enabled_Bands_Are_Added()
	{
		var sut = new SkeletonView
		{
			Content = new StackPanel { Width = 200, Children = { new TextBlock { Text = "content" } } },
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(sut);

		var placeholder = (Grid)GetOverlay(sut).Children[0];
		placeholder.Children.Count.Should().Be(1, "each placeholder should host a shimmer band");

		var band = (FrameworkElement)placeholder.Children[0];
		band.Width.Should().BeGreaterThan(0, "the band should have a concrete sweep width");
		band.RenderTransform.Should().BeOfType<TranslateTransform>("the band is animated through a translate transform");
	}

	[TestMethod]
	public async Task When_Source_Drives_IsLoading()
	{
		var loadable = new TestLoadable(isExecuting: true);
		var sut = new SkeletonView
		{
			EnableShimmer = false,
			Source = loadable,
			Content = new StackPanel { Width = 200, Children = { new TextBlock { Text = "content" } } },
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(sut);

		sut.IsLoading.Should().BeTrue();
		GetOverlay(sut).Children.Count.Should().Be(1);

		loadable.IsExecuting = false;
		await UnitTestUIContentHelperEx.WaitForIdle();

		sut.IsLoading.Should().BeFalse("Source.IsExecuting should drive IsLoading");
		GetOverlay(sut).Children.Count.Should().Be(0);
	}
}
