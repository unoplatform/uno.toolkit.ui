using System;
using System.Collections.Generic;
using System.Text;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.Samples
{
	/// <summary>
	/// Used for show casing a control, along with adjustable options on the side.
	/// </summary>
	/// <remarks>The default style has built-in support for content wrapping on small screen.</remarks>
	public partial class ControlExample : ContentControl
	{
		#region DependencyProperty: Options

		public static DependencyProperty OptionsProperty { get; } = DependencyProperty.Register(
			nameof(Options),
			typeof(object),
			typeof(ControlExample),
			new PropertyMetadata(default(object)));

		public object Options
		{
			get => (object)GetValue(OptionsProperty);
			set => SetValue(OptionsProperty, value);
		}

		#endregion
	}
}
