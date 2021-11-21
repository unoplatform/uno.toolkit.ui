using System;
using System.Collections.Generic;
using System.Text;
#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.UI.Controls
{
	[Bindable]
	public static class CommandBarExtensions
	{
		#region Property: MainCommand

		public static DependencyProperty MainCommandProperty { get; } = DependencyProperty.RegisterAttached(
			"MainCommand",
			typeof(AppBarButton),
			typeof(CommandBarExtensions),
			new PropertyMetadata(default(AppBarButton?)));



		public static AppBarButton? GetMainCommand(DependencyObject? obj) => obj?.GetValue(MainCommandProperty) as AppBarButton;
		public static void SetMainCommand(DependencyObject? obj, AppBarButton? value) => obj?.SetValue(MainCommandProperty, value);

		#endregion
	}
}
