using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Uno.Extensions;

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
	/// Provides attached properties to configure skeleton generation, and to inject an auto-generated
	/// skeleton as the loading template of state-aware controls exposing a <c>ProgressTemplate</c>
	/// (such as <c>FeedView</c> from Uno.Extensions), without any skeleton markup.
	/// </summary>
	public static class Skeleton
	{
		// FeedView-convention member names, resolved by reflection so the toolkit takes no dependency on Uno.Extensions.
		private const string ProgressTemplatePropertyName = "ProgressTemplate";
		private const string ValueTemplatePropertyName = "ValueTemplate";

		// Keyed resource (in SkeletonView.xaml) hosting the SkeletonPresenter injected as ProgressTemplate.
		internal const string DefaultProgressTemplateResourceKey = "SkeletonDefaultProgressTemplate";

		#region AttachedProperty: IsEnabled

		/// <summary>
		/// Backing property for enabling automatic skeleton injection on a control exposing a <c>ProgressTemplate</c>.
		/// </summary>
		[DynamicDependency(nameof(GetIsEnabled))]
		[DynamicDependency(nameof(SetIsEnabled))]
		public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached(
			"IsEnabled",
			typeof(bool),
			typeof(Skeleton),
			new PropertyMetadata(false, OnIsEnabledChanged));

		/// <summary>Gets whether an auto-generated skeleton is injected as the element's <c>ProgressTemplate</c>.</summary>
		public static bool GetIsEnabled(FrameworkElement element) => (bool)element.GetValue(IsEnabledProperty);

		/// <summary>
		/// Sets whether an auto-generated skeleton is injected as the element's <c>ProgressTemplate</c>.
		/// The skeleton layout is derived from the element's <c>ValueTemplate</c> unless
		/// <see cref="PlaceholderTemplateProperty"/> is set. An explicitly assigned <c>ProgressTemplate</c> is never overwritten.
		/// </summary>
		public static void SetIsEnabled(FrameworkElement element, bool value) => element.SetValue(IsEnabledProperty, value);

		#endregion

		#region AttachedProperty: PlaceholderTemplate

		/// <summary>
		/// Backing property for the template the injected skeleton is derived from,
		/// overriding the default derivation from the element's <c>ValueTemplate</c>.
		/// </summary>
		[DynamicDependency(nameof(GetPlaceholderTemplate))]
		[DynamicDependency(nameof(SetPlaceholderTemplate))]
		public static readonly DependencyProperty PlaceholderTemplateProperty = DependencyProperty.RegisterAttached(
			"PlaceholderTemplate",
			typeof(DataTemplate),
			typeof(Skeleton),
			new PropertyMetadata(default(DataTemplate)));

		/// <summary>Gets the template the injected skeleton is derived from.</summary>
		public static DataTemplate? GetPlaceholderTemplate(FrameworkElement element) => (DataTemplate?)element.GetValue(PlaceholderTemplateProperty);

		/// <summary>Sets the template the injected skeleton is derived from, instead of the element's <c>ValueTemplate</c>.</summary>
		public static void SetPlaceholderTemplate(FrameworkElement element, DataTemplate? value) => element.SetValue(PlaceholderTemplateProperty, value);

		#endregion

		#region AttachedProperty: PlaceholderCount

		/// <summary>
		/// Backing property for how many placeholder rows are stamped into empty list controls of a derived skeleton.
		/// </summary>
		[DynamicDependency(nameof(GetPlaceholderCount))]
		[DynamicDependency(nameof(SetPlaceholderCount))]
		public static readonly DependencyProperty PlaceholderCountProperty = DependencyProperty.RegisterAttached(
			"PlaceholderCount",
			typeof(int),
			typeof(Skeleton),
			new PropertyMetadata(4));

		/// <summary>Gets how many placeholder rows are stamped into empty list controls of a derived skeleton.</summary>
		public static int GetPlaceholderCount(FrameworkElement element) => (int)element.GetValue(PlaceholderCountProperty);

		/// <summary>Sets how many placeholder rows are stamped into empty list controls of a derived skeleton. Default is 4.</summary>
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

		#region AttachedProperty (private): InjectedTemplate

		// Remembers the template instance this class injected, so disabling only clears what it set.
		private static readonly DependencyProperty InjectedTemplateProperty = DependencyProperty.RegisterAttached(
			"InjectedTemplate",
			typeof(DataTemplate),
			typeof(Skeleton),
			new PropertyMetadata(default(DataTemplate)));

		#endregion

		private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is not FrameworkElement element) return;

			if (e.NewValue is true)
			{
				InjectProgressTemplate(element);
			}
			else
			{
				RemoveInjectedProgressTemplate(element);
			}
		}

		private static void InjectProgressTemplate(FrameworkElement element)
		{
			if (FindDependencyProperty(element.GetType(), ProgressTemplatePropertyName) is not { } progressTemplateProperty)
			{
				typeof(Skeleton).Log().LogWarning(
					"Skeleton.IsEnabled is set on a {Type} which does not expose a {Property} dependency property; no skeleton is injected.",
					element.GetType().Name, ProgressTemplatePropertyName);
				return;
			}

			if (element.GetValue(progressTemplateProperty) is not null)
			{
				// an explicitly assigned ProgressTemplate always wins
				return;
			}

			if (!(Application.Current?.Resources.TryGetValue(DefaultProgressTemplateResourceKey, out var resource) ?? false) ||
				resource is not DataTemplate template)
			{
				typeof(Skeleton).Log().LogWarning(
					"Unable to resolve the '{Key}' resource; ensure the Uno.Toolkit resources are merged into the application resources.",
					DefaultProgressTemplateResourceKey);
				return;
			}

			element.SetValue(progressTemplateProperty, template);
			element.SetValue(InjectedTemplateProperty, template);
		}

		private static void RemoveInjectedProgressTemplate(FrameworkElement element)
		{
			if (element.GetValue(InjectedTemplateProperty) is not DataTemplate injected) return;
			element.ClearValue(InjectedTemplateProperty);

			if (FindDependencyProperty(element.GetType(), ProgressTemplatePropertyName) is { } progressTemplateProperty &&
				ReferenceEquals(element.GetValue(progressTemplateProperty), injected))
			{
				element.ClearValue(progressTemplateProperty);
			}
		}

		/// <summary>
		/// Resolves the element's <c>ValueTemplate</c> by convention, if it exposes one.
		/// </summary>
		internal static DataTemplate? FindValueTemplate(FrameworkElement element) =>
			FindDependencyProperty(element.GetType(), ValueTemplatePropertyName) is { } dp
				? element.GetValue(dp) as DataTemplate
				: null;

		[UnconditionalSuppressMessage("Trimming", "IL2070", Justification =
			"The looked-up members belong to a control type actively used by the consuming app (e.g. FeedView), whose statics are preserved with it. " +
			"A trimmed-away member degrades gracefully: no skeleton is injected and a warning is logged.")]
		private static DependencyProperty? FindDependencyProperty(Type type, string name) =>
			(type.GetProperty($"{name}Property", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)?.GetValue(null)
				?? type.GetField($"{name}Property", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)?.GetValue(null))
			as DependencyProperty;
	}
}
