using System;
using Windows.Foundation;
using Uno.UI.Extensions;
using System.Diagnostics;


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
	private static class OuterStates
	{
		public const string OuterNonAvailableState = nameof(OuterNonAvailableState);
		public const string OuterAvailableState = nameof(OuterAvailableState);
	}
	private static class DirectionalStates
	{
		public const string Omnidirectional = nameof(Omnidirectional);
		public const string HBidirectional = nameof(HBidirectional);
		public const string VBidirectional = nameof(VBidirectional);
	}

	protected DockControl? DockControl => this.FindFirstAncestor<DockControl>();

	private Grid? _rootGrid;
	private Border? _overlayBorder;

	private DockDiamondIndicator? _centerIndicator;
	private DockDiamondIndicator? _leftIndicator, _topIndicator, _rightIndicator, _bottomIndicator;
	private DockDiamondIndicator? _outerLeftIndicator, _outerTopIndicator, _outerRightIndicator, _outerBottomIndicator;
	//private DockDiamondIndicator? _edgeLeftIndicator, _edgeTopIndicator, _edgeRightIndicator, _edgeBottomIndicator;

	private Rect? _lastPlacementRect;
	internal DockDirection Direction { get; private set; }

	protected override void OnApplyTemplate()
	{
		base.OnApplyTemplate();

		_rootGrid = GetTemplateChild("RootGrid") as Grid;
		_overlayBorder = GetTemplateChild("OverlayBorder") as Border;

		ResolveAndRegisterIndicator(ref _centerIndicator, "DockCenterIndicator", DockDirection.None);

		ResolveAndRegisterIndicator(ref _leftIndicator, "DockLeftIndicator", DockDirection.Left);
		ResolveAndRegisterIndicator(ref _topIndicator, "DockTopIndicator", DockDirection.Top);
		ResolveAndRegisterIndicator(ref _rightIndicator, "DockRightIndicator", DockDirection.Right);
		ResolveAndRegisterIndicator(ref _bottomIndicator, "DockBottomIndicator", DockDirection.Bottom);

		ResolveAndRegisterIndicator(ref _outerLeftIndicator, "DockOuterLeftIndicator", DockDirection.OuterLeft);
		ResolveAndRegisterIndicator(ref _outerTopIndicator, "DockOuterTopIndicator", DockDirection.OuterTop);
		ResolveAndRegisterIndicator(ref _outerRightIndicator, "DockOuterRightIndicator", DockDirection.OuterRight);
		ResolveAndRegisterIndicator(ref _outerBottomIndicator, "DockOuterBottomIndicator", DockDirection.OuterBottom);

		//ResolveAndRegisterIndicator(ref _edgeLeftIndicator, "DockEdgeLeftIndicator", DockDirection.EdgeLeft);
		//ResolveAndRegisterIndicator(ref _edgeTopIndicator, "DockEdgeTopIndicator", DockDirection.EdgeTop);
		//ResolveAndRegisterIndicator(ref _edgeRightIndicator, "DockEdgeRightIndicator", DockDirection.EdgeRight);
		//ResolveAndRegisterIndicator(ref _edgeBottomIndicator, "DockEdgeBottomIndicator", DockDirection.EdgeBottom);

		void ResolveAndRegisterIndicator(ref DockDiamondIndicator? indicator, string partName, DockDirection direction)
		{
			indicator = GetTemplateChild(partName) as DockDiamondIndicator;
			if (indicator is { })
			{
				indicator.Tag = direction;

				// note: avoid using drag events here, since that would cause DragLeave on the DockPane.
				// We can get by with just pointer events here.
				indicator.PointerEntered += OnIndicatorPointerEntered;
				indicator.PointerExited += OnIndicatorPointerExited;
			}
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

	public void ShowAt(ElementPane pane, DockItem? item)
	{
		Visibility = Visibility.Visible;

		var offset = pane.TransformToVisual(this).TransformPoint(default);
		var size = pane.GetActualSize();
		_lastPlacementRect = new Rect(offset, size);

		var directionalState = pane.ParentPane switch
		{
			EditorPane { IsOrientationLocked: true, Orientation: Orientation.Horizontal } => DirectionalStates.HBidirectional,
			EditorPane { IsOrientationLocked: true, Orientation: Orientation.Vertical } => DirectionalStates.VBidirectional,

			_ => DirectionalStates.Omnidirectional,
		};
		var outerState = pane is DocumentPane && item is ToolItem
			? OuterStates.OuterAvailableState
			: OuterStates.OuterNonAvailableState;

		VisualStateManager.GoToState(this, directionalState, useTransitions: IsLoaded);
		VisualStateManager.GoToState(this, outerState, useTransitions: IsLoaded);

		ShowAt(_lastPlacementRect.Value, DockDirection.None);
	}
	private void ShowAt(Rect rect, DockDirection direction)
	{
		Direction = direction;
		Debug.WriteLine($"{direction}");

		if (_rootGrid is null) return;
		if (_overlayBorder is null) return;

		_rootGrid.ColumnDefinitions[0].Width = new(rect.X);
		_rootGrid.ColumnDefinitions[1].Width = new(rect.Width);
		_rootGrid.RowDefinitions[0].Height = new(rect.Y);
		_rootGrid.RowDefinitions[1].Height = new(rect.Height);

		var overlayRect = ((HorizontalAlignment HAlign, VerticalAlignment VAlign, double Width, double Height))(direction switch
		{
			DockDirection.None => (HorizontalAlignment.Stretch, VerticalAlignment.Stretch, double.NaN, double.NaN),

			DockDirection.Left => (HorizontalAlignment.Left, VerticalAlignment.Stretch, rect.Width / 2, double.NaN),
			DockDirection.Right => (HorizontalAlignment.Right, VerticalAlignment.Stretch, rect.Width / 2, double.NaN),
			DockDirection.Top => (HorizontalAlignment.Stretch, VerticalAlignment.Top, double.NaN, rect.Height / 2),
			DockDirection.Bottom => (HorizontalAlignment.Stretch, VerticalAlignment.Bottom, double.NaN, rect.Height / 2),

			DockDirection.OuterLeft => (HorizontalAlignment.Left, VerticalAlignment.Stretch, rect.Width / 2, double.NaN),
			DockDirection.OuterRight => (HorizontalAlignment.Right, VerticalAlignment.Stretch, rect.Width / 2, double.NaN),
			DockDirection.OuterTop => (HorizontalAlignment.Stretch, VerticalAlignment.Top, double.NaN, rect.Height / 2),
			DockDirection.OuterBottom => (HorizontalAlignment.Stretch, VerticalAlignment.Bottom, double.NaN, rect.Height / 2),

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
