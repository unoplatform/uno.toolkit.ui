using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.UI;
using Uno.UI.RuntimeTests;
using Windows.Foundation;

namespace Uno.Toolkit.RuntimeTests.Tests;

/// <summary>
/// Tests for placeholder rows generated for empty lists inside a SkeletonView.
/// Validates that opted-in empty lists (Skeleton.PlaceholderCount) produce rows from their ItemTemplate,
/// reserve layout space, are clipped to their bounds, and that the list is left untouched once loaded.
/// </summary>
[TestClass]
[RunsOnUIThread]
public class SkeletonListPlaceholderTests
{
	private const double Tolerance = 1.5;
	private const double AvatarSize = 40;

	private static DataTemplate RowTemplate() => XamlHelper.LoadXaml<DataTemplate>("""
		<DataTemplate>
			<StackPanel Orientation="Horizontal" Spacing="12">
				<Ellipse Width="40" Height="40" Fill="Gray" />
				<TextBlock Text="{Binding}" />
			</StackPanel>
		</DataTemplate>
		""");

	private static Canvas GetOverlay(SkeletonView sut) =>
		sut.GetFirstDescendant<Canvas>(x => x.Name == "PART_SkeletonOverlay") ?? throw new Exception("Failed to find PART_SkeletonOverlay");

