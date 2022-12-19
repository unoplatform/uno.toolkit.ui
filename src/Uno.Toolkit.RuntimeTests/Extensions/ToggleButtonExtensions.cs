using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

#if IS_WINUI
using Microsoft.UI.Xaml.Controls.Primitives;
#else
using Windows.UI.Xaml.Controls.Primitives;
#endif

namespace Uno.Toolkit.RuntimeTests.Extensions;

internal static partial class ToggleButtonExtensions
{
	public static void Toggle(this ToggleButton toggle)
	{
		var method = toggle.GetType().GetMethod("OnToggle", BindingFlags.NonPublic | BindingFlags.Instance)
			?? throw new MissingMethodException("ToggleButton::OnToggle not found");

		method.Invoke(toggle, null);
	}
}
