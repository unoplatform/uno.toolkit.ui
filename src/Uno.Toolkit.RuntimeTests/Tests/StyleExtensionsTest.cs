using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Extensions.Specialized;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.UI;
using Uno.UI.RuntimeTests;

#if IS_WINUI
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
#else
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endif

namespace Uno.Toolkit.RuntimeTests.Tests;

[TestClass]
[RunsOnUIThread]
internal class StyleExtensionsTest
{
	[TestMethod]
	public async Task StyleExtensionsResourcesPropertyAppliedToStyleTest()
	{
		// Arrange
		var colorBrush = new SolidColorBrush(Colors.DarkGreen);
		var testKey = "TestKey";

		var resourceDictionary = new ResourceDictionary
		{
			{ testKey, colorBrush }
		};

		var style = new Style(typeof(Button))
		{
			Setters =
			{
				new Setter { Property = StyleExtensions.ResourcesProperty, Value = resourceDictionary }
			}
		};

		var button = new Button
		{
			Style = style
		};


		// Act
		await UnitTestUIContentHelperEx.SetContentAndWait(button);

		// Assert
		Assert.AreEqual(button.Resources[testKey], colorBrush);
	}

	[TestMethod]
	public async Task StyleExtensionsResourcesPropertyAppliedToControlTest()
	{
		// Arrange
		var testValue = "TestValue";
		var testKey = "TestKey";

		var button = new Button();

		var resourceDictionary = new ResourceDictionary
		{
			{ testKey, testValue }
		};

		// Act
		StyleExtensions.SetResources(button, resourceDictionary);

		await UnitTestUIContentHelperEx.SetContentAndWait(button);

		// Assert
		Assert.AreEqual(button.Resources[testKey], testValue);
	}

	[TestMethod]
	public async Task StyleExtensionsOldResourcesRemovedWhenDictionaryChangesTest()
	{
		// Arrange
		var initialColorBrush = new SolidColorBrush(Colors.DarkGreen);
		var initialKey = "InitialKey";

		var updatedColorBrush = new SolidColorBrush(Colors.Red);
		var updatedKey = "UpdatedKey";

		var initialResourceDictionary = new ResourceDictionary
		{
			{ initialKey, initialColorBrush }
		};

		var updatedResourceDictionary = new ResourceDictionary
		{
			{ updatedKey, updatedColorBrush }
		};

		var style = new Style(typeof(Button))
		{
			Setters =
			{
				new Setter { Property = StyleExtensions.ResourcesProperty, Value = initialResourceDictionary }
			}
		};

		var button = new Button
		{
			Style = style
		};

		// Act
		// Update the resource dictionary applied to the button
		StyleExtensions.SetResources(button, updatedResourceDictionary);
		await UnitTestUIContentHelperEx.SetContentAndWait(button);

		// Assert
		// The button's resources should now have the updated brush, not the initial one
		Assert.AreEqual(button.Resources[updatedKey], updatedColorBrush);
		Assert.IsFalse(button.Resources.Contains(initialKey));
	}

}
