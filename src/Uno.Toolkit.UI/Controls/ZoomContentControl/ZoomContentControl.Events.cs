using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Uno.Toolkit.UI;

partial class ZoomContentControl // events
{
#if DEBUG
	public event EventHandler? StateChanged;
#endif

	public event EventHandler? RenderedContentUpdated;
	public event ZoomLevelChangedEventHandler? ZoomLevelChanged;
	public event EventHandler? IsActiveChanged;
	public event EventHandler? ContentSizeChanged;
	public event EventHandler? ViewportSizeChanged;

	[Conditional("DEBUG")]
	[SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "It does access instance members, conditionally.")]
	private void NotifyStateChanged()
	{
#if DEBUG
		StateChanged?.Invoke(this, EventArgs.Empty);
#endif
	}

	public delegate void ZoomLevelChangedEventHandler(object sender, ZoomLevelChangedEventArgs e);

	public class ZoomLevelChangedEventArgs : EventArgs
	{
		public double OldZoomLevel { get; }
		public double NewZoomLevel { get; }
		public bool FromMouseWheelPanning { get; }

		public ZoomLevelChangedEventArgs(double oldZoomLevel, double newZoomLevel, bool fromMouseWheelPanning)
		{
			OldZoomLevel = oldZoomLevel;
			NewZoomLevel = newZoomLevel;
			FromMouseWheelPanning = fromMouseWheelPanning;
		}
	}
}
