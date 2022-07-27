using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
#endif

namespace Uno.Toolkit.UI
{
	partial class SafeAreaBehavior
	{
		#region DependencyProperty: Insets

		public static DependencyProperty InsetsProperty { get; } = DependencyProperty.RegisterAttached(
			"Insets",
			typeof(InsetMask),
			typeof(SafeAreaBehavior),
			new PropertyMetadata(InsetMask.None, OnInsetsChanged));

		public static InsetMask GetInsets(DependencyObject obj) => (InsetMask)obj.GetValue(InsetsProperty);
		public static void SetInsets(DependencyObject obj, InsetMask value) => obj.SetValue(InsetsProperty, value);

		#endregion

		#region DependencyProperty: InsetMode

		public static DependencyProperty InsetModeProperty { get; } = DependencyProperty.RegisterAttached(
			"InsetMode",
			typeof(InsetMode),
			typeof(SafeAreaBehavior),
			new PropertyMetadata(InsetMode.Padding));

		public static InsetMode GetInsetMode(DependencyObject obj) => (InsetMode)obj.GetValue(InsetModeProperty);
		public static void SetInsetMode(DependencyObject obj, InsetMode value) => obj.SetValue(InsetModeProperty, value);

		#endregion
	}
}
