using System;

namespace Uno.Toolkit.UI;

[Flags]
public enum BoundsVisibilityFlag
{
	Left = 1 << 0,
	Top = 1 << 1,
	Right = 1 << 2,
	Bottom = 1 << 3,

	None = 0,
	All = Left | Top | Right | Bottom,
}
