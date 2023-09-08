#if !__ANDROID__ && !__IOS__
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace Uno.Toolkit.UI
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

		private void OnScrolViewerViewChanged(object? sender, ScrollViewerViewChangedEventArgs e)
		{
			var scrollViewer = sender as ScrollViewer;

			if (scrollViewer == null)
			{
				return;
			}

			Console.WriteLine($"ViewChanged: offset={scrollViewer.HorizontalOffset}");
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
