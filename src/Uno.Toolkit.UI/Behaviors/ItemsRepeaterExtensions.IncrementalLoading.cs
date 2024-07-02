using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.Disposables;
using Uno.Extensions;
using Uno.Logging;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;

#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using ItemsRepeater = Microsoft.UI.Xaml.Controls.ItemsRepeater;
using UniformGridLayout = Microsoft.UI.Xaml.Controls.UniformGridLayout;
using StackLayout = Microsoft.UI.Xaml.Controls.StackLayout;

#if HAS_UNO
using FlowLayout = Microsoft.UI.Xaml.Controls.FlowLayout;
#endif

#endif

namespace Uno.Toolkit.UI
{
	// Partial class to separate the IncrementalLoading behavior from the main ItemsRepeaterExtensions class
	partial class ItemsRepeaterExtensions
	{
		#region DependencyProperty: SupportsIncrementalLoading

		/// <summary>
		/// Gets or sets whether the ItemsRepeaterExtensions should automatically prefetch more items when the user scrolls to the end of the list.
		/// </summary>
		public static DependencyProperty SupportsIncrementalLoadingProperty { [DynamicDependency(nameof(GetSupportsIncrementalLoading))] get; } = DependencyProperty.RegisterAttached(
			"SupportsIncrementalLoading",
			typeof(bool),
			typeof(ItemsRepeaterExtensions),
			new PropertyMetadata(default(bool), OnSupportsIncrementalLoadingChanged));

		[DynamicDependency(nameof(SetSupportsIncrementalLoading))]
		public static bool GetSupportsIncrementalLoading(ItemsRepeater obj) => (bool)obj.GetValue(SupportsIncrementalLoadingProperty);
		[DynamicDependency(nameof(GetSupportsIncrementalLoading))]
		public static void SetSupportsIncrementalLoading(ItemsRepeater obj, bool value) => obj.SetValue(SupportsIncrementalLoadingProperty, value);

		#endregion

		#region DependencyProperty: IsLoading

		/// <summary>
		/// Gets whether there is currently a prefetch operation in progress.
		/// </summary>
		public static DependencyProperty IsLoadingProperty { [DynamicDependency(nameof(GetIsLoading))] get; } = DependencyProperty.RegisterAttached(
			"IsLoading",
			typeof(bool),
			typeof(ItemsRepeaterExtensions),
			new PropertyMetadata(default(bool)));

		[DynamicDependency(nameof(SetIsLoading))]
		public static bool GetIsLoading(ItemsRepeater obj) => (bool)obj.GetValue(IsLoadingProperty);
		[DynamicDependency(nameof(GetIsLoading))]
		public static void SetIsLoading(ItemsRepeater obj, bool value) => obj.SetValue(IsLoadingProperty, value);

		#endregion

		#region DependencyProperty: DataFetchSize
		/// <summary>
		/// Gets or sets the amount of data to fetch for prefetch operations, in pages.
		/// A "page" is defined as the estimated number of items that fit in the ItemsRepeater's EffectiveViewport.
		/// </summary>
		public static DependencyProperty DataFetchSizeProperty { [DynamicDependency(nameof(GetDataFetchSize))] get; } = DependencyProperty.RegisterAttached(
			"DataFetchSize",
			typeof(double),
			typeof(ItemsRepeaterExtensions),
			new PropertyMetadata(3d));

		[DynamicDependency(nameof(SetDataFetchSize))]
		public static double GetDataFetchSize(ItemsRepeater obj) => (double)obj.GetValue(DataFetchSizeProperty);
		[DynamicDependency(nameof(GetDataFetchSize))]
		public static void SetDataFetchSize(ItemsRepeater obj, double value) => obj.SetValue(DataFetchSizeProperty, value);

		#endregion

		#region DependencyProperty: IncrementalLoadingThreshold

		/// <summary>
		/// Gets or sets the loading threshold, in terms of pages, that governs when the ItemsRepeaterExtensions will begin to prefetch more items.
		/// A "page" is defined as the estimated number of items that fit in the ItemsRepeater's EffectiveViewport.
		/// </summary>
		public static DependencyProperty IncrementalLoadingThresholdProperty { [DynamicDependency(nameof(GetIncrementalLoadingThreshold))] get; } = DependencyProperty.RegisterAttached(
			"IncrementalLoadingThreshold",
			typeof(double),
			typeof(ItemsRepeaterExtensions),
			new PropertyMetadata(0d));

		[DynamicDependency(nameof(SetIncrementalLoadingThreshold))]
		public static double GetIncrementalLoadingThreshold(ItemsRepeater obj) => (double)obj.GetValue(IncrementalLoadingThresholdProperty);
		[DynamicDependency(nameof(GetIncrementalLoadingThreshold))]
		public static void SetIncrementalLoadingThreshold(ItemsRepeater obj, double value) => obj.SetValue(IncrementalLoadingThresholdProperty, value);

