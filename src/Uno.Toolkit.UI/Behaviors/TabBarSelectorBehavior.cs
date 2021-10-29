using System;
using System.Collections.Generic;
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

namespace Uno.UI.ToolkitLib.Behaviors
{
	/// <summary>
	/// A behavior that, when attached to a <see cref="TabBar"/>, synchronizes the SelectedIndex of the <see cref="TabBar"/> to the specified <see cref="TabBarSelectorBehavior.SelectorProperty"/>
	/// </summary>
	public partial class TabBarSelectorBehavior
	{
		private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null)
            {
                return;
            }

            if (d is TabBar tabBar)
            {
                UpdateState(tabBar);
            }
            else
            {
                throw new InvalidOperationException("TabBarSelectorBehavior must be attached to a TabBar control");
            }
        }

        private static void UpdateState(TabBar tabBar)
		{
			var selector = GetSelector(tabBar);
			
			if (GetState(tabBar) is { } previousState)
			{
				if (selector == null || previousState.Selector != selector)
				{
					previousState.Disconnect();
					SetState(tabBar, null);
				}
			}

			if (selector != null)
			{
				var state = GetState(tabBar);
				if (state == null)
				{
					state = new TabBarSelectorBehaviorState(selector, tabBar);
					state.Connect();

					SetState(tabBar, state);
				}
			}
		}
	}
}
