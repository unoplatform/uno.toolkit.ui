using System.Reflection;
using System.Reflection;
using Uno.Extensions;

using MUXC = Microsoft.UI.Xaml.Controls;

namespace Uno.Toolkit.Samples;

partial class App
{
	private static Sample[] _samples;
	private static IDictionary<string, Type> _nestedSampleMap;

	/// <summary>
	/// Invoked when Navigation to a certain page fails
	/// </summary>
	/// <param name="sender">The Frame which failed navigation</param>
	/// <param name="e">Details about the navigation failure</param>
	void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
	{
		throw new InvalidOperationException($"Failed to load {e.SourcePageType.FullName}: {e.Exception}");
	}

	public void ShellNavigateTo(Sample sample) => ShellNavigateTo(sample, trySynchronizeCurrentItem: true);

	private void ShellNavigateTo<TPage>(bool trySynchronizeCurrentItem = true) where TPage : Page
	{
		var type = typeof(TPage);
		var attribute = type.GetCustomAttribute<SamplePageAttribute>()
			?? throw new NotSupportedException($"{type} isn't tagged with [{nameof(SamplePageAttribute)}].");
		var sample = new Sample(attribute, type);

		ShellNavigateTo(sample, trySynchronizeCurrentItem);
	}

	private void ShellNavigateTo(Sample sample, bool trySynchronizeCurrentItem)
	{
		var nv = _shell.NavigationView;
		if (nv.Content?.GetType() != sample.ViewType)
		{
			var selected = trySynchronizeCurrentItem
				? HierarchyHelper
					.Flatten(nv.MenuItems.OfType<MUXC.NavigationViewItem>(), x => x.MenuItems.OfType<MUXC.NavigationViewItem>())
					.FirstOrDefault(x => (x.DataContext as Sample)?.ViewType == sample.ViewType)
				: default;
			if (selected != null)
			{
				nv.SelectedItem = selected;
			}

			var page = (Page)Activator.CreateInstance(sample.ViewType);
			page.DataContext = sample;


			_shell.NavigationView.Content = page;
		}
	}

	private Shell BuildShell()
	{
		_shell = new Shell();
		AutomationProperties.SetAutomationId(_shell, "AppShell");
		var nv = _shell.NavigationView;
		AddNavigationItems(nv);

		// landing navigation
		ShellNavigateTo<NavigationBarSamplePage>(
#if WINDOWS_UWP
			// note: on uwp, NavigationView.SelectedItem MUST be set on launch to avoid entering compact-mode
			trySynchronizeCurrentItem: true
#else
			// workaround for uno#5069: setting NavView.SelectedItem at launch bricks it
			trySynchronizeCurrentItem: false
#endif
		);

		// navigation + setting handler
		nv.ItemInvoked += OnNavigationItemInvoked;

		return _shell;
	}

#if USE_UITESTS
	private void ForceSampleNavigation(string sampleName)
	{
		var backdoorParts = sampleName.Split("-");
		var title = backdoorParts.FirstOrDefault();
		var designName = backdoorParts.Length > 1 ? backdoorParts[1] : string.Empty;

		var sample = GetSamples()
			.FirstOrDefault(x => string.Equals(x.Title, title, StringComparison.OrdinalIgnoreCase));

		if (Enum.TryParse<Design>(designName, out var design))
		{
			SamplePageLayout.SetPreferredDesign(design);
		}

		if (sample == null)
		{
			typeof(App).Log().LogWarning($"No SampleAttribute found with a Title that matches: {sampleName}");
			return;
		}

		var page = (Page)Activator.CreateInstance(sample.ViewType);
		page.DataContext = sample;

		_shell.NavigationView.Content = page;
	}

	private void NavigateToNestedSampleCore(string sampleName)
	{
		if (GetNestedSamples().TryGetValue(sampleName, out var pageType))
		{
			_shell.ShowNestedSample(pageType, clearStack: true);
		}
	}
#endif

