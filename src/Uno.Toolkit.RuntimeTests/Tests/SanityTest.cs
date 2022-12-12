using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.UI.RuntimeTests;

#if IS_WINUI
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.RuntimeTests.Tests;

[TestClass]
public class SanityTests
{
	[TestMethod]
	public void Is_Sane()
	{
	}

	[TestMethod]
	public async Task Is_Still_Sane()
	{
		await Task.Delay(500);
	}

	[TestMethod]
	[RunsOnUIThread]
	public async Task When_Test_ContentHelper()
	{
		var SUT = new TextBlock() { Text = "New paths lead to new nightmares." };
		UnitTestsUIContentHelper.Content = SUT;

		await UnitTestsUIContentHelper.WaitForIdle();
		await UnitTestsUIContentHelper.WaitForLoaded(SUT);
	}

#if DEBUG && false // used for testing the engine
	[TestMethod]
	public async Task No_Longer_Sane() // expected to fail
	{
		await Task.Delay(500);

		throw new Exception("Great works require a touch of insanity.");
	}

	[TestMethod, Ignore]
	public void Is_An_Illusion() // expected to be ignored
	{
	}
#endif
}
