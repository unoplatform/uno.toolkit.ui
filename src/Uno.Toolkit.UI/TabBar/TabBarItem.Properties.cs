using System;
using System.Collections.Generic;
using System.Text;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.UI.ToolkitLib
{
	partial class TabBarItem
	{
		#region Icon
		public IconElement Icon
		{
			get { return (IconElement)GetValue(IconProperty); }
			set { SetValue(IconProperty, value); }
		}

		public static readonly DependencyProperty IconProperty =
			DependencyProperty.Register(nameof(Icon), typeof(IconElement), typeof(TabBarItem), new PropertyMetadata(null, OnPropertyChanged));

		#endregion
		private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			var owner = (TabBarItem)sender;
			owner.OnPropertyChanged(args);
		}
	}
}
