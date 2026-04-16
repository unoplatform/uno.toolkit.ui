using System.Collections.Generic;
using Windows.UI;

namespace Uno.Toolkit.Samples.Content.Controls;

[SamplePage(SampleCategory.Behaviors, "GridExtensions", SourceSdk.UnoToolkit, SupportedDesigns = new[] { Design.Material, Design.Cupertino, Design.Fluent, Design.Agnostic })]
public sealed partial class GridExtensionsSamplePage : Page
{
	private static readonly Orientation[] DirectionOptions = { Orientation.Horizontal, Orientation.Vertical };
	private static readonly Color[] UnoColors =
	[
		Color.FromArgb(0xFF, 0x22, 0x9D, 0xFC), // #FF229DFC UnoBlue
		Color.FromArgb(0xFF, 0x7A, 0x69, 0xF5), // #FF7A69F5 UnoPurple
		Color.FromArgb(0xFF, 0x6C, 0xE5, 0xAE), // #FF6CE5AE UnoGreen
		Color.FromArgb(0xFF, 0xF6, 0x56, 0x78), // #FFF65678 UnoRed
	];

	private int _counter;

	public GridExtensionsSamplePage()
	{
		this.InitializeComponent();

		DirectionComboBox.ItemsSource = DirectionOptions;

		Reset();
	}

	private void OnAddClick(object sender, RoutedEventArgs e)
	{
		DemoGrid.Children.Add(CreateItem(++_counter));
	}

	private void OnRemoveClick(object sender, RoutedEventArgs e)
	{
		if (DemoGrid.Children.Count > 0)
			DemoGrid.Children.RemoveAt(DemoGrid.Children.Count - 1);
	}

	private void OnResetClick(object sender, RoutedEventArgs e) => Reset();

	private void OnAutoChanged(object sender, RoutedEventArgs e)
	{
		GridExtensions.SetAuto(DemoGrid, AutoCheckBox.IsChecked == true);
	}

	private void OnDirectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (DirectionComboBox.SelectedItem is Orientation direction)
			GridExtensions.SetDirection(DemoGrid, direction);
	}

	private void OnColumnsTextChanged(object sender, TextChangedEventArgs e) => ApplyColumnDefinitions();

	private void OnRowsTextChanged(object sender, TextChangedEventArgs e) => ApplyRowDefinitions();

	private void Reset()
	{
		_counter = 0;
		DemoGrid.Children.Clear();

		// Suspend text-change events by setting Text before hooking definitions
		ColumnsTextBox.Text = "*,*,*";
		RowsTextBox.Text = "Auto,Auto,Auto";
		ApplyColumnDefinitions();
		ApplyRowDefinitions();

		AutoCheckBox.IsChecked = true;
		GridExtensions.SetAuto(DemoGrid, true);

		DirectionComboBox.SelectedItem = Orientation.Horizontal;
		GridExtensions.SetDirection(DemoGrid, Orientation.Horizontal);

		for (var i = 0; i < 4; i++)
			DemoGrid.Children.Add(CreateItem(++_counter));
	}

	private void ApplyColumnDefinitions()
	{
		DemoGrid.ColumnDefinitions.Clear();
		foreach (var length in ParseGridLengths(ColumnsTextBox.Text))
			DemoGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = length });
	}

	private void ApplyRowDefinitions()
	{
		DemoGrid.RowDefinitions.Clear();
		foreach (var length in ParseGridLengths(RowsTextBox.Text))
			DemoGrid.RowDefinitions.Add(new RowDefinition { Height = length });
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
