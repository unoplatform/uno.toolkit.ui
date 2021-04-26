using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.UITest;
using Uno.UITest.Helpers;
using Uno.UITest.Helpers.Queries;

namespace Uno.Toolkit.UITest
{
	public static class Extensions
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

		public static QueryEx MarkedAnywhere(this IApp app, string elementName) 
			=> new QueryEx(q => q.All().Marked(elementName));

		public static QueryEx ToQueryEx(this Func<IAppQuery, IAppQuery> query) => new QueryEx(query);
	}
}
