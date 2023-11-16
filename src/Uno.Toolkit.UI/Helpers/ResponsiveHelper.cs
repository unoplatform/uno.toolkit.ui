using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uno.Toolkit.UI.Helpers;

internal class ResponsiveHelper
{
	private static readonly Lazy<ResponsiveHelper> _instance = new Lazy<ResponsiveHelper>(() => new ResponsiveHelper());
	private readonly List<WeakReference> _references = new();

	private ResponsiveHelper()
	{
	}

	public static ResponsiveHelper Instance => _instance.Value;

	internal void Register(ResponsiveExtension responsiveExtension)
	{
		throw new NotImplementedException();
	}
}
