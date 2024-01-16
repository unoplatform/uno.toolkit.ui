using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.System;
using Microsoft.Extensions.Logging;
using Uno.Extensions;
using Uno.Logging;
using System.Diagnostics.CodeAnalysis;
using Uno.UI.Extensions;


#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using ItemsRepeater = Microsoft.UI.Xaml.Controls.ItemsRepeater;
#endif


namespace Uno.Toolkit.UI
{
	public static class ProgressRingExtensions
	{
		public static readonly DependencyProperty IsActiveProperty =
			DependencyProperty.RegisterAttached(
				"IsActive",
				typeof(bool),
				typeof(ProgressRingExtensions),
				new PropertyMetadata(false, IsActiveChanged));

		private static void IsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is FrameworkElement element &&
				e.NewValue is bool isActive)
			{				
				foreach (var item in element.EnumerateDescendants().OfType<ProgressRing>())
				{
					item.IsActive = isActive;
				}
			}
		}

		public static void SetIsActive(this FrameworkElement element, bool value)
		{
			element.SetValue(IsActiveProperty, value);
		}

		public static bool GetIsActive(this FrameworkElement element)
		{
			return (bool)element.GetValue(IsActiveProperty);
		}
	}
}
