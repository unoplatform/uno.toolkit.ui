using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.UITest;
using Uno.UITest.Helpers;
using Uno.UITest.Helpers.Queries;
using Uno.UITests.Helpers;

namespace Uno.Toolkit.UITest.Extensions
{
	public static class AppExtensions
	{
		private static float? _scaling;

		public static IAppRect GetPhysicalRect(this IApp app, string elementName)
		{
			var rect = app.WaitForElementWithMessage(elementName).Single().Rect;

			return AppInitializer.GetLocalPlatform() switch
			{
				Platform.Android => rect,
				Platform.iOS => rect.LogicalToPhysicalPixels(app),
				Platform.Browser => rect,
				_ => throw new PlatformNotSupportedException("Unknown current platform.")
			};
		}

		public static float GetDisplayScreenScaling(this IApp app)
		{
			return _scaling ?? (float)(_scaling = GetScaling());

			float GetScaling()
			{
				var scalingRaw = app.InvokeGeneric("browser:SampleRunner|GetDisplayScreenScaling", "0");

				if (float.TryParse(scalingRaw?.ToString(), NumberStyles.Float, NumberFormatInfo.InvariantInfo, out var scaling))
				{
					Console.WriteLine($"Display Scaling: {scaling}");
					return scaling / 100f;
				}
				else
				{
					return 1f;
				}
			}
		}
	}
}
