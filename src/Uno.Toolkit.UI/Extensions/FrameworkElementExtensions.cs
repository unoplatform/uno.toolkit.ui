using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
#endif

namespace Uno.Toolkit.UI
{
	internal static class FrameworkElementExtensions
	{
		public static void InheritDataContextFrom(this FrameworkElement fe, FrameworkElement parent)
		{
			fe.SetBinding(FrameworkElement.DataContextProperty, new Binding
			{
				Source = parent,
				Path = new PropertyPath(nameof(parent.DataContext)),
				Mode = BindingMode.OneWay,
			});
		}
	}
}
