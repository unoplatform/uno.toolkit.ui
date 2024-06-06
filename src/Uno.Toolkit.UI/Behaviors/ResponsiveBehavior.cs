using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
#endif

namespace Uno.Toolkit.UI;

public static class ResponsiveBehavior
{
	#region DependencyProperty: IsEnabled

	public static DependencyProperty IsEnabledProperty { [DynamicDependency(nameof(GetIsEnabled))] get; } = DependencyProperty.RegisterAttached(
		"IsEnabled",
		typeof(bool),
		typeof(ResponsiveBehavior),
		new PropertyMetadata(default(bool), OnIsEnabledChanged));

	[DynamicDependency(nameof(SetIsEnabled))]
	public static bool GetIsEnabled(DependencyObject obj) => (bool)obj.GetValue(IsEnabledProperty);
	[DynamicDependency(nameof(GetIsEnabled))]
	public static void SetIsEnabled(DependencyObject obj, bool value) => obj.SetValue(IsEnabledProperty, value);

	#endregion

	internal static bool IsChildSupported(DependencyObject? child) => child is
	(
		ColumnDefinition or RowDefinition or
		Inline or
		Microsoft.UI.Xaml.Controls.Layout
	);

	private static void OnIsEnabledChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
	{
		if (false) { }
		else if (sender is Grid g) g.Loaded += OnGridLoaded;
		else if (sender is TextBlock tb) tb.Loaded += OnTextBlockLoaded;
		else if (sender is ItemsRepeater ir) ir.Loaded += OnItemsRepeaterLoaded;
		else
		{
			throw new NotSupportedException($"ResponsiveBehavior is not supported on '{sender.GetType()}'.");
		}
	}

	private static void OnGridLoaded(object sender, RoutedEventArgs e)
	{
		if (sender is not Grid host) return;

		var markups = Enumerable
			.Concat<DependencyObject>(host.ColumnDefinitions, host.RowDefinitions)
			.SelectMany(ResponsiveExtension.GetAllInstancesFor);
		foreach (var markup in markups)
		{
			markup.InitializeByProxy(host);
		}
	}

	private static void OnTextBlockLoaded(object sender, RoutedEventArgs e)
	{
		if (sender is not TextBlock host) return;

		// There is actually more elements than what's defined in the xaml;
		// these come from the whitespaces... and, they are fine.
		var markups = FlattenInlines(host)
			.SelectMany(ResponsiveExtension.GetAllInstancesFor);
		foreach (var markup in markups)
		{
			markup.InitializeByProxy(host);
		}
	}

	private static void OnItemsRepeaterLoaded(object sender, RoutedEventArgs e)
	{
		if (sender is not ItemsRepeater host) return;
		if (host.Layout is null) return;

		var markups = ResponsiveExtension.GetAllInstancesFor(host.Layout);
		foreach (var markup in markups)
		{
			markup.InitializeByProxy(host);
		}
	}


	private static IEnumerable<Inline> FlattenInlines(TextBlock tb) => FlattenInlines(tb.Inlines);
	private static IEnumerable<Inline> FlattenInlines(InlineCollection inlines)
	{
		foreach (var inline in inlines)
		{
			yield return inline;

			if (inline is Span span)
			{
				foreach (var nested in FlattenInlines(span.Inlines))
				{
					yield return nested;
				}
			}
		}
	}
}
