using System;
using System.Diagnostics.CodeAnalysis;

#if IS_WINUI
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml;
#endif

namespace Uno.Toolkit.UI
{
	/// <summary>
	/// Defines the placeholder shape that skeleton generation produces for an element.
	/// </summary>
	public enum SkeletonShape
	{
		/// <summary>The shape is inferred from the element (an <c>Ellipse</c> becomes a circle, everything else a rounded rectangle).</summary>
		Auto,

		/// <summary>A rounded rectangle.</summary>
		Rectangle,

		/// <summary>A circle, sized to the smaller dimension of the element and centered within its bounds.</summary>
		Circle,
	}

	/// <summary>
	/// Provides attached properties to configure skeleton generation.
	/// </summary>
	public static class Skeleton
	{
		#region AttachedProperty: PlaceholderCount

		/// <summary>
		/// Backing property for how many placeholder rows are generated for empty list controls while loading. Default is 0 (no rows).
		/// </summary>
		[DynamicDependency(nameof(GetPlaceholderCount))]
		[DynamicDependency(nameof(SetPlaceholderCount))]
		public static readonly DependencyProperty PlaceholderCountProperty = DependencyProperty.RegisterAttached(
			"PlaceholderCount",
			typeof(int),
			typeof(Skeleton),
			new PropertyMetadata(0));

		/// <summary>Gets how many placeholder rows are generated for empty list controls while loading.</summary>
		public static int GetPlaceholderCount(FrameworkElement element) => (int)element.GetValue(PlaceholderCountProperty);

		/// <summary>
		/// Sets how many placeholder rows are generated, while loading, for empty list controls (<c>ItemsControl</c>,
		/// <c>ListView</c>, <c>GridView</c>, <c>ItemsRepeater</c>) with an <c>ItemTemplate</c>, applying to the element and its descendants.
		/// The nearest element with a value (set locally or through a <c>Style</c>) applies; an explicit 0 disables rows.
		/// A <see cref="SkeletonPresenter"/> stamps its template's lists with <see cref="SkeletonPresenter.DefaultPlaceholderCount"/> rows when no value is found.
		/// </summary>
		public static void SetPlaceholderCount(FrameworkElement element, int value) => element.SetValue(PlaceholderCountProperty, value);

		#endregion

		#region AttachedProperty: Ignore

		/// <summary>
		/// Backing property for whether an element and its subtree are excluded from skeleton generation.
		/// </summary>
		[DynamicDependency(nameof(GetIgnore))]
		[DynamicDependency(nameof(SetIgnore))]
		public static readonly DependencyProperty IgnoreProperty = DependencyProperty.RegisterAttached(
			"Ignore",
			typeof(bool),
			typeof(Skeleton),
			new PropertyMetadata(false));

		/// <summary>Gets whether the element and its subtree are excluded from skeleton generation.</summary>
		public static bool GetIgnore(FrameworkElement element) => (bool)element.GetValue(IgnoreProperty);

		/// <summary>Sets whether the element and its subtree are excluded from skeleton generation.</summary>
		public static void SetIgnore(FrameworkElement element, bool value) => element.SetValue(IgnoreProperty, value);

		#endregion

		#region AttachedProperty: Shape

		/// <summary>
		/// Backing property for the placeholder shape forced onto an element.
		/// Setting a value other than <see cref="SkeletonShape.Auto"/> also makes the element a generation leaf:
		/// a single placeholder covers it and its subtree is not visited.
		/// </summary>
		[DynamicDependency(nameof(GetShape))]
		[DynamicDependency(nameof(SetShape))]
		public static readonly DependencyProperty ShapeProperty = DependencyProperty.RegisterAttached(
			"Shape",
			typeof(SkeletonShape),
			typeof(Skeleton),
			new PropertyMetadata(SkeletonShape.Auto));

		/// <summary>Gets the placeholder shape forced onto the element.</summary>
		public static SkeletonShape GetShape(FrameworkElement element) => (SkeletonShape)element.GetValue(ShapeProperty);

		/// <summary>Sets the placeholder shape forced onto the element.</summary>
		public static void SetShape(FrameworkElement element, SkeletonShape value) => element.SetValue(ShapeProperty, value);

		#endregion

		/// <summary>
		/// Resolves the <see cref="PlaceholderCountProperty"/> of the element or its nearest ancestor with a value: one set
		/// locally (an explicit 0 counts, and vetoes an ancestor's value) or coming from a <c>Style</c> (any non-zero value).
		/// </summary>
		internal static bool TryGetInheritedPlaceholderCount(FrameworkElement element, out int count)
		{
			foreach (var ancestor in element.GetAncestors(includeCurrent: true))
			{
				count = (int)ancestor.GetValue(PlaceholderCountProperty);
				if (count != 0 || ancestor.ReadLocalValue(PlaceholderCountProperty) != DependencyProperty.UnsetValue)
				{
					return true;
				}
			}

			count = 0;
			return false;
		}
	}
}
