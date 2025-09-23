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
	[TestMethod]
	public async Task Can_Open()
	{
		var host = XamlHelper.LoadXaml<Button>("""
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

		try
		{
			await UnitTestUIContentHelperEx.SetContentAndWait(host);
			host.Flyout.ShowAt(host);

			await UnitTestUIContentHelperEx.WaitFor(() => GetOpenPopupsCompat().Any(), message: "Timeout waiting on flyout open");
		}
		finally
		{
			host.Flyout.Hide();
		}
	}

#if !HAS_UNO // uno#14372: can't use "xmlns'd attached property style setter" with XamlReader
	[TestMethod]
#endif
	public async Task AttachedProperty_Inheritance()
	{
		var host = BuildButtonFlyout($$"""
			<Flyout Placement="Full">
				<Flyout.FlyoutPresenterStyle>
					<Style BasedOn="{StaticResource DrawerFlyoutPresenterStyle}" TargetType="FlyoutPresenter">
						<Setter Property="utu:DrawerFlyoutPresenter.OpenDirection" Value="Left" />
						<Setter Property="utu:DrawerFlyoutPresenter.DrawerLength" Value="312*" />
						<Setter Property="utu:DrawerFlyoutPresenter.IsGestureEnabled" Value="True" />
						<Setter Property="utu:DrawerFlyoutPresenter.LightDismissOverlayBackground" Value="Pink" />
					</Style>
				</Flyout.FlyoutPresenterStyle>
				<Border x:Name="FlyoutContentBorder">
					<TextBlock Text="Asd" />
				</Border>
			</Flyout>
		""");

		try
		{
			await UnitTestUIContentHelperEx.SetContentAndWait(host);
			host.Flyout.ShowAt(host);

#if !HAS_UNO
			await UnitTestUIContentHelperEx.WaitFor(() => GetOpenPopupsCompat().Any(), message: "Timeout waiting on flyout open");
#endif

			var popup = GetOpenPopup();
			var child = popup.Child as FlyoutPresenter ?? throw new InvalidOperationException("FlyoutPresenter not found");
			await UnitTestsUIContentHelper.WaitForLoaded(child);

			var presenter = popup.Child.GetFirstDescendant<DrawerFlyoutPresenter>() ?? throw new InvalidOperationException("DrawerFlyoutPresenter not found");

			Assert.AreEqual(presenter.OpenDirection, DrawerOpenDirection.Left);
			Assert.AreEqual(presenter.DrawerLength, new GridLength(312, GridUnitType.Star));
			Assert.AreEqual(presenter.IsGestureEnabled, true);
			Assert.AreEqual((presenter.LightDismissOverlayBackground as SolidColorBrush)?.Color, Colors.Pink);
		}
		finally
		{
			host.Flyout.Hide();
		}
	}

	[TestMethod]
#if __ANDROID__
	[Ignore("blocked behind uno#14420: flyout having 0 size")]
#endif
	[DataRow(DrawerOpenDirection.Left)]
	[DataRow(DrawerOpenDirection.Up)]
	[DataRow(DrawerOpenDirection.Right)]
	[DataRow(DrawerOpenDirection.Down)]
	public async Task OpenDirection_Layout(DrawerOpenDirection openDirection)
	{
#if !HAS_UNO // uno#14372: can't use "xmlns'd attached property style setter" with XamlReader
		var host = BuildButtonFlyout($$"""
			<Flyout Placement="Full">
				<Flyout.FlyoutPresenterStyle>
					<Style BasedOn="{StaticResource DrawerFlyoutPresenterStyle}" TargetType="FlyoutPresenter">
						<Setter Property="utu:DrawerFlyoutPresenter.OpenDirection" Value="{{openDirection}}" />
						<Setter Property="utu:DrawerFlyoutPresenter.DrawerLength" Value="0.5*" />
						<Setter Property="Background" Value="SkyBlue" />
						<Setter Property="utu:DrawerFlyoutPresenter.LightDismissOverlayBackground" Value="#80FFC0CB" />
					</Style>
				</Flyout.FlyoutPresenterStyle>
				<Border x:Name="FlyoutContentBorder">
					<TextBlock Text="{{openDirection}}" />
				</Border>
			</Flyout>
		""");
#else
		var host = new Button
		{
			Content = "Asd",
			Flyout = new LambdaDrawerFlyout(Setup)
			{
				Content = new Border
				{
					Name = "FlyoutContentBorder",
					Background = new SolidColorBrush(Colors.Pink),
					Child = new TextBlock { Text = openDirection.ToString() },
				}
			}
		};
		void Setup(FlyoutPresenter presenter)
		{
			presenter.Background = new SolidColorBrush(Colors.Pink with { A = 127 });
			DrawerFlyoutPresenter.SetLightDismissOverlayBackground(presenter, new SolidColorBrush(Colors.SkyBlue));
			DrawerFlyoutPresenter.SetOpenDirection(presenter, openDirection);
			DrawerFlyoutPresenter.SetDrawerLength(presenter, new GridLength(0.5, GridUnitType.Star));
		}
#endif

		try
		{
			await UnitTestUIContentHelperEx.SetContentAndWait(host);
			host.Flyout.ShowAt(host);

#if !HAS_UNO
			await UnitTestUIContentHelperEx.WaitFor(() => GetOpenPopupsCompat().Any(), message: "Timeout waiting on flyout open");
#endif

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
			//Console.WriteLine(tree);
			var presenter = popup.Child.GetFirstDescendant<DrawerFlyoutPresenter>() ?? throw new InvalidOperationException("DrawerFlyoutPresenter not found");
			var content = presenter.Content is Border { Name: "FlyoutContentBorder" } border ? border : throw new InvalidOperationException("#FlyoutContentBorder not found");

#if HAS_UNO && !__MOBILE__ // this is only really needed for wasm, but we don't have a good #define to use here
			// wait until the layout settle
			await Task.Delay(1000);
#endif
			Assert.AreEqual(presenter.OpenDirection, openDirection, "Unexpected OpenDirection");
			Assert.IsFalse(presenter.ActualWidth == 0 || presenter.ActualHeight == 0, $"DrawerFlyoutPresenter Actual: {presenter.ActualWidth}x{presenter.ActualHeight}");

			var ctx = openDirection is DrawerOpenDirection.Left or DrawerOpenDirection.Right
				? (AvailableLength: presenter.ActualWidth, ContentLength: content.ActualWidth, PrimaryAxis: nameof(presenter.Width))
				: (AvailableLength: presenter.ActualHeight, ContentLength: content.ActualHeight, PrimaryAxis: nameof(presenter.Height));
			Assert.AreEqual(
				ctx.AvailableLength * 0.5,
				ctx.ContentLength,
				ctx.AvailableLength * 0.05, // error margin at 5%
				$"Invalid content size, expecting it to be half of given: {presenter.ActualWidth}x{presenter.ActualHeight} vs {content.ActualWidth}x{content.ActualHeight}, axis={ctx.PrimaryAxis}, direction={openDirection}");
		}
		finally
		{
			host.Flyout.Hide();
		}
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
			.GetOpenPopupsForXamlRoot(UnitTestsUIContentHelper.RootElement?.XamlRoot);
#else
			.GetOpenPopups(UnitTestsUIContentHelper.CurrentTestWindow);
#endif
	}

	// uno#14372: since we can't use "xmlns'd attached property style setter" with XamlReader,
	// this is a workaround to inject property without having to re-template everything.
	private class LambdaDrawerFlyout : Flyout
	{
		private readonly Action<FlyoutPresenter> _setup;

		public LambdaDrawerFlyout(Action<FlyoutPresenter> setup)
		{
			_setup = setup;

			Placement = FlyoutPlacementMode.Full;
			FlyoutPresenterStyle = (Style)Application.Current.Resources["DrawerFlyoutPresenterStyle"];
		}

		protected override Control CreatePresenter()
		{
			var wrapper = base.CreatePresenter();
			if (wrapper is FlyoutPresenter presenter)
			{
				// note: the style is already applied in base.CreatePresenter(), so we can safely override from here.
				_setup(presenter);
			}

			return wrapper;
		}
	}
}
