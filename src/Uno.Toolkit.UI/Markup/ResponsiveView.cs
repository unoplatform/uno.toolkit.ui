#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.UI;

public partial class ResponsiveView : ContentControl
{
	#region Content DependencyProperties
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

	public ResponsiveView()
	{
		this.DefaultStyleKey = typeof(ResponsiveView);
	}

	protected override void OnApplyTemplate()
	{
		base.OnApplyTemplate();

		FrameworkElement root = (FrameworkElement)GetTemplateChild("RootElement");

		if (VisualStateManager.GetVisualStateGroups(root)[0] is VisualStateGroup group)
		{
			group.CurrentStateChanged += OnVisualStateChanged;

			// TODO: When first appearing `Content` has nothing since content is only added when VisualState is changed
			// How to handle this?
			// Force the current State?
		}
	}

	private void OnVisualStateChanged(object sender, VisualStateChangedEventArgs e)
	{
		var currentState = e.NewState?.Name;

		if (currentState is null)
			return;

		DataTemplate? contentToSet = null;

		// TODO: Refactor/Improve
		if (currentState == "ExtraNarrowSize")
		{
			if (ExtraNarrowContent is not null)
			{
				contentToSet = ExtraNarrowContent;
			}
			else if (NarrowContent is not null)
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
		else if (currentState == "NarrowSize")
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
		else if (currentState == "DefaultSize")
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
		else if (currentState == "WideSize")
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
		else if (currentState == "ExtraWideSize")
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
