using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.UI
{
	internal static class PaddingHelper
	{
		public static Thickness GetPadding(UIElement uiElement)
		{
			//Fast path
			if (uiElement is FrameworkElement fe && TryGetPadding(fe, out var padding))
			{
				return padding;
			}

			var property = uiElement.FindDependencyPropertyUsingReflection<Thickness>("PaddingProperty");
			return property != null && uiElement.GetValue(property) is Thickness t ? t : default;
		}

		public static bool SetPadding(UIElement uiElement, Thickness padding)
		{
			//Fast path
			if (uiElement is FrameworkElement fe && TrySetPadding(fe, padding))
			{
				return true;
			}

			var property = uiElement.FindDependencyPropertyUsingReflection<Thickness>("PaddingProperty");
			if (property != null)
			{
				uiElement.SetValue(property, padding);
				return true;
			}

			return false;
		}

		internal static bool TryGetPadding(this FrameworkElement frameworkElement, out Thickness padding)
		{
			(var result, padding) = frameworkElement switch
			{
				Grid g => (true, g.Padding),
				StackPanel sp => (true, sp.Padding),
				Control c => (true, c.Padding),
				ContentPresenter cp => (true, cp.Padding),
				Border b => (true, b.Padding),

				_ => (false, default),
			};
			return result;
		}

		public static bool TrySetPadding(FrameworkElement frameworkElement, Thickness padding)
		{
			return frameworkElement switch
			{
				Grid g => (g.Padding = padding) is { },
				StackPanel sp => (sp.Padding = padding) is { },
				Control c => (c.Padding = padding) is { },
				ContentPresenter cp => (cp.Padding = padding) is { },
				Border b => (b.Padding = padding) is { },

				_ => false,
			};
		}
	}
}
