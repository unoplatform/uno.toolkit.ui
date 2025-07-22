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
	public class ThemeInitTests
	{
		private const string DarkColor = "#FF544793";
		private const string LightColor = "#FF5946D1";

		[TestMethod]
		[DataRow(true, DisplayName = "Dark Mode")]
		[DataRow(false, DisplayName = "Light Mode")]
		[RunsOnUIThread]
		public async Task Override_Colors(bool isDark)
		{
			bool isDarkInitial = false;
			XamlRoot? root = null;
			try
			{
				var primaryButton = new Button() { Content = "Test" };
				var grid = new Grid() { Children = { primaryButton } };

				await UnitTestUIContentHelperEx.SetContentAndWait(grid);

				root = UnitTestsUIContentHelper.Content?.XamlRoot;
				isDarkInitial = SystemThemeHelper.IsRootInDarkMode(root!);

				SystemThemeHelper.SetRootTheme(root, isDark);
				await UnitTestsUIContentHelper.WaitForIdle();

				Assert.AreEqual(isDark ? DarkColor : LightColor, (primaryButton.Background as SolidColorBrush)?.Color.ToString());
			}
			finally
			{
				SystemThemeHelper.SetRootTheme(root, isDarkInitial);
			}
		}
	}
}
