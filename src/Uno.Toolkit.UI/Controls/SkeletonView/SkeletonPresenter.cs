using System;
using System.Linq;
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
	/// A lightweight, always-loading <see cref="SkeletonView"/> that derives its skeleton from a template
	/// instead of live content. Designed to be dropped into (or injected as, via <see cref="Skeleton.IsEnabledProperty"/>)
	/// the loading template of a state-aware control such as <c>FeedView</c>.
	/// </summary>
	/// <remarks>
	/// The template resolution order is: its own <see cref="ContentControl.ContentTemplate"/>, then the
	/// <see cref="Skeleton.PlaceholderTemplateProperty"/> and <c>ValueTemplate</c> of the nearest ancestor
	/// with <see cref="Skeleton.IsEnabledProperty"/> set. Empty list controls in the resolved template are
	/// stamped with <see cref="Skeleton.PlaceholderCountProperty"/> placeholder rows.
	/// </remarks>
	public partial class SkeletonPresenter : SkeletonView
	{
		private FrameworkElement? _skeletonOwner;

		public SkeletonPresenter()
		{
			DefaultStyleKey = typeof(SkeletonPresenter);

			Loaded += (s, e) => ResolveTemplate();
		}

		protected override void OnApplyTemplate()
		{
			ResolveTemplate();

			base.OnApplyTemplate();
		}

		private void ResolveTemplate()
		{
			_skeletonOwner = this.GetAncestors().OfType<FrameworkElement>().FirstOrDefault(Skeleton.GetIsEnabled);

			if (ContentTemplate is null)
			{
				var template = _skeletonOwner is { } owner
					? Skeleton.GetPlaceholderTemplate(owner) ?? Skeleton.FindValueTemplate(owner)
					: null;
				if (template is { })
				{
					ContentTemplate = template;
				}
				else if (IsLoaded)
				{
					typeof(SkeletonPresenter).Log().LogWarning(
						"Unable to resolve a template to derive the skeleton from; set ContentTemplate, or Skeleton.PlaceholderTemplate / a ValueTemplate on the ancestor with Skeleton.IsEnabled.");
				}
			}
		}

		private protected override void PrepareSkeletonContent(DependencyObject contentRoot)
		{
			// This is our private, invisible instantiation of the template — never the app's live content —
			// so stamping placeholder items into its empty list controls cannot fight app data. Idempotent:
			// only a null ItemsSource is filled, and the dummy assignment makes it non-null.
			var count = Skeleton.GetPlaceholderCount(_skeletonOwner ?? this);
			if (count <= 0) return;

			foreach (var descendant in contentRoot.GetDescendants())
			{
				if (descendant is ItemsControl { ItemsSource: null, ItemTemplate: { } } itemsControl)
				{
					itemsControl.ItemsSource = new object[count];
				}
				else if (descendant is ItemsRepeater { ItemsSource: null, ItemTemplate: { } } itemsRepeater)
				{
					itemsRepeater.ItemsSource = new object[count];
				}
			}
		}
	}
}
