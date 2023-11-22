using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.UI.RuntimeTests;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.UI;
using Microsoft.UI.Xaml.Controls;
using Uno.Toolkit.UI.Helpers;
using Windows.Foundation;

namespace Uno.Toolkit.RuntimeTests.Tests;

[TestClass]
[RunsOnUIThread]
internal class ResponsiveExtensionsTests
{
	// todo: repeat the same tests for an enum, etc...

	[TestMethod]
	public async Task ProvideValue_String_Initial()
	{
		using (ResponsiveHelper.UsingDebuggableInstance())
		{
			ResponsiveHelper.SetDebugSize(new Size(300, 400));

			var host = XamlHelper.LoadXaml<TextBlock>("""
				<TextBlock Text="{utu:Responsive Narrow=asd, Wide=qwe}" />
			""");

			await UnitTestUIContentHelperEx.SetContentAndWait(host);

			Assert.AreEqual("asd", host.Text);
		}
	}

#if !IS_UWP || HAS_UNO
	[TestMethod]
	public async Task ProvideValue_String_SizeChange()
	{
		using (ResponsiveHelper.UsingDebuggableInstance())
		{
			ResponsiveHelper.SetDebugSize(new Size(300, 400));

			var host = XamlHelper.LoadXaml<TextBlock>("""
				<TextBlock Text="{utu:Responsive Narrow=asd, Wide=qwe}" />
			""");

			await UnitTestUIContentHelperEx.SetContentAndWait(host);

			Assert.AreEqual("asd", host.Text);
			
			ResponsiveHelper.SetDebugSize(new Size(1080, 400));
			
			Assert.AreEqual("qwe", host.Text);
		}
	}
#endif
}
