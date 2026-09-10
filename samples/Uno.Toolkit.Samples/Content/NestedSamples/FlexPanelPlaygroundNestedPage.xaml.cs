using System.Globalization;
using System.Text;
using Microsoft.UI.Xaml.Controls.Primitives;
using Windows.UI;

namespace Uno.Toolkit.Samples.Content.NestedSamples;

/// <summary>
/// Interactive surface covering every FlexPanel container and per-child property.
/// </summary>
/// <remarks>
/// Reached from <c>FlexPanelSamplePage</c> through <c>Shell.ShowNestedSample</c>. It deliberately
/// carries no <c>SamplePageAttribute</c>: nested pages are launched by their parent sample rather
/// than listed in the navigation, which also means this page behaves identically in Debug and
/// Release.
/// </remarks>
public sealed partial class FlexPanelPlaygroundNestedPage // constants, choices-values
{
	// Only the members that produce a distinct layout are offered. FlexJustify and FlexAlign both
	// carry extra members for engine parity -- FlexJustify.Auto/Stretch/Start collapse onto
	// FlexStart, and End onto FlexEnd -- so binding the raw Enum.GetValues would fill these
	// pickers with no-op entries.
	private static readonly FlexDirection[] Directions =
	{
		FlexDirection.Row, FlexDirection.RowReverse, FlexDirection.Column, FlexDirection.ColumnReverse,
	};

	private static readonly FlexWrap[] Wraps =
	{
		FlexWrap.NoWrap, FlexWrap.Wrap, FlexWrap.WrapReverse,
	};

	private static readonly FlexJustify[] Justifications =
	{
		FlexJustify.FlexStart, FlexJustify.Center, FlexJustify.FlexEnd,
		FlexJustify.SpaceBetween, FlexJustify.SpaceAround, FlexJustify.SpaceEvenly,
	};

	// Baseline is meaningful for align-items / align-self, but not for align-content.
	private static readonly FlexAlign[] ItemAlignments =
	{
		FlexAlign.Stretch, FlexAlign.FlexStart, FlexAlign.Center, FlexAlign.FlexEnd, FlexAlign.Baseline,
	};

	private static readonly FlexAlign[] ContentAlignments =
	{
		FlexAlign.FlexStart, FlexAlign.Center, FlexAlign.FlexEnd, FlexAlign.Stretch,
		FlexAlign.SpaceBetween, FlexAlign.SpaceAround, FlexAlign.SpaceEvenly,
	};

	// Auto defers to the panel's AlignItems, so it belongs in the align-self picker.
	private static readonly FlexAlign[] SelfAlignments =
	{
		FlexAlign.Auto, FlexAlign.Stretch, FlexAlign.FlexStart, FlexAlign.Center,
		FlexAlign.FlexEnd, FlexAlign.Baseline,
	};

	private static readonly FlexLayoutDirection[] LayoutDirections =
	{
		FlexLayoutDirection.LeftToRight, FlexLayoutDirection.RightToLeft,
	};

	private static readonly FlexPositionType[] Positions =
	{
		FlexPositionType.Relative, FlexPositionType.Absolute, FlexPositionType.Static,
	};

	private static readonly Color[] SwatchColors =
	{
		Color.FromArgb(0xFF, 0x5B, 0x8D, 0xEF),
		Color.FromArgb(0xFF, 0x67, 0xE5, 0xAD),
		Color.FromArgb(0xFF, 0xF2, 0x99, 0x4A),
		Color.FromArgb(0xFF, 0xE5, 0x7A, 0xB5),
		Color.FromArgb(0xFF, 0xB0, 0x8D, 0xEF),
	};

	private const int DefaultChildCount = 5;
	private const int StressChildCount = 200;
}

public sealed partial class FlexPanelPlaygroundNestedPage : Page
{
	// Guards the option controls while they are being populated or pushed back from the model,
	// so a programmatic SelectedItem / Value assignment does not re-enter as a user edit.
	private bool _isSyncing;

	public FlexPanelPlaygroundNestedPage()
	{
		this.InitializeComponent();

		Loaded += OnLoaded;
	}

