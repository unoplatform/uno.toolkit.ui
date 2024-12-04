using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
	public static class ResourceExtensions
	{
		#region DependencyProperty: Resources

		public static DependencyProperty ResourcesProperty { [DynamicDependency(nameof(GetResources))] get; } = DependencyProperty.RegisterAttached(
			"Resources",
			typeof(ResourceDictionary),
			typeof(ResourceExtensions),
			new PropertyMetadata(default(ResourceDictionary), OnResourcesChanged));

		[DynamicDependency(nameof(SetResources))]
		public static ResourceDictionary GetResources(FrameworkElement obj) => (ResourceDictionary)obj.GetValue(ResourcesProperty);
		[DynamicDependency(nameof(GetResources))]
		public static void SetResources(FrameworkElement obj, ResourceDictionary value) => obj.SetValue(ResourcesProperty, value);

		#endregion

		private static void OnResourcesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
// 			if (d is Control control && e.NewValue is ResourceDictionary newResources)
// 			{
// #if !HAS_UNO
// 				control.Resources = newResources.DeepClone();
// #else
// 				control.Resources = newResources;
// #endif
// 			}
		}
	}
}
