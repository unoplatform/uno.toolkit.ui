using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.UI;
using Uno.Toolkit.UI.Material;
using Uno.UI.RuntimeTests;

namespace Uno.Toolkit.RuntimeTests.Tests;

[TestClass]
[RunsOnUIThread]
public sealed class DividerTests
{
	private const string DefaultStyleKey = "DefaultDividerStyle";
	private const string SubHeaderTextBlockName = "SubHeaderTextBlock";

	[TestCleanup]
	public async Task Cleanup()
	{
		UnitTestsUIContentHelper.Content = null;
		await UnitTestsUIContentHelper.WaitForIdle();
	}

	[TestMethod]
	[DataRow(ElementTheme.Light)]
	[DataRow(ElementTheme.Dark)]
	public async Task When_OnlyToolkitResources_DividerRendersLine(ElementTheme requestedTheme)
	{
		var divider = new Divider();

		await UnitTestUIContentHelperEx.SetContentAndWait(CreateBaseOnlyRoot(divider, requestedTheme));

		var line = divider.GetDescendants().OfType<Rectangle>().SingleOrDefault();
		Assert.IsNotNull(line, "The base style must give Divider a template with a line.");
		Assert.AreEqual(1d, line.ActualHeight, "Line height");
		Assert.IsNotNull(divider.Foreground, "Foreground");
		Assert.AreSame(divider.Foreground, line.Fill, "Line fill");
		Assert.AreEqual(Visibility.Collapsed, GetSubHeader(divider).Visibility, "An empty subheader must not take space.");
	}

	[TestMethod]
	public async Task When_OnlyToolkitResources_SubHeaderVisibilityFollowsText()
	{
		var divider = new Divider { SubHeader = "Section" };

		await UnitTestUIContentHelperEx.SetContentAndWait(CreateBaseOnlyRoot(divider, ElementTheme.Light));

		var subHeader = GetSubHeader(divider);
		Assert.AreEqual(Visibility.Visible, subHeader.Visibility, "Set before the template is applied");
		Assert.AreEqual("Section", subHeader.Text);
		Assert.IsNotNull(divider.SubHeaderForeground, "SubHeaderForeground");

		divider.SubHeader = string.Empty;
		await UnitTestsUIContentHelper.WaitForIdle();
		Assert.AreEqual(Visibility.Collapsed, subHeader.Visibility, "Cleared");

		divider.SubHeader = "Again";
		await UnitTestsUIContentHelper.WaitForIdle();
		Assert.AreEqual(Visibility.Visible, subHeader.Visibility, "Set again");
	}

	[TestMethod]
	public async Task When_MaterialToolkitTheme_MaterialKeysWinOverBase()
	{
		// The base style declares the same lightweight-styling keys; the design-system layer must still win.
		var theme = new MaterialToolkitTheme();
		var divider = new Divider();
		var root = new Grid { Width = 300, Children = { divider } };
		root.Resources.MergedDictionaries.Add(theme);

		await UnitTestUIContentHelperEx.SetContentAndWait(root);

		var expected = ((SolidColorBrush)theme["OutlineVariantBrush"]).Color;
		Assert.AreEqual(expected, ((SolidColorBrush)divider.Foreground).Color, "DividerForeground");
	}

	private static TextBlock GetSubHeader(Divider divider) =>
		divider.GetDescendants().OfType<TextBlock>().Single(x => x.Name == SubHeaderTextBlockName);

	// Applied explicitly: the sample heads' design-system implicit style (Material, Simple) would otherwise win over the base one,
	// which is what an app with no Divider style of its own (e.g. Cupertino) gets implicitly.
	private static Grid CreateBaseOnlyRoot(Divider divider, ElementTheme requestedTheme)
	{
		var style = Application.Current.Resources[DefaultStyleKey] as Style;
		Assert.IsNotNull(style, $"ToolkitResources must provide the base '{DefaultStyleKey}'.");
		divider.Style = style;

		return new Grid { Width = 300, RequestedTheme = requestedTheme, Children = { divider } };
	}
}
