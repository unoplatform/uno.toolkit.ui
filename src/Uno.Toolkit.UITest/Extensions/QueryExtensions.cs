using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.UITest.Helpers.Queries;
using Uno.UITest;
using Uno.UITest.Helpers;

namespace Uno.Toolkit.UITest.Extensions
{
	internal static class QueryExtensions
	{
		public static Func<IAppQuery, IAppQuery> WaitThenTap(this IApp app, Func<IAppQuery, IAppQuery> query, TimeSpan? timeout = null)
		{
			app.WaitForElement(query, timeout: timeout);
			app.Tap(query);

			return query;
		}

		public static Func<IAppQuery, IAppQuery> WaitThenTap(this IApp app, string marked, TimeSpan? timeout = null)
			=> WaitThenTap(app, q => q.All().Marked(marked), timeout);

		/// <summary>
		/// Wait for element to have the expected value for its Text property.
		/// </summary>
		public static void WaitForText(this IApp app, QueryEx element, string expectedText) =>
			app.WaitForDependencyPropertyValue(element, "Text", expectedText);

		/// <summary>
		/// Wait for element to be available and to have the expected value for its Text property.
		/// </summary>
		public static void WaitForText(this IApp app, string elementName, string expectedText, TimeSpan? timeout = null)
		{
			var element = app.Marked(elementName);
			app.WaitForElement(element, timeout: timeout);
			app.WaitForText(element, expectedText);
		}

		/// <summary>
		/// Calls the <see cref="IApp.WaitForElement(string, string, TimeSpan?, TimeSpan?, TimeSpan?)"/> method with a timeout message that specifies
		/// the element name, which is useful when multiple elements are waited upon in the same test.
		/// </summary>
		public static IAppResult[] WaitForElementWithMessage(this IApp app, string elementName, string? additionalMessage = null, TimeSpan? timeout = null)
		{
			var timeoutMessage = $"Timed out waiting for element '{elementName}'";

			if (additionalMessage != null)
			{
				timeoutMessage = $"{timeoutMessage} - {additionalMessage}";
			}

			return app.WaitForElement(elementName, timeoutMessage: timeoutMessage, timeout: timeout);
		}

		public static QueryEx MarkedAnywhere(this IApp app, string elementName)
			=> new QueryEx(q => q.All().Marked(elementName));

		public static QueryEx TypeInto(this IApp app, string textBoxName, string inputText)
		{
			var tb = app.WaitThenTap(textBoxName).ToQueryEx();

			app.EnterText(inputText);
			app.WaitFor(() => inputText == GetText(tb));

			return tb;
		}
		public static string GetText(QueryEx textBox) => textBox.GetDependencyPropertyValue<string>("Text");


		public static QueryEx ToQueryEx(this Func<IAppQuery, IAppQuery> query) => new QueryEx(query);


		public static void FastTap(this IApp app, string elementName)
		{
			var tapPosition = app.GetRect(elementName);
			app.TapCoordinates(tapPosition.CenterX, tapPosition.CenterY);
		}

		public static void FastTap(this IApp app, QueryEx query)
		{
			var tapPosition = app.GetRect(query);
			app.TapCoordinates(tapPosition.CenterX, tapPosition.CenterY);
		}

		public static void FastTap(this IApp app, Func<IAppQuery, IAppQuery> query)
		{
			var tapPosition = app.GetRect(query);
			app.TapCoordinates(tapPosition.CenterX, tapPosition.CenterY);
		}

		public static QueryEx FastTap(this QueryEx query)
		{
			Helpers.App.FastTap(query);
			return query;
		}

		/// <summary>
		/// Get bounds rect for an element.
		/// </summary>
		public static IAppRect GetRect(this IApp app, string elementName)
		{
			return app.WaitForElementWithMessage(elementName).First().Rect;
		}
		public static IAppRect GetRect(this IApp app, QueryEx query)
		{
			return app.WaitForElement(query).First().Rect;
		}
		public static IAppRect GetRect(this IApp app, Func<IAppQuery, IAppQuery> query)
		{
			return app.WaitForElement(query).First().Rect;
		}

		public static FileInfo GetInAppScreenshot(this IApp app)
		{
			var byte64Image = app.InvokeGeneric("browser:SampleRunner|GetScreenshot", "0")?.ToString() ?? string.Empty;

			var array = Convert.FromBase64String(byte64Image);

			var outputFile = Path.GetTempFileName();
			File.WriteAllBytes(outputFile, array);

			var finalPath = Path.ChangeExtension(outputFile, ".png");

			File.Move(outputFile, finalPath);

			return new FileInfo(finalPath);
		}
	}
}
