using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.UI.RuntimeTests;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.UI;

#if IS_WINUI
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
#else
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
#endif

namespace Uno.Toolkit.RuntimeTests.Tests;

[TestClass]
[RunsOnUIThread]
internal class DrawerFlyoutTests
{
#if !DEBUG
	[TestCleanup] // note that this is only run, after each [TestMethod], **not** between [DataRow].
	public void CloseOpenPopup()
	{
		// killing the host, will close the associated popup.
		UnitTestsUIContentHelper.Content = null;
	}
#endif

	[TestMethod]
	public async Task Can_Open()
	{
		var SUT = XamlHelper.LoadXaml<Button>("""
			<Button Content="Asd">
				<Button.Flyout>
					<Flyout Placement="Full" FlyoutPresenterStyle="{StaticResource DrawerFlyoutPresenterStyle}">
						<Border x:Name="FlyoutContentBorder" Background="SkyBlue">
							<TextBlock Text="Asd" />
						</Border>
					</Flyout>
				</Button.Flyout>
			</Button>
		""");
		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

		SUT.Flyout.ShowAt(SUT);

		GetOpenPopup();
	}

#if false // uno#14372: can't use "xmlns'd attached property style setter" with XamlReader
	[TestMethod]
#endif
	public async Task AttachedProperty_Inheritance()
	{
		var SUT = BuildButtonFlyout($$"""
			<Flyout Placement="Full">
				<Flyout.FlyoutPresenterStyle>
					<Style BasedOn="{StaticResource DrawerFlyoutPresenterStyle}" TargetType="FlyoutPresenter">
						<Setter Property="utu:DrawerFlyoutPresenter.OpenDirection" Value="Left" />
						<Setter Property="utu:DrawerFlyoutPresenter.DrawerDepth" Value="3.12*" />
						<Setter Property="utu:DrawerFlyoutPresenter.IsGestureEnabled" Value="True" />
						<Setter Property="utu:DrawerFlyoutPresenter.LightDismissOverlayBackground" Value="Pink" />
					</Style>
				</Flyout.FlyoutPresenterStyle>
				<Border x:Name="FlyoutContentBorder">
					<TextBlock Text="Asd" />
				</Border>
			</Flyout>
		""");
		
		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);
		SUT.Flyout.ShowAt(SUT);

		var popup = GetOpenPopup();
		var presenter = popup.Child.GetFirstDescendant<DrawerFlyoutPresenter>() ?? throw new InvalidOperationException("DrawerFlyoutPresenter not found");

		// note: don't need to wait for DrawerFlyoutPresenter to be loaded here, since we don't care about its measurements...

