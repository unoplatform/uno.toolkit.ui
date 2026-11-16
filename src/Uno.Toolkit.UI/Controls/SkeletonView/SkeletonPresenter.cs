using System;
using Microsoft.Extensions.Logging;
using Uno.Extensions;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using ItemsRepeater = Microsoft.UI.Xaml.Controls.ItemsRepeater;
#endif

namespace Uno.Toolkit.UI
{
	/// <summary>
	/// A <see cref="SkeletonView"/> that derives its skeleton from a template instead of live content, loading by default.
	/// Designed to be dropped into the loading template of a state-aware control such as <c>FeedView</c>.
	/// </summary>
	/// <remarks>
	/// The skeleton is derived from its <see cref="ContentControl.ContentTemplate"/>, typically template-bound to the
	/// state-aware control's value template. Empty list controls in that template are stamped with
	/// <see cref="Skeleton.PlaceholderCountProperty"/> placeholder rows: the value of the nearest
	/// element setting it (the presenter itself first), else <see cref="DefaultPlaceholderCount"/>.
	/// </remarks>
	public partial class SkeletonPresenter : SkeletonView
	{
		/// <summary>Placeholder rows stamped into the template's empty lists when no <see cref="Skeleton.PlaceholderCountProperty"/> is found.</summary>
		public const int DefaultPlaceholderCount = 4;

		/// <summary>Initializes a new instance of the <see cref="SkeletonPresenter"/> class.</summary>
		public SkeletonPresenter()
		{
			DefaultStyleKey = typeof(SkeletonPresenter);

			// a template-bound ContentTemplate is only in place once in the tree
			Loaded += (s, e) => WarnIfNoTemplate();
		}

		private void WarnIfNoTemplate()
		{
			if (ContentTemplate is null && Content is not UIElement)
			{
				typeof(SkeletonPresenter).Log().LogWarning("No ContentTemplate is set to derive the skeleton from.");
			}
		}

		// The nearest element setting it applies, e.g. the templated control when this presenter is part of
		// a control template (where the attached property can't be template-bound).
		private int ResolvePlaceholderCount() =>
			Skeleton.TryGetInheritedPlaceholderCount(this, out var count)
				? count
				: DefaultPlaceholderCount;

		private protected override void PrepareSkeletonContent(DependencyObject contentRoot)
		{
			// Only our private, invisible instantiation of the template is stamped: content set directly is the app's
			// live content, which is never mutated (its empty lists can opt into stand-in rows like in any SkeletonView).
			// Idempotent: only an empty list with a null ItemsSource is filled, and the dummy assignment makes it non-null.
			if (ContentTemplate is null) return;

			var count = ResolvePlaceholderCount();
			if (count <= 0) return;

			// The lists' DataContext is cleared too: null items leave their containers' DataContext to inheritance,
			// so the rows would otherwise bind to the template's (or the page's) data instead of having empty values.
			// a nested skeleton owns its content, as in CollectPlaceholder
			foreach (var descendant in contentRoot.GetDescendants(x => x is not SkeletonView))
			{
				if (descendant is ItemsControl { ItemsSource: null, ItemTemplate: { } } itemsControl && itemsControl.Items.Count == 0)
				{
					itemsControl.DataContext = null;
					itemsControl.ItemsSource = new object[count];
				}
				else if (descendant is ItemsRepeater { ItemsSource: null, ItemTemplate: { } } itemsRepeater)
				{
					itemsRepeater.DataContext = null;
					itemsRepeater.ItemsSource = new object[count];
				}
			}
		}
	}
}
