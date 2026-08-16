using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Uno.UI.RuntimeTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml;

using Base = Uno.UI.RuntimeTests.UnitTestsUIContentHelper;

namespace Uno.Toolkit.RuntimeTests.Helpers
{
	internal static class UnitTestUIContentHelperEx
	{
		public static Task WaitForIdle() => Base.WaitForIdle();

		public static async Task SetContentAndWait(FrameworkElement e)
		{
			Base.Content = e;
			await Base.WaitForIdle();
			await Base.WaitForLoaded(e);
		}

		/// <summary>
		/// Wait until a specified <paramref name="condition"/> is met. 
		/// </summary>
		/// <param name="timeoutMS">The maximum time to wait before failing the test, in milliseconds.</param>
		public static async Task WaitFor(Func<bool> condition, int timeoutMS = 1000, string? message = null, [CallerMemberName] string? callerMemberName = null, [CallerLineNumber] int lineNumber = 0)
		{
			if (condition())
			{
				return;
			}

			var stopwatch = Stopwatch.StartNew();
			while (stopwatch.ElapsedMilliseconds < timeoutMS)
			{
				await Base.WaitForIdle();
				if (condition())
				{
					return;
				}
			}

			message ??= $"{callerMemberName}():{lineNumber}";

			throw new AssertFailedException("Timed out waiting for condition to be met: " + message);
		}
	}
}
