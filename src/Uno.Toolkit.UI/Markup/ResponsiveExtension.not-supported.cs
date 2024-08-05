#if WINDOWS_UWP
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Uno.Extensions;
using Uno.Logging;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
#endif

namespace Uno.Toolkit.UI;

public partial class ResponsiveExtension : MarkupExtension
{
	private static readonly ILogger _logger = typeof(ResponsiveExtension).Log();

	public object? Narrowest { get; set; }
	public object? Narrow { get; set; }
	public object? Normal { get; set; }
	public object? Wide { get; set; }
	public object? Widest { get; set; }

	public ResponsiveLayout? Layout { get; set; }

	/// <inheritdoc/>
	protected override object? ProvideValue()
	{
		_logger.WarnIfEnabled(() => "This xaml markup extension is not supported on UWP. Consider upgrading to WinUI.");

		// we don't want to throw here, since that will just crash the xaml page/control being parsed.
		return null;
	}

	[SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "blank method needed for uwp build")]
	internal void Connect(FrameworkElement proxyHost) => throw new PlatformNotSupportedException();

	[SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "blank method needed for uwp build")]
	internal void Disconnect() => throw new PlatformNotSupportedException();
}
public partial class ResponsiveExtension
{
	internal static List<(WeakReference Owner, string Property, WeakReference Extension)> TrackedInstances { get; } = new();

	internal static ResponsiveExtension[] GetAllInstancesFor(DependencyObject owner) => Array.Empty<ResponsiveExtension>();

	internal static ResponsiveExtension? GetInstanceFor(DependencyObject owner, string property) => null;
}
#endif
