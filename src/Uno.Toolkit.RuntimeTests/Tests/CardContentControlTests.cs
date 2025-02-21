using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Toolkit.RuntimeTests.Extensions;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.UI;
using Uno.UI.RuntimeTests;
using Uno.UI.Extensions;
using Windows.Foundation;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation.Metadata;

#if IS_WINUI
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
#endif

namespace Uno.Toolkit.RuntimeTests.Tests
{
	[TestClass]
	[RunsOnUIThread]
	public class CardContentControlTests
	{
		[TestMethod]
		[RequiresFullWindow]
		public async Task Should_Hug_Content()
		{
			var rootGrid = XamlHelper.LoadXaml<Grid>("""
				<Grid Width="500" Height="500">
					<utu:CardContentControl Padding="0" x:Name="MyCard" Style="{StaticResource FilledCardContentControlStyle}">
						<Grid Background="Red" Height="200" Width="200" />
					</utu:CardContentControl>
				</Grid>
			""");


			var card = (CardContentControl)rootGrid.FindName("MyCard");

			await UnitTestUIContentHelperEx.SetContentAndWait(rootGrid);

			Assert.AreEqual(200d, card.ActualWidth);
			Assert.AreEqual(200d, card.ActualHeight);
		}

		[TestMethod]
		[RequiresFullWindow]
		[DataRow("FilledCardContentControlStyle")]
		[DataRow("OutlinedCardContentControlStyle")]
		public async Task Only_Elevated_Has_Margin(string cardStyle)
		{
			var rootGrid = XamlHelper.LoadXaml<Grid>($$"""
				<Grid>
					<utu:CardContentControl Padding="0" x:Name="MyCard" Style="{StaticResource {{cardStyle}}}">
						<Grid Background="Red" Height="200" Width="200" />
					</utu:CardContentControl>
				</Grid>
			""");

			var card = (CardContentControl)rootGrid.FindName("MyCard");

			await UnitTestUIContentHelperEx.SetContentAndWait(rootGrid);

			Assert.AreEqual(default, card.Margin);
		}
	}
}
