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

namespace Uno.Toolkit.UI.Controls
{
	partial class SafeArea
	{
		#region Insets (Attached DP)
		public InsetMask Insets
		{
			get => (InsetMask)GetValue(InsetsProperty);
			set => SetValue(InsetsProperty, value);
		}

		public static DependencyProperty InsetsProperty { get; } = DependencyProperty.RegisterAttached(
			nameof(Insets),
			typeof(InsetMask),
			typeof(SafeAreaBehavior),
			new PropertyMetadata(InsetMask.None, OnInsetsChanged));

		public static InsetMask GetInsets(DependencyObject obj) => (InsetMask)obj.GetValue(InsetsProperty);
		public static void SetInsets(DependencyObject obj, InsetMask value) => obj.SetValue(InsetsProperty, value);
		#endregion

		#region Mode (Attached DP)
		public InsetMode Mode
		{
			get => (InsetMode)GetValue(ModeProperty);
			set => SetValue(ModeProperty, value);
		}

		public static DependencyProperty ModeProperty { get; } = DependencyProperty.RegisterAttached(
			nameof(Mode),
			typeof(InsetMode),
			typeof(SafeAreaBehavior),
			new PropertyMetadata(InsetMode.Padding));

		public static InsetMode GetInsetMode(DependencyObject obj) => (InsetMode)obj.GetValue(ModeProperty);
		public static void SetInsetMode(DependencyObject obj, InsetMode value) => obj.SetValue(ModeProperty, value);
		#endregion
	}
}
