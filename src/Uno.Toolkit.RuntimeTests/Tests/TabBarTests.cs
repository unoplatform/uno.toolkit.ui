using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Toolkit.RuntimeTests.Extensions;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.UI;
using Uno.UI.RuntimeTests;
using Uno.UI.Extensions;
using Windows.Foundation;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI;
using Windows.Foundation.Metadata;
using System.ComponentModel;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Automation.Provider;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
#endif

namespace Uno.Toolkit.RuntimeTests.Tests
{
	[TestClass]
	[RunsOnUIThread]
	internal partial class TabBarTests // test cases
	{
		[TestMethod]
		public async Task TabBar1285_ICS_With_TBI_ItemTemplate()
		{
			// note: this bug doesnt happen with ItemsSource = [TBI,...]
			// because IsItemItsOwnContainerOverride=true. It only occurs
			// with the ItemTemplate>DataTemplate>TBI setup (IsUsingOwnContainerAsTemplateRoot),
			// which cause a ContentPresnter to be created as the item container.
			var source = Enumerable.Range(0, 1).ToArray();
			var SUT = new TabBar
			{
				ItemsSource = source,
				ItemTemplate = XamlHelper.LoadXaml<DataTemplate>("""
					<DataTemplate>
						<utu:TabBarItem Content="{Binding}" />
					</DataTemplate>
				"""),
			};
			await UnitTestUIContentHelperEx.SetContentAndWait(SUT);
		}

		[TestMethod]
		[DataRow(new int[0], null)]
		[DataRow(new[] { 1 }, 1)]
		[DataRow(new[] { 1, 1 }, 1)]
		[DataRow(new[] { 1, 2 }, 2)]
		public async Task TabBarTapSelection(int[] selectionSequence, object? expectation)
		{
			var source = Enumerable.Range(0, 3).ToArray();
			var SUT = new TabBar
			{
				ItemsSource = source,
			};

			await UnitTestUIContentHelperEx.SetContentAndWait(SUT);
			Assert.IsNull(SUT.SelectedItem);

			foreach (var i in selectionSequence)
			{
				((TabBarItem)SUT.ContainerFromIndex(i)).ExecuteTap();
				await UnitTestsUIContentHelper.WaitForIdle();
			}

			Assert.AreEqual((int?)expectation, SUT.SelectedItem);
		}

		[TestMethod]
		public async Task SetSelectedItem()
		{
			var source = Enumerable.Range(0, 3).Select(x => new TabBarItem { Content = x }).ToArray();
			var SUT = new TabBar
			{
				ItemsSource = source,
			};

			source[0].IsSelectable = false;

			await UnitTestUIContentHelperEx.SetContentAndWait(SUT);
			Assert.IsNull(SUT.SelectedItem);

			// should not select when IsSelectable is false
			SUT.SelectedItem = source[0];
			await UnitTestsUIContentHelper.WaitForIdle();
			Assert.AreEqual(-1, SUT.SelectedIndex);
			Assert.IsFalse(source[0].IsSelected);

			// should select
			SUT.SelectedItem = source[1];
			await UnitTestsUIContentHelper.WaitForIdle();
			Assert.AreEqual(source[1], SUT.SelectedItem);
			Assert.AreEqual(1, SUT.SelectedIndex);
			Assert.IsTrue(source[1].IsSelected);
		}

		[TestMethod]
		public async Task SetSelectedItem_SimpleSource()
		{
			var source = Enumerable.Range(0, 3).ToArray();
			var SUT = new TabBar
			{
				ItemsSource = source,
				SelectedItem = 1,
			};

			await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

			Assert.AreEqual(source[1], SUT.SelectedItem);
			Assert.AreEqual(1, SUT.SelectedIndex);
		}

		[TestMethod]
		public async Task SetSelectedIndex()
		{
			var source = Enumerable.Range(0, 3).Select(x => new TabBarItem { Content = x }).ToArray();
			var SUT = new TabBar
			{
				ItemsSource = source,
			};

			source[0].IsSelectable = false;

			await UnitTestUIContentHelperEx.SetContentAndWait(SUT);
			Assert.IsNull(SUT.SelectedItem);

			// should not select when IsSelectable is false
			SUT.SelectedIndex = 0;
			await UnitTestsUIContentHelper.WaitForIdle();
			Assert.IsNull(SUT.SelectedItem);
			Assert.IsFalse(source[0].IsSelected);

			// should select
			SUT.SelectedIndex = 1;
			await UnitTestsUIContentHelper.WaitForIdle();
			Assert.AreEqual(source[1], SUT.SelectedItem);
			Assert.AreEqual(1, SUT.SelectedIndex);
			Assert.IsTrue(source[1].IsSelected);
		}

