using System;
using System.Collections.Specialized;
using System.Diagnostics;
using Microsoft.UI.Xaml.Input;
using Windows.Foundation;
using Uno.UI.Extensions;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.UI;

public partial class DockPaneItemsGrid : Grid
{
	internal LayoutPane? Owner { get; private set; }

	public DockPaneItemsGrid()
	{
		Loaded += (s, e) =>
		{
			Owner = this.FindFirstAncestor<LayoutPane>();
			Owner?.OnItemsPanelPrepared(this);
		};

		//todo@xy: extract to template and dp (from DockControl to here)
		RowSpacing = ColumnSpacing = 10;
	}

#if DEBUG
	protected override Size MeasureOverride(Size availableSize)
	{
		var result = base.MeasureOverride(availableSize);
		return result;
	}
	protected override Size ArrangeOverride(Size finalSize)
	{
		var result =  base.ArrangeOverride(finalSize);
		return result;
	}
#endif
}
