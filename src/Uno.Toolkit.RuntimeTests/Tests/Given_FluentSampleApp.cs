using System.Collections;
using System.Reflection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.UI.RuntimeTests;

namespace Uno.Toolkit.RuntimeTests.Tests;

[TestClass]
[RunsOnUIThread]
public sealed class Given_FluentSampleApp
{
	private const string SharedAppName = "Uno.Toolkit.Samples.App";
	private const string LayoutName = "Uno.Toolkit.Samples.SamplePageLayout";
	private const string NavigationPageName = "Uno.Toolkit.Samples.Content.Controls.NavigationBarSamplePage";
	private const string CardPageName = "Uno.Toolkit.Samples.Content.Controls.CardSamplePage";
	private const string RuntimeRunnerPageName = "Uno.Toolkit.Samples.Content.RuntimeTestRunner";

	[TestMethod]
	public void When_FluentAppStarts_ItUsesTheSharedShellAndCatalog()
	{
		Assert.AreEqual(SharedAppName, Application.Current.GetType().FullName, "Each theme head must use the shared sample App.");
		var shell = RequireSharedType("Uno.Toolkit.Samples.Shell");
		Assert.IsTrue(typeof(FrameworkElement).IsAssignableFrom(shell), "The shared Shell must be available to the theme head.");
		var catalog = GetCatalog();
		Assert.IsTrue(catalog.Length > 0, "The shared catalog must contain samples.");
		if (HostDesign is "Fluent" or "Material")
		{
			CollectionAssert.Contains(catalog.Select(GetViewType).Select(type => type.FullName).ToArray(), NavigationPageName,
				"NavigationBar's existing template must be reachable through the shared catalog.");
		}
		var layout = RequireSharedType(LayoutName);
		var activeDesign = Require(layout.GetProperty("ActiveDesign", BindingFlags.Public | BindingFlags.Static), "SamplePageLayout.ActiveDesign is missing.");
		Assert.AreEqual(HostDesign == "Simple" ? "Agnostic" : HostDesign, activeDesign.GetValue(null)?.ToString(), "The head must select its own design at startup.");
	}

