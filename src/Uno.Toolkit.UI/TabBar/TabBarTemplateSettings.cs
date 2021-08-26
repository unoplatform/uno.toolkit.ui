using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if IS_WINUI
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml;
#endif

namespace Uno.UI.ToolkitLib
{
	public partial class TabBarTemplateSettings : DependencyObject
	{
		internal static readonly DependencyProperty SelectionIndicatorWidthProperty =
			DependencyProperty.Register(nameof(SelectionIndicatorWidth),
				typeof(double),
				typeof(TabBarTemplateSettings),
				new PropertyMetadata(null));

		public double SelectionIndicatorWidth
		{
			get => (double)GetValue(SelectionIndicatorWidthProperty);
			internal set => SetValue(SelectionIndicatorWidthProperty, value);
		}
	}
}