// Disabled until fix is implemented for https://github.com/unoplatform/uno/issues/14620

//using System.Threading.Tasks;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Uno.UI.RuntimeTests;
//using Uno.Toolkit.RuntimeTests.Helpers;
//using Uno.Toolkit.UI.Helpers;
//using Windows.Foundation;

//#if IS_WINUI
//using Microsoft.UI.Xaml.Controls;
//using Microsoft.UI;
//using Microsoft.UI.Xaml.Media;
//#else
//using Windows.UI.Xaml.Controls;
//using Windows.UI;
//using Windows.UI.Xaml.Media;
//#endif

//namespace Uno.Toolkit.RuntimeTests.Tests;

//[TestClass]
//[RunsOnUIThread]
//internal class ResponsiveExtensionsTests
//{
//	private static readonly Size NarrowSize = new Size(300, 400);
//	private static readonly Size WideSize = new Size(800, 400);

//	[TestMethod]
//	public async Task ProvideValue_String_InitialValue()
//	{
//		using (ResponsiveHelper.UsingDebuggableInstance())
//		{
//			ResponsiveHelper.SetDebugSize(NarrowSize);

//			var host = XamlHelper.LoadXaml<TextBlock>("""
//				<TextBlock Text="{utu:Responsive Narrow=asd, Wide=qwe}" />
//			""");

//			await UnitTestUIContentHelperEx.SetContentAndWait(host);

//			Assert.AreEqual("asd", host.Text);
//		}
//	}

//#if !IS_UWP || HAS_UNO
//	[TestMethod]
//	public async Task ProvideValue_String_SizeChange()
//	{
//		using (ResponsiveHelper.UsingDebuggableInstance())
//		{
//			ResponsiveHelper.SetDebugSize(NarrowSize);

//			var host = XamlHelper.LoadXaml<TextBlock>("""
//				<TextBlock Text="{utu:Responsive Narrow=asd, Wide=qwe}" />
//			""");

//			await UnitTestUIContentHelperEx.SetContentAndWait(host);

//			Assert.AreEqual("asd", host.Text);

//			ResponsiveHelper.SetDebugSize(WideSize);

//			Assert.AreEqual("qwe", host.Text);
//		}
//	}
//#endif

//	[TestMethod]
//	public async Task ProvideValue_Color_InitialValue()
//	{
//		using (ResponsiveHelper.UsingDebuggableInstance())
//		{
//			ResponsiveHelper.SetDebugSize(NarrowSize);

//			var border = XamlHelper.LoadXaml<Border>("""
//				<Border Width="30"
//						Height="30">
//					<Border.Resources>
//						<SolidColorBrush x:Key="BorderRed">Red</SolidColorBrush>
//						<SolidColorBrush x:Key="BorderBlue">Blue</SolidColorBrush>
//					</Border.Resources>
//					<Border.Background>
//						<utu:Responsive Narrow="{StaticResource BorderRed}" Wide="{StaticResource BorderBlue}" />
//					</Border.Background>
//				</Border>
//			""");

//			await UnitTestUIContentHelperEx.SetContentAndWait(border);

//			Assert.AreEqual(Colors.Red, ((SolidColorBrush)border.Background).Color);
//		}
//	}

//#if !IS_UWP || HAS_UNO
//	[TestMethod]
//	public async Task ProvideValue_Color_SizeChange()
//	{
//		using (ResponsiveHelper.UsingDebuggableInstance())
//		{
//			ResponsiveHelper.SetDebugSize(NarrowSize);

//			var border = XamlHelper.LoadXaml<Border>("""
//				<Border Width="30"
//						Height="30">
//					<Border.Resources>
//						<SolidColorBrush x:Key="BorderRed">Red</SolidColorBrush>
//						<SolidColorBrush x:Key="BorderBlue">Blue</SolidColorBrush>
//					</Border.Resources>
//					<Border.Background>
//						<utu:Responsive Narrow="{StaticResource BorderRed}" Wide="{StaticResource BorderBlue}" />
//					</Border.Background>
//				</Border>
//			""");

//			await UnitTestUIContentHelperEx.SetContentAndWait(border);

//			Assert.AreEqual(Colors.Red, ((SolidColorBrush)border.Background).Color);

//			ResponsiveHelper.SetDebugSize(WideSize);

//			Assert.AreEqual(Colors.Blue, ((SolidColorBrush)border.Background).Color);

//		}
//	}
//#endif

//	[TestMethod]
//	public async Task ProvideValue_Orientation_InitialValue()
//	{
//		using (ResponsiveHelper.UsingDebuggableInstance())
//		{
//			ResponsiveHelper.SetDebugSize(NarrowSize);

//			var host = XamlHelper.LoadXaml<StackPanel>("""
//				<StackPanel>
//					<StackPanel.Resources>
//						<Orientation x:Key="NarrowOrientation">Vertical</Orientation>
//						<Orientation x:Key="WideOrientation">Horizontal</Orientation>
//					</StackPanel.Resources>
//					<StackPanel x:Name="MyStackPanel" Orientation="{utu:Responsive Narrow={StaticResource NarrowOrientation}, Wide={StaticResource WideOrientation}}">
//						<TextBlock Text="A" />
//						<TextBlock Text="B" />
//						<TextBlock Text="C" />
//					</StackPanel>
//				</StackPanel>
//			""");

//			var stackPanel = (StackPanel)host.FindName("MyStackPanel");
			
//			await UnitTestUIContentHelperEx.SetContentAndWait(host);

//			Assert.AreEqual(Orientation.Vertical, stackPanel.Orientation);
//		}
//	}

//#if !IS_UWP || HAS_UNO
//	[TestMethod]
//	public async Task ProvideValue_Orientation_SizeChange()
//	{
//		using (ResponsiveHelper.UsingDebuggableInstance())
//		{
//			ResponsiveHelper.SetDebugSize(NarrowSize);

//			var host = XamlHelper.LoadXaml<StackPanel>("""
//				<StackPanel>
//					<StackPanel.Resources>
//						<Orientation x:Key="NarrowOrientation">Vertical</Orientation>
//						<Orientation x:Key="WideOrientation">Horizontal</Orientation>
//					</StackPanel.Resources>
//					<StackPanel x:Name="MyStackPanel" Orientation="{utu:Responsive Narrow={StaticResource NarrowOrientation}, Wide={StaticResource WideOrientation}}">
//						<TextBlock Text="A" />
//						<TextBlock Text="B" />
//						<TextBlock Text="C" />
//					</StackPanel>
//				</StackPanel>
//			""");

//			var stackPanel = (StackPanel)host.FindName("MyStackPanel");

//			await UnitTestUIContentHelperEx.SetContentAndWait(host);

//			Assert.AreEqual(Orientation.Vertical, stackPanel.Orientation);

//			ResponsiveHelper.SetDebugSize(WideSize);

//			Assert.AreEqual(Orientation.Horizontal, stackPanel.Orientation);
//		}
//	}
//#endif

//}