	private void OnLoaded(object sender, RoutedEventArgs e)
	{
		_isSyncing = true;
		try
		{
			PopulateCombo(DirectionCombo, Directions, FlexDirection.Row);
			PopulateCombo(WrapCombo, Wraps, FlexWrap.Wrap);
			PopulateCombo(JustifyCombo, Justifications, FlexJustify.FlexStart);
			PopulateCombo(AlignItemsCombo, ItemAlignments, FlexAlign.Stretch);
			PopulateCombo(AlignContentCombo, ContentAlignments, FlexAlign.FlexStart);
			PopulateCombo(LayoutDirectionCombo, LayoutDirections, FlexLayoutDirection.LeftToRight);
			PopulateCombo(AlignSelfCombo, SelfAlignments, FlexAlign.Auto);
			PopulateCombo(PositionCombo, Positions, FlexPositionType.Relative);
		}
		finally
		{
			_isSyncing = false;
		}

		ResetChildren();
		ApplyContainerOptions();
		ApplyStageSize();
	}

	private void ExitNestedSample(object sender, RoutedEventArgs e)
	{
		Shell.GetForCurrentView()?.BackNavigateFromNestedSample();
	}

	private static void PopulateCombo<T>(ComboBox combo, T[] values, T selected)
	{
		// Boxed enum values render through ToString(), which is exactly the member name we want.
		var items = new List<object>(values.Length);
		foreach (var value in values)
		{
			items.Add(value!);
		}

		combo.ItemsSource = items;
		combo.SelectedItem = selected;
	}
}

partial class FlexPanelPlaygroundNestedPage // container
{
	private void OnContainerOptionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (_isSyncing)
		{
			return;
		}

		ApplyContainerOptions();
	}

	private void OnContainerSliderChanged(object sender, RangeBaseValueChangedEventArgs e)
	{
		if (_isSyncing || Stage is null)
		{
			return;
		}

		ApplyContainerOptions();
	}

	private void ApplyContainerOptions()
	{
		if (Stage is null)
		{
			return;
		}

		if (DirectionCombo.SelectedItem is FlexDirection direction)
		{
			Stage.Direction = direction;
		}

		if (WrapCombo.SelectedItem is FlexWrap wrap)
		{
			Stage.Wrap = wrap;
		}

		if (JustifyCombo.SelectedItem is FlexJustify justify)
		{
			Stage.JustifyContent = justify;
		}

		if (AlignItemsCombo.SelectedItem is FlexAlign alignItems)
		{
			Stage.AlignItems = alignItems;
		}

		if (AlignContentCombo.SelectedItem is FlexAlign alignContent)
		{
			Stage.AlignContent = alignContent;
		}

		if (LayoutDirectionCombo.SelectedItem is FlexLayoutDirection layoutDirection)
		{
			Stage.LayoutDirection = layoutDirection;
		}

		Stage.ColumnGap = ColumnGapSlider.Value;
		Stage.RowGap = RowGapSlider.Value;
		Stage.Padding = new Thickness(
			PaddingLeftSlider.Value,
			PaddingTopSlider.Value,
			PaddingRightSlider.Value,
			PaddingBottomSlider.Value);

		ColumnGapLabel.Text = FormatLabel("ColumnGap", ColumnGapSlider.Value);
		RowGapLabel.Text = FormatLabel("RowGap", RowGapSlider.Value);
		PaddingLabel.Text = "Padding " + Stage.Padding.ToString();

		QueueReadout();
	}

	private static string FormatLabel(string name, double value)
		=> string.Create(CultureInfo.InvariantCulture, $"{name} = {value:0}");
}

partial class FlexPanelPlaygroundNestedPage // host size and rounding
{
	private void OnStageSizeOptionChanged(object sender, RoutedEventArgs e) => ApplyStageSize();

	private void OnStageSizeSliderChanged(object sender, RangeBaseValueChangedEventArgs e) => ApplyStageSize();

