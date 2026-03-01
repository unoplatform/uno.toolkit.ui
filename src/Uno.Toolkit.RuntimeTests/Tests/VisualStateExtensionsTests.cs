using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.UI;
using Uno.UI.RuntimeTests;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace Uno.Toolkit.RuntimeTests.Tests;

[TestClass]
[RunsOnUIThread]
internal class VisualStateExtensionsTests
{
	[TestMethod]
	public async Task When_State_Set_To_Red_Background_Changes()
	{
		var (userControl, border) = CreateTestContent();

		await UnitTestUIContentHelperEx.SetContentAndWait(userControl);

		// Act: set state to Red
		VisualStateManagerExtensions.SetStates(userControl, "Red");
		await UnitTestUIContentHelperEx.WaitForIdle();
		// Allow transition to complete
		await Task.Delay(500);
		await UnitTestUIContentHelperEx.WaitForIdle();

		// Assert
		var brush = border.Background as SolidColorBrush;
		Assert.IsNotNull(brush, "Border background should be a SolidColorBrush after state change.");
		AssertColorMatch(Colors.Red, brush!.Color, "Red");
	}

	[TestMethod]
	public async Task When_State_Set_To_Green_Background_Changes()
	{
		var (userControl, border) = CreateTestContent();

		await UnitTestUIContentHelperEx.SetContentAndWait(userControl);

		// Act
		VisualStateManagerExtensions.SetStates(userControl, "Green");
		await UnitTestUIContentHelperEx.WaitForIdle();
		await Task.Delay(500);
		await UnitTestUIContentHelperEx.WaitForIdle();

		// Assert
		var brush = border.Background as SolidColorBrush;
		Assert.IsNotNull(brush, "Border background should be a SolidColorBrush after state change.");
		AssertColorMatch(Color.FromArgb(255, 0, 128, 0), brush!.Color, "Green");
	}

	[TestMethod]
	public async Task When_State_Set_To_Blue_Background_Changes()
	{
		var (userControl, border) = CreateTestContent();

		await UnitTestUIContentHelperEx.SetContentAndWait(userControl);

		// Act
		VisualStateManagerExtensions.SetStates(userControl, "Blue");
		await UnitTestUIContentHelperEx.WaitForIdle();
		await Task.Delay(500);
		await UnitTestUIContentHelperEx.WaitForIdle();

		// Assert
		var brush = border.Background as SolidColorBrush;
		Assert.IsNotNull(brush, "Border background should be a SolidColorBrush after state change.");
		AssertColorMatch(Colors.Blue, brush!.Color, "Blue");
	}

	[TestMethod]
	public async Task When_States_Cycled_Background_Updates_Each_Time()
	{
		var (userControl, border) = CreateTestContent();

		await UnitTestUIContentHelperEx.SetContentAndWait(userControl);

		var states = new[]
		{
			("Red", Colors.Red),
			("Green", Color.FromArgb(255, 0, 128, 0)),
			("Blue", Colors.Blue),
		};

		foreach (var (stateName, expectedColor) in states)
		{
			VisualStateManagerExtensions.SetStates(userControl, stateName);
			await UnitTestUIContentHelperEx.WaitForIdle();
			await Task.Delay(500);
			await UnitTestUIContentHelperEx.WaitForIdle();

			var brush = border.Background as SolidColorBrush;
			Assert.IsNotNull(brush, $"Border background should be a SolidColorBrush after setting state to {stateName}.");
			AssertColorMatch(expectedColor, brush!.Color, stateName);
		}
	}

	private static (UserControl control, Border border) CreateTestContent()
	{
		// Build the same structure as the sample page: a UserControl with
		// VisualStateManagerExtensions.States driving VisualStates that set a Border background.
		var border = new Border
		{
			Width = 200,
			Height = 200,
			Background = new SolidColorBrush(Colors.Transparent),
		};

		var grid = new Grid();
		grid.Children.Add(border);

		// Define visual states
		var redState = new VisualState { Name = "Red" };
		redState.Setters.Add(new Setter { Target = new TargetPropertyPath { Path = new PropertyPath("Background"), Target = border }, Value = new SolidColorBrush(Colors.Red) });

		var greenState = new VisualState { Name = "Green" };
		greenState.Setters.Add(new Setter { Target = new TargetPropertyPath { Path = new PropertyPath("Background"), Target = border }, Value = new SolidColorBrush(Color.FromArgb(255, 0, 128, 0)) });

		var blueState = new VisualState { Name = "Blue" };
		blueState.Setters.Add(new Setter { Target = new TargetPropertyPath { Path = new PropertyPath("Background"), Target = border }, Value = new SolidColorBrush(Colors.Blue) });

		var group = new VisualStateGroup();
		group.States.Add(redState);
		group.States.Add(greenState);
		group.States.Add(blueState);

		VisualStateManager.SetVisualStateGroups(grid, new List<VisualStateGroup> { group }	);

		var userControl = new UserControl { Content = grid };

		return (userControl, border);
	}

	private static void AssertColorMatch(Color expected, Color actual, string label)
	{
		Assert.AreEqual(expected.R, actual.R, $"{label}: Red channel mismatch.");
		Assert.AreEqual(expected.G, actual.G, $"{label}: Green channel mismatch.");
		Assert.AreEqual(expected.B, actual.B, $"{label}: Blue channel mismatch.");
	}
}
