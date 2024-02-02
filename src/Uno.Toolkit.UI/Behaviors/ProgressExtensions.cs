using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Uno.UI.Extensions;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using ItemsRepeater = Microsoft.UI.Xaml.Controls.ItemsRepeater;
#endif

namespace Uno.Toolkit.UI
{
	public static class ProgressExtensions
	{
		#region DependencyProperty: IsActive

		/// <summary>
		/// Backing property for a value which recursively sets whether the
		/// nested progress controls are displaying a loading animation.
		/// </summary>
		public static readonly DependencyProperty IsActiveProperty = DependencyProperty.RegisterAttached(
			"IsActive",
			typeof(bool),
			typeof(ProgressExtensions),
			new PropertyMetadata(false, IsActiveChanged));

		public static bool GetIsActive(FrameworkElement element) => (bool)element.GetValue(IsActiveProperty);
		public static void SetIsActive(FrameworkElement element, bool value) => element.SetValue(IsActiveProperty, value);

		#endregion

		private static void IsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is FrameworkElement element &&
				e.NewValue is bool isActive)
			{
				foreach (var item in element.GetChildren())
				{
					if (item is ProgressRing progressRing)
					{
						progressRing.IsActive = isActive;
					}
					else if (item is ProgressBar progressBar)
					{
						progressBar.IsIndeterminate = isActive;
					}
				}
			}
		}
	}
}
