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
		#region DependencyProperty: SelectionIndicatorMaxSize

		internal static DependencyProperty SelectionIndicatorMaxSizeProperty { get; } = DependencyProperty.Register(
			nameof(SelectionIndicatorMaxSize),
			typeof(Size),
			typeof(TabBarTemplateSettings),
			new PropertyMetadata(default(Size)));

		public Size SelectionIndicatorMaxSize
		{
			get => (Size)GetValue(SelectionIndicatorMaxSizeProperty);
			set => SetValue(SelectionIndicatorMaxSizeProperty, value);
		}

		#endregion
	}
}
