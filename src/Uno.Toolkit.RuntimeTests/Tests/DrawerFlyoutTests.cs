using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Extensions;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.UI;
using Uno.UI.RuntimeTests;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
#endif

namespace Uno.Toolkit.RuntimeTests.Tests;

[TestClass]
[RunsOnUIThread]
public class DrawerFlyoutTests
{
	/*	Test Plan
 *		- 
 */

	[TestMethod]
	[RunsOnUIThread]
	public async Task When_Left_DrawerFlyout()
	{
		var stackPanel = new StackPanel { MinWidth = 220, VerticalAlignment = VerticalAlignment.Stretch };
		var printTextBlock = new TextBlock { Text = "Text inside DrawerFlyout" };
		var btn = new Button { Content = "Button inside DrawerFlyout" };
		stackPanel.Children.Add(printTextBlock);
		stackPanel.Children.Add(btn);

		var flyout = new Flyout
		{
			Content = stackPanel,
			Placement = FlyoutPlacementMode.Full,
			FlyoutPresenterStyle = new Style(typeof(DrawerFlyoutPresenter))
			{
				Setters =
					{
						new Setter(DrawerFlyoutPresenter.OpenDirectionProperty, DrawerOpenDirection.Left),
					}
			}
		};

		var button = new Button()
		{
			Flyout = flyout,
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(stackPanel);

		Assert.AreEqual(220, stackPanel.ActualWidth);

		//button.Click += (s, e) =>
		//{
		//	printTextBlock.Text = "Text inside DrawerFlyout after click";
		//	btn.Content = "Button inside DrawerFlyout after click";
		//};

		//button.Click(new object(), new RoutedEventArgs());

		//Assert.AreEqual("Text inside DrawerFlyout after click", printTextBlock.Text);
		//Assert.AreEqual("Button inside DrawerFlyout after click", btn.Content);
	}
}
