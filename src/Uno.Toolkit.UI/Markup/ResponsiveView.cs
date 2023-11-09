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
	public DataTemplate ExtraNarrowContent
	{
		get { return (DataTemplate)GetValue(ExtraNarrowContentProperty); }
		set { SetValue(ExtraNarrowContentProperty, value); }
	}

	public static readonly DependencyProperty ExtraNarrowContentProperty =
		DependencyProperty.Register("ExtraNarrowContent", typeof(DataTemplate), typeof(ResponsiveView), new PropertyMetadata(null));

	public DataTemplate NarrowContent
	{
		get { return (DataTemplate)GetValue(NarrowContentProperty); }
		set { SetValue(NarrowContentProperty, value); }
	}

	public static readonly DependencyProperty NarrowContentProperty =
		DependencyProperty.Register("NarrowContent", typeof(DataTemplate), typeof(ResponsiveView), new PropertyMetadata(null));

	public DataTemplate DefaultContent
	{
		get { return (DataTemplate)GetValue(DefaultContentProperty); }
		set { SetValue(DefaultContentProperty, value); }
	}

	public static readonly DependencyProperty DefaultContentProperty =
		DependencyProperty.Register("DefaultContent", typeof(DataTemplate), typeof(ResponsiveView), new PropertyMetadata(null));

	public DataTemplate WideContent
	{
		get { return (DataTemplate)GetValue(WideContentProperty); }
		set { SetValue(WideContentProperty, value); }
	}

	public static readonly DependencyProperty WideContentProperty =
		DependencyProperty.Register("WideContent", typeof(DataTemplate), typeof(ResponsiveView), new PropertyMetadata(null));

	public DataTemplate ExtraWideContent
	{
		get { return (DataTemplate)GetValue(ExtraWideContentProperty); }
		set { SetValue(ExtraWideContentProperty, value); }
	}

	public static readonly DependencyProperty ExtraWideContentProperty =
		DependencyProperty.Register("ExtraWideContent", typeof(DataTemplate), typeof(ResponsiveView), new PropertyMetadata(null));

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
		DataTemplate? contentToSet = null;

		if(_currentWindowWidth <= ViewSize.ExtraNarrowSize)
		{
			if(ExtraNarrowContent is not null)
			{
				contentToSet = ExtraNarrowContent;
			}
			else if(NarrowContent is not null)
			{
				contentToSet = NarrowContent;
			}
			else if (DefaultContent is not null)
			{
				contentToSet = DefaultContent;
			}
			else if (WideContent is not null)
			{
				contentToSet = WideContent;
			}
			else if (ExtraWideContent is not null)
			{
				contentToSet = ExtraWideContent;
			}
		}

		if (_currentWindowWidth <= ViewSize.NarrowSize && _currentWindowWidth > ViewSize.ExtraNarrowSize)
		{
			if (NarrowContent is not null)
			{
				contentToSet = NarrowContent;
			}
			else if (ExtraNarrowContent is not null)
			{
				contentToSet = ExtraNarrowContent;
			}
			else if (DefaultContent is not null)
			{
				contentToSet = DefaultContent;
			}
			else if (WideContent is not null)
			{
				contentToSet = WideContent;
			}
			else if (ExtraWideContent is not null)
			{
				contentToSet = ExtraWideContent;
			}
		}

		if (_currentWindowWidth <= ViewSize.DefaultSize && _currentWindowWidth > ViewSize.NarrowSize)
		{
			if (DefaultContent is not null)
			{
				contentToSet = DefaultContent;
			}
			else if (NarrowContent is not null)
			{
				contentToSet = NarrowContent;
			}
			else if (WideContent is not null)
			{
				contentToSet = WideContent;
			}
			else if (ExtraWideContent is not null)
			{
				contentToSet = ExtraWideContent;
			}
			else if (ExtraNarrowContent is not null)
			{
				contentToSet = ExtraNarrowContent;
			}
		}

		if (_currentWindowWidth <= ViewSize.WideSize && _currentWindowWidth > ViewSize.DefaultSize)
		{
			if (WideContent is not null)
			{
				contentToSet = WideContent;
			}
			else if (DefaultContent is not null)
			{
				contentToSet = DefaultContent;
			}
			else if (ExtraWideContent is not null)
			{
				contentToSet = ExtraWideContent;
			}
			else if (NarrowContent is not null)
			{
				contentToSet = NarrowContent;
			}
			else if (ExtraNarrowContent is not null)
			{
				contentToSet = ExtraNarrowContent;
			}
		}

		if (_currentWindowWidth <= ViewSize.ExtraWideSize && _currentWindowWidth > ViewSize.WideSize)
		{
			if (ExtraWideContent is not null)
			{
				contentToSet = ExtraWideContent;
			}
			else if (WideContent is not null)
			{
				contentToSet = WideContent;
			}
			else if (DefaultContent is not null)
			{
				contentToSet = DefaultContent;
			}
			else if (NarrowContent is not null)
			{
				contentToSet = NarrowContent;
			}
			else if (ExtraNarrowContent is not null)
			{
				contentToSet = ExtraNarrowContent;
			}
		}

		if (contentToSet is not null)
		{
			Content = contentToSet.LoadContent() as UIElement;
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
