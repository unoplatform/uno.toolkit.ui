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
}
