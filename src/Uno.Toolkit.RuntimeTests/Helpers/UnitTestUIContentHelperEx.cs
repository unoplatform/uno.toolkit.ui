using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Uno.UI.RuntimeTests;

#if IS_WINUI
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml;
#endif

using Base = Uno.UI.RuntimeTests.UnitTestsUIContentHelper;

namespace Uno.Toolkit.RuntimeTests.Helpers
{
	internal static class UnitTestUIContentHelperEx
	{
		public static async Task SetContentAndWait(FrameworkElement e)
		{
			Base.Content = e;
			await Base.WaitForIdle();
			await Base.WaitForLoaded(e);
		}
	}
}
