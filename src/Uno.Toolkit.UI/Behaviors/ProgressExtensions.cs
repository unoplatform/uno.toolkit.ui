using System.Collections.Generic;
using System.Linq;
using Uno.UI.Extensions;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using ItemsRepeater = Microsoft.UI.Xaml.Controls.ItemsRepeater;
#endif

namespace Uno.Toolkit.UI
{
	public static class ProgressExtensions
	{
		public static readonly DependencyProperty IsActiveProperty =
			DependencyProperty.RegisterAttached(
				"IsActive",
				typeof(bool),
				typeof(ProgressExtensions),
				new PropertyMetadata(false, IsActiveChanged));

		private static void IsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is FrameworkElement element &&
				e.NewValue is bool isActive)
			{
				foreach (var item in element.EnumerateDescendants())
				{
					if (item is ProgressRing progressRing)
					{
						progressRing.IsActive = isActive;
					}
					else if (item is ProgressBar progressBar)
					{
						progressBar.IsIndeterminate = isActive;
					}
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

#if WINDOWS_UWP || WINDOWS
		// Copied from Uno.UI.Extensions.ViewExtensions for Windows targets
		private static IEnumerable<DependencyObject> EnumerateDescendants(this DependencyObject reference)
		{
			foreach (DependencyObject child in reference.EnumerateChildren())
			{
				yield return child;
				foreach (DependencyObject item in child.EnumerateDescendants())
				{
					yield return item;
				}
			}
		}

		private static IEnumerable<DependencyObject> EnumerateChildren(this DependencyObject reference)
		{
			return from x in Enumerable.Range(0, VisualTreeHelper.GetChildrenCount(reference))
											  select VisualTreeHelper.GetChild(reference, x);
		}
#endif
	}
}
