﻿using System;
using System.Linq;
using System.Reflection;
using Uno.Extensions;
using Uno.Toolkit.Samples.Entities;
#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
#endif

using MUXC = Microsoft.UI.Xaml.Controls;
using MUXCP = Microsoft.UI.Xaml.Controls.Primitives;
using Uno.Toolkit.Samples.Content.Controls;
using Uno.Toolkit.Samples.Helpers;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using VisualTreeHelperEx = Uno.Toolkit.Samples.Helpers.VisualTreeHelperEx;
using Uno.Toolkit.Samples.Content.NestedSamples;
using Uno.Toolkit.Samples.Content;

namespace Uno.Toolkit.Samples
{
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
			throw new Exception($"Failed to load {e.SourcePageType.FullName}: {e.Exception}");
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
			ShellNavigateTo<TabBarBehaviorSamplePage>(
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
			var sample = GetSamples()
				.FirstOrDefault(x => string.Equals(x.Title, sampleName, StringComparison.OrdinalIgnoreCase));

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
		}

		private void AddNavigationItems(MUXC.NavigationView nv)
		{
			var categories = GetSamples()
				.OrderByDescending(x => x.SortOrder.HasValue)
				.ThenBy(x => x.SortOrder)
				.ThenBy(x => x.Title)
				.GroupBy(x => x.Category);

			foreach (var category in categories.OrderBy(x => x.Key))
			{
				var tier = 1;

				var parentItem = default(MUXC.NavigationViewItem);
				if (category.Key != SampleCategory.None)
				{
					parentItem = new MUXC.NavigationViewItem
					{
						Content = category.Key.GetDescription() ?? category.Key.ToString(),
						SelectsOnInvoked = false,
					}.Apply(NavViewItemVisualStateFix);
					AutomationProperties.SetAutomationId(parentItem, "Section_" + parentItem.Content);

					nv.MenuItems.Add(parentItem);
				}

				foreach (var sample in category)
				{
					var item = new MUXC.NavigationViewItem
					{
						Content = sample.Title,
						DataContext = sample,
					}.Apply(NavViewItemVisualStateFix);
					AutomationProperties.SetAutomationId(item, "Section_" + item.Content);

					(parentItem?.MenuItems ?? nv.MenuItems).Add(item);
				}
			}

			void NavViewItemVisualStateFix(MUXC.NavigationViewItem nvi)
			{
				// gallery#107: on uwp and uno, deselecting a NVI by selecting another NVI will leave the former in the "Selected" state
				// to workaround this, we force reset the visual state when IsSelected becomes false
				nvi.RegisterPropertyChangedCallback(MUXC.NavigationViewItemBase.IsSelectedProperty, (s, e) =>
				{
					if (!nvi.IsSelected)
					{
						// depending on the DisplayMode, a NVIP may or may not be used.
						var nvip = VisualTreeHelperEx.GetFirstDescendant<MUXCP.NavigationViewItemPresenter>(nvi, x => x.Name == "NavigationViewItemPresenter");
						VisualStateManager.GoToState((Control)nvip ?? nvi, "Normal", true);
					}
				});
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
}
