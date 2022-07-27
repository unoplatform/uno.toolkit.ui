using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
#endif

using InsetMask = Uno.Toolkit.UI.SafeAreaBehavior.InsetMask;
using InsetMode = Uno.Toolkit.UI.SafeAreaBehavior.InsetMode;

namespace Uno.Toolkit.UI.Controls.SafeArea
{
	partial class SafeAreaView
	{
		#region Insets
		public InsetMask Insets
		{
			get => (InsetMask)GetValue(InsetsProperty);
			set => SetValue(InsetsProperty, value);
		}

		public static DependencyProperty InsetsProperty { get; } =
			DependencyProperty.Register(
				nameof(Insets),
				typeof(InsetMask),
				typeof(SafeAreaView),
				new PropertyMetadata(InsetMask.All)
			);
		#endregion

		#region Insets
		public InsetMode Mode
		{
			get => (InsetMode)GetValue(ModeProperty);
			set => SetValue(ModeProperty, value);
		}

		public static DependencyProperty ModeProperty { get; } =
			DependencyProperty.Register(
				nameof(Mode),
				typeof(InsetMode),
				typeof(SafeAreaView),
				new PropertyMetadata(InsetMode.Padding)
			);
		#endregion
	}
}
