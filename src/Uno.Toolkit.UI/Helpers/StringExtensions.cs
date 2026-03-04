using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uno.Toolkit.UI;

internal static class StringExtensions
{
	public static string RemoveHead(this string value, string? head) => head?.Length > 0 && value.StartsWith(head) ? value[head.Length..] : value;
	public static string RemoveTail(this string value, string? tail) => tail?.Length > 0 && value.EndsWith(tail) ? value[..^tail.Length] : value;

	public static string? EmptyAsNull(this string? value) => string.IsNullOrEmpty(value) ? null : value;
}
