using System;
using System.Collections.Generic;

namespace Uno.Toolkit.RuntimeTests.Extensions;

internal static class StackExtensions
{
	public static IEnumerable<T> PopWhile<T>(this Stack<T> stack, Func<T, bool> predicate)
	{
		while (stack.TryPeek(out var item) && predicate(item))
		{
			yield return stack.Pop();
		}
	}
}
