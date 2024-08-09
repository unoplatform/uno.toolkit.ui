using System.Linq;

#if IS_WINUI
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.RuntimeTests.Tests.TestPages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class AncestorBindingTest : Page
{
	public AncestorBindingTest()
	{
		this.InitializeComponent();
		this.DataContext = new
		{
			Items = Enumerable.Range(0, 5).Select(x => $"Item {x}").ToArray(),
		};
	}
}
