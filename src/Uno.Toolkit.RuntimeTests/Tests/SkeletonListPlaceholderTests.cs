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

	[TestMethod]
	public async Task When_Empty_Horizontal_ItemsControl_Generates_One_Placeholder_Per_Item()
	{
		const double ChipTextWidth = 48;
		var itemsControl = new ItemsControl
		{
			ItemTemplate = XamlHelper.LoadXaml<DataTemplate>("""
				<DataTemplate>
					<Border Padding="14,6" Background="Gray" CornerRadius="16">
						<TextBlock MinWidth="48" HorizontalAlignment="Left" Text="{Binding}" />
					</Border>
				</DataTemplate>
				"""),
			ItemsPanel = XamlHelper.LoadXaml<ItemsPanelTemplate>("""
				<ItemsPanelTemplate>
					<StackPanel Orientation="Horizontal" Spacing="8" />
				</ItemsPanelTemplate>
				"""),
			ItemsSource = new ObservableCollection<string>(),
		};
		Skeleton.SetPlaceholderCount(itemsControl, 4);
		var sut = new SkeletonView
		{
			EnableShimmer = false,
			// the page's view model: placeholder items must not bind to it
			DataContext = "A page DataContext that is much wider than a chip",
			Content = new StackPanel { Width = 400, Children = { itemsControl } },
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(sut);
		await WaitForPlaceholders(sut, x => x.Length == 4);

		var rects = GetPlaceholderRects(sut).OrderBy(x => x.Left).ToArray();
		rects.Should().HaveCount(4, "each placeholder item should produce its own placeholder");
		rects.Should().OnlyContain(x => Math.Abs(x.Width - ChipTextWidth) < Tolerance, "each placeholder should match the item's text slot");
		for (var i = 1; i < rects.Length; i++)
		{
			rects[i].Left.Should().BeGreaterThan(rects[i - 1].Right, "items of a horizontal panel should be laid out side by side");
		}
	}

	[TestMethod]
	public async Task When_Empty_List_Has_MaxHeight_Rows_Clipped()
	{
		var list = new ListView { MaxHeight = 100, ItemTemplate = RowTemplate(), ItemsSource = new ObservableCollection<string>(), SelectionMode = ListViewSelectionMode.None };
		Skeleton.SetPlaceholderCount(list, 6);
		var sut = new SkeletonView { EnableShimmer = false, Content = new StackPanel { Width = 300, Children = { list } } };

		await UnitTestUIContentHelperEx.SetContentAndWait(sut);
		await WaitForPlaceholders(sut, x => x.Length > 0);

		list.ActualHeight.Should().BeLessOrEqualTo(100 + Tolerance, "the reserved space should respect MaxHeight");
		GetPlaceholderRects(sut).Should().OnlyContain(x => x.Bottom <= GetBounds(list, GetOverlay(sut)).Bottom + Tolerance, "rows beyond the list's bounds should be clipped");
	}

	[TestMethod]
	public async Task When_ListView_Subclass_With_Own_Style_Gets_Rows()
	{
		// the stand-in is a plain ListView: a Style targeting the subclass must not be copied onto it
		var style = new Style(typeof(StyledListView)) { Setters = { new Setter(FrameworkElement.MarginProperty, new Thickness(2)) } };
		var list = new StyledListView { Style = style, ItemTemplate = RowTemplate(), ItemsSource = new ObservableCollection<string>(), SelectionMode = ListViewSelectionMode.None };
		Skeleton.SetPlaceholderCount(list, 2);
		var sut = new SkeletonView { EnableShimmer = false, Content = new StackPanel { Width = 300, Children = { list } } };

		await UnitTestUIContentHelperEx.SetContentAndWait(sut);
		await WaitForPlaceholders(sut, _ => GetAvatarRects(sut).Length == 2);

		GetAvatarRects(sut).Should().HaveCount(2);
	}

	[TestMethod]
	public async Task When_Empty_GridView_Generates_Rows()
	{
		var grid = new GridView { ItemTemplate = RowTemplate(), ItemsSource = new ObservableCollection<string>(), SelectionMode = ListViewSelectionMode.None };
		Skeleton.SetPlaceholderCount(grid, 2);
		var sut = new SkeletonView { EnableShimmer = false, Content = new StackPanel { Width = 300, Children = { grid } } };

		await UnitTestUIContentHelperEx.SetContentAndWait(sut);
		await WaitForPlaceholders(sut, _ => GetAvatarRects(sut).Length == 2);

		GetAvatarRects(sut).Should().HaveCount(2);
	}

	[TestMethod]
	public async Task When_PlaceholderCount_Zero_On_List_Vetoes_Ancestor()
	{
		var list = new ListView { ItemTemplate = RowTemplate(), ItemsSource = new ObservableCollection<string>(), SelectionMode = ListViewSelectionMode.None };
		Skeleton.SetPlaceholderCount(list, 0);
		var root = new StackPanel { Width = 300, Children = { new TextBlock { Text = "title" }, list } };
		Skeleton.SetPlaceholderCount(root, 3);
		var sut = new SkeletonView { EnableShimmer = false, Content = root };

		await UnitTestUIContentHelperEx.SetContentAndWait(sut);
		await UnitTestUIContentHelperEx.WaitForIdle();
		await UnitTestUIContentHelperEx.WaitForIdle();

		GetAvatarRects(sut).Should().BeEmpty("an explicit 0 on the list wins over the ancestor's count");
		GetPlaceholderRects(sut).Should().HaveCount(1);
	}

	[TestMethod]
	public void When_PlaceholderCount_Not_Set_Getter_Reports_Zero()
	{
		Skeleton.GetPlaceholderCount(new ListView()).Should().Be(0, "a list without a count gets no rows, and the getter says so");
	}

	[TestMethod]
	public async Task When_PlaceholderCount_From_Style_Applies()
	{
		var style = new Style(typeof(ListView)) { Setters = { new Setter(Skeleton.PlaceholderCountProperty, 2) } };
		var list = new ListView { Style = style, ItemTemplate = RowTemplate(), ItemsSource = new ObservableCollection<string>(), SelectionMode = ListViewSelectionMode.None };
		// the title gives the view a size even without rows, so a failure is the rows' count, not a load timeout
		var sut = new SkeletonView { EnableShimmer = false, Content = new StackPanel { Width = 300, Children = { new TextBlock { Text = "title" }, list } } };

		await UnitTestUIContentHelperEx.SetContentAndWait(sut);
		await WaitForPlaceholders(sut, _ => GetAvatarRects(sut).Length == 2);

		GetAvatarRects(sut).Should().HaveCount(2, "a count coming from a Style should apply");
	}

	[TestMethod]
	public async Task When_Stand_In_Does_Not_Use_Container_Style_Selector()
	{
		var selector = new RecordingContainerStyleSelector();
		var list = new ListView { ItemTemplate = RowTemplate(), ItemsSource = new ObservableCollection<string>(), ItemContainerStyleSelector = selector, SelectionMode = ListViewSelectionMode.None };
		Skeleton.SetPlaceholderCount(list, 2);
		var sut = new SkeletonView { EnableShimmer = false, Content = new StackPanel { Width = 300, Children = { list } } };

		await UnitTestUIContentHelperEx.SetContentAndWait(sut);
		await WaitForPlaceholders(sut, _ => GetAvatarRects(sut).Length == 2);

		GetAvatarRects(sut).Should().HaveCount(2);
		selector.NullItems.Should().Be(0, "placeholder rows have null items, which an app's selector cannot be expected to handle");
	}

	private sealed partial class RecordingContainerStyleSelector : StyleSelector
	{
		private readonly Style _style = new(typeof(ListViewItem));

		public int NullItems { get; private set; }

		protected override Style SelectStyleCore(object item, DependencyObject container)
		{
			if (item is null)
			{
				NullItems++;
			}

			return _style;
		}
	}

	private sealed partial class StyledListView : ListView
	{
	}
}
