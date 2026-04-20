using System.Collections.Generic;
using Windows.UI;

namespace Uno.Toolkit.Samples.Content.Controls;

[SamplePage(SampleCategory.Behaviors, "GridExtensions", SourceSdk.UnoToolkit,
	Description = "Provides attached properties that automatically assign Grid.Row and Grid.Column to children based on their order.",
	SupportedDesigns = new[] { Design.Material, Design.Cupertino, Design.Fluent, Design.Agnostic })]
public sealed partial class GridExtensionsSamplePage : Page
{
	private static readonly Color[] UnoColors =
	[
		Color.FromArgb(0xFF, 0x22, 0x9D, 0xFC), // #FF229DFC UnoBlue
		Color.FromArgb(0xFF, 0x7A, 0x69, 0xF5), // #FF7A69F5 UnoPurple
		Color.FromArgb(0xFF, 0x6C, 0xE5, 0xAE), // #FF6CE5AE UnoGreen
		Color.FromArgb(0xFF, 0xF6, 0x56, 0x78), // #FFF65678 UnoRed
	];

	public GridExtensionsSamplePage()
	{
		this.InitializeComponent();
		this.Loaded += (s, e) => SetupOptions();
	}

	private void SetupOptions()
	{
		var design = Design.Agnostic;

		var demoGrid = SamplePageLayout.GetSampleChild<Grid>(design, "DemoGrid");
		var optionAuto = SamplePageLayout.GetSampleChild<CheckBox>(design, "OptionAuto");
		var optionColumns = SamplePageLayout.GetSampleChild<TextBox>(design, "OptionColumns");
		var optionRows = SamplePageLayout.GetSampleChild<TextBox>(design, "OptionRows");
		var optionAdd = SamplePageLayout.GetSampleChild<Button>(design, "OptionAdd");
		var optionRemove = SamplePageLayout.GetSampleChild<Button>(design, "OptionRemove");
		var optionReset = SamplePageLayout.GetSampleChild<Button>(design, "OptionReset");

		var counter = 0;

		optionAuto.Click += (s, e) => GridExtensions.SetAuto(demoGrid, optionAuto.IsChecked == true);
		optionColumns.TextChanged += (s, e) => ApplyColumnDefinitions();
		optionRows.TextChanged += (s, e) => ApplyRowDefinitions();
		optionAdd.Click += (s, e) => demoGrid.Children.Add(CreateItem(++counter));
		optionRemove.Click += (s, e) =>
		{
			if (demoGrid.Children.Count > 0)
				demoGrid.Children.RemoveAt(demoGrid.Children.Count - 1);
		};
		optionReset.Click += (s, e) => Reset();

		Reset();

		void ApplyColumnDefinitions()
		{
			demoGrid.ColumnDefinitions.Clear();
			foreach (var length in ParseGridLengths(optionColumns.Text))
				demoGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = length });
		}

		void ApplyRowDefinitions()
		{
			demoGrid.RowDefinitions.Clear();
			foreach (var length in ParseGridLengths(optionRows.Text))
				demoGrid.RowDefinitions.Add(new RowDefinition { Height = length });
		}

		void Reset()
		{
			counter = 0;
			demoGrid.Children.Clear();

			optionColumns.Text = "*,*,*";
			optionRows.Text = "Auto,Auto,Auto";
			ApplyColumnDefinitions();
			ApplyRowDefinitions();

			optionAuto.IsChecked = true;
			GridExtensions.SetAuto(demoGrid, true);

			for (var i = 0; i < 4; i++)
				demoGrid.Children.Add(CreateItem(++counter));
		}
	}

	private static IEnumerable<GridLength> ParseGridLengths(string input)
	{
		if (string.IsNullOrWhiteSpace(input)) yield break;

		foreach (var token in input.Split(','))
		{
			var t = token.Trim();
			if (string.Equals(t, "Auto", StringComparison.OrdinalIgnoreCase))
			{
				yield return GridLength.Auto;
			}
			else if (t.EndsWith('*'))
			{
				var factor = t.Length == 1 ? 1d : double.TryParse(t[..^1], out var v) ? v : 1d;
				yield return new GridLength(factor, GridUnitType.Star);
			}
			else if (double.TryParse(t, out var px))
			{
				yield return new GridLength(px, GridUnitType.Pixel);
			}
		}
	}

	private static UIElement CreateItem(int number)
	{
		var color = UnoColors[(number - 1) % UnoColors.Length];

		return new Border
		{
			Background = new SolidColorBrush(color),
			MinHeight = 50,
			MinWidth = 50,
			Margin = new Thickness(2),
			Child = new TextBlock
			{
				Text = $"Item {number}",
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				Foreground = new SolidColorBrush(Colors.White),
			},
		};
	}
}
