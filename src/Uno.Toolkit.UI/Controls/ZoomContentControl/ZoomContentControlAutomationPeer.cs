using System;
using Windows.Foundation;

#if IS_WINUI
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Automation.Provider;
#else
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;
#endif

namespace Uno.Toolkit.UI;

/// <summary>
/// Exposes <see cref="ZoomContentControl"/> to Microsoft UI Automation.
/// </summary>
public partial class ZoomContentControlAutomationPeer : FrameworkElementAutomationPeer, IScrollProvider, ITransformProvider2
{
	private const double MinimumPercent = 0d;
	private const double MaximumPercent = 100d;
	private const double SmallScrollRatio = 0.1d;
	private const double SmallZoomChange = 10d;
	private const double LargeZoomChange = 50d;

	private readonly ZoomContentControl _owner;

	private bool _horizontallyScrollable;
	private bool _verticallyScrollable;
	private double _horizontalScrollPercent;
	private double _verticalScrollPercent;
	private double _horizontalViewSize;
	private double _verticalViewSize;
	private bool _canZoom;
	private double _zoomLevel;
	private double _minZoom;
	private double _maxZoom;

	/// <summary>
	/// Initializes a new instance of the <see cref="ZoomContentControlAutomationPeer"/> class.
	/// </summary>
	/// <param name="owner">The <see cref="ZoomContentControl"/> instance to create the peer for.</param>
	public ZoomContentControlAutomationPeer(ZoomContentControl owner) : base(owner)
	{
		_owner = owner;

		_horizontallyScrollable = HorizontallyScrollable;
		_verticallyScrollable = VerticallyScrollable;
		_horizontalScrollPercent = HorizontalScrollPercent;
		_verticalScrollPercent = VerticalScrollPercent;
		_horizontalViewSize = HorizontalViewSize;
		_verticalViewSize = VerticalViewSize;
		_canZoom = CanZoom;
		_zoomLevel = ZoomLevel;
		_minZoom = MinZoom;
		_maxZoom = MaxZoom;
	}

	protected override string GetClassNameCore() => nameof(ZoomContentControl);

	protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.Pane;

	protected override object? GetPatternCore(PatternInterface patternInterface) =>
		patternInterface is PatternInterface.Scroll or PatternInterface.Transform or PatternInterface.Transform2
			? this
			: base.GetPatternCore(patternInterface);

	/// <inheritdoc />
	public bool HorizontallyScrollable => _owner.IsActive && _owner.IsPanAllowed && GetHorizontalRange() > 0;

	/// <inheritdoc />
	public bool VerticallyScrollable => _owner.IsActive && _owner.IsPanAllowed && GetVerticalRange() > 0;

	/// <inheritdoc />
	public double HorizontalScrollPercent =>
		GetScrollPercent(HorizontallyScrollable, _owner.HorizontalScrollValue, _owner.HorizontalMinScroll, GetHorizontalRange());

	/// <inheritdoc />
	public double VerticalScrollPercent =>
		GetScrollPercent(VerticallyScrollable, _owner.VerticalScrollValue, _owner.VerticalMinScroll, GetVerticalRange());

	/// <inheritdoc />
	public double HorizontalViewSize => GetViewSize(GetHorizontalRange(), _owner.ClippedViewportSize.Width);

	/// <inheritdoc />
	public double VerticalViewSize => GetViewSize(GetVerticalRange(), _owner.ClippedViewportSize.Height);

	/// <inheritdoc />
	public void Scroll(ScrollAmount horizontalAmount, ScrollAmount verticalAmount)
	{
		EnsureEnabled();
		ValidateScrollAmount(horizontalAmount, nameof(horizontalAmount));
		ValidateScrollAmount(verticalAmount, nameof(verticalAmount));

		var scrollHorizontally = horizontalAmount != ScrollAmount.NoAmount;
		var scrollVertically = verticalAmount != ScrollAmount.NoAmount;
		if (!scrollHorizontally && !scrollVertically)
		{
			return;
		}

		if ((scrollHorizontally && !HorizontallyScrollable) ||
			(scrollVertically && !VerticallyScrollable))
		{
			throw new InvalidOperationException("The requested direction cannot be scrolled.");
		}

		var value = _owner.ScrollValue;
		if (scrollHorizontally)
		{
			value.X += GetScrollDelta(horizontalAmount, _owner.ClippedViewportSize.Width);
		}
		if (scrollVertically)
		{
			value.Y += GetScrollDelta(verticalAmount, _owner.ClippedViewportSize.Height);
		}

		_owner.SetScrollValue(value);
	}

