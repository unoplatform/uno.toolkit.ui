using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls.Primitives;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
#endif

namespace Uno.UI.ToolkitLib.Behaviors
{
	public class TabBarSelectorBehavior
	{
		public static Selector GetSelector(DependencyObject obj)
		{
			return (Selector)obj.GetValue(SelectorProperty);
		}

		public static void SetSelector(DependencyObject obj, Selector value)
		{
			obj.SetValue(SelectorProperty, value);
		}

		public static readonly DependencyProperty SelectorProperty =
			DependencyProperty.RegisterAttached("Selector", typeof(Selector), typeof(TabBarSelectorBehavior), new PropertyMetadata(default, OnSelectorChanged));


		private static void OnSelectorChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
		{
			if (dependencyObject is TabBar tabBar)
			{
				tabBar.SelectionChanged += TabBar_SelectionChanged;
			}
		}

		private static void TabBar_SelectionChanged(TabBar sender, TabBarSelectionChangedEventArgs args)
		{
			var selector = GetSelector(sender);
			if (selector == null)
			{
				return;
			}

			
		}

		private static void Selector_SelectionChanged(object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e)
		{
			if (sender is Selector selector)
			{
				selector.SelectedIndex
			}
		}
	}
}
