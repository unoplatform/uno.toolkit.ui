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
		#region Property: LeftCommand

		public static DependencyProperty LeftCommandProperty { get; } = DependencyProperty.RegisterAttached(
			"LeftCommand",
			typeof(AppBarButton),
			typeof(CommandBarExtensions),
			new PropertyMetadata(default(AppBarButton?)));



		public static AppBarButton? GetLeftCommand(DependencyObject? obj) => obj?.GetValue(LeftCommandProperty) as AppBarButton;
		public static void SetLeftCommand(DependencyObject? obj, AppBarButton? value) => obj?.SetValue(LeftCommandProperty, value);

		#endregion
	}
}
