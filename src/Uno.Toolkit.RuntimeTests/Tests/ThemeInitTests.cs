using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Disposables;
using Uno.Toolkit.RuntimeTests.Extensions;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.UI;
using Uno.UI.RuntimeTests;
using Uno.Material;
using Uno.Toolkit.UI.Material;
#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
#else
using Windows.UI.Xaml;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
#endif

namespace Uno.Toolkit.RuntimeTests.Tests
{
	[TestClass]
	[RunsOnUIThread]
	internal class ThemeInitTests
	{
		private const string DarkColor = "#FFFF0000";
		private const string LightColor = "#FF00FF00";
		private const string LightFontFamily = "Override_LightFontFamily";
		private const string MediumFontFamily = "Override_MediumFontFamily";
		private const string RegularFontFamily = "Override_RegularFontFamily";

		[TestMethod]
		[DataRow(true, DisplayName = "Dark Mode")]
		[DataRow(false, DisplayName = "Light Mode")]
		[RunsOnUIThread]
		public async Task Override_Colors(bool isDark)
		{
			bool isDarkInitial = false;
			XamlRoot? root = null;
			var theme = new MaterialToolkitTheme(colorOverride: BuildColorOverride(), fontOverride: BuildFontOverride());

			try
			{

				var primaryButton = new Button() { Content = "Test" };
				var grid = new Grid() { Children = { primaryButton } };
				primaryButton.Resources.MergedDictionaries.Add(theme);

				await UnitTestUIContentHelperEx.SetContentAndWait(grid);

				root = UnitTestsUIContentHelper.Content?.XamlRoot;
				isDarkInitial = SystemThemeHelper.GetRootTheme(root) == ApplicationTheme.Dark;

				SystemThemeHelper.SetRootTheme(root, isDark);
				await UnitTestsUIContentHelper.WaitForIdle();

				Assert.AreEqual(isDark ? DarkColor : LightColor, (primaryButton.Background as SolidColorBrush)?.Color.ToString());

				await UnitTestsUIContentHelper.WaitForIdle();
			}
			finally
			{
				//RevertTheme(theme);
				SystemThemeHelper.SetRootTheme(root, isDarkInitial);
			}
		}

		private static void RevertTheme(MaterialToolkitTheme theme)
		{
			theme.MergedDictionaries.Clear();
			theme.ThemeDictionaries.Clear();
			theme.Clear();
		}

		[TestMethod]
		[RunsOnUIThread]
		public async Task Override_FontFamily()
		{
			var theme = new MaterialToolkitTheme(fontOverride: BuildFontOverride());

			try
			{
				var content = (Grid)XamlReader.Load(@"
					<Grid xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
						  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
						<StackPanel>
							<TextBlock x:Name=""lightTextBlock"" Text=""Light Font Family"" FontFamily=""{ThemeResource MaterialLightFontFamily}""/>
							<TextBlock x:Name=""mediumTextBlock"" Text=""Medium Font Family"" FontFamily=""{ThemeResource MaterialMediumFontFamily}""/>
							<TextBlock x:Name=""regularTextBlock"" Text=""Regular Font Family"" FontFamily=""{ThemeResource MaterialRegularFontFamily}""/>
						</StackPanel>
					</Grid>
				");
				Application.Current.Resources.MergedDictionaries.Add(theme);
				await UnitTestUIContentHelperEx.SetContentAndWait(content);

				var lightTextBlock = content.FindName("lightTextBlock") as TextBlock;
				var mediumTextBlock = content.FindName("mediumTextBlock") as TextBlock;
				var regularTextBlock = content.FindName("regularTextBlock") as TextBlock;

				Assert.AreEqual(LightFontFamily, lightTextBlock?.FontFamily.Source);
				Assert.AreEqual(MediumFontFamily, mediumTextBlock?.FontFamily.Source);
				Assert.AreEqual(RegularFontFamily, regularTextBlock?.FontFamily.Source);
			}
			finally
			{

				RevertTheme(theme);
			}
		}

		[TestMethod]
		[RunsOnUIThread]
		public async Task Change_Palette()
		{
			var newPalette = BuildColorOverride();
			var currentTheme = Application.Current.Resources.MergedDictionaries.FirstOrDefault(x => x is MaterialToolkitTheme) as MaterialToolkitTheme;

			var grid = (Grid)XamlReader.Load(@"
				<Grid xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
						xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
						<Button x:Name=""filledButton"" Content=""Filled Button"" Style=""{StaticResource FilledButtonStyle}""/>
				</Grid>
			");

			await UnitTestUIContentHelperEx.SetContentAndWait(grid);

			var button = grid.FindName("filledButton") as Button;

			if (currentTheme is { })
			{
				currentTheme.ColorOverrideDictionary = newPalette;
			}

			await UnitTestsUIContentHelper.WaitForIdle();

			Assert.AreEqual("#FFFF0000", (button?.Background as SolidColorBrush)?.Color.ToString());
		}


		private ResourceDictionary BuildColorOverride()
		{
			var colorOverride = new ResourceDictionary();

			var darkDict = new ResourceDictionary
			{
				{ "PrimaryColor", DarkColor }
			};
			var lightDict = new ResourceDictionary
			{
				{ "PrimaryColor", LightColor }
			};

			colorOverride.ThemeDictionaries.Add("Light", lightDict);
			colorOverride.ThemeDictionaries.Add("Dark", darkDict);

			return colorOverride;
		}

		private ResourceDictionary BuildFontOverride()
		{
			return new ResourceDictionary
			{
				{ "MaterialLightFontFamily", LightFontFamily },
				{ "MaterialMediumFontFamily", MediumFontFamily },
				{ "MaterialRegularFontFamily", RegularFontFamily }
			};
		}
	}
}
