#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using XamlWindow = Microsoft.UI.Xaml.Window;
#else
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XamlWindow = Windows.UI.Xaml.Window;
#endif
namespace Uno.Toolkit.UI;

public partial class ResponsiveView : ContentControl
{
	#region DependencyProperty
	public UIElement ExtraNarrowContent
	{
		get { return (UIElement)GetValue(ExtraNarrowContentProperty); }
		set { SetValue(ExtraNarrowContentProperty, value); }
	}

	public static readonly DependencyProperty ExtraNarrowContentProperty =
		DependencyProperty.Register("ExtraNarrowContent", typeof(UIElement), typeof(ResponsiveView), new PropertyMetadata(null));

	public UIElement NarrowContent
	{
		get { return (UIElement)GetValue(NarrowContentProperty); }
		set { SetValue(NarrowContentProperty, value); }
	}

	public static readonly DependencyProperty NarrowContentProperty =
		DependencyProperty.Register("NarrowContent", typeof(UIElement), typeof(ResponsiveView), new PropertyMetadata(null));

	public UIElement DefaultContent
	{
		get { return (UIElement)GetValue(DefaultContentProperty); }
		set { SetValue(DefaultContentProperty, value); }
	}

	public static readonly DependencyProperty DefaultContentProperty =
		DependencyProperty.Register("DefaultContent", typeof(UIElement), typeof(ResponsiveView), new PropertyMetadata(null));

	public UIElement WideContent
	{
		get { return (UIElement)GetValue(WideContentProperty); }
		set { SetValue(WideContentProperty, value); }
	}

	public static readonly DependencyProperty WideContentProperty =
		DependencyProperty.Register("WideContent", typeof(UIElement), typeof(ResponsiveView), new PropertyMetadata(null));

	public UIElement ExtraWideContent
	{
		get { return (UIElement)GetValue(ExtraWideContentProperty); }
		set { SetValue(ExtraWideContentProperty, value); }
	}

	public static readonly DependencyProperty ExtraWideContentProperty =
		DependencyProperty.Register("ExtraWideContent", typeof(UIElement), typeof(ResponsiveView), new PropertyMetadata(null));

	#endregion

	private static double _currentWindowWidth => XamlWindow.Current.Bounds.Width;

	public ResponsiveView()
	{
		this.DefaultStyleKey = typeof(ResponsiveView);
		Loaded += OnLoaded;
		XamlWindow.Current.SizeChanged += Current_SizeChanged;
	}

	private void Current_SizeChanged(object sender, WindowSizeChangedEventArgs e)
	{
		UpdateContent();
	}

	private void OnLoaded(object sender, RoutedEventArgs e)
	{
		UpdateContent();
	}

	private void UpdateContent()
	{
		UIElement? contentToSet = null;

		if (_currentWindowWidth < ViewSize.ExtraNarrowSize && ExtraNarrowContent is not null)
		{
			contentToSet = ExtraNarrowContent;
		}
		else if (_currentWindowWidth >= ViewSize.ExtraNarrowSize && _currentWindowWidth < ViewSize.NarrowSize && NarrowContent is not null)
		{
			contentToSet = NarrowContent;
		}
		else if (_currentWindowWidth >= ViewSize.NarrowSize && _currentWindowWidth < ViewSize.DefaultSize && DefaultContent is not null)
		{
			contentToSet = DefaultContent;
		}
		else if (_currentWindowWidth >= ViewSize.DefaultSize && _currentWindowWidth < ViewSize.WideSize && WideContent is not null)
		{
			contentToSet = WideContent;
		}
		else if (_currentWindowWidth >= ViewSize.WideSize && _currentWindowWidth < ViewSize.ExtraWideSize && ExtraWideContent is not null)
		{
			contentToSet = ExtraWideContent;
		}

		if (contentToSet is not null)
		{
			Content = contentToSet;
		}
	}

}

public static class ViewSize
{
	public const int ExtraNarrowSize = 250;
	public const int NarrowSize = 500;
	public const int DefaultSize = 800;
	public const int WideSize = 1100;
	public const int ExtraWideSize = 1400;
}
