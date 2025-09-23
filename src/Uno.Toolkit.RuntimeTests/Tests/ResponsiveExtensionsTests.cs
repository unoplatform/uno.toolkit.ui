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
public class ResponsiveExtensionsTests
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

	[TestMethod]
	public async Task ProvideValue_ZeroSize_NoSizeChange()
	{
		var sut = XamlHelper.LoadXaml<TextBlock>("""
			<TextBlock Text="{utu:Responsive Narrow=asd, Wide=qwe}" />
		""");
		var ext = ResponsiveExtension.GetInstanceFor(sut, nameof(sut.Text)) ?? throw new InvalidOperationException("Failed to resolve the markup extension.");
		await UnitTestUIContentHelperEx.SetContentAndWait(sut);

		ext.ForceResponsiveSize(WideSize);
		Assert.AreEqual("qwe", sut.Text);

		var previousResult = ext.LastResolved?.Result;
		var previousSize = ext.LastResolved?.Size;

		ext.ForceResponsiveSize(new Size(0, 0));

		Assert.AreEqual(previousResult, sut.Text);
		Assert.AreEqual(previousSize, ext.LastResolved?.Size);
	}
}

[TestClass]
[RunsOnUIThread]
public class DynamicResponsiveExtensionsTests
{
	private readonly static ResponsiveLayout DefaultLayout = ResponsiveLayout.Create(150, 300, 600, 800, 1080);

	[TestMethod]
	public async Task Setup_And_Teardown()
	{
		try
		{
			var sut = new TextBlock() { Text = "Uninitialized" };
			await UIHelper.Load(sut);

			var markup = new ResponsiveExtension
			{
				Layout = DefaultLayout,
				Narrow = "Narrow",
				Wide = "Wide",
			};
			ResponsiveExtension.Install(sut, null, nameof(sut.Text), markup);
			Assert.AreNotEqual("Uninitialized", sut.Text, "Text should be initialized by now.");

			var provider = new ResponsiveSizeProvider();
			ResponsiveHelper.SetOverrideSizeProvider(provider);

			provider.Size = new Size(300, double.NaN);
			Assert.AreEqual("Narrow", sut.Text, $"Text should be 'Narrow', but is '{sut.Text}': result={markup.LastResolved?.Result}, width={markup.LastResolved?.Size.Width}");

			provider.Size = new Size(800, double.NaN);
			Assert.AreEqual("Wide", sut.Text, $"Text should be 'Wide', but is '{sut.Text}': result={markup.LastResolved?.Result}, width={markup.LastResolved?.Size.Width}");

			var snapshot = markup.LastResolved;
			ResponsiveExtension.Uninstall(markup);
			provider.Size = new Size(300, double.NaN);
			Assert.AreEqual("Wide", sut.Text, $"Text should still be 'Wide', but is '{sut.Text}': result={markup.LastResolved?.Result}, width={markup.LastResolved?.Size.Width}");
			Assert.AreEqual(snapshot, markup.LastResolved, "Markup should no longer update once uninstalled.");
		}
		finally
		{
			ResponsiveHelper.SetOverrideSizeProvider(null);
		}
	}
}
#endif
