using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
