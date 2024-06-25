using Windows.Foundation;

namespace Uno.Toolkit.UI;

internal static class SizeExtensions
{
	public static Size Zero { get; } = new Size(0, 0); // Value is different than Size.Empty
	public static bool IsZero(this Size target) => target.Equals(Zero);
	public static bool IsNotZero(this Size target) => !target.IsZero();
}
