#if __IOS__
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace Uno.UI.ToolkitLib.Helpers
{
	internal static class ImageHelper
	{
		public static UIImage? FromUri(Uri? uri)
		{
			if (uri == null)
			{
				return null;
			}

			var bundleName = Path.GetFileName(uri.AbsolutePath);
			var bundlePath = uri.PathAndQuery.TrimStart(new[] { '/' });

			var image = UIImage.FromFile(bundleName) ?? UIImage.FromFile(bundlePath);
			return image;
		}
	}
}
#endif