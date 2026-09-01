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
/// Tests for Skeleton.IsEnabled auto-injection against a control mimicking FeedView's shape
/// (ValueTemplate/ProgressTemplate resolved by convention — the toolkit has no Uno.Extensions reference).
/// </summary>
[TestClass]
[RunsOnUIThread]
public class SkeletonInjectionTests
{
	private const int DefaultPlaceholderCount = 4; // keep in sync with Skeleton.PlaceholderCountProperty default

	private static Canvas GetOverlay(FrameworkElement root) =>
		root.GetFirstDescendant<Canvas>(x => x.Name == "PART_SkeletonOverlay") ?? throw new Exception("Failed to find PART_SkeletonOverlay");

	private static DataTemplate SingleLineValueTemplate() => XamlHelper.LoadXaml<DataTemplate>("""
		<DataTemplate>
			<StackPanel Width="300">
				<TextBlock Text="{Binding Name}" />
			</StackPanel>
		</DataTemplate>
		""");

	// an ItemsControl realizes items without container styling concerns, keeping the stamping assertion simple
	private static DataTemplate ItemsControlValueTemplate() => XamlHelper.LoadXaml<DataTemplate>("""
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
	public void When_IsEnabled_Injects_And_Removes()
	{
		var stub = new FeedViewStub { ValueTemplate = SingleLineValueTemplate() };

		Skeleton.SetIsEnabled(stub, true);
		stub.ProgressTemplate.Should().NotBeNull("enabling should inject a ProgressTemplate");

		Skeleton.SetIsEnabled(stub, false);
		stub.ProgressTemplate.Should().BeNull("disabling should remove only the template it injected");
	}

	[TestMethod]
	public void When_User_ProgressTemplate_Preserved()
	{
		var userTemplate = SingleLineValueTemplate();
		var stub = new FeedViewStub { ProgressTemplate = userTemplate };

		Skeleton.SetIsEnabled(stub, true);
		stub.ProgressTemplate.Should().BeSameAs(userTemplate, "an explicitly assigned ProgressTemplate must not be overwritten");

		Skeleton.SetIsEnabled(stub, false);
		stub.ProgressTemplate.Should().BeSameAs(userTemplate);
	}

	[TestMethod]
	public async Task When_Progress_Skeleton_Derived_From_ValueTemplate()
	{
		var stub = new FeedViewStub { Width = 320, Height = 200, ValueTemplate = SingleLineValueTemplate() };
		Skeleton.SetIsEnabled(stub, true);

		await UnitTestUIContentHelperEx.SetContentAndWait(stub);
		stub.ShowProgress();
		await UnitTestUIContentHelperEx.WaitForIdle();

		var presenter = stub.GetFirstDescendant<SkeletonPresenter>() ?? throw new Exception("SkeletonPresenter was not injected");
		await UnitTestUIContentHelperEx.WaitFor(() => GetOverlay(presenter).Children.Count > 0, timeoutMS: 3000);

		GetOverlay(presenter).Children.Count.Should().Be(1, "the ValueTemplate has a single TextBlock leaf");
	}

	[TestMethod]
	public async Task When_List_ValueTemplate_Stamped_With_Placeholder_Rows()
	{
		var stub = new FeedViewStub { Width = 320, Height = 300, ValueTemplate = ItemsControlValueTemplate() };
		Skeleton.SetIsEnabled(stub, true);

		await UnitTestUIContentHelperEx.SetContentAndWait(stub);
		stub.ShowProgress();
		await UnitTestUIContentHelperEx.WaitForIdle();

		var presenter = stub.GetFirstDescendant<SkeletonPresenter>() ?? throw new Exception("SkeletonPresenter was not injected");
		await UnitTestUIContentHelperEx.WaitFor(() => GetOverlay(presenter).Children.Count == DefaultPlaceholderCount, timeoutMS: 3000);

		GetOverlay(presenter).Children.Count.Should().Be(DefaultPlaceholderCount, "the empty list control should be stamped with PlaceholderCount rows of one leaf each");
	}

	[TestMethod]
	public async Task When_PlaceholderCount_Attached_On_Owner()
	{
		var stub = new FeedViewStub { Width = 320, Height = 300, ValueTemplate = ItemsControlValueTemplate() };
		Skeleton.SetIsEnabled(stub, true);
		Skeleton.SetPlaceholderCount(stub, 2);

		await UnitTestUIContentHelperEx.SetContentAndWait(stub);
		stub.ShowProgress();
		await UnitTestUIContentHelperEx.WaitForIdle();

		var presenter = stub.GetFirstDescendant<SkeletonPresenter>() ?? throw new Exception("SkeletonPresenter was not injected");
		await UnitTestUIContentHelperEx.WaitFor(() => GetOverlay(presenter).Children.Count == 2, timeoutMS: 3000);

		GetOverlay(presenter).Children.Count.Should().Be(2);
	}

	[TestMethod]
	public async Task When_PlaceholderTemplate_Overrides_ValueTemplate()
	{
		var placeholderTemplate = XamlHelper.LoadXaml<DataTemplate>("""
			<DataTemplate>
				<StackPanel Width="300">
					<TextBlock Text="{Binding A}" />
					<TextBlock Text="{Binding B}" />
				</StackPanel>
			</DataTemplate>
			""");
		var stub = new FeedViewStub { Width = 320, Height = 200, ValueTemplate = SingleLineValueTemplate() };
		Skeleton.SetIsEnabled(stub, true);
		Skeleton.SetPlaceholderTemplate(stub, placeholderTemplate);

		await UnitTestUIContentHelperEx.SetContentAndWait(stub);
		stub.ShowProgress();
		await UnitTestUIContentHelperEx.WaitForIdle();

		var presenter = stub.GetFirstDescendant<SkeletonPresenter>() ?? throw new Exception("SkeletonPresenter was not injected");
		await UnitTestUIContentHelperEx.WaitFor(() => GetOverlay(presenter).Children.Count > 0, timeoutMS: 3000);

		GetOverlay(presenter).Children.Count.Should().Be(2, "the skeleton should derive from the PlaceholderTemplate (two leaves), not the ValueTemplate (one)");
	}
}

/// <summary>
/// Mimics FeedView's public shape: ValueTemplate + ProgressTemplate exposed as conventional
/// static DP fields, with the active template swapped like FeedView's visual states do.
/// </summary>
public sealed partial class FeedViewStub : ContentControl
{
	public static readonly DependencyProperty ValueTemplateProperty = DependencyProperty.Register(
		nameof(ValueTemplate), typeof(DataTemplate), typeof(FeedViewStub), new PropertyMetadata(null));

	public static readonly DependencyProperty ProgressTemplateProperty = DependencyProperty.Register(
		nameof(ProgressTemplate), typeof(DataTemplate), typeof(FeedViewStub), new PropertyMetadata(null));

	public DataTemplate? ValueTemplate
	{
		get => (DataTemplate?)GetValue(ValueTemplateProperty);
		set => SetValue(ValueTemplateProperty, value);
	}

	public DataTemplate? ProgressTemplate
	{
		get => (DataTemplate?)GetValue(ProgressTemplateProperty);
		set => SetValue(ProgressTemplateProperty, value);
	}

	public FeedViewStub()
	{
		HorizontalContentAlignment = HorizontalAlignment.Stretch;
		VerticalContentAlignment = VerticalAlignment.Stretch;
		Content = new object(); // ensure the ContentTemplate materializes
	}

	public void ShowProgress() => ContentTemplate = ProgressTemplate;
	public void ShowValue() => ContentTemplate = ValueTemplate;
}
