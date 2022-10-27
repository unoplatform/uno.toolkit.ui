using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using Uno.Extensions;
using Uno.Logging;
using Windows.System;
using Windows.UI.ViewManagement;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
#endif

namespace Uno.Toolkit.UI;

public static class FlipViewExtensions
{
	#region DependencyProperty: Next
	public static DependencyProperty NextProperty { get; } =
	DependencyProperty.RegisterAttached("Next", typeof(FlipView), typeof(FlipViewExtensions), new PropertyMetadata(null, OnNextChanged));

	public static void SetNext(Button element, FlipView value) => element.SetValue(NextProperty, value);

	public static FlipView GetNext(Button element) => (FlipView)element.GetValue(NextProperty);

	#endregion

	#region DependencyProperty: Previous
	public static DependencyProperty PreviousProperty { get; } =
	DependencyProperty.RegisterAttached("Previous", typeof(FlipView), typeof(FlipViewExtensions), new PropertyMetadata(null, OnPreviousChanged));

	public static void SetPrevious(Button element, FlipView value) => element.SetValue(PreviousProperty, value);

	public static FlipView GetPrevious(Button element) => (FlipView)element.GetValue(PreviousProperty);
	#endregion

	static void OnNextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		var btn = (ButtonBase)d;

		if (e.NewValue is null)
		{
			btn.Click -= OnBtnClick;
			return;
		}

		var flipView = (FlipView)e.NewValue;

		btn.Click -= OnBtnClick;
		btn.Click += OnBtnClick;


		static void OnBtnClick(object sender, RoutedEventArgs e)
		{
			var flipView = GetNext((Button)sender);
			GoNext(flipView);
		}
	}

	static void OnPreviousChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		var btn = (ButtonBase)d;

		if (e.NewValue is null)
		{
			btn.Click -= OnBtnClick;
			return;
		}

		var flipView = (FlipView)e.NewValue;

		btn.Click -= OnBtnClick;
		btn.Click += OnBtnClick;

		static void OnBtnClick(object sender, RoutedEventArgs e)
		{
			var flipView = GetPrevious((Button)sender);
			GoBack(flipView);
		}
	}

	static void GoBack(FlipView element)
	{
		var index = element.SelectedIndex - 1;

		if (index < 0)
			return;

		element.SelectedIndex = index;
	}

	static void GoNext(FlipView element)
	{
		var index = element.SelectedIndex + 1;

		if (index >= element.Items.Count)
			return;

		element.SelectedIndex = index;
	}
}
