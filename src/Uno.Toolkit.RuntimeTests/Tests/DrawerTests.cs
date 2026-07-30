using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.UI.RuntimeTests;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.UI;
using Uno.UI.Extensions;
using Windows.System;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Automation.Provider;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Animation;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
#endif

namespace Uno.Toolkit.RuntimeTests.Tests;

[TestClass]
[RunsOnUIThread]
internal class DrawerTests
{
	[TestMethod]
#if __IOS__
	[Ignore("Opacity is not supposed to be modified from non ui-thread")]
#endif
	public async Task IsOpen_FromNonUIThread()
	{
		var drawer = new DrawerControl
		{
			Content = new Grid()
		};

		// don't wait for loaded, start the task immediately
		UIHelper.Content = drawer;
		await Task.Run(async () =>
		{
			drawer.IsOpen = true;

			await UIHelper.WaitForLoaded(drawer);
			await UIHelper.WaitForIdle();
			await UnitTestUIContentHelperEx.WaitFor(() => drawer.AnimationStoryboard?.GetCurrentState() == ClockState.Stopped);

			drawer.IsOpen = false;
		});

		// leave time for IsOpen=false (animation or not) to finish (if it doesn't throw)
		await UIHelper.WaitForIdle();
		await UnitTestUIContentHelperEx.WaitFor(() => drawer.AnimationStoryboard?.GetCurrentState() == ClockState.Stopped);

		var lightDismissOverlay = drawer.GetFirstDescendant<Border>(x => x.Name == DrawerControl.TemplateParts.LightDismissOverlayName) ??
			throw new Exception($"Failed to find {DrawerControl.TemplateParts.LightDismissOverlayName}");

		await UnitTestUIContentHelperEx.WaitFor(
			() => lightDismissOverlay.Opacity == 0,
			message: $"Expected lightDismissOverlay.Opacity to be 0, got {lightDismissOverlay.Opacity}");
	}

	[TestMethod]
	public async Task AutomationPeers_Expose_Drawer_Semantics_And_Dismiss()
	{
		var drawer = new DrawerControl
		{
			DrawerDepth = 200,
			IsOpen = true,
			DrawerContent = new Button { Content = "Drawer action" },
		};
		AutomationProperties.SetName(drawer, "Navigation drawer");

		await UnitTestUIContentHelperEx.SetContentAndWait(drawer);

		var drawerPeer = FrameworkElementAutomationPeer.CreatePeerForElement(drawer);
		Assert.IsInstanceOfType(drawerPeer, typeof(DrawerControlAutomationPeer));
		Assert.AreEqual(AutomationControlType.Group, drawerPeer!.GetAutomationControlType());

		var pane = drawer.GetFirstDescendant<ContentControl>(x => x.Name == DrawerControl.TemplateParts.DrawerContentControlName) ??
			throw new Exception($"Failed to find {DrawerControl.TemplateParts.DrawerContentControlName}");
		var panePeer = FrameworkElementAutomationPeer.CreatePeerForElement(pane);
		Assert.IsNotNull(panePeer);
		Assert.AreEqual(AutomationControlType.Window, panePeer!.GetAutomationControlType());
		Assert.AreEqual("Navigation drawer", panePeer.GetName());

		var windowProvider = panePeer.GetPattern(PatternInterface.Window) as IWindowProvider;
		Assert.IsNotNull(windowProvider);
		Assert.IsTrue(windowProvider!.IsModal);
		Assert.IsTrue(windowProvider.IsTopmost);

		var lightDismiss = drawer.GetFirstDescendant<Border>(x => x.Name == DrawerControl.TemplateParts.LightDismissOverlayName) ??
			throw new Exception($"Failed to find {DrawerControl.TemplateParts.LightDismissOverlayName}");

		var openChildren = drawerPeer.GetChildren() ?? throw new Exception("Drawer peer should expose children");
		Assert.IsTrue(openChildren.OfType<FrameworkElementAutomationPeer>().Any(x => ReferenceEquals(x.Owner, pane)));
		var lightDismissPeer = openChildren
			.OfType<FrameworkElementAutomationPeer>()
			.Single(x => ReferenceEquals(x.Owner, lightDismiss));
		Assert.AreEqual(AutomationControlType.Button, lightDismissPeer.GetAutomationControlType());
		Assert.AreEqual("Close", lightDismissPeer.GetName());
		Assert.AreEqual("LightDismiss", lightDismissPeer.GetAutomationId());

		var invokeProvider = lightDismissPeer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
		Assert.IsNotNull(invokeProvider);
		invokeProvider!.Invoke();

		Assert.IsFalse(drawer.IsOpen);
		Assert.AreEqual(AccessibilityView.Raw, AutomationProperties.GetAccessibilityView(pane));
		Assert.IsFalse(lightDismiss.IsHitTestVisible);
		Assert.IsNull(panePeer.GetPattern(PatternInterface.Window));
		Assert.AreEqual(0, panePeer.GetChildren()?.Count ?? 0);

		var closedChildren = drawerPeer.GetChildren();
		Assert.IsFalse(closedChildren?.OfType<FrameworkElementAutomationPeer>().Any(x =>
			ReferenceEquals(x.Owner, pane) || ReferenceEquals(x.Owner, lightDismiss)) == true);
	}

