using System.Diagnostics.CodeAnalysis;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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

					if (!ApplyTextWrapping(button, GetTextWrapping(button)))
					{
						// Visual tree may not yet be built at Loaded time, defer to LayoutUpdated
						void OnLayoutUpdated(object? s, object ea)
						{
							button.LayoutUpdated -= OnLayoutUpdated;
							ApplyTextWrapping(button, GetTextWrapping(button));
						}

						button.LayoutUpdated += OnLayoutUpdated;
					}
				}

				button.Loaded += OnLoaded;
			}
		}
	}

	private static bool ApplyTextWrapping(AppBarButton button, TextWrapping wrapping)
	{
		if (button.GetFirstDescendant<ContentPresenter>("ContentPresenter") is not { } presenter)
		{
			return false;
		}

		presenter.TextWrapping = wrapping;

		if (wrapping != TextWrapping.NoWrap)
		{
			presenter.HorizontalAlignment = HorizontalAlignment.Stretch;

			if (presenter.Parent is FrameworkElement contentRoot)
			{
				contentRoot.HorizontalAlignment = HorizontalAlignment.Stretch;
			}
		}
		else
		{
			presenter.ClearValue(FrameworkElement.HorizontalAlignmentProperty);

			if (presenter.Parent is FrameworkElement contentRoot)
			{
				contentRoot.ClearValue(FrameworkElement.HorizontalAlignmentProperty);
			}
		}

		if (button.GetFirstDescendant<TextBlock>() is { } textBlock)
		{
			textBlock.TextWrapping = wrapping;
			textBlock.TextAlignment = wrapping != TextWrapping.NoWrap ? TextAlignment.Center : TextAlignment.Start;
		}

		return true;
	}
}