	[TestMethod]
	public async Task When_SharedPagesHaveFluentTemplates_CatalogIncludesAndSelectsEveryOne()
	{
		var layoutType = RequireSharedType(LayoutName);
		var design = HostDesign == "Simple" ? "Agnostic" : HostDesign;
		var activePresenterMethod = Require(layoutType.GetMethod("GetActivePresenter"), "SamplePageLayout.GetActivePresenter is missing.");
		var catalog = GetCatalog();
		var themedPageCount = 0;
		var loadedPageCount = 0;
		var runnerPageCount = 0;
		var fluentPageCount = 0;
		var previousContent = UnitTestsUIContentHelper.Content;
		var activeDesign = Require(layoutType.GetProperty("ActiveDesign", BindingFlags.Public | BindingFlags.Static), "ActiveDesign is missing.");
		var previousDesign = activeDesign.GetValue(null);

		try
		{
			foreach (var sample in catalog)
			{
				var pageType = GetViewType(sample);
				if (pageType.FullName == RuntimeRunnerPageName)
				{
					// A second UnitTestsControl replaces the engine's global UI helper.
					// The current host already exercises runner startup; validate its catalog entry here.
					Assert.IsTrue(typeof(Page).IsAssignableFrom(pageType));
					Assert.IsNotNull(pageType.GetConstructor(Type.EmptyTypes));
					runnerPageCount++;
					continue;
				}
				var page = Require(Activator.CreateInstance(pageType) as Page, $"Could not create {pageType.Name}.");
				var layout = FindLayout(page, layoutType);
				page.DataContext = sample;
				UnitTestsUIContentHelper.Content = page;
				await UnitTestsUIContentHelper.WaitForLoaded(page);
				await UnitTestsUIContentHelper.WaitForIdle();
				loadedPageCount++;
				Assert.IsTrue(VisualTreeHelper.GetChildrenCount(page) > 0, $"{pageType.Name} did not realize a visual tree.");
				Assert.IsTrue(page.ActualHeight > 0, $"{pageType.Name} has no visible height.");
				if (layout is null)
				{
					continue;
				}

				var pageDesign = GetProperty(layout, "IsDesignAgnostic") is true ? "Agnostic" : design;
				if (HostDesign == "Fluent")
				{
					Assert.IsNull(GetProperty(layout, "MaterialTemplate"), "Fluent must not instantiate hidden Material content.");
					Assert.IsNull(GetProperty(layout, "M3MaterialTemplate"), "Fluent must not instantiate hidden M3 content.");
					Assert.IsNull(GetProperty(layout, "CupertinoTemplate"), "Fluent must not instantiate hidden Cupertino content.");
				}
				var templateName = pageDesign == "Agnostic" ? "DesignAgnosticTemplate" : pageDesign + "Template";
				var template = Require(GetProperty(layout, templateName) as DataTemplate, $"{pageType.Name} has no {templateName} for the active host.");
				themedPageCount++;
				if (pageDesign == "Fluent")
				{
					fluentPageCount++;
				}
				var presenter = Require(activePresenterMethod.Invoke(layout, null) as ContentPresenter, $"{pageType.Name} has no active presenter.");
				if (pageDesign == "Material")
				{
					CollectionAssert.Contains(new[] { "M2MaterialContentPresenter", "M3MaterialContentPresenter" }, presenter.Name,
						$"{pageType.Name} selected a different design.");
					var m3Template = layoutType.GetProperty("M3MaterialTemplate")?.GetValue(layout);
					Assert.IsTrue(ReferenceEquals(template, presenter.ContentTemplate) || ReferenceEquals(m3Template, presenter.ContentTemplate),
						$"{pageType.Name} did not apply a Material template.");
				}
				else
				{
					Assert.AreEqual(pageDesign + "ContentPresenter", presenter.Name, $"{pageType.Name} selected a different design.");
					Assert.AreSame(template, presenter.ContentTemplate, $"{pageType.Name} did not apply its {templateName}.");
				}
				Assert.AreEqual(Visibility.Visible, presenter.Visibility, $"{pageType.Name} hides its {pageDesign} presenter.");
				Assert.IsTrue(VisualTreeHelper.GetChildrenCount(presenter) > 0, $"{pageType.Name} did not realize its {pageDesign} template.");
			}
			Assert.AreEqual(catalog.Length, loadedPageCount + runnerPageCount, "Every catalog entry must be verified.");
			Assert.IsTrue(themedPageCount > 0, $"The shared sample project must contribute its existing {design} templates.");
			if (HostDesign == "Fluent")
			{
				Assert.IsTrue(fluentPageCount > 0, "The Fluent catalog must include genuine Fluent templates as well as agnostic samples.");
			}
		}
		finally
		{
			UnitTestsUIContentHelper.Content = previousContent;
			activeDesign.SetValue(null, previousDesign);
		}
	}

	[TestMethod]
	public async Task When_SharedShellIsBuilt_ItLoadsTheThemeLandingPage()
	{
		var app = Application.Current;
		var appType = RequireSharedType(SharedAppName);
		var buildShell = Require(appType.GetMethod("BuildShell", BindingFlags.Instance | BindingFlags.NonPublic), "The head must reuse the shared BuildShell implementation.");
		var shellField = Require(appType.GetField("_shell", BindingFlags.Instance | BindingFlags.NonPublic), "Shared navigation requires the app's shell field.");
		var previousShell = shellField.GetValue(app);
		var previousContent = UnitTestsUIContentHelper.Content;
		try
		{
			var shell = Require(buildShell.Invoke(app, null) as FrameworkElement, "BuildShell did not create the shared shell.");
			UnitTestsUIContentHelper.Content = shell;
			await UnitTestsUIContentHelper.WaitForLoaded(shell);
			await UnitTestsUIContentHelper.WaitForIdle();
			var navigation = Require(GetProperty(shell, "NavigationView") as NavigationView, "The shared shell must expose its navigation catalog.");
			Assert.IsTrue(navigation.MenuItems.Count > 0, "BuildShell did not populate the sample navigation menu.");
			var page = Require(navigation.Content as Page, "BuildShell did not navigate to a landing page.");
			var expectedLandingPage = HostDesign is "Fluent" or "Material" ? NavigationPageName : "Uno.Toolkit.Samples.Content.Controls.TabBarSamplePage";
			Assert.AreEqual(expectedLandingPage, page.GetType().FullName);
			Assert.IsTrue(page.IsLoaded, "The landing page must load inside the shared shell.");
			Assert.IsTrue(VisualTreeHelper.GetChildrenCount(page) > 0, "The landing page must realize content.");
		}
		finally
		{
			UnitTestsUIContentHelper.Content = previousContent;
			shellField.SetValue(app, previousShell);
		}
	}

