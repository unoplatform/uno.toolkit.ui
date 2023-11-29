using System.Linq;
using Windows.Foundation;
using Uno.Toolkit.UI.Helpers;
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
	#region DependencyProperties

	#region Narrowest DP
	public DataTemplate NarrowestTemplate
	{
		get { return (DataTemplate)GetValue(NarrowestTemplateProperty); }
		set { SetValue(NarrowestTemplateProperty, value); }
	}

	public static readonly DependencyProperty NarrowestTemplateProperty =
		DependencyProperty.Register("NarrowestTemplate", typeof(DataTemplate), typeof(ResponsiveView), new PropertyMetadata(null, OnNarrowestTemplateChanged));

	private static void OnNarrowestTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> OnResponsiveTemplateChanged(d, e);

	#endregion

	#region Narrow DP
	public DataTemplate NarrowTemplate
	{
		get { return (DataTemplate)GetValue(NarrowTemplateProperty); }
		set { SetValue(NarrowTemplateProperty, value); }
	}

	public static readonly DependencyProperty NarrowTemplateProperty =
		DependencyProperty.Register("NarrowTemplate", typeof(DataTemplate), typeof(ResponsiveView), new PropertyMetadata(null, OnNarrowTemplateChanged));

	private static void OnNarrowTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> OnResponsiveTemplateChanged(d, e);
	#endregion

	#region Normal DP
	public DataTemplate NormalTemplate
	{
		get { return (DataTemplate)GetValue(NormalTemplateProperty); }
		set { SetValue(NormalTemplateProperty, value); }
	}

	public static readonly DependencyProperty NormalTemplateProperty =
		DependencyProperty.Register("NormalTemplate", typeof(DataTemplate), typeof(ResponsiveView), new PropertyMetadata(null, OnNormalTemplateChanged));

	private static void OnNormalTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> OnResponsiveTemplateChanged(d, e);
	#endregion

	#region Wide DP
	public DataTemplate WideTemplate
	{
		get { return (DataTemplate)GetValue(WideTemplateProperty); }
		set { SetValue(WideTemplateProperty, value); }
	}

	public static readonly DependencyProperty WideTemplateProperty =
		DependencyProperty.Register("WideTemplate", typeof(DataTemplate), typeof(ResponsiveView), new PropertyMetadata(null, OnWideTemplateChanged));

	private static void OnWideTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> OnResponsiveTemplateChanged(d, e);
	#endregion

	#region Widest DP
	public DataTemplate WidestTemplate
	{
		get { return (DataTemplate)GetValue(WidestTemplateProperty); }
		set { SetValue(WidestTemplateProperty, value); }
	}

	public static readonly DependencyProperty WidestTemplateProperty =
		DependencyProperty.Register("WidestTemplate", typeof(DataTemplate), typeof(ResponsiveView), new PropertyMetadata(null, OnWidestTemplateChanged));

	private static void OnWidestTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> OnResponsiveTemplateChanged(d, e);
	#endregion

	#region ResponsiveLayout DP
	public static DependencyProperty ResponsiveLayoutProperty { get; } = DependencyProperty.Register(
		nameof(ResponsiveLayout),
		typeof(ResponsiveLayout),
		typeof(ResponsiveView),
		new PropertyMetadata(default));

	public ResponsiveLayout ResponsiveLayout
	{
		get => (ResponsiveLayout)GetValue(ResponsiveLayoutProperty);
		set => SetValue(ResponsiveLayoutProperty, value);
	}
	#endregion

	private static void OnResponsiveTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is ResponsiveView { IsLoaded: true } view)
		{
			var dataTemplate = view.GetInitialValue();
			view.Content = dataTemplate?.LoadContent() as UIElement;
		}
	}
	#endregion

	private DataTemplate? _currentContent;

	public ResponsiveView()
	{
		this.DefaultStyleKey = typeof(ResponsiveView);

		ResponsiveHelper.GetForCurrentView().Register(this);

		Loaded += ResponsiveView_Loaded;
	}

	private void ResponsiveView_Loaded(object sender, RoutedEventArgs e)
	{
		_currentContent = GetInitialValue();

		Content = _currentContent?.LoadContent() as UIElement;
	}

	private DataTemplate? GetInitialValue()
	{
		var helper = ResponsiveHelper.GetForCurrentView();

		return GetValueForSize(helper.WindowSize, ResponsiveLayout ?? helper.Layout);
	}

	private DataTemplate? GetValueForSize(Size size, ResponsiveLayout layout)
	{
		var defs = new (double MinWidth, DataTemplate? Value)?[]
		{
			(layout.Narrowest, NarrowestTemplate),
			(layout.Narrow, NarrowTemplate),
			(layout.Normal, NormalTemplate),
			(layout.Wide, WideTemplate),
			(layout.Widest, WidestTemplate),
		}.Where(x => x?.Value != null).ToArray();

		var match = defs.FirstOrDefault(y => y?.MinWidth >= size.Width) ?? defs.LastOrDefault();

		return match?.Value;
	}

	public void OnSizeChanged(Size size, ResponsiveLayout layout)
	{
		var dataTemplate = GetValueForSize(size, ResponsiveLayout ?? layout);

		if (dataTemplate != _currentContent)
		{
			_currentContent = dataTemplate;
			Content = dataTemplate?.LoadContent() as UIElement;
		}
	}
}
