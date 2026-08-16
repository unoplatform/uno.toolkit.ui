using System;
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

public partial class ResponsiveView : Control;

partial class ResponsiveView
{
	private class TemplateParts
	{
		public const string ResponsiveRoot = "ResponsiveRoot";
	}
}

[TemplatePart(Name = TemplateParts.ResponsiveRoot, Type = typeof(Border))]
partial class ResponsiveView
{
	public Layout? CurrentLayout { get; private set; }
	internal ResolvedLayout? LastResolved { get; private set; }

	private Border? _responsiveRoot;

	public ResponsiveView()
	{
		DefaultStyleKey = typeof(ResponsiveView);

		Loaded += OnLoaded;
		Unloaded += OnUnloaded;
	}

	protected override void OnApplyTemplate()
	{
		base.OnApplyTemplate();

		_responsiveRoot = GetTemplateChild(TemplateParts.ResponsiveRoot) as Border
			?? throw new Exception($"The template part '{TemplateParts.ResponsiveRoot}' is missing or is not of type Border.");
	}

	private void OnLoaded(object sender, RoutedEventArgs e)
	{
		if (XamlRoot is null) return;

		ResponsiveHelper.InitializeIfNeeded(XamlRoot);
		ResponsiveHelper.WindowSizeChanged += OnWindowSizeChanged;

		UpdateTemplate(forceApplyValue: true);
	}

	private void OnUnloaded(object sender, RoutedEventArgs e)
	{
		ResponsiveHelper.WindowSizeChanged -= OnWindowSizeChanged;
	}

	private void OnWindowSizeChanged(object sender, Size size)
	{
		if (size == LastResolved?.Size) return;

		UpdateTemplate();
	}

	internal void ForceResponsiveSize(Size size) // test backdoor
	{
		var resolved = ResponsiveHelper.ResolveLayout(size, GetAppliedLayout(), GetAvailableLayoutOptions());
		UpdateTemplate(resolved, forceApplyValue: true);
	}

	private void UpdateTemplate(bool forceApplyValue = false)
	{
		if (!IsLoaded) return;

		var resolved = ResponsiveHelper.ResolveLayout(ResponsiveHelper.WindowSize, GetAppliedLayout(), GetAvailableLayoutOptions());
		UpdateTemplate(resolved, forceApplyValue);
	}

	private void UpdateTemplate(ResolvedLayout resolved, bool forceApplyValue = false)
	{
		if (forceApplyValue || CurrentLayout != resolved.Result)
		{
			if (_responsiveRoot is { })
			{
				_responsiveRoot.Child = GetTemplateFor(resolved.Result)?.LoadContent() as UIElement;
			}

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

	internal UIElement? GetResolvedContent() => _responsiveRoot?.Child;

	internal ResponsiveLayout? GetAppliedLayout() =>
		ResponsiveLayout ??
		this.ResolveLocalResource<ResponsiveLayout>(ResponsiveLayout.DefaultResourceKey) ??
		Application.Current.ResolveLocalResource<ResponsiveLayout>(ResponsiveLayout.DefaultResourceKey);
}
