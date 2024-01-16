#if WINDOWS_UWP
using System;
using System.Collections.Generic;
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
		return null;
	}
}
#endif
