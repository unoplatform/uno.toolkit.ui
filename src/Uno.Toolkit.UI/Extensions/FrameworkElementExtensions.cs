using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Uno.UI.Extensions;


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

		public static Size GetActualSize(this FrameworkElement fe) => new Size(fe.ActualWidth, fe.ActualHeight);

		public static void WhenLoaded(this FrameworkElement fe, Action action)
		{
			if (fe.IsLoaded)
			{
				action();
			}
			else
			{
				fe.Loaded += OnLoaded;
				void OnLoaded(object sender, RoutedEventArgs e)
				{
					action();
					fe.Loaded -= OnLoaded;
				}
			}
		}
	}
}
