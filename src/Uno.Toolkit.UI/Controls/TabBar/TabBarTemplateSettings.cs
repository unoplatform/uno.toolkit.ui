using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

#if IS_WINUI
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml;
#endif

namespace Uno.Toolkit.UI
{
	public partial class TabBarTemplateSettings : DependencyObject
	{
		internal static readonly DependencyProperty SelectionIndicatorMaxSizeProperty =
			DependencyProperty.Register(nameof(SelectionIndicatorMaxSize),
				typeof(Size),
				typeof(TabBarTemplateSettings),
				new PropertyMetadata(default));

		public Size SelectionIndicatorMaxSize
		{
			get => (Size)GetValue(SelectionIndicatorMaxSizeProperty);
			internal set => SetValue(SelectionIndicatorMaxSizeProperty, value);
		}
	}
}
