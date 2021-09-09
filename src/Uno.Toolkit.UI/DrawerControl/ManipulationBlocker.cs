using System;
using System.Collections.Generic;
using System.Text;

#if IS_WINUI
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#endif

namespace Uno.UI.ToolkitLib
{
	public partial class ManipulationBlocker : ContentControl
	{
		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			this.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
			this.ManipulationStarted += (s, e) => e.Handled = true;
		}
	}
}
