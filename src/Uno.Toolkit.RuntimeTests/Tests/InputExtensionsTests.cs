using System;
using Uno.Toolkit.UI;
using Uno.UI.RuntimeTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
#if HAS_UNO_WINUI
using Uno.UI.Xaml.Controls;
#endif

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#endif

namespace Uno.Toolkit.RuntimeTests.Tests
{
	[TestClass]
	[RunsOnUIThread]
	public class InputExtensionsTests
	{
		[TestMethod]
		public void ReturnType_Property()
		{
			var tb = new TextBox();
			InputExtensions.SetReturnType(tb, InputReturnType.Send);
			Assert.AreEqual(InputReturnType.Send, InputExtensions.GetReturnType(tb));
			InputExtensions.SetReturnType(tb, InputReturnType.Next);
			Assert.AreEqual(InputReturnType.Next,InputExtensions.GetReturnType(tb));
		}

		[TestMethod]
		public void AutoDismiss_Property()
		{
			var tb = new TextBox();
			InputExtensions.SetAutoDismiss(tb, true);
			Assert.IsTrue(InputExtensions.GetAutoDismiss(tb));
		}

		[TestMethod]
		public void AutoFocusNext_Property()
		{
			var tb = new TextBox();
			InputExtensions.SetAutoFocusNext(tb, true);
			Assert.IsTrue(InputExtensions.GetAutoFocusNext(tb));
		}

		[TestMethod]
		public void AutoFocusNextElement_Property()
		{
			var tb1 = new TextBox();
			var tb2 = new TextBox();
			InputExtensions.SetAutoFocusNextElement(tb1, tb2);
			Assert.AreEqual(tb2, InputExtensions.GetAutoFocusNextElement(tb1));
		}

		[TestMethod]
		public void Element_Precendence_Over_Flag()
		{
			var tb1 = new TextBox();
			var tb2 = new TextBox();
			InputExtensions.SetAutoFocusNext(tb1, true);
			InputExtensions.SetAutoFocusNextElement(tb1, tb2);
			Assert.AreEqual(tb2, InputExtensions.GetAutoFocusNextElement(tb1));
			Assert.IsTrue(InputExtensions.GetAutoFocusNext(tb1));
		}

	}
}
