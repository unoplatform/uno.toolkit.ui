using System;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Uno.Disposables;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.UI;

public static partial class AutoGrid;

partial class AutoGrid
{
	#region DependencyProperty: Enable

	public static DependencyProperty EnableProperty { [DynamicDependency(nameof(GetEnable))] get; } = DependencyProperty.RegisterAttached(
		"Enable",
		typeof(bool),
		typeof(AutoGrid),
		new PropertyMetadata(default(bool), OnEnableChanged));

	/// <summary>
	/// Gets or sets whether the AutoGrid behavior is enabled on the specified Grid.
	/// </summary>
	[DynamicDependency(nameof(SetEnable))]
	public static bool GetEnable(Grid obj) => (bool)obj.GetValue(EnableProperty);
	[DynamicDependency(nameof(GetEnable))]
	public static void SetEnable(Grid obj, bool value) => obj.SetValue(EnableProperty, value);

	#endregion
	#region DependencyProperty: Orientation = Horizontal

	public static DependencyProperty OrientationProperty { [DynamicDependency(nameof(GetOrientation))] get; } = DependencyProperty.RegisterAttached(
		"Orientation",
		typeof(Orientation),
		typeof(AutoGrid),
		new PropertyMetadata(Orientation.Horizontal, OnOrientationChanged));

	/// <summary>
	/// Gets or sets the orientation in which the AutoGrid fills its children first.
	/// </summary>
	[DynamicDependency(nameof(SetOrientation))]
	public static Orientation GetOrientation(Grid obj) => (Orientation)obj.GetValue(OrientationProperty);
	[DynamicDependency(nameof(GetOrientation))]
	public static void SetOrientation(Grid obj, Orientation value) => obj.SetValue(OrientationProperty, value);

	#endregion
	#region DependencyProperty: [private] Subscription

	private static DependencyProperty SubscriptionProperty { [DynamicDependency(nameof(GetSubscription))] get; } = DependencyProperty.RegisterAttached(
		"Subscription",
		typeof(IDisposable),
		typeof(AutoGrid),
		new PropertyMetadata(default(IDisposable)));

	[DynamicDependency(nameof(SetSubscription))]
	private static IDisposable? GetSubscription(Grid obj) => (IDisposable?)obj.GetValue(SubscriptionProperty);
	[DynamicDependency(nameof(GetSubscription))]
	private static void SetSubscription(Grid obj, IDisposable? value) => obj.SetValue(SubscriptionProperty, value);

	#endregion

	private static void OnEnableChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
	{
#if !HAS_UNO
		throw new PlatformNotSupportedException("AutoGrid is not supported on WinUI, since we cannot subscribe to vector changes on Panel.Children.");
#else
		if (sender is Grid g)
		{
			if (GetSubscription(g) is { } subscription)
			{
				subscription.Dispose();
				SetSubscription(g, null);
			}
			if (e.NewValue is true)
			{
				void OnGridChildrenChanged(object? sender, NotifyCollectionChangedEventArgs e)
				{
					RefreshLayout(g, e);
				}
				g.Children.CollectionChanged += OnGridChildrenChanged;
				SetSubscription(g, Disposable.Create(() => g.Children.CollectionChanged -= OnGridChildrenChanged));
			}
			if (e.OldValue is false)
			{
				RefreshLayout(g);
			}
		}
#endif
	}
	private static void OnOrientationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
	{
		if (sender is Grid g)
		{
			RefreshLayout(g);
		}
	}
	private static void RefreshLayout(Grid g, NotifyCollectionChangedEventArgs? e = default)
	{
		if (GetEnable(g) is not true) return;

		void SetCoordinates(UIElement? uie, int row, int column)
		{
			if (uie is not FrameworkElement fe) return;

			Grid.SetRow(fe, row);
			Grid.SetColumn(fe, column);
		}

		if (e?.OldItems is { Count: > 0 } removed)
		{
			foreach (var item in removed)
			{
				SetCoordinates(item as UIElement, 0, 0);
			}
		}

		var endIndex = g.Children.Count - 1;
		var startIndex = e?.Action switch
		{
			NotifyCollectionChangedAction.Add => e.NewStartingIndex == -1 ? endIndex : e.NewStartingIndex,
			NotifyCollectionChangedAction.Remove => e.OldStartingIndex == -1 ? endIndex : e.OldStartingIndex,
			_ => 0,
		};
		var rowCount = Math.Max(g.RowDefinitions.Count, 1);
		var columnCount = Math.Max(g.ColumnDefinitions.Count, 1);
		var fillByColumnsFirst = GetOrientation(g) is Orientation.Horizontal;

		for(var i = startIndex; i <= endIndex; i++)
		{
			var (row, column) = fillByColumnsFirst
				? (i / columnCount, i % columnCount)
				: (i % rowCount, i / rowCount);

			SetCoordinates(g.Children[i], row, column);
		}
	}
}