	private static Rect[] GetPlaceholderRects(SkeletonView sut) => GetOverlay(sut).Children
		.OfType<Grid>()
		.Select(x => new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height))
		.ToArray();

	// The row template's avatar: a 40x40 circle.
	private static Rect[] GetAvatarRects(SkeletonView sut) => GetOverlay(sut).Children
		.OfType<Grid>()
		.Where(x => Math.Abs(x.Width - AvatarSize) < Tolerance && Math.Abs(x.Height - AvatarSize) < Tolerance && x.CornerRadius.TopLeft > (AvatarSize / 2) - Tolerance)
		.Select(x => new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height))
		.ToArray();

	private static Rect GetBounds(FrameworkElement element, UIElement relativeTo) =>
		element.TransformToVisual(relativeTo).TransformBounds(new Rect(0, 0, element.ActualWidth, element.ActualHeight));

	private static async Task WaitForPlaceholders(SkeletonView sut, Func<Rect[], bool> predicate)
	{
		// placeholder rows realize over a few layout passes
		for (var i = 0; i < 20 && !predicate(GetPlaceholderRects(sut)); i++)
		{
			await UnitTestUIContentHelperEx.WaitForIdle();
		}
	}

	[TestMethod]
	public async Task When_Nested_Empty_List_Generates_Rows_And_Reserves_Space()
	{
		var list = new ListView { ItemTemplate = RowTemplate(), ItemsSource = new ObservableCollection<string>(), SelectionMode = ListViewSelectionMode.None };
		Skeleton.SetPlaceholderCount(list, 3);
		var below = new TextBlock { Text = "below the list" };
		var root = new Grid
		{
			Width = 300,
			RowDefinitions = { new RowDefinition { Height = GridLength.Auto }, new RowDefinition { Height = GridLength.Auto }, new RowDefinition { Height = GridLength.Auto } },
			Children = { new Button { Content = "action" }, list, below },
		};
		Grid.SetRow(list, 1);
		Grid.SetRow(below, 2);
		var sut = new SkeletonView { EnableShimmer = false, Content = root };

		await UnitTestUIContentHelperEx.SetContentAndWait(sut);
		await WaitForPlaceholders(sut, _ => GetAvatarRects(sut).Length == 3);

		var overlay = GetOverlay(sut);
		var avatars = GetAvatarRects(sut);
		avatars.Should().HaveCount(3, "the empty list should produce one row per placeholder item");

		var listBounds = GetBounds(list, overlay);
		listBounds.Height.Should().BeGreaterOrEqualTo((3 * AvatarSize) - Tolerance, "the list should reserve the space of its placeholder rows");
		avatars.Should().OnlyContain(x => x.Top >= listBounds.Top - Tolerance && x.Bottom <= listBounds.Bottom + Tolerance, "rows should be drawn over the list");
		var belowWhileLoading = GetBounds(below, overlay).Top;

		sut.IsLoading = false;
		await UnitTestUIContentHelperEx.WaitForIdle();

		var belowLoaded = GetBounds(below, root).Top;
		belowWhileLoading.Should().BeGreaterThan(belowLoaded + (2 * AvatarSize), "content below the list should be pushed down while loading");
		list.ReadLocalValue(FrameworkElement.MinHeightProperty).Should().Be(DependencyProperty.UnsetValue, "the list's MinHeight should be restored once loaded");
	}

	[TestMethod]
	public async Task When_Empty_List_Constrained_Rows_Clipped_To_List()
	{
		var list = new ListView { ItemTemplate = RowTemplate(), ItemsSource = new ObservableCollection<string>(), SelectionMode = ListViewSelectionMode.None };
		Skeleton.SetPlaceholderCount(list, 6);
		var root = new Grid { Width = 300, Height = 100, Children = { list } };
		var sut = new SkeletonView { EnableShimmer = false, Content = root };

		await UnitTestUIContentHelperEx.SetContentAndWait(sut);
		await WaitForPlaceholders(sut, x => x.Length > 0);

		var overlay = GetOverlay(sut);
		var rects = GetPlaceholderRects(sut);
		rects.Should().NotBeEmpty();
		rects.Should().OnlyContain(x => x.Bottom <= 100 + Tolerance, "rows beyond the list's bounds should be clipped");
		root.ActualHeight.Should().BeApproximately(100, Tolerance);
		list.ReadLocalValue(FrameworkElement.MinHeightProperty).Should().Be(DependencyProperty.UnsetValue, "a list granted more space than it needs should keep its size");
	}

	[TestMethod]
	public async Task When_Items_Arrive_While_Loading_Real_Rows_Mirrored()
	{
		var items = new ObservableCollection<string>();
		var list = new ListView { ItemTemplate = RowTemplate(), ItemsSource = items, SelectionMode = ListViewSelectionMode.None };
		Skeleton.SetPlaceholderCount(list, 3);
		var sut = new SkeletonView { EnableShimmer = false, Content = new StackPanel { Width = 300, Children = { list } } };

		await UnitTestUIContentHelperEx.SetContentAndWait(sut);
		await WaitForPlaceholders(sut, _ => GetAvatarRects(sut).Length == 3);
		GetAvatarRects(sut).Should().HaveCount(3);

		items.Add("one");
		await WaitForPlaceholders(sut, _ => GetAvatarRects(sut).Length == 1);

		GetAvatarRects(sut).Should().HaveCount(1, "once the list has items, its real rows should be mirrored instead");
		list.ReadLocalValue(FrameworkElement.MinHeightProperty).Should().Be(DependencyProperty.UnsetValue, "the reserved space should be released once real items arrive");
	}

	[TestMethod]
	public async Task When_Loaded_List_Original_MinHeight_Restored()
	{
		var list = new ListView { MinHeight = 10, ItemTemplate = RowTemplate(), ItemsSource = new ObservableCollection<string>() };
		Skeleton.SetPlaceholderCount(list, 3);
		var sut = new SkeletonView { EnableShimmer = false, Content = new StackPanel { Width = 300, Children = { list } } };

		await UnitTestUIContentHelperEx.SetContentAndWait(sut);
		await WaitForPlaceholders(sut, _ => GetAvatarRects(sut).Length == 3);
		list.MinHeight.Should().BeGreaterThan(10, "the list should reserve the space of its placeholder rows while loading");

		sut.IsLoading = false;
		await UnitTestUIContentHelperEx.WaitForIdle();

		list.ReadLocalValue(FrameworkElement.MinHeightProperty).Should().Be(10d, "the original local MinHeight should be restored");
	}

	[TestMethod]
	public async Task When_No_PlaceholderCount_Empty_List_Skipped()
	{
		var list = new ListView { ItemTemplate = RowTemplate(), ItemsSource = new ObservableCollection<string>() };
		var sut = new SkeletonView
		{
			EnableShimmer = false,
			Content = new StackPanel { Width = 300, Children = { new TextBlock { Text = "title" }, list } },
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(sut);
		await UnitTestUIContentHelperEx.WaitForIdle();

		GetAvatarRects(sut).Should().BeEmpty("placeholder rows are opt-in through Skeleton.PlaceholderCount");
		GetPlaceholderRects(sut).Should().HaveCount(1);
		list.ReadLocalValue(FrameworkElement.MinHeightProperty).Should().Be(DependencyProperty.UnsetValue);
	}

	[TestMethod]
	public async Task When_PlaceholderCount_Set_On_Ancestor_Applies_To_List()
	{
		var list = new ListView { ItemTemplate = RowTemplate(), ItemsSource = new ObservableCollection<string>(), SelectionMode = ListViewSelectionMode.None };
		var root = new StackPanel { Width = 300, Children = { list } };
		Skeleton.SetPlaceholderCount(root, 2);
		var sut = new SkeletonView { EnableShimmer = false, Content = root };

		await UnitTestUIContentHelperEx.SetContentAndWait(sut);
		await WaitForPlaceholders(sut, _ => GetAvatarRects(sut).Length == 2);

		GetAvatarRects(sut).Should().HaveCount(2, "the nearest ancestor setting PlaceholderCount should apply");
	}

	[TestMethod]
	public async Task When_Empty_ItemsRepeater_Generates_Rows()
	{
		var repeater = new ItemsRepeater { ItemTemplate = RowTemplate(), ItemsSource = new ObservableCollection<string>() };
		Skeleton.SetPlaceholderCount(repeater, 2);
		var sut = new SkeletonView { EnableShimmer = false, Content = new StackPanel { Width = 300, Children = { repeater } } };

		await UnitTestUIContentHelperEx.SetContentAndWait(sut);
		await WaitForPlaceholders(sut, _ => GetAvatarRects(sut).Length == 2);

		GetAvatarRects(sut).Should().HaveCount(2);
		repeater.ActualHeight.Should().BeGreaterOrEqualTo((2 * AvatarSize) - Tolerance);
	}
}
