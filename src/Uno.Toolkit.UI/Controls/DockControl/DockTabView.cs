using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.UI;

public enum DockTabDirection
{
	Left, Top, Right, Bottom,
}

public partial class DockTabView : TabView
{

	#region DependencyProperty: TabDirection

	public static DependencyProperty TabDirectionProperty { get; } = DependencyProperty.Register(
		nameof(TabDirection),
		typeof(DockTabDirection),
		typeof(DockTabView),
		new PropertyMetadata(default(DockTabDirection), OnTabDirectionChanged));

	public DockTabDirection TabDirection
	{
		get => (DockTabDirection)GetValue(TabDirectionProperty);
		set => SetValue(TabDirectionProperty, value);
	}

	private static void OnTabDirectionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => (sender as DockTabView)?.OnTabDirectionChanged(e);
	#endregion

	private void OnTabDirectionChanged(DependencyPropertyChangedEventArgs e)
	{
		UpdateTabDirection();
	}

	private void UpdateTabDirection()
	{
		var state = TabDirection switch
		{
			_ => "",
		};
		VisualStateManager.GoToState(this, state, useTransitions: IsLoaded);
	}
}
