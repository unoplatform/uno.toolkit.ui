#if IS_WINUI
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.UI
{
	internal partial class SafeAreaPresenter : ContentControl
	{
		public SafeAreaPresenter()
		{
			DefaultStyleKey = typeof(SafeAreaPresenter);
		}
	}
}
