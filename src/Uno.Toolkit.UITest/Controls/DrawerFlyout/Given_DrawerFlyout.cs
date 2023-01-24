using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;
using Uno.Toolkit.UITest.Extensions;
using Uno.UITest.Helpers;
using static System.Net.Mime.MediaTypeNames;
using Uno.UITest.Helpers.Queries;
using Uno.Toolkit.UITest.Framework;
using System.Drawing;

namespace Uno.Toolkit.UITest.Controls.DrawerFlyout
{
	public class Given_DrawerFlyout : TestBase
	{
		protected override string SampleName => "DrawerFlyout";

		[Test]
		public void When_Left_DrawerFlyout()
		{
			CheckDrawerFlyout("Left");
		}

		[Test]
		public void When_Top_DrawerFlyout()
		{
			CheckDrawerFlyout("Top");
		}

		[Test]
		public void When_Right_DrawerFlyout()
		{
			CheckDrawerFlyout("Right");
		}

		[Test]
		public void When_Bottom_DrawerFlyout()
		{
			CheckDrawerFlyout("Bottom");
		}

		void CheckDrawerFlyout(string direction)
		{
			// Navigate to the DrawerFlyout sample page
			NavigateToSample("DrawerFlyoutSamplePage");

			// RootPage area rectangle
			var rootPage = App.WaitForElement("RootPage").Single().Rect;
			// Button to test the DrawerFlyout
			var drawerButton = App.Marked($"{direction}DrawerButton");
			// Button inside the DrawerFlyout
			var drawerFlyoutButton = App.Marked($"{direction}DrawerFlyoutButton");

			// Wait on the DrawerFlyout test button
			App.WaitForElement(drawerButton);

			// Tap on the DrawerFlyout test button
			App.FastTap(drawerButton);

			// Wait on the Button inside the DrawerFlyout
			App.WaitForElement(drawerFlyoutButton);

			// Assert the Content of the Button
			Assert.AreEqual($"Button {direction} Drawer", drawerFlyoutButton.GetDependencyPropertyValue("Content")?.ToString());

			// Assert if the DrawerFlyout is correctly opened by checking the LightDismissOverlayBackground color
			using var screenshotOpened = TakeScreenshot($"{direction} drawer opened");
			ImageAssert.HasColorAt(screenshotOpened, rootPage.CenterX, rootPage.CenterY, Color.FromArgb(255, 191, 191, 191), 8);

			// Tap in the middle of the LightDismissOverlayBackground in order to close the DrawerFlyout
			App.TapCoordinates(rootPage.CenterX, rootPage.CenterY);
			App.TapCoordinates(rootPage.CenterX, rootPage.CenterY);

			// Assert if the DrawerFlyout is correctly closed by checking the LightDismissOverlayBackground color
			using var screenshotClosed = TakeScreenshot($"{direction} drawer closed");
			ImageAssert.HasColorAt(screenshotClosed, rootPage.CenterX, rootPage.CenterY, Color.White, 8);
		}
	}
}
