using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
#endif

namespace Uno.Toolkit.UI
{
	/// <summary>
	/// Extensions for <see cref="Selector"/>
	/// </summary>
	[Bindable]
	public static partial class SelectorExtensions
	{
		#region SelectionOffset Attached Property
		public static double GetSelectionOffset(DependencyObject obj)
		{
			return (double)obj.GetValue(SelectionOffsetProperty);
		}

		public static void SetSelectionOffset(DependencyObject obj, double value)
		{
			obj.SetValue(SelectionOffsetProperty, value);
		}

		/// <summary>
		/// Property that can be used to observe the position of the currently selected item within a <see cref="Selector"/>
		/// </summary>
		public static DependencyProperty SelectionOffsetProperty { get; } =
			DependencyProperty.RegisterAttached("SelectionOffset", typeof(double), typeof(SelectorExtensions), new PropertyMetadata(0d));
		#endregion
	}
}