	private void ApplyStageSize()
	{
		if (StageHost is null)
		{
			return;
		}

		StageHost.Width = StageAutoWidthCheck.IsChecked == true
			? double.NaN
			: StageWidthSlider.Value;
		StageHost.Height = StageAutoHeightCheck.IsChecked == true
			? double.NaN
			: StageHeightSlider.Value;

		StageWidthSlider.IsEnabled = StageAutoWidthCheck.IsChecked != true;
		StageHeightSlider.IsEnabled = StageAutoHeightCheck.IsChecked != true;

		QueueReadout();
	}

	private void OnRoundingChanged(object sender, RoutedEventArgs e)
	{
		if (Stage is null)
		{
			return;
		}

		// FR-6: UseLayoutRounding=false makes the engine emit unrounded values.
		Stage.UseLayoutRounding = UseLayoutRoundingCheck.IsChecked == true;
		Stage.InvalidateMeasure();
		QueueReadout();
	}

	private void OnFlowDirectionChanged(object sender, RoutedEventArgs e)
	{
		if (Stage is null)
		{
			return;
		}

		// FlexPanel ignores FlowDirection; this exists to make the double-mirror visible.
		Stage.FlowDirection = FlowDirectionRtlCheck.IsChecked == true
			? FlowDirection.RightToLeft
			: FlowDirection.LeftToRight;
		QueueReadout();
	}
}

partial class FlexPanelPlaygroundNestedPage // children
{
	private int _nextChildNumber;

	private void OnAddChild(object sender, RoutedEventArgs e)
	{
		Stage.Children.Add(CreateChild(_nextChildNumber++));
		RefreshChildSelector();
		QueueReadout();
	}

	private void OnRemoveChild(object sender, RoutedEventArgs e)
	{
		if (Stage.Children.Count == 0)
		{
			return;
		}

		Stage.Children.RemoveAt(Stage.Children.Count - 1);
		RefreshChildSelector();
		QueueReadout();
	}

	private void OnResetChildren(object sender, RoutedEventArgs e)
	{
		if (StressCheck.IsChecked == true)
		{
			_isSyncing = true;
			try
			{
				StressCheck.IsChecked = false;
			}
			finally
			{
				_isSyncing = false;
			}
		}

		ResetChildren();
	}

	private void OnStressChanged(object sender, RoutedEventArgs e)
	{
		if (_isSyncing)
		{
			return;
		}

		ResetChildren();
	}

	private void ResetChildren()
	{
		// Release before allocate: drop the previous graph before building the replacement, so a
		// 200-child run does not hold two full sets alive at once.
		Stage.Children.Clear();
		_nextChildNumber = 0;

		var count = StressCheck.IsChecked == true ? StressChildCount : DefaultChildCount;
		for (var i = 0; i < count; i++)
		{
			Stage.Children.Add(CreateChild(_nextChildNumber++));
		}

		RefreshChildSelector();
		QueueReadout();
	}

