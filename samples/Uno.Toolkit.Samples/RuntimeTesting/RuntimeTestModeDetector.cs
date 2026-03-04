using System;
using System.Collections.Generic;

namespace Uno.Toolkit.Samples.RuntimeTesting;

/// <summary>
/// Detects whether the application was launched in runtime test mode.
/// Supports both interactive (--mode=rt) and headless CI (--runtime-tests=path) modes.
/// </summary>
internal static class RuntimeTestModeDetector
{
	private const string RuntimeModeToken = "mode=rt";
	private const string RuntimeModeArgument = "--mode=rt";
	private const string RuntimeTestsArgument = "--runtime-tests";
	private const string RuntimeTestsArgumentWithValuePrefix = "--runtime-tests=";
	private const string UnoRuntimeTestsToken = "UNO_RUNTIME_TESTS_RUN_TESTS";

	public static bool IsRuntimeTestMode(
		string? launchArguments,
		IEnumerable<string>? commandLineArguments)
	{
		if (!string.IsNullOrWhiteSpace(launchArguments))
		{
			if (launchArguments!.Contains(RuntimeModeToken, StringComparison.OrdinalIgnoreCase) ||
				launchArguments.Contains(UnoRuntimeTestsToken, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}

		if (commandLineArguments is null)
		{
			return false;
		}

		foreach (var argument in commandLineArguments)
		{
			if (string.IsNullOrWhiteSpace(argument))
			{
				continue;
			}

			if (argument.Equals(RuntimeModeArgument, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}

			if (argument.Equals(RuntimeTestsArgument, StringComparison.OrdinalIgnoreCase)
				|| argument.StartsWith(RuntimeTestsArgumentWithValuePrefix, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}

		return false;
	}
}
