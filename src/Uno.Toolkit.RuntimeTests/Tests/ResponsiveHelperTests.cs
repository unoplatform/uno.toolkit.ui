using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Toolkit.UI;

namespace Uno.Toolkit.RuntimeTests.Tests;

[TestClass]
public class ResponsiveHelperTests
{
	private readonly static ResponsiveLayout DefaultLayout = ResponsiveLayout.Create(150, 300, 600, 800, 1080);

	// note: not to scale; '[' = inclusive to the right
	//       0        150         300      600      800     1080     ...
	// Narrowest(also) - Narrowest [ Narrow [ Normal [ Wide  [ Widest - // full layout
	// Normal          -           - 

	[TestMethod]
	public void When_Resolving_AllLayout()
	{
		var layout = DefaultLayout;
		var options = Enum.GetValues<Layout>();

		Assert.AreEqual(Layout.Narrowest, ResponsiveHelper.ResolveLayoutCore(layout, 149, options), "149");
		Assert.AreEqual(Layout.Narrowest, ResponsiveHelper.ResolveLayoutCore(layout, 150, options), "150"); // breakpoint=Narrowest
		Assert.AreEqual(Layout.Narrowest, ResponsiveHelper.ResolveLayoutCore(layout, 151, options), "151");
		Assert.AreEqual(Layout.Narrowest, ResponsiveHelper.ResolveLayoutCore(layout, 299, options), "299");
		Assert.AreEqual(Layout.Narrow, ResponsiveHelper.ResolveLayoutCore(layout, 300, options), "300"); // breakpoint=Narrow
		Assert.AreEqual(Layout.Narrow, ResponsiveHelper.ResolveLayoutCore(layout, 301, options), "301");
		Assert.AreEqual(Layout.Narrow, ResponsiveHelper.ResolveLayoutCore(layout, 599, options), "599");
		Assert.AreEqual(Layout.Normal, ResponsiveHelper.ResolveLayoutCore(layout, 600, options), "600"); // breakpoint=Normal
		Assert.AreEqual(Layout.Normal, ResponsiveHelper.ResolveLayoutCore(layout, 601, options), "601");
		Assert.AreEqual(Layout.Normal, ResponsiveHelper.ResolveLayoutCore(layout, 799, options), "799");
		Assert.AreEqual(Layout.Wide, ResponsiveHelper.ResolveLayoutCore(layout, 800, options), "800"); // breakpoint=Wide
		Assert.AreEqual(Layout.Wide, ResponsiveHelper.ResolveLayoutCore(layout, 801, options), "801");
		Assert.AreEqual(Layout.Wide, ResponsiveHelper.ResolveLayoutCore(layout, 1079, options), "1079");
		Assert.AreEqual(Layout.Widest, ResponsiveHelper.ResolveLayoutCore(layout, 1080, options), "1080"); // breakpoint=Widest
		Assert.AreEqual(Layout.Widest, ResponsiveHelper.ResolveLayoutCore(layout, 1081, options), "1081");
	}

	[TestMethod]
	public void When_Resolving_PartialLayout()
	{
		var layout = DefaultLayout;
		var options = new[] { Layout.Narrow, Layout.Wide, Layout.Widest };

		Assert.AreEqual(Layout.Narrow, ResponsiveHelper.ResolveLayoutCore(layout, 149, options), "149");
		Assert.AreEqual(Layout.Narrow, ResponsiveHelper.ResolveLayoutCore(layout, 150, options), "150"); // breakpoint=Narrowest (unavailable)
		Assert.AreEqual(Layout.Narrow, ResponsiveHelper.ResolveLayoutCore(layout, 151, options), "151");
		Assert.AreEqual(Layout.Narrow, ResponsiveHelper.ResolveLayoutCore(layout, 299, options), "299");
		Assert.AreEqual(Layout.Narrow, ResponsiveHelper.ResolveLayoutCore(layout, 300, options), "300"); // breakpoint=Narrow
		Assert.AreEqual(Layout.Narrow, ResponsiveHelper.ResolveLayoutCore(layout, 301, options), "301");
		Assert.AreEqual(Layout.Narrow, ResponsiveHelper.ResolveLayoutCore(layout, 599, options), "599");
		Assert.AreEqual(Layout.Narrow, ResponsiveHelper.ResolveLayoutCore(layout, 600, options), "600"); // breakpoint=Normal (unavailable)
		Assert.AreEqual(Layout.Narrow, ResponsiveHelper.ResolveLayoutCore(layout, 601, options), "601");
		Assert.AreEqual(Layout.Narrow, ResponsiveHelper.ResolveLayoutCore(layout, 799, options), "799");
		Assert.AreEqual(Layout.Wide, ResponsiveHelper.ResolveLayoutCore(layout, 800, options), "800"); // breakpoint=Wide
		Assert.AreEqual(Layout.Wide, ResponsiveHelper.ResolveLayoutCore(layout, 801, options), "801");
		Assert.AreEqual(Layout.Wide, ResponsiveHelper.ResolveLayoutCore(layout, 1079, options), "1079");
		Assert.AreEqual(Layout.Widest, ResponsiveHelper.ResolveLayoutCore(layout, 1080, options), "1080"); // breakpoint=Widest
		Assert.AreEqual(Layout.Widest, ResponsiveHelper.ResolveLayoutCore(layout, 1081, options), "1081");
	}
}