	/// <inheritdoc />
	public void SetScrollPercent(double horizontalPercent, double verticalPercent)
	{
		EnsureEnabled();

		var scrollHorizontally = horizontalPercent != ScrollPatternIdentifiers.NoScroll;
		var scrollVertically = verticalPercent != ScrollPatternIdentifiers.NoScroll;
		if (!scrollHorizontally && !scrollVertically)
		{
			return;
		}

		if ((scrollHorizontally && !HorizontallyScrollable) ||
			(scrollVertically && !VerticallyScrollable))
		{
			throw new InvalidOperationException("The requested direction cannot be scrolled.");
		}

		ValidateScrollPercent(horizontalPercent, scrollHorizontally, nameof(horizontalPercent));
		ValidateScrollPercent(verticalPercent, scrollVertically, nameof(verticalPercent));

		var value = _owner.ScrollValue;
		if (scrollHorizontally)
		{
			value.X = _owner.HorizontalMinScroll + (GetHorizontalRange() * horizontalPercent / MaximumPercent);
		}
		if (scrollVertically)
		{
			value.Y = _owner.VerticalMinScroll + (GetVerticalRange() * verticalPercent / MaximumPercent);
		}

		_owner.SetScrollValue(value);
	}

	/// <inheritdoc />
	public bool CanMove => false;

	/// <inheritdoc />
	public bool CanResize => false;

	/// <inheritdoc />
	public bool CanRotate => false;

	/// <inheritdoc />
	public void Move(double x, double y) => ThrowUnsupportedTransformOperation();

	/// <inheritdoc />
	public void Resize(double width, double height) => ThrowUnsupportedTransformOperation();

	/// <inheritdoc />
	public void Rotate(double degrees) => ThrowUnsupportedTransformOperation();

	/// <inheritdoc />
	public bool CanZoom => _owner.IsActive && _owner.IsZoomAllowed;

	/// <inheritdoc />
	public double MaxZoom => _owner.MaxZoomLevel * MaximumPercent;

	/// <inheritdoc />
	public double MinZoom => _owner.MinZoomLevel * MaximumPercent;

	/// <inheritdoc />
	public double ZoomLevel => _owner.ZoomLevel * MaximumPercent;

	/// <inheritdoc />
	public void Zoom(double zoom)
	{
		EnsureEnabled();
		EnsureCanZoom();

		if (!double.IsFinite(zoom))
		{
			throw new ArgumentOutOfRangeException(nameof(zoom));
		}

		_owner.ZoomLevel = Math.Clamp(zoom, MinZoom, MaxZoom) / MaximumPercent;
	}

	/// <inheritdoc />
	public void ZoomByUnit(ZoomUnit zoomUnit)
	{
		EnsureEnabled();

		var change = zoomUnit switch
		{
			ZoomUnit.NoAmount => 0d,
			ZoomUnit.SmallDecrement => -SmallZoomChange,
			ZoomUnit.LargeDecrement => -LargeZoomChange,
			ZoomUnit.SmallIncrement => SmallZoomChange,
			ZoomUnit.LargeIncrement => LargeZoomChange,
			_ => throw new ArgumentOutOfRangeException(nameof(zoomUnit)),
		};

		if (change == 0)
		{
			return;
		}

		EnsureCanZoom();
		Zoom(ZoomLevel + change);
	}

	internal void UpdateScrollPatternProperties()
	{
		UpdateProperty(
			ScrollPatternIdentifiers.HorizontallyScrollableProperty,
			ref _horizontallyScrollable,
			HorizontallyScrollable);
		UpdateProperty(
			ScrollPatternIdentifiers.VerticallyScrollableProperty,
			ref _verticallyScrollable,
			VerticallyScrollable);
		UpdateProperty(
			ScrollPatternIdentifiers.HorizontalViewSizeProperty,
			ref _horizontalViewSize,
			HorizontalViewSize);
		UpdateProperty(
			ScrollPatternIdentifiers.VerticalViewSizeProperty,
			ref _verticalViewSize,
			VerticalViewSize);
		UpdateProperty(
			ScrollPatternIdentifiers.HorizontalScrollPercentProperty,
			ref _horizontalScrollPercent,
			HorizontalScrollPercent);
		UpdateProperty(
			ScrollPatternIdentifiers.VerticalScrollPercentProperty,
			ref _verticalScrollPercent,
			VerticalScrollPercent);
	}