	[TestMethod]
	public async Task Opening_And_Closing_Manages_Focus_And_Accessibility()
	{
		var opener = new Button { Content = "Open" };
		var drawerButton = new Button { Content = "Drawer action" };
		var drawer = new DrawerControl
		{
			DrawerDepth = 200,
			Content = opener,
			DrawerContent = drawerButton,
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(drawer);
		var pane = drawer.GetFirstDescendant<ContentControl>(x => x.Name == DrawerControl.TemplateParts.DrawerContentControlName) ??
			throw new Exception($"Failed to find {DrawerControl.TemplateParts.DrawerContentControlName}");
		Assert.AreEqual(Visibility.Collapsed, pane.Visibility);
		Assert.IsFalse(pane.IsEnabled);

		Assert.IsTrue(opener.Focus(FocusState.Programmatic));

		drawer.IsOpen = true;
		await UnitTestUIContentHelperEx.WaitFor(() => drawerButton.FocusState != FocusState.Unfocused);

		Assert.AreEqual(AccessibilityView.Control, AutomationProperties.GetAccessibilityView(pane));
		Assert.AreEqual(KeyboardNavigationMode.Cycle, pane.TabFocusNavigation);

		drawer.IsOpen = false;
		await UnitTestUIContentHelperEx.WaitFor(() => opener.FocusState != FocusState.Unfocused);

		Assert.AreEqual(AccessibilityView.Raw, AutomationProperties.GetAccessibilityView(pane));
		Assert.IsFalse(pane.IsEnabled);
		Assert.IsFalse(pane.IsHitTestVisible);
		Assert.IsFalse(drawerButton.Focus(FocusState.Programmatic));
		await UnitTestUIContentHelperEx.WaitFor(() => drawer.AnimationStoryboard.GetCurrentState() == ClockState.Stopped);
		Assert.AreEqual(Visibility.Collapsed, pane.Visibility);
	}

	[TestMethod]
	public async Task LightDismiss_Peer_Is_Only_Exposed_When_Enabled()
	{
		var drawer = new DrawerControl
		{
			DrawerDepth = 200,
			IsOpen = true,
			IsLightDismissEnabled = false,
			DrawerContent = new Button { Content = "Drawer action" },
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(drawer);

		var lightDismiss = drawer.GetFirstDescendant<Border>(x => x.Name == DrawerControl.TemplateParts.LightDismissOverlayName) ??
			throw new Exception($"Failed to find {DrawerControl.TemplateParts.LightDismissOverlayName}");
		Assert.AreEqual(AccessibilityView.Raw, AutomationProperties.GetAccessibilityView(lightDismiss));

		var drawerPeer = FrameworkElementAutomationPeer.CreatePeerForElement(drawer);
		Assert.IsNotNull(drawerPeer);
		Assert.IsFalse(drawerPeer!.GetChildren()?.OfType<FrameworkElementAutomationPeer>()
			.Any(x => ReferenceEquals(x.Owner, lightDismiss)) == true);
	}

	[TestMethod]
	public async Task Escape_Dismisses_Only_When_LightDismiss_Is_Enabled()
	{
		var drawer = new DrawerControl
		{
			DrawerDepth = 200,
			IsOpen = true,
			DrawerContent = new Button { Content = "Drawer action" },
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(drawer);

		Assert.IsTrue(drawer.TryHandleDismissKey(VirtualKey.Escape));
		Assert.IsFalse(drawer.IsOpen);

		drawer.IsLightDismissEnabled = false;
		drawer.IsOpen = true;

		Assert.IsFalse(drawer.TryHandleDismissKey(VirtualKey.Escape));
		Assert.IsTrue(drawer.IsOpen);
	}

	[TestMethod]
	public async Task Custom_Template_Base_Parts_Get_Dedicated_Automation_Peers()
	{
		var drawer = new DrawerControl
		{
			DrawerDepth = 200,
			IsOpen = true,
			DrawerContent = new Button { Content = "Drawer action" },
			Template = XamlHelper.LoadXaml<ControlTemplate>("""
				<ControlTemplate TargetType="utu:DrawerControl">
					<Grid>
						<ContentPresenter x:Name="MainContentPresenter" />
						<Border x:Name="LightDismissOverlay" />
						<ContentControl x:Name="DrawerContentControl"
										Content="{TemplateBinding DrawerContent}">
							<ContentControl.RenderTransform>
								<TranslateTransform />
							</ContentControl.RenderTransform>
						</ContentControl>
						<Border x:Name="GestureInterceptor" />
					</Grid>
				</ControlTemplate>
				"""),
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(drawer);

		var pane = drawer.GetFirstDescendant<ContentControl>(x => x.Name == DrawerControl.TemplateParts.DrawerContentControlName) ??
			throw new Exception($"Failed to find {DrawerControl.TemplateParts.DrawerContentControlName}");
		var lightDismiss = drawer.GetFirstDescendant<Border>(x => x.Name == DrawerControl.TemplateParts.LightDismissOverlayName) ??
			throw new Exception($"Failed to find {DrawerControl.TemplateParts.LightDismissOverlayName}");
		var drawerPeer = FrameworkElementAutomationPeer.CreatePeerForElement(drawer) ??
			throw new Exception("Drawer peer was not created");
		var children = drawerPeer.GetChildren() ?? throw new Exception("Drawer peer should expose children");

		var panePeer = children.OfType<FrameworkElementAutomationPeer>().Single(x => ReferenceEquals(x.Owner, pane));
		var lightDismissPeer = children.OfType<FrameworkElementAutomationPeer>().Single(x => ReferenceEquals(x.Owner, lightDismiss));

		Assert.AreEqual(AutomationControlType.Window, panePeer.GetAutomationControlType());
		Assert.IsInstanceOfType(panePeer.GetPattern(PatternInterface.Window), typeof(IWindowProvider));
		Assert.AreEqual(AutomationControlType.Button, lightDismissPeer.GetAutomationControlType());
		Assert.IsInstanceOfType(lightDismissPeer.GetPattern(PatternInterface.Invoke), typeof(IInvokeProvider));
	}

	[TestMethod]
	public async Task FitToContent_Drawer_Measures_After_Being_Collapsed()
	{
		var drawer = new DrawerControl
		{
			Width = 400,
			Height = 400,
			Content = new Grid(),
			DrawerContent = new Button
			{
				Content = "Drawer action",
				Width = 180,
			},
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(drawer);

		var pane = drawer.GetFirstDescendant<ContentControl>(x => x.Name == DrawerControl.TemplateParts.DrawerContentControlName) ??
			throw new Exception($"Failed to find {DrawerControl.TemplateParts.DrawerContentControlName}");
		Assert.AreEqual(Visibility.Collapsed, pane.Visibility);

		drawer.IsOpen = true;

		await UnitTestUIContentHelperEx.WaitFor(() => pane.Visibility == Visibility.Visible && pane.ActualWidth >= 180);
		Assert.IsTrue(drawer.IsOpen);
	}

	[TestMethod]
	public async Task Opening_Without_Focusable_Content_Focuses_The_Pane()
	{
		var opener = new Button { Content = "Open" };
		var drawer = new DrawerControl
		{
			DrawerDepth = 200,
			Content = opener,
			DrawerContent = new TextBlock { Text = "Drawer content" },
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(drawer);

		var pane = drawer.GetFirstDescendant<ContentControl>(x => x.Name == DrawerControl.TemplateParts.DrawerContentControlName) ??
			throw new Exception($"Failed to find {DrawerControl.TemplateParts.DrawerContentControlName}");
		Assert.IsTrue(opener.Focus(FocusState.Programmatic));

		drawer.IsOpen = true;

		await UnitTestUIContentHelperEx.WaitFor(() => pane.FocusState != FocusState.Unfocused);

		drawer.IsOpen = false;
		await UnitTestUIContentHelperEx.WaitFor(() => opener.FocusState != FocusState.Unfocused);
	}
}
