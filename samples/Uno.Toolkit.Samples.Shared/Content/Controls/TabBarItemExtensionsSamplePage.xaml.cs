using System;
using Uno.Toolkit.Samples.Content.NestedSamples;
using Uno.Toolkit.Samples.Entities;
using Uno.Toolkit.UI;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.Samples.Content.Controls
{
	[SamplePage(SampleCategory.Behaviors, nameof(TabBarItemExtensions))]
	public sealed partial class TabBarItemExtensionsSamplePage : Page
	{
		public TabBarItemExtensionsSamplePage()
		{
			this.InitializeComponent();

			NestedNestedFrame.Navigate(typeof(TabBarItemExtensions_NestedPage1));
		}

		private void ScrollTop(object sender, RoutedEventArgs e)
		{
			if (!(sender is Button button)) return;
			if (!(button.Tag is UIElement contentHost)) return;

			switch (contentHost)
			{
				case ListView lv: ScrollableHelper.SmoothScrollTop(lv); break;
				case ScrollViewer sv: sv.ChangeView(0, 0, zoomFactor: default, disableAnimation: false); break;

				default: throw new InvalidOperationException();
			}
		}

		private void ScrollBottom(object sender, RoutedEventArgs e)
		{
			if (!(sender is Button button)) return;
			if (!(button.Tag is UIElement contentHost)) return;

			switch (contentHost)
			{
				case ListView lv: ScrollableHelper.SmoothScrollBottom(lv); break;

				default: throw new InvalidOperationException();
			}
		}
	}
}
