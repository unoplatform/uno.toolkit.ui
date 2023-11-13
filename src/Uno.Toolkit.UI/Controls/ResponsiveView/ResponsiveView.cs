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

			var currentState = group.CurrentState;

			if (currentState is { })
				SetContent(currentState.Name);
		}
	}

	private void OnVisualStateChanged(object sender, VisualStateChangedEventArgs e)
	{
		var currentState = e.NewState?.Name;

		if (currentState is null)
			return;

		SetContent(currentState);
	}

	private void SetContent(string visualState)
	{
		DataTemplate? contentToSet = null;

		switch (visualState)
		{
			case "ExtraNarrowSize":
				contentToSet = ExtraNarrowContent ?? NarrowContent ?? DefaultContent ?? WideContent ?? ExtraWideContent;
				break;
			case "NarrowSize":
				contentToSet = NarrowContent ?? ExtraNarrowContent ?? DefaultContent ?? WideContent ?? ExtraWideContent;
				break;
			case "DefaultSize":
				contentToSet = DefaultContent ?? NarrowContent ?? WideContent ?? ExtraWideContent ?? ExtraNarrowContent;
				break;
			case "WideSize":
				contentToSet = WideContent ?? DefaultContent ?? ExtraWideContent ?? NarrowContent ?? ExtraNarrowContent;
				break;
			case "ExtraWideSize":
				contentToSet = ExtraWideContent ?? WideContent ?? DefaultContent ?? NarrowContent ?? ExtraNarrowContent;
				break;
		}

		if (contentToSet is not null)
		{
			Content = contentToSet.LoadContent() as UIElement;
		}
	}
}