	private static Border CreateChild(int index)
	{
		return new Border
		{
			Background = new SolidColorBrush(SwatchColors[index % SwatchColors.Length]),
			Padding = new Thickness(12, 8, 12, 8),
			CornerRadius = new CornerRadius(4),
			Child = new TextBlock
			{
				Text = "Item " + index.ToString(CultureInfo.InvariantCulture),
				FontSize = 13,
				Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0x10, 0x14, 0x18)),
			},
		};
	}

	private void RefreshChildSelector()
	{
		_isSyncing = true;
		try
		{
			var items = new List<object>(Stage.Children.Count);
			for (var i = 0; i < Stage.Children.Count; i++)
			{
				items.Add("Item " + i.ToString(CultureInfo.InvariantCulture));
			}

			ChildSelector.ItemsSource = items;
			if (items.Count > 0)
			{
				ChildSelector.SelectedIndex = 0;
			}
		}
		finally
		{
			_isSyncing = false;
		}

		PullSelectedChildIntoEditor();
	}

	private UIElement? SelectedChild
	{
		get
		{
			var index = ChildSelector.SelectedIndex;
			return index >= 0 && index < Stage.Children.Count
				? Stage.Children[index]
				: null;
		}
	}

	private void OnSelectedChildChanged(object sender, SelectionChangedEventArgs e)
	{
		if (_isSyncing)
		{
			return;
		}

		PullSelectedChildIntoEditor();
	}

	/// <summary>
	/// Loads the selected child's current values into the editor controls, so switching selection
	/// shows that child's state rather than overwriting it with whatever was last edited.
	/// </summary>
	private void PullSelectedChildIntoEditor()
	{
		if (SelectedChild is not FrameworkElement child)
		{
			return;
		}

		_isSyncing = true;
		try
		{
			GrowSlider.Value = FlexPanel.GetGrow(child);
			ShrinkSlider.Value = FlexPanel.GetShrink(child);

			var basis = FlexPanel.GetBasis(child);
			BasisAutoCheck.IsChecked = double.IsNaN(basis);
			BasisSlider.Value = double.IsNaN(basis) ? 0 : basis;

			AlignSelfCombo.SelectedItem = FlexPanel.GetAlignSelf(child);
			PositionCombo.SelectedItem = FlexPanel.GetPosition(child);

			var minWidth = FlexPanel.GetFlexMinWidth(child);
			FlexMinWidthAutoCheck.IsChecked = double.IsNaN(minWidth);
			FlexMinWidthSlider.Value = double.IsNaN(minWidth) ? 0 : minWidth;

			var minHeight = FlexPanel.GetFlexMinHeight(child);
			FlexMinHeightAutoCheck.IsChecked = double.IsNaN(minHeight);
			FlexMinHeightSlider.Value = double.IsNaN(minHeight) ? 0 : minHeight;

			var left = FlexPanel.GetLeft(child);
			InsetsEnabledCheck.IsChecked = !double.IsNaN(left);
			LeftSlider.Value = double.IsNaN(left) ? 0 : left;
			var top = FlexPanel.GetTop(child);
			TopSlider.Value = double.IsNaN(top) ? 0 : top;
			var right = FlexPanel.GetRight(child);
			RightSlider.Value = double.IsNaN(right) ? 0 : right;
			var bottom = FlexPanel.GetBottom(child);
			BottomSlider.Value = double.IsNaN(bottom) ? 0 : bottom;

			WidthAutoCheck.IsChecked = double.IsNaN(child.Width);
			if (!double.IsNaN(child.Width))
			{
				WidthSlider.Value = child.Width;
			}

			HeightAutoCheck.IsChecked = double.IsNaN(child.Height);
			if (!double.IsNaN(child.Height))
			{
				HeightSlider.Value = child.Height;
			}

			MarginSlider.Value = child.Margin.Left;
			ChildVisibleCheck.IsChecked = child.Visibility == Visibility.Visible;
		}
		finally
		{
			_isSyncing = false;
		}

		UpdateChildEditorEnabledState();
		QueueReadout();
	}

	private void OnChildOptionChanged(object sender, RoutedEventArgs e)
	{
		if (_isSyncing)
		{
			return;
		}

		ApplyChildOptions();
	}

	private void OnChildComboChanged(object sender, SelectionChangedEventArgs e)
	{
		if (_isSyncing)
		{
			return;
		}

		ApplyChildOptions();
	}

	private void OnChildSliderChanged(object sender, RangeBaseValueChangedEventArgs e)
	{
		if (_isSyncing || Stage is null)
		{
			return;
		}

		ApplyChildOptions();
	}

	private void ApplyChildOptions()
	{
		if (SelectedChild is not FrameworkElement child)
		{
			return;
		}

		FlexPanel.SetGrow(child, GrowSlider.Value);
		FlexPanel.SetShrink(child, ShrinkSlider.Value);
		FlexPanel.SetBasis(child, BasisAutoCheck.IsChecked == true ? double.NaN : BasisSlider.Value);

		if (AlignSelfCombo.SelectedItem is FlexAlign alignSelf)
		{
			FlexPanel.SetAlignSelf(child, alignSelf);
		}

		if (PositionCombo.SelectedItem is FlexPositionType position)
		{
			FlexPanel.SetPosition(child, position);
		}

		FlexPanel.SetFlexMinWidth(child, FlexMinWidthAutoCheck.IsChecked == true ? double.NaN : FlexMinWidthSlider.Value);
		FlexPanel.SetFlexMinHeight(child, FlexMinHeightAutoCheck.IsChecked == true ? double.NaN : FlexMinHeightSlider.Value);

		var insetsOn = InsetsEnabledCheck.IsChecked == true;
		FlexPanel.SetLeft(child, insetsOn ? LeftSlider.Value : double.NaN);
		FlexPanel.SetTop(child, insetsOn ? TopSlider.Value : double.NaN);
		FlexPanel.SetRight(child, insetsOn ? RightSlider.Value : double.NaN);
		FlexPanel.SetBottom(child, insetsOn ? BottomSlider.Value : double.NaN);

		child.Width = WidthAutoCheck.IsChecked == true ? double.NaN : WidthSlider.Value;
		child.Height = HeightAutoCheck.IsChecked == true ? double.NaN : HeightSlider.Value;
		child.Margin = new Thickness(MarginSlider.Value);
		child.Visibility = ChildVisibleCheck.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;

		UpdateChildEditorEnabledState();
		QueueReadout();
	}

	private void UpdateChildEditorEnabledState()
	{
		BasisSlider.IsEnabled = BasisAutoCheck.IsChecked != true;
		FlexMinWidthSlider.IsEnabled = FlexMinWidthAutoCheck.IsChecked != true;
		FlexMinHeightSlider.IsEnabled = FlexMinHeightAutoCheck.IsChecked != true;
		WidthSlider.IsEnabled = WidthAutoCheck.IsChecked != true;
		HeightSlider.IsEnabled = HeightAutoCheck.IsChecked != true;

		var insetsOn = InsetsEnabledCheck.IsChecked == true;
		LeftSlider.IsEnabled = insetsOn;
		TopSlider.IsEnabled = insetsOn;
		RightSlider.IsEnabled = insetsOn;
		BottomSlider.IsEnabled = insetsOn;
	}
}

