using System;
using Windows.Devices.Input;

namespace Uno.UI.RuntimeTests;

public sealed class RequiresFullWindowAttribute : Attribute { }

public sealed class RunsOnUIThreadAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class InjectedPointerAttribute : Attribute
{
	public PointerDeviceType Type { get; }

	public InjectedPointerAttribute(PointerDeviceType type)
	{
		Type = type;
	}
}
