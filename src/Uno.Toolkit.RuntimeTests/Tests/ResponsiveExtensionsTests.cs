using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.UI.RuntimeTests;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.UI.Helpers;
using Windows.Foundation;

#if IS_WINUI
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
#else
using Windows.UI.Xaml.Controls;
using Windows.UI;
using Windows.UI.Xaml.Media;
#endif

namespace Uno.Toolkit.RuntimeTests.Tests;

[TestClass]
[RunsOnUIThread]
internal class ResponsiveExtensionsTests
{
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

				ResponsiveHelper.SetDebugSize(new Size(800, 400));

				Assert.AreEqual("qwe", host.Text);
			}
		}
#endif

	[TestMethod]
	public async Task ProvideValue_Color_Initial()
	{
		using (ResponsiveHelper.UsingDebuggableInstance())
		{
			ResponsiveHelper.SetDebugSize(new Size(300, 400));

			var host = XamlHelper.LoadXaml<StackPanel>("""
				<StackPanel>
					<StackPanel.Resources>
						<SolidColorBrush x:Key="BorderRed">Red</SolidColorBrush>
						<SolidColorBrush x:Key="BorderBlue">Blue</SolidColorBrush>
					</StackPanel.Resources>
					<Border x:Name="MyBorder" Width="30" Height="30" Background="{utu:Responsive Normal={StaticResource BorderRed}, Wide={StaticResource BorderBlue}}" />
				</StackPanel>
			""");

			var border = (Border)host.FindName("MyBorder");

			await UnitTestUIContentHelperEx.SetContentAndWait(host);

			Assert.AreEqual(Colors.Red, ((SolidColorBrush)border.Background).Color);
		}
	}

	[TestMethod]
	public async Task ProvideValue_Orientation_Initial()
	{
		using (ResponsiveHelper.UsingDebuggableInstance())
		{
			ResponsiveHelper.SetDebugSize(new Size(800, 400));

			var host = XamlHelper.LoadXaml<StackPanel>("""
				<StackPanel>
					<StackPanel.Resources>
						<Orientation x:Key="NarrowOrientation">Vertical</Orientation>
						<Orientation x:Key="WideOrientation">Horizontal</Orientation>
					</StackPanel.Resources>
					<StackPanel x:Name="MyStackPanel" Orientation="{utu:Responsive Normal={StaticResource NarrowOrientation}, Wide={StaticResource WideOrientation}}">
						<TextBlock Text="A" />
						<TextBlock Text="B" />
						<TextBlock Text="C" />
					</StackPanel>
				</StackPanel>
			""");

			var stackPanel = (StackPanel)host.FindName("MyStackPanel");
			
			await UnitTestUIContentHelperEx.SetContentAndWait(host);

			Assert.AreEqual(Orientation.Horizontal, stackPanel.Orientation);
		}
	}
}
