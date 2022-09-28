using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
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

namespace Uno.Toolkit.UI
{
	partial class SafeArea
	{
		#region Insets (Attached DP)

		/// <summary>
		/// Sets the type of insets that SafeArea will use when determining the "unsafe" areas to avoid.
		/// <para>A mask of <see cref="InsetMask.VisibleBounds"/> will apply insets on all sides,
		/// a mask of <see cref="InsetMask.SoftInput"/> will adjust only the bottom inset to keep the control above any input panel such as the on-screen keyboard.</para>
		/// <para>The different options can be combined as bit flags.</para>
		/// </summary>
		public static DependencyProperty InsetsProperty { get; } = DependencyProperty.RegisterAttached(
			"Insets",
			typeof(InsetMask),
			typeof(SafeArea),
			new PropertyMetadata(InsetMask.None, OnInsetsChanged));

		public static InsetMask GetInsets(DependencyObject obj) => (InsetMask)obj.GetValue(InsetsProperty);
		public static void SetInsets(DependencyObject obj, InsetMask value) => obj.SetValue(InsetsProperty, value);
		#endregion

		#region Mode (Attached DP)

		/// <summary>
		/// Used by the SafeArea to determine how the insets should be applied to a control that is in an "unsafe" area.
		/// <para>Defaults to <see cref="InsetMask.Padding"/></para>
		/// </summary>
		public static DependencyProperty ModeProperty { get; } = DependencyProperty.RegisterAttached(
			"Mode",
			typeof(InsetMode),
			typeof(SafeArea),
			new PropertyMetadata(InsetMode.Padding, OnInsetModeChanged));

		public static InsetMode GetMode(DependencyObject obj) => (InsetMode)obj.GetValue(ModeProperty);
		public static void SetMode(DependencyObject obj, InsetMode value) => obj.SetValue(ModeProperty, value);
		#endregion

		#region SafeAreaOverride (Attached DP)
		public static DependencyProperty SafeAreaOverrideProperty { get; } = DependencyProperty.RegisterAttached(
			"SafeAreaOverride",
			typeof(Thickness?),
			typeof(SafeArea),
			new PropertyMetadata(default, OnSafeAreaOverrideChanged));

		public static Thickness? GetSafeAreaOverride(DependencyObject obj) => (Thickness?)obj.GetValue(SafeAreaOverrideProperty);
		public static void SetSafeAreaOverride(DependencyObject obj, Thickness? value) => obj.SetValue(SafeAreaOverrideProperty, value);
		#endregion
	}
}
