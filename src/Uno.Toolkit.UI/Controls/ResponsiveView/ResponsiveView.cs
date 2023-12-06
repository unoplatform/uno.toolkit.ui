using System.Collections.Generic;
using Windows.Foundation;

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
	public Layout? CurrentLayout { get; private set; }
	internal (ResponsiveLayout Layout, Size Size, Layout? Result) LastResolved { get; private set; }

	public ResponsiveView()
	{
		this.DefaultStyleKey = typeof(ResponsiveView);

		ResponsiveHelper.GetForCurrentView().Register(this);

		Loaded += ResponsiveView_Loaded;
	}

	private void ResponsiveView_Loaded(object sender, RoutedEventArgs e) => UpdateTemplate(forceApplyValue: true);

	public void OnSizeChanged(ResponsiveHelper helper) => UpdateTemplate(helper);

	private void UpdateTemplate(ResponsiveHelper? helper = null, bool forceApplyValue = false)
	{
		if (!IsLoaded) return;

		helper ??= ResponsiveHelper.GetForCurrentView();
		var resolved = helper.ResolveLayout(GetAppliedLayout(), GetAvailableLayoutOptions());

		if (forceApplyValue || CurrentLayout != resolved.Result)
		{
			Content = GetTemplateFor(resolved.Result)?.LoadContent() as UIElement;

			CurrentLayout = resolved.Result;
			LastResolved = resolved;
		}
	}

	private DataTemplate? GetTemplateFor(Layout? layout)
	{
		return layout switch
		{
			UI.Layout.Narrowest => NarrowestTemplate,
			UI.Layout.Narrow => NarrowTemplate,
			UI.Layout.Normal => NormalTemplate,
			UI.Layout.Wide => WideTemplate,
			UI.Layout.Widest => WidestTemplate,
			_ => null,
		};
	}

	private IEnumerable<Layout> GetAvailableLayoutOptions()
	{
		if (NarrowestTemplate != null) yield return UI.Layout.Narrowest;
		if (NarrowTemplate != null) yield return UI.Layout.Narrow;
		if (NormalTemplate != null) yield return UI.Layout.Normal;
		if (WideTemplate != null) yield return UI.Layout.Wide;
		if (WidestTemplate != null) yield return UI.Layout.Widest;
	}

	internal ResponsiveLayout? GetAppliedLayout() =>
		ResponsiveLayout ??
		this.ResolveLocalResource<ResponsiveLayout>(ResponsiveLayout.DefaultResourceKey) ??
		Application.Current.ResolveLocalResource<ResponsiveLayout>(ResponsiveLayout.DefaultResourceKey);
}
