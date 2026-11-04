using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Toolkit.UI;

namespace Uno.Toolkit.RuntimeTests.Tests;

[TestClass]
public class WasmAppManifestTests
{
	private static Dictionary<string, string> Manifest(params (string Key, string Value)[] entries)
	{
		Dictionary<string, string> manifest = new();
		foreach (var (key, value) in entries)
		{
			manifest[key] = value;
		}

		return manifest;
	}

	[TestMethod]
	public void Parse_Plain_Manifest()
	{
		var manifest = WasmAppManifest.Parse("var UnoAppManifest = {\n\tsplashScreenImage: \"logo.png\",\n\tsplashScreenColor: \"#00FF00\",\n\tdisplayName: \"My App\"\n}");

		Assert.AreEqual(3, manifest.Count);
		Assert.AreEqual("logo.png", manifest["splashScreenImage"]);
		Assert.AreEqual("#00FF00", manifest["splashScreenColor"]);
		Assert.AreEqual("My App", manifest["displayName"]);
	}

	[TestMethod]
	public void Parse_Quoted_Keys_And_Single_Quotes()
	{
		var manifest = WasmAppManifest.Parse("var UnoAppManifest = { \"splashScreenImage\": 'a.png', 'splashScreenColor' : \"#112233\" };");

		Assert.AreEqual("a.png", manifest["splashScreenImage"]);
		Assert.AreEqual("#112233", manifest["splashScreenColor"]);
	}

	[TestMethod]
	public void Parse_Commas_And_Colons_Inside_Values()
	{
		var manifest = WasmAppManifest.Parse("var UnoAppManifest = { displayName: \"Hello, World: the sequel }\", splashScreenImage: \"https://x.test/a,b.png\" }");

		Assert.AreEqual("Hello, World: the sequel }", manifest["displayName"]);
		Assert.AreEqual("https://x.test/a,b.png", manifest["splashScreenImage"]);
	}

	[TestMethod]
	public void Parse_Escaped_Quotes_And_Sequences()
	{
		var manifest = WasmAppManifest.Parse("""var UnoAppManifest = { a: "say \"hi\"", b: 'it\'s', c: "back\\slash", d: "A\u0042\n", e: "x\/y" }""");

		Assert.AreEqual("say \"hi\"", manifest["a"]);
		Assert.AreEqual("it's", manifest["b"]);
		Assert.AreEqual("back\\slash", manifest["c"]);
		Assert.AreEqual("AB\n", manifest["d"]);
		Assert.AreEqual("x/y", manifest["e"]);
	}

