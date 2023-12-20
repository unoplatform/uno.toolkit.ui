using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Toolkit.UI;
using Uno.UI.RuntimeTests;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.RuntimeTests.Tests;

[TestClass]
[RunsOnUIThread]
internal class DependencyObjectExtensionTests
{
	[TestMethod]
	public void When_Type_FindDependencyProperty()
	{
		var dp = typeof(Grid).FindDependencyProperty<Thickness>(nameof(Grid.PaddingProperty));

		Assert.AreEqual(Grid.PaddingProperty, dp);
	}

	[TestMethod]
	public void When_Type_FindDependencyProperty_Attached()
	{
		var dp = typeof(Grid).FindDependencyProperty<int>(nameof(Grid.RowProperty));

		Assert.AreEqual(Grid.RowProperty, dp);
	}

	[TestMethod]
	public void When_DO_FindDependencyProperty()
	{
		var dp = new Grid().FindDependencyProperty<Thickness>(nameof(Grid.PaddingProperty));

		Assert.AreEqual(Grid.PaddingProperty, dp);
	}

	[TestMethod]
	public void When_DO_FindDependencyProperty_Attached()
	{
		var dp = new Grid().FindDependencyProperty<int>(nameof(Grid.RowProperty));

		Assert.AreEqual(Grid.RowProperty, dp);
	}
}
