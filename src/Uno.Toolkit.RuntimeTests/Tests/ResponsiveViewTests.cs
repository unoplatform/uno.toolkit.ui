using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.UI.RuntimeTests;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.UI.Helpers;
using Windows.Foundation;
using Uno.Toolkit.UI;

#if IS_WINUI
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Shapes;
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;
#endif

namespace Uno.Toolkit.RuntimeTests.Tests;

[TestClass]
[RunsOnUIThread]
internal class ResponsiveViewTests
{
	[TestMethod]
	public async Task ResponsiveView_NarrowContent_TextBlock()
	{
		using (ResponsiveHelper.UsingDebuggableInstance())
		{
			ResponsiveHelper.SetDebugSize(new Size(300, 400));

			var host = XamlHelper.LoadXaml<ResponsiveView>("""
				<utu:ResponsiveView>
					<utu:ResponsiveView.NarrowContent>
						<DataTemplate>
							<TextBlock Text="Narrow" />
						</DataTemplate>
					</utu:ResponsiveView.NarrowContent>
					<utu:ResponsiveView.WideContent>
						<DataTemplate>
							<TextBlock Text="Wide" />
						</DataTemplate>
					</utu:ResponsiveView.WideContent>
				</utu:ResponsiveView>
			""");

			await UnitTestUIContentHelperEx.SetContentAndWait(host);

			var element = (TextBlock)host.ResponsiveContent;

			Assert.AreEqual("Narrow", element.Text);
		}
	}

	[TestMethod]
	public async Task ResponsiveView_NormalContent_Rectangle()
	{
		using (ResponsiveHelper.UsingDebuggableInstance())
		{
			ResponsiveHelper.SetDebugSize(new Size(599, 400));

			var host = XamlHelper.LoadXaml<ResponsiveView>("""
				<utu:ResponsiveView>
					<utu:ResponsiveView.NarrowContent>
						<DataTemplate>
							<TextBlock Text="Narrow" />
						</DataTemplate>
					</utu:ResponsiveView.NarrowContent>
					<utu:ResponsiveView.NormalContent>
						<DataTemplate>
							<Rectangle Width="400" Height="400" />
						</DataTemplate>
					</utu:ResponsiveView.NormalContent>
					<utu:ResponsiveView.WideContent>
						<DataTemplate>
							<TextBlock Text="Wide" />
						</DataTemplate>
					</utu:ResponsiveView.WideContent>
				</utu:ResponsiveView>
			""");

			await UnitTestUIContentHelperEx.SetContentAndWait(host);

			Assert.AreEqual(typeof(Rectangle), host.ResponsiveContent.GetType());
		}
	}

	[TestMethod]
	public async Task ResponsiveView_NormalContent_ResponsiveLayout()
	{
		using (ResponsiveHelper.UsingDebuggableInstance())
		{
			ResponsiveHelper.SetDebugSize(new Size(322, 400));

			var host = XamlHelper.LoadXaml<ResponsiveView>("""
				<utu:ResponsiveView xmlns:helpers="using:Uno.Toolkit.UI.Helpers">
					<utu:ResponsiveView.ResponsiveLayout>
						<helpers:ResponsiveLayout>
							<helpers:ResponsiveLayout.Narrowest>350</helpers:ResponsiveLayout.Narrowest>
							<helpers:ResponsiveLayout.Narrow>450</helpers:ResponsiveLayout.Narrow>
							<helpers:ResponsiveLayout.Normal>800</helpers:ResponsiveLayout.Normal>
							<helpers:ResponsiveLayout.Wide>1200</helpers:ResponsiveLayout.Wide>
							<helpers:ResponsiveLayout.Widest>1500</helpers:ResponsiveLayout.Widest>
						</helpers:ResponsiveLayout>
					</utu:ResponsiveView.ResponsiveLayout>
					<utu:ResponsiveView.NarrowestContent>
						<DataTemplate>
							<Ellipse Width="100" Height="100" />
						</DataTemplate>
					</utu:ResponsiveView.NarrowestContent>
					<utu:ResponsiveView.NarrowContent>
						<DataTemplate>
							<Rectangle Width="100" Height="100" />
						</DataTemplate>
					</utu:ResponsiveView.NarrowContent>
					<utu:ResponsiveView.WideContent>
						<DataTemplate>
							<TextBlock Text="Wide" />
						</DataTemplate>
					</utu:ResponsiveView.WideContent>
				</utu:ResponsiveView>
			""");

			await UnitTestUIContentHelperEx.SetContentAndWait(host);

			Assert.AreEqual(typeof(Ellipse), host.ResponsiveContent.GetType());
		}
	}

	[TestMethod]
	public async Task ResponsiveView_WidestContent_Ellipse()
	{
		using (ResponsiveHelper.UsingDebuggableInstance())
		{
			ResponsiveHelper.SetDebugSize(new Size(2000, 400));

			var host = XamlHelper.LoadXaml<ResponsiveView>("""
				<utu:ResponsiveView>
					<utu:ResponsiveView.NarrowContent>
						<DataTemplate>
							<TextBlock Text="Narrow" />
						</DataTemplate>
					</utu:ResponsiveView.NarrowContent>
					<utu:ResponsiveView.NormalContent>
						<DataTemplate>
							<Rectangle Width="400" Height="400" />
						</DataTemplate>
					</utu:ResponsiveView.NormalContent>
					<utu:ResponsiveView.WideContent>
						<DataTemplate>
							<TextBlock Text="Wide" />
						</DataTemplate>
					</utu:ResponsiveView.WideContent>
					<utu:ResponsiveView.WidestContent>
						<DataTemplate>
							<Ellipse Width="400" Height="400" />
						</DataTemplate>
					</utu:ResponsiveView.WidestContent>
				</utu:ResponsiveView>
			""");

			await UnitTestUIContentHelperEx.SetContentAndWait(host);

			Assert.AreEqual(typeof(Ellipse), host.ResponsiveContent.GetType());
		}
	}

	[TestMethod]
	public async Task ResponsiveView_WideContent_SizeChanged()
	{
		using (ResponsiveHelper.UsingDebuggableInstance())
		{
			ResponsiveHelper.SetDebugSize(new Size(150, 400));

			var host = XamlHelper.LoadXaml<ResponsiveView>("""
				<utu:ResponsiveView>
					<utu:ResponsiveView.NarrowContent>
						<DataTemplate>
							<TextBlock Text="Narrow" />
						</DataTemplate>
					</utu:ResponsiveView.NarrowContent>
					<utu:ResponsiveView.NormalContent>
						<DataTemplate>
							<Rectangle Width="400" Height="400" />
						</DataTemplate>
					</utu:ResponsiveView.NormalContent>
					<utu:ResponsiveView.WideContent>
						<DataTemplate>
							<TextBox Text="Wide" />
						</DataTemplate>
					</utu:ResponsiveView.WideContent>
					<utu:ResponsiveView.WidestContent>
						<DataTemplate>
							<Ellipse Width="400" Height="400" />
						</DataTemplate>
					</utu:ResponsiveView.WidestContent>
				</utu:ResponsiveView>
			""");

			await UnitTestUIContentHelperEx.SetContentAndWait(host);
			Assert.AreEqual(typeof(TextBlock), host.ResponsiveContent.GetType());

			ResponsiveHelper.SetDebugSize(new Size(800, 400));
			Assert.AreEqual(typeof(TextBox), host.ResponsiveContent.GetType());
		}
	}
}
