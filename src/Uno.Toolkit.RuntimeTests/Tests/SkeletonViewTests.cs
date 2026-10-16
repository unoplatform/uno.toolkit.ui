using System;
using System.Linq;
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
		var text = new TextBlock { Text = "content" };
		var sut = new SkeletonView
		{
			EnableShimmer = false,
			Content = new StackPanel { Width = 200, Children = { text } },
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(sut);

		var overlay = GetOverlay(sut);
		var presenter = GetPresenter(sut);
		overlay.Children.Count.Should().Be(1);
		text.Opacity.Should().Be(0, "an element covered by a placeholder should be hidden while loading");
		presenter.IsHitTestVisible.Should().BeFalse("content should not be interactive while loading");

		sut.IsLoading = false;
		await UnitTestUIContentHelperEx.WaitForIdle();

		overlay.Children.Count.Should().Be(0, "placeholders should be released once loaded");
		overlay.Visibility.Should().Be(Visibility.Collapsed);
		text.Opacity.Should().Be(1, "content should be visible once loaded");
		presenter.IsHitTestVisible.Should().BeTrue();

		sut.IsLoading = true;
		await UnitTestUIContentHelperEx.WaitForIdle();

		overlay.Children.Count.Should().Be(1, "placeholders should be regenerated when loading restarts");
		text.Opacity.Should().Be(0);
	}

	[TestMethod]
	public async Task When_Ignore_Set_Element_Stays_Visible()
	{
		var header = new TextBlock { Text = "static header" };
		Skeleton.SetIgnore(header, true);
		var text = new TextBlock { Text = "loading content" };
		var card = new Border { Background = new SolidColorBrush(Microsoft.UI.Colors.Gray), Child = text };
		var sut = new SkeletonView
		{
			EnableShimmer = false,
			Content = new StackPanel { Width = 200, Children = { header, card } },
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(sut);

		GetOverlay(sut).Children.Count.Should().Be(1, "only the non-ignored text gets a placeholder");
		header.Opacity.Should().Be(1, "an ignored element stays visible while loading");
		card.Opacity.Should().Be(1, "containers stay visible: only placeholder-covered elements are hidden");
		text.Opacity.Should().Be(0);
	}

	[TestMethod]
	public async Task When_Loaded_Original_Opacity_Restored()
	{
		var local = new TextBlock { Text = "local", Opacity = 0.5 };
		var bound = new TextBlock { Text = "bound" };
		bound.SetBinding(UIElement.OpacityProperty, new Microsoft.UI.Xaml.Data.Binding { Source = 0.7 });
		var sut = new SkeletonView
		{
			EnableShimmer = false,
			Content = new StackPanel { Width = 200, Children = { local, bound } },
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(sut);
		local.Opacity.Should().Be(0);
		bound.Opacity.Should().Be(0);

		sut.IsLoading = false;
		await UnitTestUIContentHelperEx.WaitForIdle();

		local.Opacity.Should().Be(0.5, "a local Opacity value should be restored");
		bound.Opacity.Should().BeApproximately(0.7, 0.001, "an Opacity binding should be restored");
		bound.GetBindingExpression(UIElement.OpacityProperty).Should().NotBeNull();
	}

	[TestMethod]
	public async Task When_ContentControl_Chrome_Skipped()
	{
		// Mimics a styled container such as Material's ListViewItem: a full-size background shape behind the content.
		var template = XamlHelper.LoadXaml<ControlTemplate>("""
			<ControlTemplate TargetType="ContentControl">
				<Grid>
					<Rectangle Fill="Gray" />
					<ContentPresenter />
				</Grid>
			</ControlTemplate>
			""");
		var text = new TextBlock { Text = "content", HorizontalAlignment = HorizontalAlignment.Left };
		var container = new ContentControl { Width = 200, Height = 60, Template = template, Content = text };
		var sut = new SkeletonView
		{
			EnableShimmer = false,
			Content = new StackPanel { Children = { container } },
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(sut);

		var overlay = GetOverlay(sut);
		overlay.Children.Count.Should().Be(1, "only the container's content should produce a placeholder, not its template chrome");
		var placeholder = (FrameworkElement)overlay.Children[0];
		placeholder.Width.Should().BeApproximately(text.ActualWidth, Tolerance);
	}

	[TestMethod]
	public async Task When_ListView_Items_First_Loading_Pass_Skips_Item_Chrome()
	{
		// Mirrors a FeedView refresh: the list is realized while loaded, then loading turns on for the first time.
		var itemTemplate = XamlHelper.LoadXaml<DataTemplate>("""
			<DataTemplate>
				<StackPanel Orientation="Horizontal" Spacing="12">
					<Ellipse Width="40" Height="40" Fill="Gray" />
					<TextBlock Text="{Binding}" />
				</StackPanel>
			</DataTemplate>
			""");
		var list = new ListView { Width = 300, ItemTemplate = itemTemplate, ItemsSource = new[] { "one", "two", "three" } };
		var sut = new SkeletonView { EnableShimmer = false, IsLoading = false, Content = list };

		await UnitTestUIContentHelperEx.SetContentAndWait(sut);

		sut.IsLoading = true;
		await UnitTestUIContentHelperEx.WaitForIdle();

		var placeholders = GetOverlay(sut).Children.OfType<FrameworkElement>().ToArray();
		placeholders.Should().HaveCount(6, "each item contributes its Ellipse and TextBlock, not its container chrome");
		placeholders.Should().OnlyContain(x => x.Width < 100, "no placeholder should span the item container");
	}

	[TestMethod]
	public async Task When_Ignore_Set_Subtree_Skipped()
	{
		var ignored = new StackPanel { Children = { new TextBlock { Text = "ignored" }, new TextBlock { Text = "also ignored" } } };
		Skeleton.SetIgnore(ignored, true);
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
		Skeleton.SetShape(square, SkeletonShape.Circle);
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

	[TestMethod]
	public async Task When_Source_Is_LoadableSource_DataContext_Inherited()
	{
		// a LoadableSource declared as Source lives outside the tree: it needs the view's DataContext for its binding
		var loadable = new TestLoadable(isExecuting: true);
		var source = new LoadableSource();
		source.SetBinding(LoadableSource.SourceProperty, new Microsoft.UI.Xaml.Data.Binding { Path = new PropertyPath(nameof(SourceHost.Loadable)) });
		var sut = new SkeletonView
		{
			EnableShimmer = false,
			DataContext = new SourceHost(loadable),
			Source = source,
			Content = new StackPanel { Width = 200, Children = { new TextBlock { Text = "content" } } },
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(sut);

		sut.IsLoading.Should().BeTrue("the LoadableSource binding should resolve against the view's DataContext");

		loadable.IsExecuting = false;
		await UnitTestUIContentHelperEx.WaitForIdle();

		sut.IsLoading.Should().BeFalse();
	}

	[TestMethod]
	public async Task When_Nested_SkeletonView_Outer_Finishes_First()
	{
		var (outer, inner, text) = BuildNested();

		await UnitTestUIContentHelperEx.SetContentAndWait(outer);
		await UnitTestUIContentHelperEx.WaitForIdle();

		GetOwnOverlay(outer).Children.Count.Should().Be(1, "a nested SkeletonView owns its subtree: the outer view only mirrors its own leaves");
		GetOwnOverlay(inner).Children.Count.Should().Be(1);
		text.Opacity.Should().Be(0);

		outer.IsLoading = false;
		await UnitTestUIContentHelperEx.WaitForIdle();
		text.Opacity.Should().Be(0, "the inner view is still loading");

		inner.IsLoading = false;
		await UnitTestUIContentHelperEx.WaitForIdle();
		text.Opacity.Should().Be(1, "only the view that hid the element restores it, whichever finishes last");
	}

	[TestMethod]
	public async Task When_Nested_SkeletonView_Inner_Finishes_First()
	{
		var (outer, inner, text) = BuildNested();

		await UnitTestUIContentHelperEx.SetContentAndWait(outer);
		await UnitTestUIContentHelperEx.WaitForIdle();

		inner.IsLoading = false;
		await UnitTestUIContentHelperEx.WaitForIdle();
		text.Opacity.Should().Be(1, "the outer view never hid the inner view's content");

		outer.IsLoading = false;
		await UnitTestUIContentHelperEx.WaitForIdle();
		text.Opacity.Should().Be(1);
	}

	[TestMethod]
	public async Task When_Content_Replaced_Same_Size_While_Loading()
	{
		var first = new TextBlock { Text = "first", HorizontalAlignment = HorizontalAlignment.Left };
		var second = new TextBlock { Text = "second, noticeably longer", HorizontalAlignment = HorizontalAlignment.Left };
		var sut = new SkeletonView { EnableShimmer = false, Content = new Grid { Width = 200, Height = 100, Children = { first } } };

		await UnitTestUIContentHelperEx.SetContentAndWait(sut);
		first.Opacity.Should().Be(0);

		// same size: no SizeChanged to trigger a regeneration
		sut.Content = new Grid { Width = 200, Height = 100, Children = { second } };
		await UnitTestUIContentHelperEx.WaitForIdle();

		first.Opacity.Should().Be(1, "content removed while loading is no longer covered");
		second.Opacity.Should().Be(0, "the new content is covered");
		var overlay = GetOverlay(sut);
		overlay.Children.Count.Should().Be(1);
		((FrameworkElement)overlay.Children[0]).Width.Should().BeApproximately(second.ActualWidth, Tolerance, "the placeholder should mirror the new content");
	}

	[TestMethod]
	public async Task When_Content_Set_Null_While_Loading()
	{
		var text = new TextBlock { Text = "content" };
		var sut = new SkeletonView { EnableShimmer = false, Content = new StackPanel { Width = 200, Children = { text } } };

		await UnitTestUIContentHelperEx.SetContentAndWait(sut);
		GetOverlay(sut).Children.Count.Should().Be(1);

		sut.Content = null;
		await UnitTestUIContentHelperEx.WaitForIdle();

		text.Opacity.Should().Be(1, "content removed while loading is no longer covered");
		GetOverlay(sut).Children.Count.Should().Be(0, "no content, no placeholders");
	}

	[TestMethod]
	public async Task When_ContentTemplate_Changed_While_Loading()
	{
		var oneLine = XamlHelper.LoadXaml<DataTemplate>("""
			<DataTemplate>
				<StackPanel Width="200" Height="100">
					<TextBlock Text="{Binding}" />
				</StackPanel>
			</DataTemplate>
			""");
		var twoLines = XamlHelper.LoadXaml<DataTemplate>("""
			<DataTemplate>
				<StackPanel Width="200" Height="100">
					<TextBlock Text="{Binding}" />
					<TextBlock Text="{Binding}" />
				</StackPanel>
			</DataTemplate>
			""");
		var sut = new SkeletonView { EnableShimmer = false, Content = "data", ContentTemplate = oneLine };

		await UnitTestUIContentHelperEx.SetContentAndWait(sut);
		GetOverlay(sut).Children.Count.Should().Be(1);

		sut.ContentTemplate = twoLines;
		await UnitTestUIContentHelperEx.WaitForIdle();

		GetOverlay(sut).Children.Count.Should().Be(2, "the placeholders should follow the new template");
	}

	[TestMethod]
	public async Task When_Template_Swapped_While_Loading()
	{
		var text = new TextBlock { Text = "content" };
		var sut = new SkeletonView { Content = new StackPanel { Width = 200, Children = { text } } };

		await UnitTestUIContentHelperEx.SetContentAndWait(sut);
		GetOverlay(sut).Children.Count.Should().Be(1);

		// same parts as the default template, so the placeholders' geometry doesn't change
		sut.Template = XamlHelper.LoadXaml<ControlTemplate>("""
			<ControlTemplate TargetType="utu:SkeletonView">
				<Grid>
					<VisualStateManager.VisualStateGroups>
						<VisualStateGroup x:Name="SkeletonStates">
							<VisualState x:Name="SkeletonHidden" />
							<VisualState x:Name="SkeletonVisible">
								<VisualState.Setters>
									<Setter Target="PART_ContentPresenter.IsHitTestVisible" Value="False" />
									<Setter Target="PART_SkeletonOverlay.Visibility" Value="Visible" />
								</VisualState.Setters>
							</VisualState>
						</VisualStateGroup>
					</VisualStateManager.VisualStateGroups>
					<ContentPresenter x:Name="PART_ContentPresenter" Content="{TemplateBinding Content}" />
					<Canvas x:Name="PART_SkeletonOverlay" IsHitTestVisible="False" Visibility="Collapsed" />
				</Grid>
			</ControlTemplate>
			""");
		await UnitTestUIContentHelperEx.WaitForIdle();

		GetOverlay(sut).Children.Count.Should().Be(1, "the new template's overlay should be populated");
		text.Opacity.Should().Be(0);

		sut.IsLoading = false;
		await UnitTestUIContentHelperEx.WaitForIdle();

		text.Opacity.Should().Be(1);
		GetOverlay(sut).Children.Count.Should().Be(0);
	}

	private static (SkeletonView Outer, SkeletonView Inner, TextBlock Text) BuildNested()
	{
		var text = new TextBlock { Text = "nested" };
		var inner = new SkeletonView { EnableShimmer = false, Content = new StackPanel { Width = 200, Children = { text } } };
		var outer = new SkeletonView { EnableShimmer = false, Content = new StackPanel { Width = 300, Children = { new TextBlock { Text = "outer" }, inner } } };
		return (outer, inner, text);
	}

	// The view's own overlay: GetFirstDescendant is depth-first and would reach a nested view's overlay first.
	private static Canvas GetOwnOverlay(SkeletonView sut) =>
		((Panel)VisualTreeHelper.GetChild(sut, 0)).Children.OfType<Canvas>().First(x => x.Name == "PART_SkeletonOverlay");

	private sealed class SourceHost
	{
		public SourceHost(ILoadable loadable) => Loadable = loadable;

		public ILoadable Loadable { get; }
	}
}
