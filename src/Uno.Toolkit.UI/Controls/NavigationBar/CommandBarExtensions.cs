using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics.CodeAnalysis;
#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.UI
{
	[Bindable]
	public static class CommandBarExtensions
	{
		#region Property: MainCommand

		public static DependencyProperty MainCommandProperty { [DynamicDependency(nameof(GetMainCommand))] get; } = DependencyProperty.RegisterAttached(
			"MainCommand",
			typeof(AppBarButton),
			typeof(CommandBarExtensions),
			new PropertyMetadata(default(AppBarButton?)));



		[DynamicDependency(nameof(SetMainCommand))]
		public static AppBarButton? GetMainCommand(DependencyObject? obj) => obj?.GetValue(MainCommandProperty) as AppBarButton;
		[DynamicDependency(nameof(GetMainCommand))]
		public static void SetMainCommand(DependencyObject? obj, AppBarButton? value) => obj?.SetValue(MainCommandProperty, value);

		#endregion
	}
}
