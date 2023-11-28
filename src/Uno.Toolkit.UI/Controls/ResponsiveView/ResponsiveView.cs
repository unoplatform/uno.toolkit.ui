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
	public DataTemplate NarrowestContent
	{
		get { return (DataTemplate)GetValue(NarrowestContentProperty); }
		set { SetValue(NarrowestContentProperty, value); }
	}

	public static readonly DependencyProperty NarrowestContentProperty =
		DependencyProperty.Register("NarrowestContent", typeof(DataTemplate), typeof(ResponsiveView), new PropertyMetadata(null, OnNarrowestContentChanged));

	private static void OnNarrowestContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> OnResponsiveContentChanged(d, e);

	#endregion

	#region Narrow DP
	public DataTemplate NarrowContent
	{
		get { return (DataTemplate)GetValue(NarrowContentProperty); }
		set { SetValue(NarrowContentProperty, value); }
	}

	public static readonly DependencyProperty NarrowContentProperty =
		DependencyProperty.Register("NarrowContent", typeof(DataTemplate), typeof(ResponsiveView), new PropertyMetadata(null, OnNarrowContentChanged));

	private static void OnNarrowContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> OnResponsiveContentChanged(d, e);
	#endregion

	#region Normal DP
	public DataTemplate NormalContent
	{
		get { return (DataTemplate)GetValue(NormalContentProperty); }
		set { SetValue(NormalContentProperty, value); }
	}

	public static readonly DependencyProperty NormalContentProperty =
		DependencyProperty.Register("NormalContent", typeof(DataTemplate), typeof(ResponsiveView), new PropertyMetadata(null, OnNormalContentChanged));

	private static void OnNormalContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> OnResponsiveContentChanged(d, e);
	#endregion

	#region Wide DP
	public DataTemplate WideContent
	{
		get { return (DataTemplate)GetValue(WideContentProperty); }
		set { SetValue(WideContentProperty, value); }
	}

	public static readonly DependencyProperty WideContentProperty =
		DependencyProperty.Register("WideContent", typeof(DataTemplate), typeof(ResponsiveView), new PropertyMetadata(null, OnWideContentChanged));

	private static void OnWideContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> OnResponsiveContentChanged(d, e);
	#endregion

	#region Widest DP
	public DataTemplate WidestContent
	{
		get { return (DataTemplate)GetValue(WidestContentProperty); }
		set { SetValue(WidestContentProperty, value); }
	}

	public static readonly DependencyProperty WidestContentProperty =
		DependencyProperty.Register("WidestContent", typeof(DataTemplate), typeof(ResponsiveView), new PropertyMetadata(null, OnWidestContentChanged));

	private static void OnWidestContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> OnResponsiveContentChanged(d, e);
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

	private static void OnResponsiveContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is ResponsiveView { IsLoaded: true } view)
		{
			var dataTemplate = view.GetInitialValue();
			view.Content = dataTemplate?.LoadContent() as UIElement;
		}
	}
	#endregion

#if WINDOWS || WINDOWS_UWP
	public object ResponsiveContent => Content;
#else
	public object ResponsiveContent => ContentTemplateRoot;
#endif

	public ResponsiveView()
	{
		this.DefaultStyleKey = typeof(ResponsiveView);

		ResponsiveHelper.GetForCurrentView().Register(this);

		Loaded += ResponsiveView_Loaded;
	}

	private void ResponsiveView_Loaded(object sender, RoutedEventArgs e)
	{
		var dataTemplate = GetInitialValue();
#if WINDOWS || WINDOWS_UWP
		Content = dataTemplate?.LoadContent() as UIElement;
#else
        ContentTemplate = dataTemplate;
#endif
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
			(layout.Narrowest, NarrowestContent),
			(layout.Narrow, NarrowContent),
			(layout.Normal, NormalContent),
			(layout.Wide, WideContent),
			(layout.Widest, WidestContent),
		}.Where(x => x?.Value != null).ToArray();

		var match = defs.FirstOrDefault(y => y?.MinWidth >= size.Width) ?? defs.LastOrDefault();

		return match?.Value;
	}

	public void OnSizeChanged(Size size, ResponsiveLayout layout)
	{
		var dataTemplate = GetValueForSize(size, ResponsiveLayout ?? layout);
#if WINDOWS || WINDOWS_UWP
		Content = dataTemplate?.LoadContent() as UIElement;
#else
        ContentTemplate = dataTemplate;
#endif
	}
}
