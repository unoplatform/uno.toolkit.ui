using System;
using System.Collections.Generic;
using System.Text;
using Divider = Uno.Toolkit.UI.Divider;
#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endif

namespace Uno.Toolkit.UI
{
	/// <summary>
	/// A thin line that groups content in lists and layouts, with an optional subheader below it.
	/// </summary>
	[TemplateVisualState(GroupName = VisualStateNames.SubHeaderStates, Name = VisualStateNames.SubHeaderVisible)]
	[TemplateVisualState(GroupName = VisualStateNames.SubHeaderStates, Name = VisualStateNames.SubHeaderCollapsed)]
	public partial class Divider : Control
	{
		private static class VisualStateNames
		{
			public const string SubHeaderStates = nameof(SubHeaderStates);
			public const string SubHeaderVisible = nameof(SubHeaderVisible);
			public const string SubHeaderCollapsed = nameof(SubHeaderCollapsed);
		}

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			UpdateSubHeaderState(useTransitions: false);
		}

		private static void OnSubHeaderChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is Divider divider)
			{
				divider.UpdateSubHeaderState(useTransitions: true);
			}
		}

		// Lets templates collapse the subheader without a converter (the base library doesn't depend on Uno.Themes' converters).
		private void UpdateSubHeaderState(bool useTransitions)
		{
			var state = string.IsNullOrEmpty(SubHeader)
				? VisualStateNames.SubHeaderCollapsed
				: VisualStateNames.SubHeaderVisible;

			VisualStateManager.GoToState(this, state, useTransitions);
		}

		public string SubHeader
		{
			get { return (string)GetValue(SubHeaderProperty); }
			set { SetValue(SubHeaderProperty, value); }
		}

		public static readonly DependencyProperty SubHeaderProperty =
			DependencyProperty.Register("SubHeader", typeof(string), typeof(Divider), new PropertyMetadata(string.Empty, OnSubHeaderChanged));

		public Brush SubHeaderForeground
		{
			get { return (Brush)GetValue(SubHeaderForegroundProperty); }
			set { SetValue(SubHeaderForegroundProperty, value); }
		}

		public static readonly DependencyProperty SubHeaderForegroundProperty =
			DependencyProperty.Register("SubHeaderForeground", typeof(Brush), typeof(Divider), new PropertyMetadata(null));
	}
}
