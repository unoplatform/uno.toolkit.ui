using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Toolkit.RuntimeTests.Extensions;
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

namespace Uno.Toolkit.RuntimeTests.Tests
{
	[TestClass]
	[RunsOnUIThread]
	internal class CardAutomationTests
	{
		[TestMethod]
		[RequiresFullWindow]
		public async Task Card_AutomationPeer_Reflects_Clickability_And_Invokes()
		{
			var root = XamlHelper.LoadXaml<StackPanel>("""
				<StackPanel>
					<utu:Card x:Name="ClickableCard"
							  HeaderContent="Open details"
							  IsClickable="True"
							  Style="{StaticResource FilledCardStyle}" />
					<utu:Card x:Name="StructuralCard"
							  HeaderContent="Details"
							  IsClickable="False"
							  Style="{StaticResource FilledCardStyle}" />
				</StackPanel>
			""");
			var clickableCard = (Card)root.FindName("ClickableCard");
			var structuralCard = (Card)root.FindName("StructuralCard");
			var clickCount = 0;
			clickableCard.Click += (_, _) => clickCount++;

			await UnitTestUIContentHelperEx.SetContentAndWait(root);

			var clickablePeer = FrameworkElementAutomationPeer.CreatePeerForElement(clickableCard) as CardAutomationPeer;
			var structuralPeer = FrameworkElementAutomationPeer.CreatePeerForElement(structuralCard) as CardAutomationPeer;

			Assert.IsNotNull(clickablePeer);
			Assert.IsNotNull(structuralPeer);
			Assert.AreEqual(AutomationControlType.Button, clickablePeer!.GetAutomationControlType());
			Assert.AreEqual(AutomationControlType.Group, structuralPeer!.GetAutomationControlType());
			Assert.IsTrue(clickableCard.IsTabStop);
			Assert.IsFalse(structuralCard.IsTabStop);

			var invokeProvider = clickablePeer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
			Assert.IsNotNull(invokeProvider);
			Assert.IsNull(structuralPeer.GetPattern(PatternInterface.Invoke));

			invokeProvider!.Invoke();

			Assert.AreEqual(1, clickCount);
		}

		[TestMethod]
		[RequiresFullWindow]
		public async Task CardContentControl_AutomationPeer_Reflects_Clickability_And_Invokes()
		{
			var root = XamlHelper.LoadXaml<StackPanel>("""
				<StackPanel>
					<utu:CardContentControl x:Name="ClickableCard"
											IsClickable="True"
											Style="{StaticResource FilledCardContentControlStyle}">
						<TextBlock Text="Open details" />
					</utu:CardContentControl>
					<utu:CardContentControl x:Name="StructuralCard"
											IsClickable="False"
											Style="{StaticResource FilledCardContentControlStyle}">
						<TextBlock Text="Details" />
					</utu:CardContentControl>
				</StackPanel>
			""");
			var clickableCard = (CardContentControl)root.FindName("ClickableCard");
			var structuralCard = (CardContentControl)root.FindName("StructuralCard");
			var clickCount = 0;
			clickableCard.Click += (_, _) => clickCount++;

			await UnitTestUIContentHelperEx.SetContentAndWait(root);

			var clickablePeer = FrameworkElementAutomationPeer.CreatePeerForElement(clickableCard) as CardContentControlAutomationPeer;
			var structuralPeer = FrameworkElementAutomationPeer.CreatePeerForElement(structuralCard) as CardContentControlAutomationPeer;

			Assert.IsNotNull(clickablePeer);
			Assert.IsNotNull(structuralPeer);
			Assert.AreEqual(AutomationControlType.Button, clickablePeer!.GetAutomationControlType());
			Assert.AreEqual(AutomationControlType.Group, structuralPeer!.GetAutomationControlType());
			Assert.IsTrue(clickableCard.IsTabStop);
			Assert.IsFalse(structuralCard.IsTabStop);

			var invokeProvider = clickablePeer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
			Assert.IsNotNull(invokeProvider);
			Assert.IsNull(structuralPeer.GetPattern(PatternInterface.Invoke));

			invokeProvider!.Invoke();

			Assert.AreEqual(1, clickCount);
		}

		[TestMethod]
		[RequiresFullWindow]
		public async Task Card_AutomationPeer_Name_Honors_Explicit_Name_And_Rendered_Header()
		{
			var root = XamlHelper.LoadXaml<StackPanel>("""
				<StackPanel>
					<utu:Card x:Name="ExplicitNameCard"
							  HeaderContent="Ignored header"
							  Style="{StaticResource FilledCardStyle}" />
					<utu:Card x:Name="RenderedHeaderCard"
							  Style="{StaticResource FilledCardStyle}">
						<utu:Card.HeaderContentTemplate>
							<DataTemplate>
								<Grid>
									<TextBlock Text="{Binding Name}" />
								</Grid>
							</DataTemplate>
						</utu:Card.HeaderContentTemplate>
					</utu:Card>
				</StackPanel>
			""");
			var explicitNameCard = (Card)root.FindName("ExplicitNameCard");
			var renderedHeaderCard = (Card)root.FindName("RenderedHeaderCard");
			AutomationProperties.SetName(explicitNameCard, "Open account");
			renderedHeaderCard.HeaderContent = new CardTestData("Rendered account");

			await UnitTestUIContentHelperEx.SetContentAndWait(root);

			var explicitNamePeer = FrameworkElementAutomationPeer.CreatePeerForElement(explicitNameCard) as CardAutomationPeer;
			var renderedHeaderPeer = FrameworkElementAutomationPeer.CreatePeerForElement(renderedHeaderCard) as CardAutomationPeer;

			Assert.IsNotNull(explicitNamePeer);
			Assert.IsNotNull(renderedHeaderPeer);
			Assert.AreEqual("Open account", explicitNamePeer!.GetName());
			Assert.AreEqual("Rendered account", renderedHeaderPeer!.GetName());
		}

		[TestMethod]
		[RequiresFullWindow]
		public async Task CardContentControl_AutomationPeer_Name_Honors_LabeledBy_And_Rendered_Content()
		{
			var label = new TextBlock
			{
				Text = "Open settings",
			};
			var labeledCard = new CardContentControl
			{
				Content = "Ignored content",
			};
			AutomationProperties.SetLabeledBy(labeledCard, label);

			var renderedContentCard = new CardContentControl
			{
				Content = new CardTestData("Rendered settings"),
				ContentTemplate = XamlHelper.LoadXaml<DataTemplate>("""
					<DataTemplate>
						<Grid>
							<TextBlock Text="{Binding Name}" />
						</Grid>
					</DataTemplate>
				"""),
			};
			var root = new StackPanel();
			root.Children.Add(label);
			root.Children.Add(labeledCard);
			root.Children.Add(renderedContentCard);

			await UnitTestUIContentHelperEx.SetContentAndWait(root);

			var labeledPeer = FrameworkElementAutomationPeer.CreatePeerForElement(labeledCard) as CardContentControlAutomationPeer;
			var renderedContentPeer = FrameworkElementAutomationPeer.CreatePeerForElement(renderedContentCard) as CardContentControlAutomationPeer;

			Assert.IsNotNull(labeledPeer);
			Assert.IsNotNull(renderedContentPeer);
			Assert.AreEqual("Open settings", labeledPeer!.GetName());
			Assert.AreEqual("Rendered settings", renderedContentPeer!.GetName());
		}

		private sealed record CardTestData(string Name);
	}
}