		[TestMethod]
		public async Task Verify_Indicator_Max_Size()
		{
			var source = Enumerable.Range(0, 3).Select(x => new TabBarItem { Content = x }).ToArray();
			var SUT = new TabBar
			{
				ItemsSource = source,
				SelectionIndicatorContent = "asd",
				SelectionIndicatorContentTemplate = XamlHelper.LoadXaml<DataTemplate>("""
					<DataTemplate>
						<Border x:Name="SutIndicator" Height="5" Background="Red" />
					</DataTemplate>
				"""),
			};

			await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

			var presenter = SUT.GetFirstDescendant<TabBarSelectionIndicatorPresenter>(x => x.Visibility == Visibility.Visible);
			var indicator = presenter?.GetFirstDescendant<Border>("SutIndicator")!;
			Assert.IsNotNull(indicator, "Failed to find Border#SutIndicator");

			source[0].IsSelected = true;
			await UnitTestsUIContentHelper.WaitForIdle();

			var expectedWidth = SUT.ActualWidth / 3;
			var indicatorWidth = indicator.ActualWidth;
			Assert.AreEqual(expectedWidth, indicatorWidth, delta: 1f);

			source[1].Visibility = Visibility.Collapsed;

			await UnitTestsUIContentHelper.WaitForIdle();
			await UnitTestUIContentHelperEx.WaitFor(() => indicator.ActualWidth > indicatorWidth, timeoutMS: 2000);

			expectedWidth = SUT.ActualWidth / 2;
			Assert.AreEqual(expectedWidth, indicator.ActualWidth, delta: 1f);
		}

		[TestMethod]
		[DataRow(Orientation.Horizontal, IndicatorTransitionMode.Snap)]
		[DataRow(Orientation.Horizontal, IndicatorTransitionMode.Slide)]
		[DataRow(Orientation.Vertical, IndicatorTransitionMode.Snap)]
		[DataRow(Orientation.Vertical, IndicatorTransitionMode.Slide)]
		public async Task Verify_Indicator_Transitions(Orientation orientation, IndicatorTransitionMode transitionMode)
		{
			const int NumItems = 3;
			const double ItemSize = 100d;

			var source = Enumerable.Range(0, NumItems).ToArray();
			var SUT = new TabBar
			{
				Orientation = orientation,
				ItemsSource = source,
				Width = orientation == Orientation.Horizontal ? ItemSize * NumItems : double.NaN,
				Height = orientation == Orientation.Vertical ? ItemSize * NumItems : double.NaN,
				SelectionIndicatorContent = "asd",
				SelectionIndicatorContentTemplate = XamlHelper.LoadXaml<DataTemplate>($"""
					<DataTemplate>
						<Border x:Name="SutIndicator"
								{(orientation == Orientation.Horizontal ? "Height" : "Width")}="5"
								Background="Red" />
					</DataTemplate>
				"""),
				SelectionIndicatorTransitionMode = transitionMode,
			};

			await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

			var presenter = SUT.GetFirstDescendant<TabBarSelectionIndicatorPresenter>(x => x.Visibility == Visibility.Visible);
			var indicator = presenter?.GetFirstDescendant<Border>("SutIndicator")!;
			Assert.IsNotNull(indicator, "Failed to find Border#SutIndicator");

			for (int i = 0; i < NumItems; i++)
			{
				SUT.SelectedIndex = i;
				await UnitTestsUIContentHelper.WaitForIdle();

				await UnitTestUIContentHelperEx.WaitFor(() => i * ItemSize == GetTestCoordinate(indicator.TransformToVisual(SUT).TransformPoint(default)), timeoutMS: 2000);

				var currentPos = indicator.TransformToVisual(SUT).TransformPoint(default);
				Assert.AreEqual(i * ItemSize, GetTestCoordinate(currentPos), delta: 1f);
			}

			double GetTestCoordinate(Windows.Foundation.Point testPoint)
			{
				return orientation switch
				{
					Orientation.Horizontal => testPoint.X,
					Orientation.Vertical => testPoint.Y,
					_ => throw new ArgumentOutOfRangeException(nameof(orientation))
				};
			}
		}

