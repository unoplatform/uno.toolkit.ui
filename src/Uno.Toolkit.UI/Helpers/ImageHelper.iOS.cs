#if __IOS__
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace Uno.Toolkit.UI
{
	internal static class ImageHelper
	{
		private static readonly char[] trimChars = new[] { '/' };

		public static UIImage? FromUri(Uri? uri)
		{
			if (uri == null)
			{
				return null;
			}

			var bundleName = Path.GetFileName(uri.AbsolutePath);
			var bundlePath = uri.PathAndQuery.TrimStart(trimChars);

			var image = UIImage.FromFile(bundleName) ?? UIImage.FromFile(bundlePath);
			return image;
		}
	}
}
#endif
