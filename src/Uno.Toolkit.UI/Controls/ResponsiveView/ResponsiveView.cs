using System.Linq;
using Windows.Foundation;
using System;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.UI;

public partial class ResponsiveView : ContentControl, IResponsiveCallback
{
	internal ResolvedLayout<DataTemplate?>? ResolvedLayout { get; private set; }

	public ResponsiveView()
	{
		this.DefaultStyleKey = typeof(ResponsiveView);

		ResponsiveHelper.GetForCurrentView().Register(this);

		Loaded += ResponsiveView_Loaded;
	}

	private void ResponsiveView_Loaded(object sender, RoutedEventArgs e)
	{
		ResolveTemplate();
	}

	public void OnSizeChanged(Size size, ResponsiveLayout layout)
	{
		ResolveTemplate(size, GetAppliedLayout() ?? layout);
	}

	private void ResolveTemplate()
	{
		if (!IsLoaded) return;

		var helper = ResponsiveHelper.GetForCurrentView();

		ResolveTemplate(helper.WindowSize, GetAppliedLayout() ?? helper.Layout);
	}

	private void ResolveTemplate(Size size, ResponsiveLayout layout)
	{
		if (!IsLoaded) return;

		var defs = new (double MinWidth, ResolvedLayout<DataTemplate?> Value)[]
		{
			(layout.Narrowest, new(nameof(layout.Narrowest), NarrowestTemplate)),
			(layout.Narrow, new(nameof(layout.Narrow), NarrowTemplate)),
			(layout.Normal, new(nameof(layout.Normal), NormalTemplate)),
			(layout.Wide, new(nameof(layout.Wide), WideTemplate)),
			(layout.Widest, new(nameof(layout.Widest), WidestTemplate)),
		}.Where(x => x.Value.Value != null).ToArray();
		var match = defs.FirstOrNull(y => y.MinWidth >= size.Width) ?? defs.LastOrNull();
		var resolved = match?.Value;

		if (resolved != ResolvedLayout)
		{
			Content = resolved?.Value?.LoadContent() as UIElement;
			ResolvedLayout = resolved;
		}
	}

	internal ResponsiveLayout? GetAppliedLayout() =>
		ResponsiveLayout ??
		this.ResolveLocalResource<ResponsiveLayout>(ResponsiveLayout.DefaultResourceKey);
}