		#endregion

		#region DependencyProperty: IncrementalLoadingSubscription [Private]

		private static DependencyProperty IncrementalLoadingSubscriptionProperty { [DynamicDependency(nameof(GetIncrementalLoadingSubscription))] get; } = DependencyProperty.RegisterAttached(
			"IncrementalLoadingSubscription",
			typeof(IDisposable),
			typeof(ItemsRepeaterExtensions),
			new PropertyMetadata(default(IDisposable)));

		[DynamicDependency(nameof(SetIncrementalLoadingSubscription))]
		private static IDisposable GetIncrementalLoadingSubscription(ItemsRepeater obj) => (IDisposable)obj.GetValue(IncrementalLoadingSubscriptionProperty);
		[DynamicDependency(nameof(GetIncrementalLoadingSubscription))]
		private static void SetIncrementalLoadingSubscription(ItemsRepeater obj, IDisposable value) => obj.SetValue(IncrementalLoadingSubscriptionProperty, value);

		#endregion

		private static void OnSupportsIncrementalLoadingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not ItemsRepeater ir) return;

			if (e.NewValue is bool value && value)
			{
				ir.EffectiveViewportChanged += OnEffectiveViewportChanged;

				SetIncrementalLoadingSubscription(ir, Disposable.Create(() => ir.EffectiveViewportChanged -= OnEffectiveViewportChanged));
			}
			else
			{
				GetIncrementalLoadingSubscription(ir)?.Dispose();
			}
		}

		// The EffectiveViewport is the intersection of all known viewports that contain the ItemsRepeater in their sub-tree
		private static async void OnEffectiveViewportChanged(FrameworkElement sender, EffectiveViewportChangedEventArgs args)
		{
			if (!(sender is ItemsRepeater { ItemsSource: ISupportIncrementalLoading il } ir)) return;
			if (GetIsLoading(ir)) return;

			var orientation = GetOrientation(ir);

			var averageItemLength = GetAverageItemLength(ir, orientation);

			(double itemsRepeaterMajorSize, double viewPortLength, double viewportEdge) = orientation switch
			{
				Orientation.Vertical => (ir.ActualHeight, args.EffectiveViewport.Height, args.EffectiveViewport.Bottom),
				Orientation.Horizontal => (ir.ActualWidth, args.EffectiveViewport.Width, args.EffectiveViewport.Right),
				_ => throw new InvalidOperationException($"Invalid orientation value: {orientation}"),
			};

			// A "page" is defined as the estimated number of items that could fit in the ItemsRepeater's EffectiveViewport.
			var pageSize = averageItemLength > 0 ? viewPortLength / averageItemLength : 0d;

			var desiredItemBuffer = (GetIncrementalLoadingThreshold(ir) + 1) * pageSize;

			var distanceToEnd = itemsRepeaterMajorSize - viewportEdge.FiniteOrDefault(0d);

			var remainingItems = averageItemLength > 0 ? distanceToEnd / averageItemLength : 0d;

			if (remainingItems <= desiredItemBuffer && il.HasMoreItems)
			{
				SetIsLoading(ir, true);
				try
				{
					await il.LoadMoreItemsAsync((uint)(GetDataFetchSize(ir) * Math.Max(1, pageSize)));
				}
				catch (Exception e)
				{
					ir.Log().Error("An error occurred while loading more items", e);
				}
				finally
				{
					SetIsLoading(ir, false);
				}
			}
		}

		private static Orientation GetOrientation(ItemsRepeater ir)
		{
			if (TryGetOrientation(ir, out var orientation) && orientation is { } o)
			{
				return o;
			}

			var layout = ir.Layout;
			var property = layout.FindDependencyProperty<Orientation>("Orientation");
			return property != null && layout.GetValue(property) is Orientation orien ? orien : Orientation.Vertical;
		}

		private static bool TryGetOrientation(ItemsRepeater ir, out Orientation? orientation)
		{
			(bool result, orientation) = ir.Layout switch
			{
				StackLayout sl => (true, sl.Orientation),
				UniformGridLayout ugl => (true, ugl.Orientation),
#if HAS_UNO
				FlowLayout flow => (true, flow.Orientation),
#endif
				_ => (false, default)
			};

			return result;
		}

		private static double GetAverageItemLength(ItemsRepeater ir, Orientation orientation)
			=> ir.GetChildren()
				 .Select(x => (orientation == Orientation.Vertical ? (x as FrameworkElement)?.ActualHeight : (x as FrameworkElement)?.ActualWidth) ?? 0d)
				 .DefaultIfEmpty(0d)
				 .Average();
	}
}
