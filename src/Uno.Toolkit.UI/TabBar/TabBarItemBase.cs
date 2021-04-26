using System.Windows.Input;
using System.Xml.Linq;


#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Controls;	
using Windows.UI.Xaml.Controls.Primitives;
#endif
namespace Uno.UI.ToolkitLib
{
	public abstract partial class TabBarItemBase : ListViewItem
	{
		protected override void OnPointerReleased(PointerRoutedEventArgs e)
		{
			var command = Command;
			if (command != null)
			{
				var commandParameter = CommandParameter;
				if (command.CanExecute(commandParameter))
				{
					command.Execute(commandParameter);
				}
			}

			base.OnPointerReleased(e);
		}
	}
}
