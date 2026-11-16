using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.UI;
using Uno.UI.RuntimeTests;

namespace Uno.Toolkit.RuntimeTests.Tests;

/// <summary>
/// Tests for <see cref="SkeletonPresenter"/>: skeletons derived from its template, and placeholder rows stamped into the template's empty lists.
/// </summary>
[TestClass]
[RunsOnUIThread]
public class SkeletonPresenterTests
{
	private const int DefaultPlaceholderCount = SkeletonPresenter.DefaultPlaceholderCount;

	private static Canvas GetOverlay(FrameworkElement root) =>
		root.GetFirstDescendant<Canvas>(x => x.Name == "PART_SkeletonOverlay") ?? throw new Exception("Failed to find PART_SkeletonOverlay");

	private static DataTemplate SingleLineTemplate() => XamlHelper.LoadXaml<DataTemplate>("""
		<DataTemplate>
			<StackPanel Width="300">
				<TextBlock Text="{Binding Name}" />
			</StackPanel>
		</DataTemplate>
		""");

	// an ItemsControl realizes items without container styling concerns, keeping the stamping assertion simple
	private static DataTemplate ItemsControlTemplate() => XamlHelper.LoadXaml<DataTemplate>("""
		<DataTemplate>
			<ItemsControl>
				<ItemsControl.ItemTemplate>
					<DataTemplate>
						<TextBlock Height="20" Text="{Binding Name}" />
					</DataTemplate>
				</ItemsControl.ItemTemplate>
			</ItemsControl>
		</DataTemplate>
		""");

	[TestMethod]
	public async Task When_ContentTemplate_Skeleton_Derived_From_It()
	{
		var presenter = new SkeletonPresenter { Width = 320, Height = 200, ContentTemplate = SingleLineTemplate() };

		await UnitTestUIContentHelperEx.SetContentAndWait(presenter);
		await UnitTestUIContentHelperEx.WaitFor(() => GetOverlay(presenter).Children.Count > 0, timeoutMS: 3000);

		GetOverlay(presenter).Children.Count.Should().Be(1, "the template has a single TextBlock leaf");
	}

	[TestMethod]
	public async Task When_No_ContentTemplate_Nothing_Generated()
	{
		var presenter = new SkeletonPresenter { Width = 320, Height = 200 };

		await UnitTestUIContentHelperEx.SetContentAndWait(presenter);
		await UnitTestUIContentHelperEx.WaitForIdle();

		GetOverlay(presenter).Children.Count.Should().Be(0, "without a template there is nothing to derive placeholders from (a warning is logged)");
	}

	[TestMethod]
	public async Task When_List_Template_Stamped_With_Default_Placeholder_Rows()
	{
		var presenter = new SkeletonPresenter { Width = 320, Height = 300, ContentTemplate = ItemsControlTemplate() };

		await UnitTestUIContentHelperEx.SetContentAndWait(presenter);
		await UnitTestUIContentHelperEx.WaitFor(() => GetOverlay(presenter).Children.Count == DefaultPlaceholderCount, timeoutMS: 3000);

		GetOverlay(presenter).Children.Count.Should().Be(DefaultPlaceholderCount, "the empty list control should be stamped with DefaultPlaceholderCount rows of one leaf each");
	}

	[TestMethod]
	public async Task When_Hosted_In_Control_Template_Count_From_Templated_Control()
	{
		// the SkeletonFeedViewStyle shape: the presenter's ContentTemplate is template-bound,
		// and PlaceholderCount is set on the templated control (attached properties can't be template-bound)
		var host = new ContentControl
		{
			Width = 320,
			Height = 300,
			ContentTemplate = ItemsControlTemplate(),
			Template = XamlHelper.LoadXaml<ControlTemplate>("""
				<ControlTemplate TargetType="ContentControl">
					<utu:SkeletonPresenter ContentTemplate="{TemplateBinding ContentTemplate}" />
				</ControlTemplate>
				"""),
		};
		Skeleton.SetPlaceholderCount(host, 2);

		await UnitTestUIContentHelperEx.SetContentAndWait(host);

		var presenter = host.GetFirstDescendant<SkeletonPresenter>() ?? throw new Exception("SkeletonPresenter was not created");
		await UnitTestUIContentHelperEx.WaitFor(() => GetOverlay(presenter).Children.Count > 0, timeoutMS: 3000);
		await UnitTestUIContentHelperEx.WaitForIdle();

		GetOverlay(presenter).Children.Count.Should().Be(2);
	}

