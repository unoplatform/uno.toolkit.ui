#if __ANDROID__
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AndroidX.ViewPager.Widget;
using System.Diagnostics;
using Windows.Foundation;
using Uno.Disposables;
using Uno.Toolkit.UI.Controls;
using Uno.UI;

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

namespace Uno.Toolkit.UI.Behaviors
{
	partial class TabBarSelectorBehaviorState
	{
		private readonly SerialDisposable _flipViewSizeChangedRevoker = new SerialDisposable();
		private readonly SerialDisposable _scrolledRevoker = new SerialDisposable();

		partial void ConnectPartial()
		{
			_flipViewSizeChangedRevoker.Disposable = null;

			if (Selector is FlipView flipView)
			{
				flipView.SizeChanged += OnFlipViewSizeChanged;
				_flipViewSizeChangedRevoker.Disposable = Disposable.Create(() => flipView.SizeChanged -= OnFlipViewSizeChanged);
			}
		}

		partial void DisconnectPartial()
		{
			_flipViewSizeChangedRevoker.Disposable = null;
			_scrolledRevoker.Disposable = null;
		}

		private void OnFlipViewSizeChanged(object sender, SizeChangedEventArgs args)
		{
			_scrolledRevoker.Disposable = null;

			//The ViewPager does not exist in the Visual Tree in the Loaded event so we hook up to SizeChanged instead.
			// We wait until we have a valid size, sometimes the Height may be measured as 0 if the NativePagedView is measured before its children are loaded
			if (args.NewSize.Height > 0
				&& sender is FlipView flipView
				&& flipView.FindFirstChild<ViewPager>() is { } viewPager)
			{
				viewPager.PageScrolled += OnPageScrolled;
				_scrolledRevoker.Disposable = Disposable.Create(() => viewPager.PageScrolled -= OnPageScrolled);

				_flipViewSizeChangedRevoker.Disposable = null;
			}
		}

		private void OnPageScrolled(object sender, ViewPager.PageScrolledEventArgs e)
		{
			UpdateOffset(e.Position, e.PositionOffset, GetOffset(e));
		}

		private double GetOffset(ViewPager.PageScrolledEventArgs e)
		{
			return (Selector.ActualWidth * e.Position) + e.PositionOffsetPixels;
		}
	}
}
#endif