		[TestMethod]
		[DataRow(Orientation.Horizontal, new[] { 0, 0, 0, 0 }, DisplayName = "Horizontal at [0,0,0,0]")]
		[DataRow(Orientation.Horizontal, new[] { 0, 30, 0, 0 }, DisplayName = "Horizontal at [0,30,0,0]")]
		[DataRow(Orientation.Horizontal, new[] { 0, 0, 0, 30 }, DisplayName = "Horizontal at [0,0,0,30]")]
		[DataRow(Orientation.Horizontal, new[] { 0, 30, 0, 30 }, DisplayName = "Horizontal at [0,30,0,30]")]
		[DataRow(Orientation.Horizontal, new[] { 0, 50, 0, 0 }, DisplayName = "Horizontal at [0,50,0,0]")]
		[DataRow(Orientation.Horizontal, new[] { 0, 0, 0, 50 }, DisplayName = "Horizontal at [0,0,0,50]")]
		[DataRow(Orientation.Horizontal, new[] { 0, 30, 0, 20 }, DisplayName = "Horizontal at [0,30,0,20]")]
		[DataRow(Orientation.Vertical, new[] { 0, 0, 0, 0 }, DisplayName = "Vertical at [0,0,0,0]")]
		[DataRow(Orientation.Vertical, new[] { 30, 0, 0, 0 }, DisplayName = "Vertical at [30,0,0,0]")]
		[DataRow(Orientation.Vertical, new[] { 0, 0, 30, 0 }, DisplayName = "Vertical at [0,0,30,0]")]
		[DataRow(Orientation.Vertical, new[] { 30, 0, 30, 0 }, DisplayName = "Vertical at [30,0,30,0]")]
		[DataRow(Orientation.Vertical, new[] { 50, 0, 0, 0 }, DisplayName = "Vertical at [50,0,0,0]")]
		[DataRow(Orientation.Vertical, new[] { 0, 0, 50, 0 }, DisplayName = "Vertical at [0,0,50,0]")]
		[DataRow(Orientation.Vertical, new[] { 30, 0, 20, 0 }, DisplayName = "Vertical at [30,0,20,0]")]
		public async Task Verify_Padding(
			Orientation orientation,
			int[] padding)
		{
			var minDimen = (double)Application.Current.Resources["TabBarHeightOrWidth"];
			var styleName = orientation switch
			{
				Orientation.Horizontal => "BaseHorizontalTabBarStyle",
				Orientation.Vertical => "BaseVerticalTabBarStyle",
				_ => throw new ArgumentOutOfRangeException(nameof(orientation))
			};

			var rootGrid = XamlHelper.LoadXaml<Grid>(@$"
				<Grid>
					<utu:TabBar xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" Padding=""{padding[0]},{padding[1]},{padding[2]},{padding[3]}"" Background=""Red"" x:Name=""MyTabBar"" Style=""{{StaticResource {styleName}}}"">
						<utu:TabBar.Items>
							<utu:TabBarItem Content=""1"" />
							<utu:TabBarItem Content=""2"" />
							<utu:TabBarItem Content=""3"" />
						</utu:TabBar.Items>
					</utu:TabBar>
				</Grid>
			");

			var tabBar = (TabBar)rootGrid.FindName("MyTabBar");

			await UnitTestUIContentHelperEx.SetContentAndWait(rootGrid);


			var c = GetMinCalculatedDimen();
			var expectedDimen = Math.Max(c, minDimen);
			double actualDimen = orientation switch
			{
				Orientation.Horizontal => tabBar.ActualHeight,
				Orientation.Vertical => tabBar.ActualWidth,
				_ => throw new ArgumentOutOfRangeException(nameof(orientation))
			};

			Assert.AreEqual(expectedDimen, actualDimen, 1d);

			double GetMinCalculatedDimen()
			{
				var tabBarItem = (TabBarItem)tabBar.ContainerFromIndex(0);

				return orientation switch
				{
					Orientation.Horizontal => padding[1] + padding[3] + tabBarItem.ActualHeight,
					Orientation.Vertical => padding[0] + padding[2] + tabBarItem.ActualWidth,
					_ => throw new ArgumentOutOfRangeException(nameof(orientation))
				};
			}
		}

		[TestMethod]
		public async Task Verify_Indicator_Display_On_Selection()
		{
			if (!ImageAssertHelper.IsScreenshotSupported())
			{
				Assert.Inconclusive(); // System.NotImplementedException: RenderTargetBitmap is not supported on this platform.;
			}

			const int NumItems = 3;
			var SUT = new TabBar
			{
				Background = new SolidColorBrush(Colors.Transparent),
				SelectionIndicatorContentTemplate = XamlHelper.LoadXaml<DataTemplate>(@"
					<DataTemplate>
						<Border Background=""Red"" />
					</DataTemplate>
				"),
				SelectionIndicatorPlacement = IndicatorPlacement.Above,
			};

			foreach (var item in Enumerable.Range(0, NumItems).Select(_ => CreateItem()))
			{
				SUT.Items.Add(item);
			}

			await UnitTestUIContentHelperEx.SetContentAndWait(SUT);



			for (int i = 0; i < NumItems; i++)
			{
				SUT.SelectedIndex = i;
				await UnitTestsUIContentHelper.WaitForIdle();

				var container = SUT.ContainerFromItem(SUT.SelectedItem);
				var selectedItem = SUT.GetInnerContainer(container); // see comment on GetInnerContainer

				Assert.IsNotNull(selectedItem);

				var renderer = await SUT.TakeScreenshot();
				var centerPoint = selectedItem!.TransformToVisual(SUT).TransformPoint(new Point(selectedItem.ActualWidth / 2, selectedItem.ActualHeight / 2));

				await renderer.AssertColorAt(Colors.Red, (int)centerPoint.X, (int)centerPoint.Y);

				foreach (var nonSelected in SUT.Items.Cast<TabBarItem>().Where(x => !x.IsSelected))
				{
					centerPoint = nonSelected.TransformToVisual(SUT).TransformPoint(new Point(nonSelected.ActualWidth / 2, nonSelected.ActualHeight / 2));
					await renderer.AssertColorAt(Colors.Green, (int)centerPoint.X, (int)centerPoint.Y);
				}
			}

			TabBarItem CreateItem()
			{
				return new TabBarItem
				{
					Content = new Border
					{
						Padding = new Thickness(20),
						Background = new SolidColorBrush(Colors.Green),
					}
				};
			}
		}

		[TestMethod]
		[DataRow(IndicatorPlacement.Above, DisplayName = "Verify Indicator Above")]
		[DataRow(IndicatorPlacement.Below, DisplayName = "Verify Indicator Below")]
		public async Task Verify_Indicator_Placement(IndicatorPlacement placement)
		{
			var item = new TabBarItem
			{
				Content = new Border
				{
					Padding = new Thickness(20),
					Background = new SolidColorBrush(Colors.Green),
				}
			};

			var SUT = new TabBar
			{
				Background = new SolidColorBrush(Colors.Transparent),
				SelectionIndicatorContentTemplate = XamlHelper.LoadXaml<DataTemplate>(@"
					<DataTemplate>
						<Border Background=""Red"" />
					</DataTemplate>
				"),
				SelectedIndex = 0,
				SelectionIndicatorPlacement = placement,
			};
			SUT.Items.Add(item);

			await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

			var belowPresenter = VisualTreeHelperEx
				.GetFirstDescendant<TabBarSelectionIndicatorPresenter>(SUT, x => x.Name == "BelowSelectionIndicatorPresenter");
			var abovePresenter = VisualTreeHelperEx
				.GetFirstDescendant<TabBarSelectionIndicatorPresenter>(SUT, x => x.Name == "AboveSelectionIndicatorPresenter");


			var renderer = await SUT.TakeScreenshot();
			var centerPoint = item.TransformToVisual(SUT).TransformPoint(new Point(item.ActualWidth / 2, item.ActualHeight / 2));


			if (placement == IndicatorPlacement.Above)
			{
				Assert.AreEqual(Visibility.Collapsed, belowPresenter!.Visibility);
				Assert.AreEqual(Visibility.Visible, abovePresenter!.Visibility);

				if (renderer is { })
				{
					await renderer.AssertColorAt(Colors.Red, (int)centerPoint.X, (int)centerPoint.Y);
				}
			}
			else
			{
				Assert.AreEqual(Visibility.Visible, belowPresenter!.Visibility);
				Assert.AreEqual(Visibility.Collapsed, abovePresenter!.Visibility);

				if (renderer is { })
				{
					await renderer.AssertColorAt(Colors.Green, (int)centerPoint.X, (int)centerPoint.Y);
				}
			}
		}

		[TestMethod]
		public async Task Verify_SelectedIndex_Not_Set_Unnecessarily()
		{
			var item1 = new TabBarItem();
			var item2 = new TabBarItem();

			var SUT = new TabBar();
			SUT.Items.Add(item1);
			SUT.Items.Add(item2);
			SUT.DataContext = new SelectedIndexTestViewModel();
			SUT.SetBinding(TabBar.SelectedIndexProperty, new Binding() { Mode = BindingMode.OneWay, Path = new PropertyPath(nameof(SelectedIndexTestViewModel.P)) });

			await UnitTestUIContentHelperEx.SetContentAndWait(SUT);
			Assert.IsNotNull(SUT.GetBindingExpression(TabBar.SelectedIndexProperty));
			SUT.SelectedIndex = 0;
			// Possible Uno bug? Explicit setting of SelectedIndex should have cleared the binding.
			//Assert.IsNull(SUT.GetBindingExpression(TabBar.SelectedIndexProperty));
		}

		[TestMethod]
		public async Task Verify_ItemTemplate_Has_No_Nested_TabBarItem()
		{
			var source = new[]
			{
				new TestRecord("True", true),
				new TestRecord("False", false),
				new TestRecord("True", true)
			};

			var dt = XamlHelper.LoadXaml<DataTemplate>("""
				<DataTemplate>
					<utu:TabBarItem Content="{Binding Name}" IsSelectable="{Binding IsSelectable}" />
				</DataTemplate>
			""");

			var SUT = new TabBar
			{
				Style = (Style)Application.Current.Resources["TopTabBarStyle"],
				ItemsSource = source,
				ItemTemplate = dt,
				SelectedIndex = 0
			};

			await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

			// Ensure the container is a `ContentPresenter` and not a `TabBarItem`
			var container = SUT.ContainerFromItem(SUT.SelectedItem);
			Assert.IsInstanceOfType(container, typeof(ContentPresenter));

			// Ensure the inner container is a `TabBarItem`
			var selectedItem = SUT.GetInnerContainer(container); // see comment on GetInnerContainer
			Assert.IsInstanceOfType(selectedItem, typeof(TabBarItem));
		}

		[TestMethod]
		public async Task Verify_ItemTemplate_Disabled_Not_Selectable()
		{
			var source = new[]
			{
				new TestRecord("True", true),
				new TestRecord("False", false),
				new TestRecord("True", true)
			};

			var dt = XamlHelper.LoadXaml<DataTemplate>("""
				<DataTemplate>
					<utu:TabBarItem Content="{Binding Name}" IsSelectable="{Binding IsSelectable}" />
				</DataTemplate>
			""");

			var SUT = new TabBar
			{
				Style = (Style)Application.Current.Resources["TopTabBarStyle"],
				ItemsSource = source,
				ItemTemplate = dt,
			};

			await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

			Assert.IsNull(SUT.SelectedItem);

			// Make sure the first item is selectable
			SUT.SelectedIndex = 0;
			await UnitTestsUIContentHelper.WaitForIdle();
			Assert.AreSame(SUT.SelectedItem, source[0]);

			SUT.SelectedIndex = 1;
			await UnitTestsUIContentHelper.WaitForIdle();
			// Assert the second item is not selected
			Assert.AreNotSame(SUT.SelectedItem, source[1]);
		}
		
		[TestMethod]
		public async Task Initial_Selection()
		{
			var setup = XamlHelper.LoadXaml<TabBar>("""
				<utu:TabBar>
					<utu:TabBar.Items>
						<utu:TabBarItem Content="Tab Uno" IsSelected="True"/>
						<utu:TabBarItem Content="Tab Deux" />
						<utu:TabBarItem Content="Tab Three" />
					</utu:TabBar.Items>
				</utu:TabBar>
			""");
			await UnitTestUIContentHelperEx.SetContentAndWait(setup);

			var selected = setup.ContainerFromIndex(0) as TabBarItem ?? throw new Exception("Container#0 not found");

			Assert.AreEqual(0, setup.SelectedIndex, "SelectedIndex is expected to be 0");
			Assert.AreEqual(selected, setup.SelectedItem, "SelectedItem is expected to be container#0");
			Assert.AreEqual(true, selected.IsSelected, "Container#0 should be selected");
		}

		[TestMethod]
		public async Task Verify_TabBar_AutomationPeer_ControlType_And_Selection_Pattern()
		{
			var source = Enumerable.Range(0, 3).ToArray();
			var SUT = new TabBar
			{
				ItemsSource = source,
			};

			await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

			var peer = FrameworkElementAutomationPeer.CreatePeerForElement(SUT) as TabBarAutomationPeer;
			Assert.IsNotNull(peer, "TabBar should expose a TabBarAutomationPeer");
			Assert.AreEqual(AutomationControlType.Tab, peer!.GetAutomationControlType());

			var selectionProvider = peer.GetPattern(PatternInterface.Selection) as ISelectionProvider;
			Assert.IsNotNull(selectionProvider, "TabBar peer should support the Selection pattern");
			Assert.IsFalse(selectionProvider!.CanSelectMultiple);
			Assert.IsFalse(selectionProvider.IsSelectionRequired);

			// No selection yet: GetSelection should be empty.
			Assert.AreEqual(0, selectionProvider.GetSelection().Length);

			SUT.SelectedIndex = 1;
			await UnitTestsUIContentHelper.WaitForIdle();

			var selection = selectionProvider.GetSelection();
			Assert.AreEqual(1, selection.Length);
			Assert.IsNotNull(selection[0]);

			var selectedItem = (TabBarItem)SUT.ContainerFromIndex(1);
			var nonSelectableItem = (TabBarItem)SUT.ContainerFromIndex(2);
			nonSelectableItem.IsSelectable = false;
			SUT.SelectedIndex = 2;
			await UnitTestsUIContentHelper.WaitForIdle();

			Assert.IsTrue(selectedItem.IsSelected);
			Assert.IsFalse(nonSelectableItem.IsSelected);
			Assert.AreEqual(1, selectionProvider.GetSelection().Length, "GetSelection should return the actual selected item");
		}

		[TestMethod]
		public async Task Verify_TabBar_AutomationPeer_Selection_With_TabBarItem_ItemTemplate()
		{
			var source = new[] { new TestRecord("Home", true) };
			var SUT = new TabBar
			{
				ItemsSource = source,
				ItemTemplate = XamlHelper.LoadXaml<DataTemplate>("""
					<DataTemplate>
						<utu:TabBarItem Content="{Binding Name}" />
					</DataTemplate>
				"""),
				SelectedIndex = 0,
			};

			await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

			var peer = FrameworkElementAutomationPeer.CreatePeerForElement(SUT) as TabBarAutomationPeer;
			Assert.IsNotNull(peer);

			var selectionProvider = peer!.GetPattern(PatternInterface.Selection) as ISelectionProvider;
			Assert.IsNotNull(selectionProvider);
			Assert.AreEqual(1, selectionProvider!.GetSelection().Length);
		}

		[TestMethod]
		public async Task Verify_TabBarItem_AutomationPeer_ControlType_And_Selection_State()
		{
			var source = Enumerable.Range(0, 2).ToArray();
			var SUT = new TabBar
			{
				ItemsSource = source,
			};

			await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

			var item0 = (TabBarItem)SUT.ContainerFromIndex(0);
			var item1 = (TabBarItem)SUT.ContainerFromIndex(1);

			var peer0 = FrameworkElementAutomationPeer.CreatePeerForElement(item0) as TabBarItemAutomationPeer;
			var peer1 = FrameworkElementAutomationPeer.CreatePeerForElement(item1) as TabBarItemAutomationPeer;
			Assert.IsNotNull(peer0, "TabBarItem should expose a TabBarItemAutomationPeer");
			Assert.IsNotNull(peer1, "TabBarItem should expose a TabBarItemAutomationPeer");
			Assert.AreEqual(AutomationControlType.TabItem, peer0!.GetAutomationControlType());

			var selectionItem0 = peer0.GetPattern(PatternInterface.SelectionItem) as ISelectionItemProvider;
			var selectionItem1 = peer1!.GetPattern(PatternInterface.SelectionItem) as ISelectionItemProvider;
			Assert.IsNotNull(selectionItem0, "TabBarItem peer should support the SelectionItem pattern");
			Assert.IsNotNull(selectionItem1, "TabBarItem peer should support the SelectionItem pattern");

			Assert.IsFalse(selectionItem0!.IsSelected);
			Assert.IsFalse(selectionItem1!.IsSelected);
			Assert.IsNotNull(selectionItem0.SelectionContainer);

			item0.IsSelectable = false;
			SUT.SelectedIndex = 0;
			await UnitTestsUIContentHelper.WaitForIdle();

			Assert.IsFalse(selectionItem0.IsSelected);
			selectionItem1.AddToSelection();
			await UnitTestsUIContentHelper.WaitForIdle();

			Assert.IsTrue(selectionItem1.IsSelected, "A rejected SelectedIndex must not count as an existing selection");
			selectionItem1.RemoveFromSelection();
			item0.IsSelectable = true;

			selectionItem0.AddToSelection();
			await UnitTestsUIContentHelper.WaitForIdle();

			Assert.IsTrue(selectionItem0.IsSelected, "AddToSelection should select when the TabBar has no selection");
			Assert.IsFalse(selectionItem1.IsSelected);
			Assert.AreEqual(0, SUT.SelectedIndex);
			Assert.ThrowsException<InvalidOperationException>(
				selectionItem1.AddToSelection,
				"AddToSelection should reject a second selection");

			selectionItem1.Select();
			await UnitTestsUIContentHelper.WaitForIdle();

			Assert.IsFalse(selectionItem0.IsSelected, "Select should replace the current selection");
			Assert.IsTrue(selectionItem1.IsSelected);
			Assert.AreEqual(1, SUT.SelectedIndex);

			selectionItem1.RemoveFromSelection();
			await UnitTestsUIContentHelper.WaitForIdle();

			Assert.IsFalse(selectionItem1.IsSelected, "RemoveFromSelection should clear an optional selection");
			Assert.AreEqual(-1, SUT.SelectedIndex);
			Assert.IsNull(SUT.SelectedItem);

			item0.IsSelectable = false;
			Assert.ThrowsException<InvalidOperationException>(
				selectionItem0.Select,
				"Select should reject a non-selectable TabBarItem");
		}

		[TestMethod]
		public async Task Verify_TabBarItem_AutomationPeer_Name_Honors_Explicit_And_Falls_Back_To_Content()
		{
			var plainItem = new TabBarItem { Content = "Home" };
			var namedItem = new TabBarItem { Content = "Home" };
			var templatedItem = new TabBarItem
			{
				Content = new TestRecord("Settings", true),
				ContentTemplate = XamlHelper.LoadXaml<DataTemplate>("""
					<DataTemplate>
						<Grid>
							<TextBlock Text="{Binding Name}" />
						</Grid>
					</DataTemplate>
				"""),
			};
			AutomationProperties.SetName(namedItem, "Go to home screen");

			var SUT = new TabBar();
			SUT.Items.Add(plainItem);
			SUT.Items.Add(namedItem);
			SUT.Items.Add(templatedItem);

			await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

			var plainPeer = FrameworkElementAutomationPeer.CreatePeerForElement(plainItem) as TabBarItemAutomationPeer;
			var namedPeer = FrameworkElementAutomationPeer.CreatePeerForElement(namedItem) as TabBarItemAutomationPeer;
			var templatedPeer = FrameworkElementAutomationPeer.CreatePeerForElement(templatedItem) as TabBarItemAutomationPeer;
			Assert.IsNotNull(plainPeer);
			Assert.IsNotNull(namedPeer);
			Assert.IsNotNull(templatedPeer);

			Assert.AreEqual("Home", plainPeer!.GetName(), "Name should fall back to the item's textual content");
			Assert.AreEqual("Go to home screen", namedPeer!.GetName(), "Explicit AutomationProperties.Name should be honored");
			Assert.AreEqual("Settings", templatedPeer!.GetName(), "Name should use text rendered by the ContentTemplate");
		}
	}
	
	internal partial class TabBarTests // supporting classes/methods
	{
		private class SelectedIndexTestViewModel : INotifyPropertyChanged
		{
			private int _p;

			public int P
			{
				get => _p; set
				{
					_p = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(P)));
				}
			}

			public event PropertyChangedEventHandler? PropertyChanged;
		}

		public record TestRecord(string Name, bool IsSelectable);

	}
}
