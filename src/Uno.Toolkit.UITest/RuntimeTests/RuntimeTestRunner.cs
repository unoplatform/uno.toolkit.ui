using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Uno.UITest.Helpers.Queries;
using Uno.UITest;
using Uno.UITests.Helpers;
using Uno.Toolkit.UITest.Extensions;

namespace Uno.Toolkit.UITest.RuntimeTests
{
	[TestFixture]
	public partial class RuntimeTestRunner : TestBase
	{
		private readonly TimeSpan TestRunTimeout = TimeSpan.FromMinutes(2);
		private const string TestResultsOutputFilePath = "UITEST_RUNTIMETESTS_RESULTS_FILE_PATH";

		protected override string SampleName => "RuntimeTest Runner";

		[Test]
		public async Task RunRuntimeTests()
		{
			IAppQuery AllQuery(IAppQuery query)
				// .All() is not yet supported for wasm.
				=> AppInitializer.GetLocalPlatform() == Platform.Browser ? query : query.All();

			var runButton = new QueryEx(q => AllQuery(q).Marked("runButton"));
			var failedTests = new QueryEx(q => AllQuery(q).Marked("failedTests"));
			var failedTestsDetails = new QueryEx(q => AllQuery(q).Marked("failedTestDetails"));
			var unitTestsControl = new QueryEx(q => AllQuery(q).Marked("UnitTestsRootControl"));

			async Task<bool> IsTestExecutionDone()
			{
				return await GetWithRetry("IsTestExecutionDone", () => unitTestsControl.GetDependencyPropertyValue("RunningStateForUITest")?.ToString().Equals("Finished", StringComparison.OrdinalIgnoreCase) ?? false);
			}

			App.WaitForElement(runButton);

			if (Environment.GetEnvironmentVariable(TestResultsOutputFilePath) is { } path)
			{
				// Used to disable showing the test output visually
				unitTestsControl.SetDependencyPropertyValue("IsRunningOnCI", "true");
			}

			App.FastTap(runButton);

			var lastChange = DateTimeOffset.Now;
			var lastValue = "";

			while (DateTimeOffset.Now - lastChange < TestRunTimeout)
			{
				var newValue = await GetWithRetry("GetRunTestCount", () => unitTestsControl.GetDependencyPropertyValue("RunTestCountForUITest")?.ToString());

				if (lastValue != newValue)
				{
					lastChange = DateTimeOffset.Now;
				}

				await Task.Delay(TimeSpan.FromSeconds(.5));

				if (await IsTestExecutionDone())
				{
					break;
				}
			}

			if (!await IsTestExecutionDone())
			{
				Assert.Fail("A test run timed out");
			}

			TestContext.AddTestAttachment(await ArchiveResults(unitTestsControl), "runtimetests-results.zip");

			var count = GetValue(nameof(unitTestsControl), unitTestsControl, "FailedTestCountForUITest");
			if (count != "0")
			{
				var tests = GetValue(nameof(failedTests), failedTests)
					.Split(new char[] { '§' }, StringSplitOptions.RemoveEmptyEntries)
					.Select((x, i) => $"\t{i + 1}. {x}\n")
					.ToArray();
				var details = GetValue(nameof(failedTestsDetails), failedTestsDetails);

				Assert.Fail($"{tests.Length} unit test(s) failed (count={count}).\n\tFailing Tests:\n{string.Join("", tests)}\n\n---\n\tDetails:\n{details}");
			}

			TakeScreenshot("Runtime Tests Results");
		}

		async Task<T> GetWithRetry<T>(string logName, Func<T> getter, int timeoutSeconds = 120)
		{
			var sw = Stopwatch.StartNew();
			Exception? lastException = null;
			do
			{
				try
				{
					var result = getter();

					if (sw.Elapsed > TimeSpan.FromSeconds(timeoutSeconds / 2))
					{
						Console.WriteLine($"{logName} succeeded after retries");
					}

					return result;
				}
				catch (Exception e)
				{
					lastException = e;
					Console.WriteLine($"{logName} failed with {e.Message}");
				}

				await Task.Delay(TimeSpan.FromSeconds(2));

				Console.WriteLine($"{logName} retrying");
			}
			while (sw.Elapsed < TimeSpan.FromSeconds(timeoutSeconds));

			throw lastException;
		}

		private static async Task<string> ArchiveResults(QueryEx unitTestsControl)
		{
			var document = await GetNUnitTestResultsDocument(unitTestsControl);

			var file = Path.GetTempFileName();
			File.WriteAllText(file, document, Encoding.Unicode);

			if (Environment.GetEnvironmentVariable(TestResultsOutputFilePath) is { } path)
			{
				File.Copy(file, path, true);
			}
			else
			{
				Console.WriteLine($"The environment variable {TestResultsOutputFilePath} is not defined, skipping file system extraction");
			}

			var finalFile = Path.Combine(Path.GetDirectoryName(file), "test-results.xml");

			if (File.Exists(finalFile))
			{
				File.Delete(finalFile);
			}

			File.Move(file, finalFile);

			return finalFile;
		}

		private static async Task<string> GetNUnitTestResultsDocument(QueryEx unitTestsControl)
		{
			int counter = 0;

			do
			{
				var document = GetValue(nameof(unitTestsControl), unitTestsControl, "NUnitTestResultsDocument");

				if (!string.IsNullOrEmpty(document))
				{
					return document;
				}

				// The results are built asynchronously, it may not be available right away.
				await Task.Delay(1000);

			} while (counter++ < 3);

			throw new InvalidOperationException($"Failed to get the test results document");
		}

		private static string GetValue(string elementName, QueryEx element, string dpName = "Text", [CallerLineNumber] int line = -1)
		{
			try
			{
				return element
					?.GetDependencyPropertyValue(dpName)
					?.ToString() ?? string.Empty;
			}
			catch (Exception e)
			{
				Assert.Fail($"Failed to get DP ${dpName} on {elementName} (@{line}), {e}", e);
				throw new InvalidOperationException($"Failed to get DP ${dpName} on {elementName} (@{line}), {e}");
			}
		}
	}
}
