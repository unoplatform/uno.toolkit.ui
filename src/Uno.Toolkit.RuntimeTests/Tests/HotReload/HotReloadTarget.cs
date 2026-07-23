using System.Linq;

namespace Uno.Toolkit.RuntimeTests.Tests.HotReload;

/// <summary>
/// Target class for hot-reload tests. The method body is modified at runtime
/// by Given_HotReload via HotReloadHelper.
/// </summary>
internal static class HotReloadTarget
{
	internal static string GetValue()
	{
		return "original";
	}

	internal static int GetNumber()
	{
		return 1;
	}

	internal static bool GetFlag()
	{
		return false;
	}

	internal static string Greet(string name)
	{
		return $"Hello, {name}!";
	}

	internal static string GetConditional(bool condition)
	{
		if (condition)
		{
			return "condition-true";
		}
		return "condition-false";
	}

	// --- Complex type returns ---

	internal enum Status { Active, Inactive, Pending }

	internal static Status GetStatus()
	{
		return Status.Active;
	}

	internal static List<string> GetItems()
	{
		return new List<string> { "A", "B", "C" };
	}

	internal static (string Name, int Age) GetTuple()
	{
		return ("Alice", 30);
	}

	internal static Dictionary<string, int> GetLookup()
	{
		return new Dictionary<string, int> { ["x"] = 1 };
	}

	// --- Control flow ---

	internal static string GetSwitch(int code)
	{
		return code switch
		{
			1 => "one",
			2 => "two",
			_ => "other",
		};
	}

	internal static string GetRepeated(int n)
	{
		var result = "";
		for (var i = 0; i < n; i++)
		{
			result += "a";
		}
		return result;
	}

	internal static string GetSafe(bool shouldThrow)
	{
		try
		{
			if (shouldThrow)
			{
				throw new InvalidOperationException("boom");
			}
			return "no-throw";
		}
		catch
		{
			return "caught-original";
		}
	}

	internal static List<int> GetFiltered()
	{
		var source = new List<int> { 1, 2, 3, 4, 5 };
		return source.Where(x => x > 2).ToList();
	}

	// --- State & instance ---

	private static int _counter = 0;

	internal static int GetCounter()
	{
		_counter += 1;
		return _counter;
	}

	internal static void ResetCounter()
	{
		_counter = 0;
	}

	private class Inner
	{
		public string Value => "inner-original";
	}

	internal static string GetNested()
	{
		return new Inner().Value;
	}

	// --- Edge cases ---

	internal static string GetOptional()
	{
		return "";
	}

	internal static string GetMultiLine()
	{
		return "line-one\n" +
			"line-two-original\n" +
			"line-three";
	}
}
