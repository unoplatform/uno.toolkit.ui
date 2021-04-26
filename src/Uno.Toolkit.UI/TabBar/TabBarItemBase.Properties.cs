using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

#if IS_WINUI
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml;
#endif

namespace Uno.UI.ToolkitLib
{
	partial class TabBarItemBase
	{
		#region Command
		public ICommand Command
		{
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}

		public static DependencyProperty CommandProperty { get; } =
			DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(TabBarItemBase), new PropertyMetadata(null));
		#endregion

		#region CommandParameter
		public object CommandParameter
		{
			get { return (object)GetValue(CommandParameterProperty); }
			set { SetValue(CommandParameterProperty, value); }
		}

		public static DependencyProperty CommandParameterProperty { get; } =
			DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(TabBarItemBase), new PropertyMetadata(null)); 
		#endregion
	}
}
