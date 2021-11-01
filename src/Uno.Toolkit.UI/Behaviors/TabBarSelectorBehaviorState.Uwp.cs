#if !__ANDROID__ && !__IOS__
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.UI.ToolkitLib.Extensions;
using Uno.Disposables;
using System.Diagnostics;

#if IS_WINUI
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#endif

namespace Uno.UI.ToolkitLib.Behaviors
{
	partial class TabBarSelectorBehaviorState
	{
		private readonly SerialDisposable _flipViewLoadedRevoker = new SerialDisposable();
		private readonly SerialDisposable _viewChangedRevoker = new SerialDisposable();

		partial void ConnectPartial()
		{
			_viewChangedRevoker.Disposable = null;
			_flipViewLoadedRevoker.Disposable = null;

			if (Selector is FlipView flipView)
			{
				flipView.Loaded += OnFlipViewLoaded;
				_flipViewLoadedRevoker.Disposable = Disposable.Create(() => flipView.Loaded -= OnFlipViewLoaded);
			}
		}

		private void OnFlipViewLoaded(object sender, RoutedEventArgs e)
		{
			if (sender is FlipView flipView
				&& flipView.FindChild<ScrollViewer>() is { } scrollViewer)
			{
				scrollViewer.ViewChanged += OnScrolViewerViewChanged;
				_viewChangedRevoker.Disposable = Disposable.Create(() => scrollViewer.ViewChanged -= OnScrolViewerViewChanged);

				_flipViewLoadedRevoker.Disposable = null;
			}
		}

		partial void DisconnectPartial()
		{
			_flipViewLoadedRevoker.Disposable = null;
			_viewChangedRevoker.Disposable = null;
		}

		private void OnScrolViewerViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
		{
			var scrollViewer = sender as ScrollViewer;

			if (scrollViewer == null)
			{
				return;
			}

			Console.WriteLine($"ViewChanged: offset={scrollViewer.HorizontalOffset}");

			// The inner ScrollViewer of a FlipView on UWP uses strange values for HorizontalOffset.
			// It seems that there is a 1-based index of the FlipViewItems.
			// So if we are on the first FlipViewItem, the HorizontalOffset of the ScrollViewer will actually be 2,
			// since the offset is relative to the end of the viewable item
			// -------------
			// |   |   |   |
			// |   |   |   |
			// -------------
			// 1   2   3   4    
			var offset =
#if WINDOWS_UWP
				(scrollViewer.HorizontalOffset - 2) * Selector.ActualWidth;
#else
				scrollViewer.HorizontalOffset;
#endif
			if (GetProgress(offset) is (int position, double positionOffset))
			{
				UpdateOffset(position, positionOffset, offset);
			}
		}

		private (int, double)? GetProgress(double offset)
		{
			for (int i = 0; i < Selector.Items.Count; i++)
			{
				var width = Selector.ActualWidth * (i + 1);
				if (width > offset)
				{
					var previousWidth = Selector.ActualWidth * (i);
					return (i, (offset - previousWidth) / (width - previousWidth));
				}
			}

			return null;
		}
	}
}
#endif