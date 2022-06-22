using System;
using System.Collections.Generic;
using System.Text;

#if IS_WINUI
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
#endif

namespace Uno.Toolkit.Samples.Content.Controls
{
	/// <summary>
	/// Provides a mechanism to handle back navigation (<see cref="Shell.BackNavigateFromNestedSample"/>) that exited the nested sample.
	/// </summary>
	/// <remarks>
	/// <see cref="Page.OnNavigatedTo(NavigationEventArgs)"/> will only work between pages of nested sample which uses frame navigation.
	/// Between nested sample pages and content pages, it is based on a nested frame visibility.
	/// Content pages navigation are based on navigation-view's not frame's.
	/// </remarks>
	public interface IExitNestedSampleHandler
	{
		void OnExitedFromNestedSample(object sender);
	}
}