	[TestMethod]
	public void When_CatalogIsFiltered_OnlyFluentAndAgnosticPagesRemain()
	{
		var catalog = GetCatalog();
		var pageNames = catalog.Select(GetViewType).Select(type => type.FullName).ToArray();
		Assert.IsTrue(catalog.Length > 0, "Filtering must retain supported samples.");
		if (HostDesign == "Fluent")
		{
			CollectionAssert.Contains(pageNames, CardPageName, "The shared Card sample declares Agnostic support and must remain reachable.");
			CollectionAssert.DoesNotContain(pageNames, "Uno.Toolkit.Samples.Content.OverviewPage", "Overview has no Fluent or Agnostic design.");
			CollectionAssert.DoesNotContain(pageNames, "Uno.Toolkit.Samples.Content.Controls.SeedColorSamplePage", "SeedColor has no Fluent or Agnostic design.");
		}
		foreach (var sample in catalog)
		{
			var designs = Require(GetProperty(sample, "SupportedDesigns") as IEnumerable, "A sample has no SupportedDesigns metadata.");
			var supportsHost = designs.Cast<object>().Any(design => design.ToString() == HostDesign || design.ToString() == "Agnostic");
			var sourceExemption = HostDesign != "Fluent" && GetProperty(sample, "Source")?.ToString() is "WinUI" or "Uno" or "UnoToolkit";
			Assert.IsTrue(supportsHost || sourceExemption, $"{GetViewType(sample).Name} is not supported by the {HostDesign} catalog.");
		}
	}

	private static string HostDesign => Application.Current.GetType().Assembly.GetName().Name switch
	{
		"FluentSampleApp" => "Fluent",
		"MaterialSampleApp" => "Material",
		"CupertinoSampleApp" => "Cupertino",
		"SimpleSampleApp" => "Simple",
		var name => throw new AssertFailedException($"Unrecognized sample host: {name}."),
	};

	private static object[] GetCatalog()
	{
		var app = RequireSharedType(SharedAppName);
		var method = Require(app.GetMethod("GetSamples", BindingFlags.Public | BindingFlags.Static), "The Fluent head must import App.GetSamples from the shared sample project.");
		var catalog = Require(method.Invoke(null, null) as IEnumerable, "App.GetSamples did not return a catalog.");
		return catalog.Cast<object>().ToArray();
	}

	private static Type GetViewType(object sample)
		=> Require(GetProperty(sample, "ViewType") as Type, "A sample catalog entry has no ViewType.");

	private static object? GetProperty(object instance, string name)
		=> Require(instance.GetType().GetProperty(name), $"{instance.GetType().Name}.{name} is missing.").GetValue(instance);

	private static Type RequireSharedType(string name)
		=> Require(Application.Current.GetType().Assembly.GetType(name), $"The Fluent head must import the shared sample project: {name} is missing.");

	private static T Require<T>(T? value, string message) where T : class
		=> value ?? throw new AssertFailedException(message);

	private static FrameworkElement? FindLayout(DependencyObject? element, Type layoutType)
	{
		if (element is FrameworkElement layout && layoutType.IsInstanceOfType(layout))
		{
			return layout;
		}
		if (element is Page page)
		{
			return FindLayout(page.Content as DependencyObject, layoutType);
		}
		if (element is UserControl userControl)
		{
			return FindLayout(userControl.Content as DependencyObject, layoutType);
		}
		if (element is ContentControl content && content.Content is DependencyObject child)
		{
			return FindLayout(child, layoutType);
		}
		if (element is Border border)
		{
			return FindLayout(border.Child, layoutType);
		}
		if (element is Panel panel)
		{
			foreach (var panelChild in panel.Children)
			{
				if (FindLayout(panelChild, layoutType) is { } nestedLayout)
				{
					return nestedLayout;
				}
			}
		}
		return null;
	}
}
