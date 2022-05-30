using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#if IS_WINUI
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.UI
{
	/// <summary>
	/// Provides a binding to the closest parent ItemsControl.
	/// This markup can be used to access the parent ItemsControl from inside of the ItemTemplate.
	/// </summary>
	public class ItemsControlBindingExtension : AncestorBindingExtension
	{
		public ItemsControlBindingExtension()
		{
			AncestorType = typeof(ItemsControl);
		}
	}
}
