using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.Toolkit.UI.Controls;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
#endif

namespace Uno.Toolkit.UI.Behaviors
{
	partial class TabBarSelectorBehavior
	{
		#region Selector Attached Property
		public static Selector? GetSelector(DependencyObject obj)
		{
			return (Selector)obj.GetValue(SelectorProperty);
		}

		public static void SetSelector(DependencyObject obj, Selector? value)
		{
			obj.SetValue(SelectorProperty, value);
		}

		/// <summary>
		/// The <see cref="Selector"/> that will have its SelectedIndex synchronized to the attached <see cref="TabBar"/>
		/// </summary>
		public static DependencyProperty SelectorProperty { get; } =
			DependencyProperty.RegisterAttached("Selector", typeof(Selector), typeof(TabBarSelectorBehavior), new PropertyMetadata(null, OnPropertyChanged));
		#endregion

		#region State Attached Property (private)
		private static TabBarSelectorBehaviorState? GetState(DependencyObject obj)
		{
			return (TabBarSelectorBehaviorState)obj.GetValue(StateProperty);
		}

		private static void SetState(DependencyObject obj, TabBarSelectorBehaviorState? value)
		{
			obj.SetValue(StateProperty, value);
		}

		private static readonly DependencyProperty StateProperty =
			DependencyProperty.RegisterAttached("State", typeof(TabBarSelectorBehaviorState), typeof(TabBarSelectorBehavior), new PropertyMetadata(null));
		#endregion
	}
}
