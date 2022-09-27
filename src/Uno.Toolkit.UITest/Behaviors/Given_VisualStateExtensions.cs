using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Uno.Toolkit.UITest.Extensions;
using Uno.Toolkit.UITest.Framework;
using Uno.UITest.Helpers;
using Uno.UITest.Helpers.Queries;

namespace Uno.Toolkit.UITest.Behaviors
{
	public class Given_VisualStateManagerExtensions : TestBase
	{
		protected override string SampleName => "VisualStateManagerExtensions";

		[Test]
		public void When_Value_Set()
		{
			const float VisualTransitionDuration = 1; // actual duration is 333ms; extra is given to ensure it doesnt fail from random lag.
			var borderRect = App.GetPhysicalRect("SUT_BackgroundBorder");

			foreach (var color in "Red,Green,Blue".Split(','))
			{
				App.FastTap($"{color}Button");
				App.Wait(VisualTransitionDuration);

				using var screenshot = TakeScreenshot($"Post_{color}Button_Press");
				ImageAssert.HasColorAt(screenshot, borderRect.CenterX, borderRect.CenterY, color switch
				{
					"Red" => "#FF0000",
					"Green" => "#008000",
					"Blue" => "#0000FF",

					_ => throw new ArgumentOutOfRangeException($"Unexpected color: {color}"),
				});
			}
		}
	}
}
