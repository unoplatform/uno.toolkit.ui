using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using Uno.Toolkit.UITest.Extensions;
using Uno.Toolkit.UITest.Framework;
using Uno.UITest;
using Uno.UITest.Helpers;
using Uno.UITest.Helpers.Queries;
using Uno.UITest.Selenium;
using Uno.UITests.Helpers;

namespace Uno.Toolkit.UITest
{
	[TestFixture]
	public abstract class TestBase
	{
		/// <summary>
		/// The <c>SamplePageAttribute.Title</c> of sample page to test.
		/// </summary>
		protected abstract string SampleName { get; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		private IApp _app;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

		static TestBase()
		{
			AppInitializer.TestEnvironment.AndroidAppName = Constants.AndroidAppName;
			AppInitializer.TestEnvironment.WebAssemblyDefaultUri = Constants.WebAssemblyDefaultUri;
			AppInitializer.TestEnvironment.iOSAppName = Constants.iOSAppName;
			AppInitializer.TestEnvironment.iOSDeviceNameOrId = Constants.iOSDeviceNameOrId;
			AppInitializer.TestEnvironment.CurrentPlatform = Constants.CurrentPlatform;

#if DEBUG
			AppInitializer.TestEnvironment.WebAssemblyHeadless = false;
#endif

			// Uncomment to align with your own environment
			// Environment.SetEnvironmentVariable("ANDROID_HOME", @"C:\Program Files (x86)\Android\android-sdk");
			// Environment.SetEnvironmentVariable("JAVA_HOME", @"C:\Program Files\Microsoft\jdk-11.0.12.7-hotspot");

			AppInitializer.ColdStartApp();
		}

		protected IApp App
		{
			get => _app;
			private set
			{
				_app = value;
			}
		}

		[SetUp]
		[AutoRetry]
		public virtual void SetUpTest()
		{
			// Check if the test needs to be ignore or not
			// If nothing specified, it is considered as a global test
			var platforms = GetActivePlatforms()?.Distinct().ToArray() ?? Array.Empty<Platform>();
			if (platforms.Length != 0)
			{
				// Otherwise, we need to define on which platform the test is running and compare it with targeted platform
				var shouldIgnore = false;
				var currentPlatform = AppInitializer.GetLocalPlatform();

				if (_app is Uno.UITest.Xamarin.XamarinApp xa)
				{
					if (Xamarin.UITest.TestEnvironment.Platform == Xamarin.UITest.TestPlatform.Local)
					{
						shouldIgnore = !platforms.Contains(currentPlatform);
					}
					else
					{
						var testCloudPlatform = Xamarin.UITest.TestEnvironment.Platform == Xamarin.UITest.TestPlatform.TestCloudiOS
							? Platform.iOS
							: Platform.Android;

						shouldIgnore = !platforms.Contains(testCloudPlatform);
					}
				}
				else
				{
					shouldIgnore = !platforms.Contains(currentPlatform);
				}

				if (shouldIgnore)
				{
					var list = string.Join(", ", platforms.Select(p => p.ToString()));

					Assert.Ignore($"This test is ignored on this platform (runs on {list})");
				}
			}

			var app = AppInitializer.AttachToApp();
			_app = app ?? _app;

			Helpers.App = _app;

			_app.WaitForElementWithMessage("AppShell");
			NavigateToSample(SampleName);
		}

		[TearDown]
		public void TearDownTest()
		{
			if (TestContext.CurrentContext.Result.Outcome != ResultState.Success
				&& TestContext.CurrentContext.Result.Outcome != ResultState.Skipped
				&& TestContext.CurrentContext.Result.Outcome != ResultState.Ignored)
			{
				TakeScreenshot($"{TestContext.CurrentContext.Test.Name} - Tear down on error");
			}

			if (App.Marked("NestedSampleFrame").HasResults())
			{
				ExitNestedSample();
			}
		}

		protected void NavigateBackFromNestedSample()
		{
			InvokeBackdoor("browser:SampleRunner|NavBackFromNestedPage");
		}

		protected void NavigateToSample(string sample)
		{
			InvokeBackdoor("browser:SampleRunner|ForceNavigation", sample);
		}

		protected void NavigateToNestedSample(string pageName)
		{
			InvokeBackdoor("browser:SampleRunner|NavigateToNestedSample", pageName);
		}

		protected void ExitNestedSample()
		{
			InvokeBackdoor("browser:SampleRunner|ExitNestedSample");
		}

		private void InvokeBackdoor(string methodName, object? arg = null)
		{
			if (AppInitializer.GetLocalPlatform() == Platform.iOS)
			{
				arg ??= string.Empty;
			}
			_app?.InvokeGeneric(methodName, arg);
		}

		public ScreenshotInfo TakeScreenshot(string stepName)
		{
			if (_app == null)
			{
				throw new InvalidOperationException("App must be set before test code runs");
			}

			var title = Regex.Replace($"{TestContext.CurrentContext.Test.Name}_{stepName}", "[^A-z0-9]", "_");
			var fileInfo = GetNativeScreenshot(title);

			var fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileInfo.Name);
			if (fileNameWithoutExt != title && fileInfo.DirectoryName != null)
			{
				var destFileName = Path
					.Combine(fileInfo.DirectoryName, title + Path.GetExtension(fileInfo.Name));

				if (File.Exists(destFileName))
				{
					File.Delete(destFileName);
				}

				File.Move(fileInfo.FullName, destFileName);

				TestContext.AddTestAttachment(destFileName, stepName);

				fileInfo = new FileInfo(destFileName);
			}
			else
			{
				TestContext.AddTestAttachment(fileInfo.FullName, stepName);
			}

			return new ScreenshotInfo(fileInfo, stepName);
		}

		private IEnumerable<Platform> GetActivePlatforms()
		{
			var currentTest = TestContext.CurrentContext.Test;
			if (currentTest.ClassName == null)
			{
				yield break;
			}
			if (Type.GetType(currentTest.ClassName) is { } classType)
			{
				if (classType.GetCustomAttributes(typeof(ActivePlatformsAttribute), false) is
					ActivePlatformsAttribute[] classAttributes)
				{
					foreach (var attr in classAttributes)
					{
						if (attr.Platforms == null)
						{
							continue;
						}

						foreach (var platform in attr.Platforms)
						{
							yield return platform;
						}
					}
				}

				if (currentTest.MethodName is { })
				{
					var testMethodInfo = classType.GetMethod(currentTest.MethodName);

					if (testMethodInfo is { } mi &&
						mi.GetCustomAttributes(typeof(ActivePlatformsAttribute), false) is
							ActivePlatformsAttribute[] methodAttributes)
					{
						foreach (var attr in methodAttributes)
						{
							if (attr.Platforms == null)
							{
								continue;
							}

							foreach (var platform in attr.Platforms)
							{
								yield return platform;
							}
						}
					}
				}

			}
		}

		private FileInfo GetNativeScreenshot(string title)
		{
			if (AppInitializer.GetLocalPlatform() == Platform.Android)
			{
				return _app.GetInAppScreenshot();
			}
			else
			{
				return _app.Screenshot(title);
			}
		}
	}
}
