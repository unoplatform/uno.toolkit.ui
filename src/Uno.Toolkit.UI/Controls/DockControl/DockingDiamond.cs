using System;
using Windows.Foundation;
using Uno.UI.Extensions;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
#endif

namespace Uno.Toolkit.UI;

public partial class DockingDiamond : Control
{
	protected DockControl? DockControl => this.FindFirstAncestor<DockControl>();

	private Grid? _rootGrid;
	private Border? _overlayBorder;
	private DockDiamondIndicator? _leftIndicator, _topIndicator, _rightIndicator, _bottomIndicator, _centerIndicator;

	private Rect? _lastPlacementRect;
	internal DockDirection Direction { get; private set; }

	protected override void OnApplyTemplate()
	{
		base.OnApplyTemplate();

		_rootGrid = GetTemplateChild("RootGrid") as Grid;
		_overlayBorder = GetTemplateChild("OverlayBorder") as Border;

		_leftIndicator = GetTemplateChild("DockLeftIndicator") as DockDiamondIndicator;
		_topIndicator = GetTemplateChild("DockTopIndicator") as DockDiamondIndicator;
		_rightIndicator = GetTemplateChild("DockRightIndicator") as DockDiamondIndicator;
		_bottomIndicator = GetTemplateChild("DockBottomIndicator") as DockDiamondIndicator;
		_centerIndicator = GetTemplateChild("DockCenterIndicator") as DockDiamondIndicator;

		RegisterIndicatorEvents(_leftIndicator, DockDirection.Left);
		RegisterIndicatorEvents(_topIndicator, DockDirection.Top);
		RegisterIndicatorEvents(_rightIndicator, DockDirection.Right);
		RegisterIndicatorEvents(_bottomIndicator, DockDirection.Bottom);
		RegisterIndicatorEvents(_centerIndicator, DockDirection.None);

		void RegisterIndicatorEvents(DockDiamondIndicator? indicator, DockDirection direction)
		{
			if (indicator is null) return;
			indicator.Tag = direction;

			// note: avoid using drag events here, since that would cause DragLeave on the DockPane.
			// We can get by with just pointer events here.
			indicator.PointerEntered += OnIndicatorPointerEntered;
			indicator.PointerExited += OnIndicatorPointerExited;
		}
	}

	private void OnIndicatorPointerEntered(object sender, PointerRoutedEventArgs e)
	{
		if (_lastPlacementRect is { } rect)
		{
			var direction = (sender as DockDiamondIndicator)?.Tag as DockDirection? ?? DockDirection.None;

			ShowAt(rect, direction);
		}
	}

	private void OnIndicatorPointerExited(object sender, PointerRoutedEventArgs e)
	{
		if (_lastPlacementRect is { } rect)
		{
			ShowAt(rect, DockDirection.None);
		}
	}

	public void ShowAt(ElementPane pane)
	{
		Visibility = Visibility.Visible;

		var offset = pane.TransformToVisual(this).TransformPoint(default);
		var size = pane.GetActualSize();
		_lastPlacementRect = new Rect(offset, size);

		ShowAt(_lastPlacementRect.Value, DockDirection.None);
	}
	private void ShowAt(Rect rect, DockDirection direction)
	{
		Direction = direction;

		if (_rootGrid is null) return;
		if (_overlayBorder is null) return;

		_rootGrid.ColumnDefinitions[0].Width = new(rect.X);
		_rootGrid.ColumnDefinitions[1].Width = new(rect.Width);
		_rootGrid.RowDefinitions[0].Height = new(rect.Y);
		_rootGrid.RowDefinitions[1].Height = new(rect.Height);

		var overlayRect = ((HorizontalAlignment HAlign, VerticalAlignment VAlign, double Width, double Height))(direction switch
		{
			DockDirection.Left => (HorizontalAlignment.Left, VerticalAlignment.Stretch, rect.Width / 2, double.NaN),
			DockDirection.Right => (HorizontalAlignment.Right, VerticalAlignment.Stretch, rect.Width / 2, double.NaN),
			DockDirection.Top => (HorizontalAlignment.Stretch, VerticalAlignment.Top, double.NaN, rect.Height / 2),
			DockDirection.Bottom => (HorizontalAlignment.Stretch, VerticalAlignment.Bottom, double.NaN, rect.Height / 2),
			DockDirection.None => (HorizontalAlignment.Stretch, VerticalAlignment.Stretch, double.NaN, double.NaN),
#if DEBUG
			_ => throw new ArgumentOutOfRangeException($"DockDirection: {direction}"),
#else
			_ => (HorizontalAlignment.Stretch, VerticalAlignment.Stretch, double.NaN, double.NaN),
#endif
		});

		_overlayBorder.HorizontalAlignment = overlayRect.HAlign;
		_overlayBorder.VerticalAlignment = overlayRect.VAlign;
		_overlayBorder.Width = overlayRect.Width;
		_overlayBorder.Height = overlayRect.Height;
	}

	public void Hide()
	{
		Visibility = Visibility.Collapsed;

		_lastPlacementRect = null;
	}
}
public partial class DockDiamondIndicator : Control
{
	#region DependencyProperty: Data

	public static DependencyProperty DataProperty { get; } = DependencyProperty.Register(
		nameof(Data),
		typeof(Geometry),
		typeof(DockDiamondIndicator),
		new PropertyMetadata(default(Geometry)));

	public Geometry Data
	{
		get => (Geometry)GetValue(DataProperty);
		set => SetValue(DataProperty, value);
	}

	#endregion

	public DockDiamondIndicator()
	{
	}
}
