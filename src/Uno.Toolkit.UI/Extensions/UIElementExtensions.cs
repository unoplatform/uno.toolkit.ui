using System;
using System.Collections.Generic;
using System.Text;

#if IS_WINUI
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml;
#endif

namespace Uno.Toolkit.UI;

internal static class UIElementExtensions
{
	public static DispatcherCompat GetDispatcherCompat(this UIElement x) =>
#if IS_WINUI
		new DispatcherCompat(x.DispatcherQueue);
#else
		new DispatcherCompat(x.Dispatcher);
#endif
}
