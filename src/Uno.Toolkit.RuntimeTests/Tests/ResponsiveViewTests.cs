using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.UI.RuntimeTests;
using Uno.Toolkit.RuntimeTests.Helpers;
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
					<utu:ResponsiveView.NarrowTemplate>
						<DataTemplate>
							<TextBlock Text="Narrow" />
						</DataTemplate>
					</utu:ResponsiveView.NarrowTemplate>
					<utu:ResponsiveView.WideTemplate>
						<DataTemplate>
							<TextBlock Text="Wide" />
						</DataTemplate>
					</utu:ResponsiveView.WideTemplate>
				</utu:ResponsiveView>
			""");

			await UnitTestUIContentHelperEx.SetContentAndWait(host);

			var element = (TextBlock)host.Content;

			Assert.AreEqual("Narrow", element.Text);
		}
	}

	[TestMethod]
	public async Task ResponsiveView_NormalContent_Rectangle()
	{
		using (ResponsiveHelper.UsingDebuggableInstance())
		{
			ResponsiveHelper.SetDebugSize(new Size(600, 400));

			var host = XamlHelper.LoadXaml<ResponsiveView>("""
				<utu:ResponsiveView>
					<utu:ResponsiveView.NarrowTemplate>
						<DataTemplate>
							<TextBlock Text="Narrow" />
						</DataTemplate>
					</utu:ResponsiveView.NarrowTemplate>
					<utu:ResponsiveView.NormalTemplate>
						<DataTemplate>
							<Rectangle Width="400" Height="400" />
						</DataTemplate>
					</utu:ResponsiveView.NormalTemplate>
					<utu:ResponsiveView.WideTemplate>
						<DataTemplate>
							<TextBlock Text="Wide" />
						</DataTemplate>
					</utu:ResponsiveView.WideTemplate>
				</utu:ResponsiveView>
			""");

			await UnitTestUIContentHelperEx.SetContentAndWait(host);

			Assert.AreEqual(typeof(Rectangle), host.Content.GetType());
		}
	}

	[TestMethod]
	public async Task ResponsiveView_NormalContent_ResponsiveLayout()
	{
		using (ResponsiveHelper.UsingDebuggableInstance())
		{
			ResponsiveHelper.SetDebugSize(new Size(322, 400));

			var host = XamlHelper.LoadXaml<ResponsiveView>("""
				<utu:ResponsiveView>
					<utu:ResponsiveView.ResponsiveLayout>
						<utu:ResponsiveLayout>
							<utu:ResponsiveLayout.Narrowest>350</utu:ResponsiveLayout.Narrowest>
							<utu:ResponsiveLayout.Narrow>450</utu:ResponsiveLayout.Narrow>
							<utu:ResponsiveLayout.Normal>800</utu:ResponsiveLayout.Normal>
							<utu:ResponsiveLayout.Wide>1200</utu:ResponsiveLayout.Wide>
							<utu:ResponsiveLayout.Widest>1500</utu:ResponsiveLayout.Widest>
						</utu:ResponsiveLayout>
					</utu:ResponsiveView.ResponsiveLayout>
					<utu:ResponsiveView.NarrowestTemplate>
						<DataTemplate>
							<Ellipse Width="100" Height="100" />
						</DataTemplate>
					</utu:ResponsiveView.NarrowestTemplate>
					<utu:ResponsiveView.NarrowTemplate>
						<DataTemplate>
							<Rectangle Width="100" Height="100" />
						</DataTemplate>
					</utu:ResponsiveView.NarrowTemplate>
					<utu:ResponsiveView.WideTemplate>
						<DataTemplate>
							<TextBlock Text="Wide" />
						</DataTemplate>
					</utu:ResponsiveView.WideTemplate>
				</utu:ResponsiveView>
			""");

			await UnitTestUIContentHelperEx.SetContentAndWait(host);

			Assert.AreEqual(typeof(Ellipse), host.Content.GetType());
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
					<utu:ResponsiveView.NarrowTemplate>
						<DataTemplate>
							<TextBlock Text="Narrow" />
						</DataTemplate>
					</utu:ResponsiveView.NarrowTemplate>
					<utu:ResponsiveView.NormalTemplate>
						<DataTemplate>
							<Rectangle Width="400" Height="400" />
						</DataTemplate>
					</utu:ResponsiveView.NormalTemplate>
					<utu:ResponsiveView.WideTemplate>
						<DataTemplate>
							<TextBlock Text="Wide" />
						</DataTemplate>
					</utu:ResponsiveView.WideTemplate>
					<utu:ResponsiveView.WidestTemplate>
						<DataTemplate>
							<Ellipse Width="400" Height="400" />
						</DataTemplate>
					</utu:ResponsiveView.WidestTemplate>
				</utu:ResponsiveView>
			""");

			await UnitTestUIContentHelperEx.SetContentAndWait(host);

			Assert.AreEqual(typeof(Ellipse), host.Content.GetType());
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
					<utu:ResponsiveView.NarrowTemplate>
						<DataTemplate>
							<TextBlock Text="Narrow" />
						</DataTemplate>
					</utu:ResponsiveView.NarrowTemplate>
					<utu:ResponsiveView.NormalTemplate>
						<DataTemplate>
							<Rectangle Width="400" Height="400" />
						</DataTemplate>
					</utu:ResponsiveView.NormalTemplate>
					<utu:ResponsiveView.WideTemplate>
						<DataTemplate>
							<TextBox Text="Wide" />
						</DataTemplate>
					</utu:ResponsiveView.WideTemplate>
					<utu:ResponsiveView.WidestTemplate>
						<DataTemplate>
							<Ellipse Width="400" Height="400" />
						</DataTemplate>
					</utu:ResponsiveView.WidestTemplate>
				</utu:ResponsiveView>
			""");

			await UnitTestUIContentHelperEx.SetContentAndWait(host);
			Assert.AreEqual(typeof(TextBlock), host.Content.GetType());

			ResponsiveHelper.SetDebugSize(new Size(800, 400));
			Assert.AreEqual(typeof(TextBox), host.Content.GetType());
		}
	}
}