partial class FlexPanelPlaygroundNestedPage // readout
{
	private bool _readoutQueued;

	/// <summary>
	/// Defers the arranged-rect readout to after the next layout pass, since the values only
	/// become meaningful once the panel has actually arranged.
	/// </summary>
	private void QueueReadout()
	{
		if (_readoutQueued)
		{
			return;
		}

		_readoutQueued = true;
		Stage.LayoutUpdated += OnStageLayoutUpdated;
	}

	private void OnStageLayoutUpdated(object? sender, object e)
	{
		Stage.LayoutUpdated -= OnStageLayoutUpdated;
		_readoutQueued = false;
		UpdateReadout();
	}

	private void UpdateReadout()
	{
		var builder = new StringBuilder();
		builder.Append(string.Create(
			CultureInfo.InvariantCulture,
			$"panel  {Stage.ActualWidth:0.##} x {Stage.ActualHeight:0.##}   children={Stage.Children.Count}   rounding={(Stage.UseLayoutRounding ? "on" : "off")}"));

		// Listing 200 rows would dominate the panel and cost more than it tells us.
		var limit = Math.Min(Stage.Children.Count, 12);
		for (var i = 0; i < limit; i++)
		{
			if (Stage.Children[i] is not FrameworkElement child)
			{
				continue;
			}

			var offset = child.TransformToVisual(Stage).TransformPoint(default);
			builder.AppendLine();
			builder.Append(string.Create(
				CultureInfo.InvariantCulture,
				$"[{i,3}]  x={offset.X,8:0.##}  y={offset.Y,8:0.##}  w={child.ActualWidth,8:0.##}  h={child.ActualHeight,8:0.##}"));
		}

		if (Stage.Children.Count > limit)
		{
			builder.AppendLine();
			builder.Append(string.Create(
				CultureInfo.InvariantCulture,
				$"... {Stage.Children.Count - limit} more"));
		}

		Readout.Text = builder.ToString();
	}
}
