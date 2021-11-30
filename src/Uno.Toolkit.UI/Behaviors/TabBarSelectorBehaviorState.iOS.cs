#if __IOS__
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;
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
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml;
#endif

namespace Uno.Toolkit.UI
{
	partial class TabBarSelectorBehaviorState
	{
		private readonly SerialDisposable _flipViewLoadedRevoker = new SerialDisposable();
		private readonly SerialDisposable _scrolledRevoker = new SerialDisposable();

		partial void ConnectPartial()
		{ 
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
				&& flipView.FindFirstChild<UIScrollView>() is { } scrollView
				&& flipView.FindFirstChild<NativeFlipView>() is { } nativeFlipView)
			{
				scrollView.Delegate = new ScrollViewDelegate(this, nativeFlipView.Source as FlipViewSource);
				_scrolledRevoker.Disposable = Disposable.Create(() => scrollView.Delegate = null);
			}

			_flipViewLoadedRevoker.Disposable = null;
		}

		partial void DisconnectPartial()
		{
			_scrolledRevoker.Disposable = null;
			_flipViewLoadedRevoker.Disposable = null;
		}
	}

	//On Xamarin.iOS, events will only be called if you have not set a delegate. In the case of the NativeFlipView in Uno, there is a Delegate that is set internally.
	//Therefore, subscribing to the Scrolled event would not work. So we need to replace the internal delegate with our own custom one and forward the appropriate events
	//to the internal FlipViewSource.
	internal class ScrollViewDelegate : UIScrollViewDelegate
	{
		private readonly TabBarSelectorBehaviorState _state;
		private readonly FlipViewSource? _flipViewSource;

		public ScrollViewDelegate(TabBarSelectorBehaviorState state, FlipViewSource? flipViewSource)
		{
			_state = state;
			_flipViewSource = flipViewSource;
		}

		public override void Scrolled(UIScrollView scrollView)
		{
			var offset = scrollView.ContentOffset.X;

			if (GetProgress(offset) is (int position, double positionOffset))
			{
				_state.UpdateOffset(position, positionOffset, offset);
			}
		}
		public override void DecelerationEnded(UIScrollView scrollView)
		{
			_flipViewSource?.DecelerationEnded(scrollView);
		}

		public override void DraggingEnded(UIScrollView scrollView, bool willDecelerate)
		{
			_flipViewSource?.DecelerationEnded(scrollView);
		}

		private (int, double)? GetProgress(double offset)
		{
			for (int i = 0; i < _state.Selector.Items.Count; i++)
			{
				var width = _state.Selector.ActualWidth * (i + 1);
				if (width > offset)
				{
					var previousWidth = _state.Selector.ActualWidth * (i);
					return (i, (offset - previousWidth) / (width - previousWidth));
				}
			}

			return null;
		}
	}
}
#endif
