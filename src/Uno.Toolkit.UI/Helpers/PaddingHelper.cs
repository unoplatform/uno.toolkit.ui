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

		public static bool TryGetPadding(FrameworkElement frameworkElement, out Thickness padding)
		{
			switch (frameworkElement)
			{
				case Grid g:
					padding = g.Padding;
					return true;

				case StackPanel sp:
					padding = sp.Padding;
					return true;

				case Control c:
					padding = c.Padding;
					return true;

				case ContentPresenter cp:
					padding = cp.Padding;
					return true;

				case Border b:
					padding = b.Padding;
					return true;
			}

			padding = default;
			return false;
		}

		public static bool TrySetPadding(FrameworkElement frameworkElement, Thickness padding)
		{
			switch (frameworkElement)
			{
				case Grid g:
					g.Padding = padding;
					return true;

				case StackPanel sp:
					sp.Padding = padding;
					return true;

				case Control c:
					c.Padding = padding;
					return true;

				case ContentPresenter cp:
					cp.Padding = padding;
					return true;

				case Border b:
					b.Padding = padding;
					return true;
			}

			return false;
		}
	}
}
