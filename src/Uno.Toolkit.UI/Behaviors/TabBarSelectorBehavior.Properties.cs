using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
#endif

namespace Uno.Toolkit.UI
{
	partial class TabBarSelectorBehavior
	{
		#region Selector Attached Property
		[DynamicDependency(nameof(SetSelector))]
		public static Selector? GetSelector(DependencyObject obj)
		{
			return (Selector)obj.GetValue(SelectorProperty);
		}

		[DynamicDependency(nameof(GetSelector))]
		public static void SetSelector(DependencyObject obj, Selector? value)
		{
			obj.SetValue(SelectorProperty, value);
		}

		/// <summary>
		/// The <see cref="Selector"/> that will have its SelectedIndex synchronized to the attached <see cref="TabBar"/>
		/// </summary>
		public static DependencyProperty SelectorProperty { [DynamicDependency(nameof(GetSelector))] get; } =
			DependencyProperty.RegisterAttached("Selector", typeof(Selector), typeof(TabBarSelectorBehavior), new PropertyMetadata(null, OnPropertyChanged));
		#endregion

		#region State Attached Property (private)
		[DynamicDependency(nameof(SetState))]
		private static TabBarSelectorBehaviorState? GetState(DependencyObject obj)
		{
			return (TabBarSelectorBehaviorState)obj.GetValue(StateProperty);
		}

		[DynamicDependency(nameof(GetState))]
		private static void SetState(DependencyObject obj, TabBarSelectorBehaviorState? value)
		{
			obj.SetValue(StateProperty, value);
		}

		[DynamicDependency(nameof(GetState))]
		private static readonly DependencyProperty StateProperty =
			DependencyProperty.RegisterAttached("State", typeof(TabBarSelectorBehaviorState), typeof(TabBarSelectorBehavior), new PropertyMetadata(null));
		#endregion
	}
}
