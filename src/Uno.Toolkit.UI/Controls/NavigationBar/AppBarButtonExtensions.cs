using System.Diagnostics.CodeAnalysis;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endif

namespace Uno.Toolkit.UI;

/// <summary>
/// Attached properties for <see cref="AppBarButton"/>.
/// </summary>
public static class AppBarButtonExtensions
{
	#region Property: TextWrapping

	public static DependencyProperty TextWrappingProperty { [DynamicDependency(nameof(GetTextWrapping))] get; } = DependencyProperty.RegisterAttached(
		"TextWrapping",
		typeof(TextWrapping),
		typeof(AppBarButtonExtensions),
		new PropertyMetadata(TextWrapping.NoWrap, OnTextWrappingChanged));

	[DynamicDependency(nameof(SetTextWrapping))]
	public static TextWrapping GetTextWrapping(DependencyObject obj) => (TextWrapping)obj.GetValue(TextWrappingProperty);

	[DynamicDependency(nameof(GetTextWrapping))]
	public static void SetTextWrapping(DependencyObject obj, TextWrapping value) => obj.SetValue(TextWrappingProperty, value);

	#endregion

	private static void OnTextWrappingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is AppBarButton button)
		{
			if (button.IsLoaded)
			{
				ApplyTextWrapping(button, (TextWrapping)e.NewValue);
			}
			else
			{
				void OnLoaded(object sender, RoutedEventArgs args)
				{
					button.Loaded -= OnLoaded;
					ApplyTextWrapping(button, GetTextWrapping(button));
				}

				button.Loaded += OnLoaded;
			}
		}
	}

	private static void ApplyTextWrapping(AppBarButton button, TextWrapping wrapping)
	{
		if (FindChildByName<ContentPresenter>(button, "ContentPresenter") is { } presenter)
		{
			presenter.TextWrapping = wrapping;
		}
	}

	private static T? FindChildByName<T>(DependencyObject parent, string name) where T : FrameworkElement
	{
		for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
		{
			var child = VisualTreeHelper.GetChild(parent, i);

			if (child is T match && match.Name == name)
			{
				return match;
			}

			if (FindChildByName<T>(child, name) is { } result)
			{
				return result;
			}
		}

		return null;
	}
}
