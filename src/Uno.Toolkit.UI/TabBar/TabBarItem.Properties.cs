﻿using System;
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
		public IconElement? Icon
		{
			get { return (IconElement)GetValue(IconProperty); }
			set { SetValue(IconProperty, value); }
		}

		public static readonly DependencyProperty IconProperty =
			DependencyProperty.Register(nameof(Icon), typeof(IconElement), typeof(TabBarItem), new PropertyMetadata(null, OnPropertyChanged));
		#endregion

		#region IsSelected
		public bool IsSelected
		{
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}

		public static DependencyProperty IsSelectedProperty { get; } =
			DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(TabBarItem), new PropertyMetadata(false, OnPropertyChanged));
		#endregion


		#region IsSelectable
		public bool IsSelectable
		{
			get { return (bool)GetValue(IsSelectableProperty); }
			set { SetValue(IsSelectableProperty, value); }
		}

		public static DependencyProperty IsSelectableProperty { get; } =
			DependencyProperty.Register(nameof(IsSelectable), typeof(bool), typeof(TabBarItem), new PropertyMetadata(true, OnPropertyChanged));
		#endregion

		private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			var owner = (TabBarItem)sender;
			owner.OnPropertyChanged(args);
		}
	}
}
