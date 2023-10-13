using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if IS_WINUI
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.UI
{
	public static class StyleExtensions
	{
		public static readonly DependencyProperty ResourcesProperty =
		DependencyProperty.RegisterAttached("Resources", typeof(ResourceDictionary), typeof(StyleExtensions), new PropertyMetadata(null, OnResourcesChanged));

		public static ResourceDictionary GetResources(UIElement element)
		{
			return (ResourceDictionary)element.GetValue(ResourcesProperty);
		}

		public static void SetResources(UIElement element, ResourceDictionary value)
		{
			element.SetValue(ResourcesProperty, value);
		}

		private static void OnResourcesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is Control control && e.NewValue is ResourceDictionary newResources)
			{
				control.Resources.MergedDictionaries.Add(newResources);
			}
		}
	}
}
