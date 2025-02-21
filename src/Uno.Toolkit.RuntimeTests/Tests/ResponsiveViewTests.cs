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
public class ResponsiveViewTests
{
	[TestMethod]
	public async Task ResponsiveView_NarrowContent_TextBlock()
	{
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

		host.ForceResponsiveSize(new Size(300, 400));
		Assert.AreEqual("Narrow", (host.Content as TextBlock)?.Text);
	}

	[TestMethod]
	public async Task ResponsiveView_NormalContent_Rectangle()
	{
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

		host.ForceResponsiveSize(new Size(600, 400));
		Assert.AreEqual(typeof(Rectangle), host.Content?.GetType());
	}

	[TestMethod]
	public async Task ResponsiveView_NormalContent_ResponsiveLayout()
	{
		var host = XamlHelper.LoadXaml<ResponsiveView>("""
			<utu:ResponsiveView>
				<!--
				<utu:ResponsiveView.ResponsiveLayout>
					<utu:ResponsiveLayout Narrowest="350"
										  Narrow="450"
										  Normal="800"
										  Wide="1200"
										  Widest="1500" />
				</utu:ResponsiveView.ResponsiveLayout>
				-->

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
		host.ResponsiveLayout =
			// somehow neither attribute or member syntax work on windows...
			// The attachable property 'Narrowest' was not found in type 'ResponsiveLayout'. [Line: 4 Position: 6]'
			ResponsiveLayout.Create(350, 450, 800, 1200, 1500);
		await UnitTestUIContentHelperEx.SetContentAndWait(host);

		host.ForceResponsiveSize(new Size(322, 400));
		Assert.AreEqual(typeof(Ellipse), host.Content?.GetType());
	}

	[TestMethod]
	public async Task ResponsiveView_WidestContent_Ellipse()
	{
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

		host.ForceResponsiveSize(new Size(2000, 400));
		Assert.AreEqual(typeof(Ellipse), host.Content?.GetType());
	}

	[TestMethod]
	public async Task ResponsiveView_WideContent_SizeChanged()
	{
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

		host.ForceResponsiveSize(new Size(150, 400));
		Assert.AreEqual(typeof(TextBlock), host.Content?.GetType());

		host.ForceResponsiveSize(new Size(800, 400));
		Assert.AreEqual(typeof(TextBox), host.Content?.GetType());
	}
}
