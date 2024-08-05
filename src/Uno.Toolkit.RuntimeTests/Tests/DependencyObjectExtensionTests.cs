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

using DependencyObjectExtensions = Uno.Toolkit.UI.DependencyObjectExtensions;

namespace Uno.Toolkit.RuntimeTests.Tests;

[TestClass]
[RunsOnUIThread]
internal partial class DependencyObjectExtensionTests
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
	public void When_Type_FindDependencyProperty_Invalid()
	{
		var dp = typeof(Grid).FindDependencyProperty<InvalidCastException>(nameof(Grid.PaddingProperty));

		Assert.AreEqual(null, dp);
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

	[TestMethod]
	public void When_FindDependencyProperty_Ambiguous()
	{
		var dp = DependencyObjectExtensions.FindDependencyPropertyInfo(typeof(Ambiguity), nameof(Ambiguity.SomeValue))
			?? throw new Exception("FindDependencyPropertyInfo returned null");

		Assert.AreEqual(Ambiguity.SomeValueProperty, dp.Definition);
		Assert.AreEqual(typeof(int), dp.PropertyType);
	}
}

internal partial class DependencyObjectExtensionTests
{
	private partial class AmbiguityBase : DependencyObject
	{
		public object? SomeValue { get; set; }
	}
	private class Ambiguity : AmbiguityBase
	{
		#region DependencyProperty: SomeValue

		public static DependencyProperty SomeValueProperty { get; } = DependencyProperty.Register(
			nameof(SomeValue),
			typeof(int),
			typeof(Ambiguity),
			new PropertyMetadata(default(int)));

		public new int SomeValue
		{
			get => (int)GetValue(SomeValueProperty);
			set => SetValue(SomeValueProperty, value);
		}

		#endregion
	}
}
