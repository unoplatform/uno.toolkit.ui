using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
#endif

#if __IOS__ || __ANDROID__
using Uno.UI.Helpers;
#endif

#if __IOS__
using Foundation;
using UIKit;
#endif

namespace Uno.Toolkit.UI
{
	public partial class ScrollableHelper
	{
		public static void SmoothScrollTop(ListView lv) => SmoothScrollTopImpl(lv);

		public static void SmoothScrollBottom(ListView lv) => SmoothScrollBottomImpl(lv);

		static partial void SmoothScrollTopImpl(ListView lv);

		static partial void SmoothScrollBottomImpl(ListView lv);
	}

#if !HAS_UNO
	static partial class ScrollableHelper // uwp
	{
		static partial void SmoothScrollTopImpl(ListView lv) => lv.ScrollToIndex(0);

		static partial void SmoothScrollBottomImpl(ListView lv) => lv.ScrollToIndex(lv.Items.Count - 1);

		private static Task ScrollToIndex(this ListViewBase lvb, int index) =>
			lvb.ScrollTo(lvb.Items[index], () => lvb.ContainerFromIndex(index));

		private static Task ScrollToItem(this ListViewBase lvb, object item) =>
			lvb.ScrollTo(item, () => lvb.ContainerFromItem(item));

		// credit: JustinXL https://stackoverflow.com/a/32559623

		private static async Task ScrollTo(this ListViewBase lvb, object item, Func<DependencyObject> getContainer)
		{
			bool isVirtualizing = default(bool);
			double previousHorizontalOffset = default(double), previousVerticalOffset = default(double);

			var scrollViewer = lvb.GetFirstDescendant<ScrollViewer>();
			var selectorItem = getContainer() as SelectorItem;

			// when it's null, means virtualization is on and the item hasn't been realized yet
			if (selectorItem == null)
			{
				isVirtualizing = true;

				previousHorizontalOffset = scrollViewer.HorizontalOffset;
				previousVerticalOffset = scrollViewer.VerticalOffset;

				// call task-based ScrollIntoViewAsync to realize the item
				await lvb.ScrollIntoViewAsync(item);

				// this time the item shouldn't be null again
				selectorItem = (SelectorItem)getContainer();
			}

			// calculate the position object in order to know how much to scroll to
			var transform = selectorItem.TransformToVisual((UIElement)scrollViewer.Content);
			var position = transform.TransformPoint(default);

			// when virtualized, scroll back to previous position without animation
			if (isVirtualizing)
			{
				await scrollViewer.ChangeViewAsync(previousHorizontalOffset, previousVerticalOffset, true);
			}

			// scroll to desired position with animation!
			scrollViewer.ChangeView(position.X, position.Y, null);
		}

		private static async Task ScrollIntoViewAsync(this ListViewBase listViewBase, object item)
		{
			var tcs = new TaskCompletionSource<object>();
			var scrollViewer = listViewBase.GetFirstDescendant<ScrollViewer>();

			EventHandler<ScrollViewerViewChangedEventArgs> viewChanged = (s, e) => tcs.TrySetResult(default!);

			try
			{
				scrollViewer.ViewChanged += viewChanged;
				listViewBase.ScrollIntoView(item, ScrollIntoViewAlignment.Leading);

				await tcs.Task;
			}
			finally
			{
				scrollViewer.ViewChanged -= viewChanged;
			}
		}

		private static async Task ChangeViewAsync(this ScrollViewer scrollViewer, double? horizontalOffset, double? verticalOffset, bool disableAnimation)
		{
			var tcs = new TaskCompletionSource<object>();

			EventHandler<ScrollViewerViewChangedEventArgs> viewChanged = (s, e) => tcs.TrySetResult(default!);
			try
			{
				scrollViewer.ViewChanged += viewChanged;
				scrollViewer.ChangeView(horizontalOffset, verticalOffset, null, disableAnimation);

				await tcs.Task;
			}
			finally
			{
				scrollViewer.ViewChanged -= viewChanged;
			}
		}
	}
#elif __IOS__ || __ANDROID__
	static partial class ScrollableHelper // ios and droid
	{
		static partial void SmoothScrollTopImpl(ListView lv) => lv.SmoothScrollToIndex(0);

		static partial void SmoothScrollBottomImpl(ListView lv) => lv.SmoothScrollToIndex(lv.Items.Count - 1);
	}
#else
	static partial class ScrollableHelper
	{
		static partial void SmoothScrollTopImpl(ListView lv) => throw new NotImplementedException("Not yet implemented for this platform.");

		static partial void SmoothScrollBottomImpl(ListView lv) => throw new NotImplementedException("Not yet implemented for this platform.");
	}
#endif
}