	private void OnNavigationItemInvoked(MUXC.NavigationView sender, MUXC.NavigationViewItemInvokedEventArgs e)
	{
		if (e.InvokedItemContainer.DataContext is Sample sample)
		{
			ShellNavigateTo(sample, trySynchronizeCurrentItem: false);
		}
		else if (e.IsSettingsInvoked)
		{
			_shell.ActivateDebugPanel();
		}
	}

	private void AddNavigationItems(MUXC.NavigationView nv)
	{
		var categories = GetSamples()
			.OrderByDescending(x => x.SortOrder.HasValue)
			.ThenBy(x => x.SortOrder)
			.ThenBy(x => x.Title)
			.GroupBy(x => x.Category)
#if !DEBUG
			.Where(x => x.Key != SampleCategory.Tests)
#endif
			;

		foreach (var category in categories.OrderBy(x => x.Key))
		{
			var parentItem = default(MUXC.NavigationViewItem);
			if (category.Key != SampleCategory.None)
			{
				parentItem = new MUXC.NavigationViewItem
				{
					Content = category.Key.GetDescription() ?? category.Key.ToString(),
					Icon = CreateIconElement(GetCategoryIconSource(category.Key)),
					SelectsOnInvoked = false,
				};
				AutomationProperties.SetAutomationId(parentItem, "Section_" + parentItem.Content);

				nv.MenuItems.Add(parentItem);
			}

			foreach (var sample in category)
			{
				var item = new MUXC.NavigationViewItem
				{
					Content = sample.Title,
					Icon = CreateIconElement(sample.IconSource ?? GetCategoryItemIconSource(category.Key)),
					DataContext = sample,
				};
				AutomationProperties.SetAutomationId(item, "Section_" + item.Content);

				(parentItem?.MenuItems ?? nv.MenuItems).Add(item);
			}
		}

		object GetCategoryIconSource(SampleCategory category)
		{
			switch (category)
			{
				case SampleCategory.Behaviors: return Icons.Behaviors.CategoryHeader;
				case SampleCategory.Controls: return Icons.Controls.CategoryHeader;
				case SampleCategory.Helpers: return Icons.Helpers.CategoryHeader;
				case SampleCategory.Tests: return Icons.Tests.CategoryHeader;

				default: return Icons.Controls.CategoryHeader;
			}
		}
		object GetCategoryItemIconSource(SampleCategory category)
		{
			switch (category)
			{
				case SampleCategory.Behaviors: return Icons.Behaviors.Behavior;
				case SampleCategory.Controls: return Icons.Controls.Control;
				case SampleCategory.Tests: return Icons.Tests.Test;

				case SampleCategory.Helpers:
				default: return Icons.Placeholder;
			}
		}
		IconElement CreateIconElement(object source)
		{
			if (source is string path)
			{
				return new PathIcon()
				{
					Data = (Geometry)XamlBindingHelper.ConvertValue(typeof(Geometry), path)
				};
			}
			if (source is Symbol symbol && symbol != default)
			{
				return new SymbolIcon(symbol);
			}

			return new PathIcon()
			{
				Data = (Geometry)XamlBindingHelper.ConvertValue(typeof(Geometry), Icons.Placeholder)
			};
		}
	}

	public static IEnumerable<Sample> GetSamples()
		=> _samples = _samples ?? Assembly.GetExecutingAssembly()
			.DefinedTypes
			.Where(x => x.Namespace?.StartsWith("Uno.Toolkit.Samples") == true)
			.Select(x => new { TypeInfo = x, SamplePageAttribute = x.GetCustomAttribute<SamplePageAttribute>() })
			.Where(x => x.SamplePageAttribute != null)
			.Select(x => new Sample(x.SamplePageAttribute, x.TypeInfo.AsType()))
			.ToArray();

	public static IDictionary<string, Type> GetNestedSamples()
		=> _nestedSampleMap = _nestedSampleMap ?? Assembly.GetExecutingAssembly()
			.GetTypes()
			.Where(t => typeof(Page).IsAssignableFrom(t) && t.Namespace.Equals("Uno.Toolkit.Samples.Content.NestedSamples", StringComparison.OrdinalIgnoreCase))
			.ToDictionary(t => t.Name);
}