		Assert.AreEqual(presenter.OpenDirection, DrawerOpenDirection.Left);
		Assert.AreEqual(presenter.DrawerDepth, new GridLength(3.12, GridUnitType.Star));
		Assert.AreEqual(presenter.IsGestureEnabled, true);
		Assert.AreEqual((presenter.LightDismissOverlayBackground as SolidColorBrush)?.Color, Colors.Pink);
	}

	[TestMethod]
	[DataRow(DrawerOpenDirection.Left)]
	[DataRow(DrawerOpenDirection.Up)]
	[DataRow(DrawerOpenDirection.Right)]
	[DataRow(DrawerOpenDirection.Down)]
	public async Task OpenDirection_Layout(DrawerOpenDirection openDirection)
	{
#if false // uno#14372: can't use "xmlns'd attached property style setter" with XamlReader
		//var SUT = BuildButtonFlyout($$"""
		//	<Flyout Placement="Full">
		//		<Flyout.FlyoutPresenterStyle>
		//			<Style BasedOn="{StaticResource DrawerFlyoutPresenterStyle}" TargetType="FlyoutPresenter">
		//				<Setter Property="utu:DrawerFlyoutPresenter.OpenDirection" Value="{{openDirection}}" />
		//				<Setter Property="utu:DrawerFlyoutPresenter.DrawerDepth" Value="0.5*" />
		//				<Setter Property="Background" Value="SkyBlue" />
		//				<Setter Property="utu:DrawerFlyoutPresenter.LightDismissOverlayBackground" Value="#80FFC0CB" />
		//			</Style>
		//		</Flyout.FlyoutPresenterStyle>
		//		<Border x:Name="FlyoutContentBorder">
		//			<TextBlock Text="{{openDirection}}" />
		//		</Border>
		//	</Flyout>
		//""");
#else
		var SUT = new Button
		{
			Content = "Asd",
			Flyout = new LambdaDrawerFlyout(Setup)
			{
				Content = new Border
				{
					Name = "FlyoutContentBorder",
					Child = new TextBlock { Text = "Asd" },
				}
			}
		};
		void Setup(DrawerFlyoutPresenter presenter)
		{
			presenter.Background = new SolidColorBrush(Colors.Pink with { A = 127 });
			presenter.LightDismissOverlayBackground = new SolidColorBrush(Colors.SkyBlue);
			presenter.OpenDirection = openDirection;
			presenter.DrawerDepth = new GridLength(0.5, GridUnitType.Star);
		}
#endif

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);
		SUT.Flyout.ShowAt(SUT);

		// Flyout.ShowAt guarantees its presence in visual tree, but it is not loaded, and we wont be able to read any measurements.
		var popup = GetOpenPopup();
		var child = popup.Child as FlyoutPresenter ?? throw new InvalidOperationException("FlyoutPresenter not found");
		await UnitTestsUIContentHelper.WaitForLoaded(child);

		//var tree = popup.Child.TreeGraph();
		//FlyoutPresenter // Actual=1024x800, Constraints=[96,NaN,NaN]x[40,NaN,NaN], HV=Stretch/Stretch, HVC=Stretch/Stretch, CornerRadius=0, Margin=0, Padding=0, Opacity=1, Visibility=Visible
		//	DrawerFlyoutPresenter // Actual=1024x800, Constraints=[0,NaN,∞]x[0,NaN,∞], HV=Stretch/Stretch, HVC=Stretch/Stretch, CornerRadius=0, Margin=0, Padding=0, Opacity=1, Visibility=Visible
		//		Grid#RootPanel // Actual=1024x800, Constraints=[0,NaN,∞]x[0,NaN,∞], HV=Stretch/Stretch, CornerRadius=0, Margin=0, Padding=0, Opacity=1, Visibility=Visible
		//			Border#LightDismissOverlay // Actual=1024x800, Constraints=[0,NaN,∞]x[0,NaN,∞], HV=Stretch/Stretch, CornerRadius=0, Margin=0, Padding=0, Opacity=0, Visibility=Visible
		//			ContentPresenter#DrawerContentPresenter // Actual=512x800, Constraints=[0,512,∞]x[0,NaN,∞], HV=Right/Stretch, HVC=Stretch/Stretch, CornerRadius=0, Margin=0, Padding=0, Opacity=1, Visibility=Visible
		//				Border#FlyoutContentBorder // Actual=510x798, Constraints=[0,NaN,∞]x[0,NaN,∞], HV=Stretch/Stretch, CornerRadius=0, Margin=0, Padding=0, Opacity=1, Visibility=Visible
		//					TextBlock // Actual=23.1875x18.62109375, Constraints=[0,NaN,∞]x[0,NaN,∞], HV=Stretch/Stretch, Text="Asd", Margin=0, Padding=0, Opacity=1, Visibility=Visible
		
		var presenter = popup.Child.GetFirstDescendant<DrawerFlyoutPresenter>() ?? throw new InvalidOperationException("DrawerFlyoutPresenter not found");
		var content = presenter.Content is Border { Name: "FlyoutContentBorder" } border ? border : throw new InvalidOperationException("#FlyoutContentBorder not found");

		Assert.AreEqual(presenter.OpenDirection, openDirection, "Unexpected OpenDirection");
		Assert.IsFalse(presenter.ActualWidth == 0 || presenter.ActualHeight == 0, $"DrawerFlyoutPresenter Actual: {presenter.ActualWidth}x{presenter.ActualHeight}");

		var ctx = openDirection is DrawerOpenDirection.Left or DrawerOpenDirection.Right
			? (AvailableLength: presenter.ActualWidth, ContentLength: content.ActualWidth, PrimaryAxis: nameof(presenter.Width))
			: (AvailableLength: presenter.ActualHeight, ContentLength: content.ActualHeight, PrimaryAxis: nameof(presenter.Height));
		Assert.AreEqual(ctx.AvailableLength * 0.5, ctx.ContentLength, ctx.AvailableLength * 0.05, $"Invalid content size, expecting it to be half of given: {presenter.ActualWidth}x{presenter.ActualHeight} vs {content.ActualWidth}x{content.ActualHeight}, axis={ctx.PrimaryAxis}, direction={openDirection}");
	}

	private static Button BuildButtonFlyout(string flyoutXaml, string? header = null)
	{
		return XamlHelper.LoadXaml<Button>($$"""
			<Button Content="{{header ?? "Asd"}}">
				<Button.Flyout>
					{{flyoutXaml}}
				</Button.Flyout>
			</Button>
		""");
	}

	private static Popup GetOpenPopup() => GetOpenPopupsCompat().LastOrDefault() ?? throw new InvalidOperationException("No open popup found.");

	private static IReadOnlyList<Popup> GetOpenPopupsCompat()
	{
		return VisualTreeHelper
#if IS_WINUI
			.GetOpenPopupsForXamlRoot(Window.Current.Content.XamlRoot);
#else
			.GetOpenPopups(UnitTestsUIContentHelper.CurrentTestWindow);
#endif
	}

	// uno#14372: since we can't use "xmlns'd attached property style setter" with XamlReader,
	// this is a workaround to inject property without having to re-template everything.
	private class LambdaDrawerFlyout : Flyout
	{
		private readonly Action<DrawerFlyoutPresenter> _setup;

		public LambdaDrawerFlyout(Action<DrawerFlyoutPresenter> setup)
		{
			_setup = setup;

			Placement = FlyoutPlacementMode.Full;
			FlyoutPresenterStyle = (Style)Application.Current.Resources["DrawerFlyoutPresenterStyle"];
		}

		protected override Control CreatePresenter()
		{
			var wrapper = base.CreatePresenter();
			if (wrapper.GetTemplateRoot() is DrawerFlyoutPresenter presenter)
			{
				// note: the style is already applied in base.CreatePresenter(), so we can safely override from here.
				_setup(presenter);
			}

			return wrapper;
		}
	}
}
