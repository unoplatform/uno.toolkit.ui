using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.UI;
using Uno.UI.RuntimeTests;

#if IS_WINUI
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Automation.Provider;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.RuntimeTests.Tests;

[TestClass]
[RunsOnUIThread]
public class ZoomContentControlAutomationPeerTests
{
	[TestMethod]
	public async Task When_Loaded_Then_ExposesPaneAndAutomationPatterns()
	{
		var sut = CreateZoomContentControl();
		AutomationProperties.SetName(sut, "Zoom viewport");

		await UnitTestUIContentHelperEx.SetContentAndWait(sut);

		var peer = GetPeer(sut);

		Assert.AreEqual(AutomationControlType.Pane, peer.GetAutomationControlType());
		Assert.AreEqual(nameof(ZoomContentControl), peer.GetClassName());
		Assert.AreEqual("Zoom viewport", peer.GetName());
		Assert.AreSame(peer, peer.GetPattern(PatternInterface.Scroll));
		Assert.AreSame(peer, peer.GetPattern(PatternInterface.Transform));
		Assert.AreSame(peer, peer.GetPattern(PatternInterface.Transform2));
		Assert.IsTrue(sut.IsTabStop);
	}

	[TestMethod]
	public async Task When_ScrollPatternInvoked_Then_ViewportPans()
	{
		var sut = CreateZoomContentControl();

		await UnitTestUIContentHelperEx.SetContentAndWait(sut);

		var provider = GetPeer(sut).GetPattern(PatternInterface.Scroll) as IScrollProvider
			?? throw new InvalidOperationException("ZoomContentControl should expose IScrollProvider.");

		Assert.IsTrue(provider.HorizontallyScrollable);
		Assert.IsTrue(provider.VerticallyScrollable);
		Assert.IsTrue(provider.HorizontalViewSize < 100);
		Assert.IsTrue(provider.VerticalViewSize < 100);

		provider.SetScrollPercent(50, 25);

		Assert.AreEqual(50, provider.HorizontalScrollPercent, 0.001);
		Assert.AreEqual(25, provider.VerticalScrollPercent, 0.001);

		var previousHorizontalOffset = sut.HorizontalScrollValue;
		provider.Scroll(ScrollAmount.SmallIncrement, ScrollAmount.NoAmount);

		Assert.IsTrue(sut.HorizontalScrollValue > previousHorizontalOffset);

		sut.IsPanAllowed = false;

		Assert.IsFalse(provider.HorizontallyScrollable);
		Assert.IsFalse(provider.VerticallyScrollable);
		Assert.AreEqual(ScrollPatternIdentifiers.NoScroll, provider.HorizontalScrollPercent);
		Assert.AreEqual(ScrollPatternIdentifiers.NoScroll, provider.VerticalScrollPercent);
	}

	[TestMethod]
	public async Task When_TransformPatternInvoked_Then_ZoomLevelChanges()
	{
		var sut = CreateZoomContentControl();
		sut.MinZoomLevel = 0.5;
		sut.MaxZoomLevel = 3;

		await UnitTestUIContentHelperEx.SetContentAndWait(sut);

		sut.ZoomLevel = 1.5;

		var provider = GetPeer(sut).GetPattern(PatternInterface.Transform2) as ITransformProvider2
			?? throw new InvalidOperationException("ZoomContentControl should expose ITransformProvider2.");

		Assert.IsTrue(provider.CanZoom);
		Assert.AreEqual(50, provider.MinZoom);
		Assert.AreEqual(300, provider.MaxZoom);
		Assert.AreEqual(150, provider.ZoomLevel);

		provider.Zoom(200);

		Assert.AreEqual(2, sut.ZoomLevel, 0.001);
		Assert.AreEqual(200, provider.ZoomLevel, 0.001);

		provider.ZoomByUnit(ZoomUnit.SmallIncrement);

		Assert.IsTrue(sut.ZoomLevel > 2);

		sut.IsZoomAllowed = false;

		Assert.IsFalse(provider.CanZoom);
	}

	private static ZoomContentControl CreateZoomContentControl() =>
		new()
		{
			Width = 200,
			Height = 200,
			AutoCenterContent = false,
			AutoFitToCanvas = false,
			Content = new Border
			{
				Width = 600,
				Height = 600,
			},
		};

	private static ZoomContentControlAutomationPeer GetPeer(ZoomContentControl owner) =>
		FrameworkElementAutomationPeer.CreatePeerForElement(owner) as ZoomContentControlAutomationPeer
		?? throw new InvalidOperationException("ZoomContentControl should create its dedicated automation peer.");
}