	[TestMethod]
	public async Task When_PlaceholderCount_Set_On_Ancestor()
	{
		// e.g. SkeletonPresenter hosted in a control template, with the count set on the templated control
		var presenter = new SkeletonPresenter { ContentTemplate = ItemsControlTemplate() };
		var host = new Border { Width = 320, Height = 300, Child = presenter };
		Skeleton.SetPlaceholderCount(host, 2);

		await UnitTestUIContentHelperEx.SetContentAndWait(host);
		await UnitTestUIContentHelperEx.WaitFor(() => GetOverlay(presenter).Children.Count > 0, timeoutMS: 3000);
		await UnitTestUIContentHelperEx.WaitForIdle();

		GetOverlay(presenter).Children.Count.Should().Be(2, "the nearest ancestor setting PlaceholderCount should apply");
	}

	[TestMethod]
	public async Task When_Presenter_Has_No_Height_Constraint()
	{
		// e.g. in a vertical StackPanel: the empty list measures 0 tall until placeholder rows are stamped into it
		var presenter = new SkeletonPresenter { ContentTemplate = ItemsControlTemplate() };
		Skeleton.SetPlaceholderCount(presenter, 3);
		var host = new StackPanel { Width = 320, Children = { presenter } };

		await UnitTestUIContentHelperEx.SetContentAndWait(host);
		await UnitTestUIContentHelperEx.WaitFor(() => GetOverlay(presenter).Children.Count > 0, timeoutMS: 3000);
		await UnitTestUIContentHelperEx.WaitForIdle();

		GetOverlay(presenter).Children.Count.Should().Be(3, "rows should be stamped even though the empty list initially has no height");
	}

	[TestMethod]
	public async Task When_Inherited_DataContext_Placeholder_Rows_Stay_Empty()
	{
		const double TextWidth = 48;
		var presenter = new SkeletonPresenter
		{
			ContentTemplate = XamlHelper.LoadXaml<DataTemplate>("""
				<DataTemplate>
					<ItemsControl>
						<ItemsControl.ItemTemplate>
							<DataTemplate>
								<TextBlock Height="20" MinWidth="48" HorizontalAlignment="Left" Text="{Binding}" />
							</DataTemplate>
						</ItemsControl.ItemTemplate>
					</ItemsControl>
				</DataTemplate>
				"""),
		};
		Skeleton.SetPlaceholderCount(presenter, 2);
		// the page's view model: placeholder items must not bind to it
		var host = new Border { Width = 320, DataContext = "A page DataContext that is much wider than a row", Child = presenter };

		await UnitTestUIContentHelperEx.SetContentAndWait(host);
		await UnitTestUIContentHelperEx.WaitFor(() => GetOverlay(presenter).Children.Count == 2, timeoutMS: 3000);
		await UnitTestUIContentHelperEx.WaitForIdle();

		GetOverlay(presenter).Children.OfType<FrameworkElement>().Should().HaveCount(2)
			.And.OnlyContain(x => Math.Abs(x.Width - TextWidth) < 1.5, "placeholder rows should have empty values, not the inherited DataContext");
	}

	[TestMethod]
	public async Task When_Presenter_Direct_Content_Not_Stamped()
	{
		// direct content is live content: only a ContentTemplate instantiation gets placeholder rows stamped in
		var list = new ItemsControl { ItemTemplate = ItemsControlTemplate() };
		var presenter = new SkeletonPresenter { Width = 320, Height = 200, Content = list };

		await UnitTestUIContentHelperEx.SetContentAndWait(presenter);
		await UnitTestUIContentHelperEx.WaitForIdle();

		list.ItemsSource.Should().BeNull("the app's own list must never be mutated");
	}

	[TestMethod]
	public async Task When_Template_List_Has_Items_Not_Stamped()
	{
		var template = XamlHelper.LoadXaml<DataTemplate>("""
			<DataTemplate>
				<ItemsControl>
					<ItemsControl.ItemTemplate>
						<DataTemplate>
							<TextBlock Height="20" Text="{Binding}" />
						</DataTemplate>
					</ItemsControl.ItemTemplate>
					<x:String>one</x:String>
				</ItemsControl>
			</DataTemplate>
			""");
		var presenter = new SkeletonPresenter { Width = 320, Height = 200, ContentTemplate = template };

		await UnitTestUIContentHelperEx.SetContentAndWait(presenter);
		await UnitTestUIContentHelperEx.WaitFor(() => GetOverlay(presenter).Children.Count > 0, timeoutMS: 3000);
		await UnitTestUIContentHelperEx.WaitForIdle();

		GetOverlay(presenter).Children.Count.Should().Be(1, "a list that already has items is left alone");
	}
}
