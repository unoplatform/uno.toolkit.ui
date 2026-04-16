using System.Collections.Generic;

namespace Uno.Toolkit.Samples.Content.Controls;

[SamplePage(SampleCategory.Behaviors, "AutoGrid", SourceSdk.UnoToolkit, SupportedDesigns = new[] { Design.Material, Design.Cupertino, Design.Fluent, Design.Agnostic })]
public sealed partial class AutoGridSamplePage : Page
{
	private static readonly AutoGridMode[] ModeOptions = { AutoGridMode.None, AutoGridMode.Column, AutoGridMode.Row };
	private static readonly string[] ItemColors = { "#E57373", "#81C784", "#64B5F6", "#FFD54F", "#BA68C8", "#4DB6AC", "#FF8A65", "#A1A1A1" };

	private int _counter;

	public AutoGridSamplePage()
	{
		this.InitializeComponent();

		ModeComboBox.ItemsSource = ModeOptions;

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

	private void OnModeChanged(object sender, SelectionChangedEventArgs e)
	{
		if (ModeComboBox.SelectedItem is AutoGridMode mode)
			AutoGrid.SetMode(DemoGrid, mode);
	}

	private void OnColumnsTextChanged(object sender, TextChangedEventArgs e) => ApplyColumnDefinitions();

	private void OnRowsTextChanged(object sender, TextChangedEventArgs e) => ApplyRowDefinitions();

	private void Reset()
	{
		_counter = 0;
		DemoGrid.Children.Clear();

		// Suspend text-change events by setting Text before hooking definitions
		ColumnsTextBox.Text = "*,*,*";
		RowsTextBox.Text = "Auto,Auto";
		ApplyColumnDefinitions();
		ApplyRowDefinitions();

		ModeComboBox.SelectedItem = AutoGridMode.Column;
		AutoGrid.SetMode(DemoGrid, AutoGridMode.Column);

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
		var color = Windows.UI.ColorHelper.FromArgb(0xFF,
			byte.Parse(ItemColors[(number - 1) % ItemColors.Length][1..3], System.Globalization.NumberStyles.HexNumber),
			byte.Parse(ItemColors[(number - 1) % ItemColors.Length][3..5], System.Globalization.NumberStyles.HexNumber),
			byte.Parse(ItemColors[(number - 1) % ItemColors.Length][5..7], System.Globalization.NumberStyles.HexNumber));

		return new Border
		{
			Background = new SolidColorBrush(color),
			MinHeight = 60,
			MinWidth = 60,
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
