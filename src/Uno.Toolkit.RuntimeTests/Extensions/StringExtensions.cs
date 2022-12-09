using System;

namespace System;

internal static class StringExtensions
{
#if NETSTANDARD2_0
	// on netstandard2.0, this signature is missing...
	public static bool Contains(this string s, string value, StringComparison comparisonType)
	{
		return s.IndexOf(value, comparisonType) >= 0;
	}
#endif
}