	[TestMethod]
	public void Parse_Comments_Are_Ignored()
	{
		var manifest = WasmAppManifest.Parse(@"// generated
var UnoAppManifest = {
	/* the logo, { not a brace */
	splashScreenImage: ""logo.png"", // trailing, comment: here
	splashScreenColor: /* inline */ ""#010203""
}");

		Assert.AreEqual(2, manifest.Count);
		Assert.AreEqual("logo.png", manifest["splashScreenImage"]);
		Assert.AreEqual("#010203", manifest["splashScreenColor"]);
	}

	[TestMethod]
	public void Parse_Trailing_Comma()
	{
		var manifest = WasmAppManifest.Parse("var UnoAppManifest = { a: \"1\", b: \"2\", }");

		Assert.AreEqual(2, manifest.Count);
		Assert.AreEqual("2", manifest["b"]);
	}

	[TestMethod]
	public void Parse_Non_String_Values_Are_Ignored()
	{
		var manifest = WasmAppManifest.Parse("var UnoAppManifest = { n: 42, flag: true, nothing: null, nested: { x: \"no\", y: [1, \"}\", 2] }, list: [\"a\", \"b\"], keep: \"yes\" }");

		Assert.AreEqual(1, manifest.Count);
		Assert.AreEqual("yes", manifest["keep"]);
	}

	[TestMethod]
	public void Parse_Last_Duplicate_Wins()
	{
		var manifest = WasmAppManifest.Parse("var UnoAppManifest = { a: \"1\", a: \"2\" }");

		Assert.AreEqual("2", manifest["a"]);
	}

	[TestMethod]
	[DataRow(null)]
	[DataRow("")]
	[DataRow("   ")]
	[DataRow("not javascript at all")]
	[DataRow("var UnoAppManifest = ")]
	[DataRow("var UnoAppManifest = {")]
	[DataRow("var UnoAppManifest = { a")]
	[DataRow("var UnoAppManifest = { a: ")]
	[DataRow("var UnoAppManifest = { a: \"unterminated")]
	[DataRow("var UnoAppManifest = { /* never closed")]
	[DataRow("var UnoAppManifest = { : : , , }")]
	[DataRow("}{")]
	public void Parse_Garbage_Does_Not_Throw(string? js)
	{
		var manifest = WasmAppManifest.Parse(js);

		Assert.IsNotNull(manifest);
	}

	[TestMethod]
	public void Parse_Truncated_Keeps_Complete_Entries()
	{
		var manifest = WasmAppManifest.Parse("var UnoAppManifest = { a: \"1\", b: \"2\", c: \"cut");

		Assert.AreEqual("1", manifest["a"]);
		Assert.AreEqual("2", manifest["b"]);
		Assert.IsFalse(manifest.ContainsKey("c"));
	}

	[TestMethod]
	public void Select_Per_Theme_Keys_Win_Over_SplashScreenColor()
	{
		var manifest = Manifest(
			("splashScreenColor", "#123456"),
			("lightThemeBackgroundColor", "#AAAAAA"),
			("darkThemeBackgroundColor", "#111111"));

		Assert.AreEqual("#AAAAAA", WasmAppManifest.SelectSplash(manifest, isDark: false).Background);
		Assert.AreEqual("#111111", WasmAppManifest.SelectSplash(manifest, isDark: true).Background);
	}

	[TestMethod]
	public void Select_Only_Light_Key_Dark_Uses_Default()
	{
		var manifest = Manifest(("splashScreenColor", "#123456"), ("lightThemeBackgroundColor", "#AAAAAA"));

		Assert.AreEqual("#AAAAAA", WasmAppManifest.SelectSplash(manifest, isDark: false).Background);
		Assert.AreEqual("#202020", WasmAppManifest.SelectSplash(manifest, isDark: true).Background);
	}

	[TestMethod]
	public void Select_Only_Dark_Key_Light_Uses_Default()
	{
		var manifest = Manifest(("splashScreenColor", "#123456"), ("darkThemeBackgroundColor", "#111111"));

		Assert.AreEqual("#F3F3F3", WasmAppManifest.SelectSplash(manifest, isDark: false).Background);
		Assert.AreEqual("#111111", WasmAppManifest.SelectSplash(manifest, isDark: true).Background);
	}

	[TestMethod]
	public void Select_Only_SplashScreenColor_Applies_To_Both_Themes()
	{
		var manifest = Manifest(("splashScreenColor", "#123456"));

		Assert.AreEqual("#123456", WasmAppManifest.SelectSplash(manifest, isDark: false).Background);
		Assert.AreEqual("#123456", WasmAppManifest.SelectSplash(manifest, isDark: true).Background);
	}

	[TestMethod]
	public void Select_Transparent_Or_Missing_Color_Uses_Theme_Defaults()
	{
		foreach (var manifest in new[] { Manifest(), Manifest(("splashScreenColor", "transparent")), Manifest(("splashScreenColor", "")) })
		{
			Assert.AreEqual("#F3F3F3", WasmAppManifest.SelectSplash(manifest, isDark: false).Background);
			Assert.AreEqual("#202020", WasmAppManifest.SelectSplash(manifest, isDark: true).Background);
		}
	}

	[TestMethod]
	public void Select_Empty_Per_Theme_Value_Counts_As_Absent()
	{
		var manifest = Manifest(("splashScreenColor", "#123456"), ("lightThemeBackgroundColor", ""));

		Assert.AreEqual("#123456", WasmAppManifest.SelectSplash(manifest, isDark: true).Background);
	}

	[TestMethod]
	public void Select_Dark_Image_Falls_Back_To_Light_Image()
	{
		var withDark = Manifest(("splashScreenImage", "light.png"), ("splashScreenImageDark", "dark.png"));
		var withoutDark = Manifest(("splashScreenImage", "light.png"));
		var onlyDark = Manifest(("splashScreenImageDark", "dark.png"));

		Assert.AreEqual("light.png", WasmAppManifest.SelectSplash(withDark, isDark: false).Image);
		Assert.AreEqual("dark.png", WasmAppManifest.SelectSplash(withDark, isDark: true).Image);
		Assert.AreEqual("light.png", WasmAppManifest.SelectSplash(withoutDark, isDark: true).Image);
		Assert.IsNull(WasmAppManifest.SelectSplash(onlyDark, isDark: false).Image);
		Assert.AreEqual("dark.png", WasmAppManifest.SelectSplash(onlyDark, isDark: true).Image);
		Assert.IsNull(WasmAppManifest.SelectSplash(Manifest(), isDark: true).Image);
	}

	[TestMethod]
	public void Default_Backgrounds()
	{
		Assert.AreEqual("#F3F3F3", WasmAppManifest.GetDefaultBackground(isDark: false));
		Assert.AreEqual("#202020", WasmAppManifest.GetDefaultBackground(isDark: true));
	}
}
