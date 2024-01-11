#if HAS_UNO || !IS_UWP
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Windows.Foundation;
using Uno.UI.RuntimeTests;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.UI;

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
#if HAS_UNO
[Ignore("blocked by #14620: dynamically loaded MarkupExtension are not initialized.")] // https://github.com/unoplatform/uno/issues/14620
#elif IS_UWP
[Ignore("ResponsiveExtension is not supported on UWP.")]
#else
// just to be clear, the tests is currently only running on WINUI_DESKTOP
#endif
[RunsOnUIThread]
internal class ResponsiveExtensionsTests
{
	private static readonly Size NarrowSize = new Size(300, 400);
	private static readonly Size WideSize = new Size(800, 400);

	[TestMethod]
	public async Task ProvideValue_String_Value()
	{
		var sut = XamlHelper.LoadXaml<TextBlock>("""
			<TextBlock Text="{utu:Responsive Narrow=asd, Wide=qwe}" />
		""");
		var ext = ResponsiveExtension.GetInstanceFor(sut, nameof(sut.Text)) ?? throw new InvalidOperationException("Failed to resolve the markup extension.");
		await UnitTestUIContentHelperEx.SetContentAndWait(sut);

		ext.ForceResponsiveSize(NarrowSize);
		Assert.AreEqual("asd", sut.Text);
	}

	[TestMethod]
	public async Task ProvideValue_String_SizeChange()
	{
		var sut = XamlHelper.LoadXaml<TextBlock>("""
			<TextBlock Text="{utu:Responsive Narrow=asd, Wide=qwe}" />
		""");
		var ext = ResponsiveExtension.GetInstanceFor(sut, nameof(sut.Text)) ?? throw new InvalidOperationException("Failed to resolve the markup extension.");
		await UnitTestUIContentHelperEx.SetContentAndWait(sut);

		ext.ForceResponsiveSize(NarrowSize);
		Assert.AreEqual("asd", sut.Text);

		ext.ForceResponsiveSize(WideSize);
		Assert.AreEqual("qwe", sut.Text);
	}

	[TestMethod]
	public async Task ProvideValue_Color_Value()
	{
		var sut = XamlHelper.LoadXaml<Border>("""
			<Border Width="30"
					Height="30">
				<Border.Resources>
					<SolidColorBrush x:Key="BorderRed">Red</SolidColorBrush>
					<SolidColorBrush x:Key="BorderBlue">Blue</SolidColorBrush>
				</Border.Resources>
				<Border.Background>
					<utu:Responsive Narrow="{StaticResource BorderRed}" Wide="{StaticResource BorderBlue}" />
				</Border.Background>
			</Border>
		""");
		var ext = ResponsiveExtension.GetInstanceFor(sut, nameof(sut.Background)) ?? throw new InvalidOperationException("Failed to resolve the markup extension.");
		await UnitTestUIContentHelperEx.SetContentAndWait(sut);

		ext.ForceResponsiveSize(NarrowSize);
		Assert.AreEqual(Colors.Red, ((SolidColorBrush)sut.Background).Color);
	}

	[TestMethod]
	public async Task ProvideValue_Color_SizeChange()
	{
		var sut = XamlHelper.LoadXaml<Border>("""
			<Border Width="30"
					Height="30">
				<Border.Resources>
					<SolidColorBrush x:Key="BorderRed">Red</SolidColorBrush>
					<SolidColorBrush x:Key="BorderBlue">Blue</SolidColorBrush>
				</Border.Resources>
				<Border.Background>
					<utu:Responsive Narrow="{StaticResource BorderRed}" Wide="{StaticResource BorderBlue}" />
				</Border.Background>
			</Border>
		""");
		var ext = ResponsiveExtension.GetInstanceFor(sut, nameof(sut.Background)) ?? throw new InvalidOperationException("Failed to resolve the markup extension.");
		await UnitTestUIContentHelperEx.SetContentAndWait(sut);

		ext.ForceResponsiveSize(NarrowSize);
		Assert.AreEqual(Colors.Red, ((SolidColorBrush)sut.Background).Color);

		ext.ForceResponsiveSize(WideSize);
		Assert.AreEqual(Colors.Blue, ((SolidColorBrush)sut.Background).Color);
	}

	[TestMethod]
	public async Task ProvideValue_Orientation_Value()
	{
		var container = XamlHelper.LoadXaml<Border>("""
			<Border>
				<Border.Resources>
					<Orientation x:Key="NarrowOrientation">Vertical</Orientation>
					<Orientation x:Key="WideOrientation">Horizontal</Orientation>
				</Border.Resources>
				<StackPanel Orientation="{utu:Responsive Narrow={StaticResource NarrowOrientation}, Wide={StaticResource WideOrientation}}">
					<TextBlock Text="A" />
					<TextBlock Text="B" />
					<TextBlock Text="C" />
				</StackPanel>
			</Border>
		""");
		var sut = container.Child as StackPanel ?? throw new InvalidOperationException("Failed to resolve the SUT");
		var ext = ResponsiveExtension.GetInstanceFor(sut, nameof(sut.Orientation)) ?? throw new InvalidOperationException("Failed to resolve the markup extension.");
		await UnitTestUIContentHelperEx.SetContentAndWait(container);

		ext.ForceResponsiveSize(NarrowSize);
		Assert.AreEqual(Orientation.Vertical, (container.Child as StackPanel)?.Orientation);
	}

	[TestMethod]
	public async Task ProvideValue_Orientation_SizeChange()
	{
		var container = XamlHelper.LoadXaml<Border>("""
			<Border>
				<Border.Resources>
					<Orientation x:Key="NarrowOrientation">Vertical</Orientation>
					<Orientation x:Key="WideOrientation">Horizontal</Orientation>
				</Border.Resources>
				<StackPanel Orientation="{utu:Responsive Narrow={StaticResource NarrowOrientation}, Wide={StaticResource WideOrientation}}">
					<TextBlock Text="A" />
					<TextBlock Text="B" />
					<TextBlock Text="C" />
				</StackPanel>
			</Border>
		""");
		var sut = container.Child as StackPanel ?? throw new InvalidOperationException("Failed to resolve the SUT");
		var ext = ResponsiveExtension.GetInstanceFor(sut, nameof(sut.Orientation)) ?? throw new InvalidOperationException("Failed to resolve the markup extension.");
		await UnitTestUIContentHelperEx.SetContentAndWait(container);

		ext.ForceResponsiveSize(NarrowSize);
		Assert.AreEqual(Orientation.Vertical, sut.Orientation);

		ext.ForceResponsiveSize(WideSize);
		Assert.AreEqual(Orientation.Horizontal, sut.Orientation);
	}
}
#endif