	internal void UpdateTransformPatternProperties()
	{
		UpdateProperty(TransformPattern2Identifiers.CanZoomProperty, ref _canZoom, CanZoom);
		UpdateProperty(TransformPattern2Identifiers.MinZoomProperty, ref _minZoom, MinZoom);
		UpdateProperty(TransformPattern2Identifiers.MaxZoomProperty, ref _maxZoom, MaxZoom);
		UpdateProperty(TransformPattern2Identifiers.ZoomLevelProperty, ref _zoomLevel, ZoomLevel);
	}

	private double GetHorizontalRange() => GetRange(_owner.HorizontalMinScroll, _owner.HorizontalMaxScroll);

	private double GetVerticalRange() => GetRange(_owner.VerticalMinScroll, _owner.VerticalMaxScroll);

	private static double GetRange(double minimum, double maximum)
	{
		var range = maximum - minimum;
		return double.IsFinite(range) && range > 0 ? range : 0;
	}

	private static double GetScrollPercent(bool scrollable, double value, double minimum, double range)
	{
		if (!scrollable || range == 0)
		{
			return ScrollPatternIdentifiers.NoScroll;
		}

		var percent = (value - minimum) * MaximumPercent / range;
		return double.IsFinite(percent)
			? Math.Clamp(percent, MinimumPercent, MaximumPercent)
			: MinimumPercent;
	}

	private static double GetViewSize(double range, double viewport)
	{
		if (range == 0)
		{
			return MaximumPercent;
		}

		viewport = double.IsFinite(viewport) && viewport > 0 ? viewport : 0;
		var extent = viewport + range;
		return extent > 0
			? Math.Clamp(viewport * MaximumPercent / extent, MinimumPercent, MaximumPercent)
			: MaximumPercent;
	}

	private static double GetScrollDelta(ScrollAmount amount, double viewport)
	{
		viewport = double.IsFinite(viewport) && viewport > 0 ? viewport : 0;
		var smallChange = Math.Max(1d, viewport * SmallScrollRatio);
		var largeChange = Math.Max(smallChange, viewport);

		return amount switch
		{
			ScrollAmount.LargeDecrement => -largeChange,
			ScrollAmount.SmallDecrement => -smallChange,
			ScrollAmount.SmallIncrement => smallChange,
			ScrollAmount.LargeIncrement => largeChange,
			_ => 0,
		};
	}

	private static void ValidateScrollAmount(ScrollAmount amount, string parameterName)
	{
		if (amount is not ScrollAmount.NoAmount and
			not ScrollAmount.SmallDecrement and
			not ScrollAmount.LargeDecrement and
			not ScrollAmount.SmallIncrement and
			not ScrollAmount.LargeIncrement)
		{
			throw new ArgumentOutOfRangeException(parameterName);
		}
	}

	private static void ValidateScrollPercent(double percent, bool requested, string parameterName)
	{
		if (requested &&
			(!double.IsFinite(percent) || percent < MinimumPercent || percent > MaximumPercent))
		{
			throw new ArgumentOutOfRangeException(parameterName);
		}
	}

	private void EnsureEnabled()
	{
		if (!IsEnabled())
		{
			throw new ElementNotEnabledException();
		}
	}

	private void EnsureCanZoom()
	{
		if (!CanZoom)
		{
			throw new InvalidOperationException("Zooming is not available.");
		}
	}

	private void ThrowUnsupportedTransformOperation()
	{
		EnsureEnabled();
		throw new InvalidOperationException("Moving, resizing, and rotating are not supported.");
	}

	private void UpdateProperty(AutomationProperty property, ref bool currentValue, bool newValue)
	{
		if (currentValue == newValue)
		{
			return;
		}

		var oldValue = currentValue;
		currentValue = newValue;
		RaisePropertyChangedEvent(property, oldValue, newValue);
	}

	private void UpdateProperty(AutomationProperty property, ref double currentValue, double newValue)
	{
		if (currentValue == newValue)
		{
			return;
		}

		var oldValue = currentValue;
		currentValue = newValue;
		RaisePropertyChangedEvent(property, oldValue, newValue);
	}
}
