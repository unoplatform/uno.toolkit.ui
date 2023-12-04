#if IS_WINUI
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml;
#endif

namespace Uno.Toolkit.UI;

public partial class ResponsiveView
{
	#region DependencyProperty: NarrowestTemplate

	public static DependencyProperty NarrowestTemplateProperty { get; } = DependencyProperty.Register(
		nameof(NarrowestTemplate),
		typeof(DataTemplate),
		typeof(ResponsiveView),
		new PropertyMetadata(default(DataTemplate?), OnNarrowestTemplateChanged));

	public DataTemplate? NarrowestTemplate
	{
		get => (DataTemplate?)GetValue(NarrowestTemplateProperty);
		set => SetValue(NarrowestTemplateProperty, value);
	}

	#endregion
	#region DependencyProperty: NarrowTemplate

	public static DependencyProperty NarrowTemplateProperty { get; } = DependencyProperty.Register(
		nameof(NarrowTemplate),
		typeof(DataTemplate),
		typeof(ResponsiveView),
		new PropertyMetadata(default(DataTemplate?), OnNarrowTemplateChanged));

	public DataTemplate? NarrowTemplate
	{
		get => (DataTemplate?)GetValue(NarrowTemplateProperty);
		set => SetValue(NarrowTemplateProperty, value);
	}

	#endregion
	#region DependencyProperty: NormalTemplate

	public static DependencyProperty NormalTemplateProperty { get; } = DependencyProperty.Register(
		nameof(NormalTemplate),
		typeof(DataTemplate),
		typeof(ResponsiveView),
		new PropertyMetadata(default(DataTemplate?), OnNormalTemplateChanged));

	public DataTemplate? NormalTemplate
	{
		get => (DataTemplate?)GetValue(NormalTemplateProperty);
		set => SetValue(NormalTemplateProperty, value);
	}

	#endregion
	#region DependencyProperty: WideTemplate

	public static DependencyProperty WideTemplateProperty { get; } = DependencyProperty.Register(
		nameof(WideTemplate),
		typeof(DataTemplate),
		typeof(ResponsiveView),
		new PropertyMetadata(default(DataTemplate?), OnWideTemplateChanged));

	public DataTemplate? WideTemplate
	{
		get => (DataTemplate?)GetValue(WideTemplateProperty);
		set => SetValue(WideTemplateProperty, value);
	}

	#endregion
	#region DependencyProperty: WidestTemplate

	public static DependencyProperty WidestTemplateProperty { get; } = DependencyProperty.Register(
		nameof(WidestTemplate),
		typeof(DataTemplate),
		typeof(ResponsiveView),
		new PropertyMetadata(default(DataTemplate?), OnWidestTemplateChanged));

	public DataTemplate? WidestTemplate
	{
		get => (DataTemplate?)GetValue(WidestTemplateProperty);
		set => SetValue(WidestTemplateProperty, value);
	}

	#endregion

	#region DependencyProperty: ResponsiveLayout

	public static DependencyProperty ResponsiveLayoutProperty { get; } = DependencyProperty.Register(
		nameof(ResponsiveLayout),
		typeof(ResponsiveLayout),
		typeof(ResponsiveView),
		new PropertyMetadata(default(ResponsiveLayout), OnResponsiveLayoutChanged));

	public ResponsiveLayout ResponsiveLayout
	{
		get => (ResponsiveLayout)GetValue(ResponsiveLayoutProperty);
		set => SetValue(ResponsiveLayoutProperty, value);
	}

	#endregion

	private static void OnNarrowestTemplateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => (sender as ResponsiveView)?.ResolveTemplate();
	private static void OnNarrowTemplateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => (sender as ResponsiveView)?.ResolveTemplate();
	private static void OnNormalTemplateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => (sender as ResponsiveView)?.ResolveTemplate();
	private static void OnWideTemplateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => (sender as ResponsiveView)?.ResolveTemplate();
	private static void OnWidestTemplateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => (sender as ResponsiveView)?.ResolveTemplate();

	private static void OnResponsiveLayoutChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => (sender as ResponsiveView)?.ResolveTemplate();
}
