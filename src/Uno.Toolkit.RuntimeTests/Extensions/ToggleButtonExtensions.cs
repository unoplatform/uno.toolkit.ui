using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace Uno.Toolkit.RuntimeTests.Extensions;

[UnconditionalSuppressMessage("Trimming", "IL2075")]
internal static partial class ToggleButtonExtensions
{
	public static void Toggle(this ToggleButton toggle)
	{
		var method = toggle.GetType().GetMethod("OnToggle", BindingFlags.NonPublic | BindingFlags.Instance)
			?? throw new MissingMethodException("ToggleButton::OnToggle not found");

		method.Invoke(toggle, null);
	}
}
