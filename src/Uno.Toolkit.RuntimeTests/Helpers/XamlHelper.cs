using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Toolkit.RuntimeTests.Extensions;

#if IS_WINUI
using Microsoft.UI.Xaml.Markup;
#else
using Windows.UI.Xaml.Markup;
#endif

namespace Uno.Toolkit.RuntimeTests.Helpers
{
	internal static class XamlHelper
	{
		public static readonly IReadOnlyDictionary<string, string> KnownXmlnses = new Dictionary<string, string>
		{
			[string.Empty] = "http://schemas.microsoft.com/winfx/2006/xaml/presentation",
			["x"] = "http://schemas.microsoft.com/winfx/2006/xaml",
			["toolkit"] = "using:Uno.UI.Toolkit", // uno utilities
			["utu"] = "using:Uno.Toolkit.UI", // this library
			["muxc"] = "using:Microsoft.UI.Xaml.Controls",
		};

		/// <summary>
		/// Matches right before the &gt; or \&gt; tail of any tag.
		/// </summary>
		/// <remarks>
		/// It will match an opening or closing or self-closing tag.
		/// </remarks>
		private static readonly Regex EndOfTagRegex = new Regex(@"(?=( ?/)?>)");

		/// <summary>
		/// Matches any tag without xmlns prefix.
		/// </summary>
		private static readonly Regex NonXmlnsTagRegex = new Regex(@"<\w+[ />]");

		/// <summary>
		/// Matches any open/open-hanging/self-close/close tag.
		/// </summary>
		/// <remarks>open-hanging refers to xml tag that opens, but span on multiple lines.</remarks>
		private static readonly Regex XmlTagRegex = new Regex("<[^>]+(>|$)");

		/// <summary>
		/// Auto complete any unclosed tag.
		/// </summary>
		/// <param name="xaml"></param>
		/// <returns></returns>
		internal static string XamlAutoFill(string xaml)
		{
			var buffer = new StringBuilder();

			// we assume the input is either space or tab indented, not mixed.
			// it doesnt really matter here if we count the depth in 1 or 2 or 4,
			// since they will be compared against themselves, which hopefully follow the same "style".
			var stack = new Stack<(string Indent, string Name)>();
			void PopFrame((string Indent, string Name) frame)
			{
				buffer.AppendLine($"{frame.Indent}</{frame.Name}>");
			}
			void PopStack(Stack<(string Indent, string Name)> stack)
			{
				while (stack.TryPop(out var item))
				{
					PopFrame(item);
				}
			}

			var lines = string.Concat(xaml.Split('\r')).Split('\n');
			foreach (var line in lines)
			{
				if (line.TrimStart() is { Length: > 0 } content)
				{
					var depth = line.Length - content.Length;
					var indent = line[0..depth];

					// we should parse all tags on this line: Open OpenHanging SelfClose Close
					// then close all 'open/open-hanging' tags in the stack with higher depth
					// while pairing `Close` in the left-most part of current line with whats in stack that match name and depth, and eliminate them

					var overflows = new Stack<(string Indent, string Name)>(stack.PopWhile(x => x.Indent.Length >= depth).Reverse());
					var tags = XmlTagRegex.Matches(content).Select(x => x.Value).ToArray();
					foreach (var tag in tags)
					{
						if (tag.StartsWith("<!"))
						{
							PopStack(overflows);
						}
						else if (tag.EndsWith("/>"))
						{
							PopStack(overflows);
						}
						else if (tag.StartsWith("</"))
						{
							var name = tag.Split(' ', '>')[0][2..];
							while (overflows.TryPop(out var overflow))
							{
								if (overflow.Name == name) break;

								PopFrame(overflow);
							}
						}
						else
						{
							PopStack(overflows);

							var name = tag.Split(' ', '/', '>')[0][1..];
							stack.Push((indent, name));
						}
					}
				}
				buffer.AppendLine(line);
			}

			PopStack(new(stack.Reverse()));
			return buffer.ToString();
		}

		/// <summary>
		/// Inject any required xmlns.
		/// </summary>
		/// <param name="xaml"></param>
		/// <param name="xmlnses">Optional; used to override <see cref="KnownXmlnses"/>.</param>
		/// <param name="complementaryXmlnses">Completary xmlnses that adds to <paramref name="xmlnses"/></param>
		/// <returns></returns>
		internal static string InjectXmlns(string xaml, IDictionary<string, string>? xmlnses = null, IDictionary<string, string>? complementaryXmlnses = null)
		{
			var xmlnsLookup = (xmlnses?.AsReadOnly() ?? KnownXmlnses).Combine(complementaryXmlnses?.AsReadOnly());
			var injectables = new Dictionary<string, string>();

			foreach (var xmlns in xmlnsLookup)
			{
				var match = xmlns.Key == string.Empty
					? NonXmlnsTagRegex.IsMatch(xaml)
					// naively match the xmlns-prefix regardless if it is quoted,
					// since false positive doesn't matter.
					: xaml.Contains($"{xmlns.Key}:");
				if (match)
				{
					injectables.Add(xmlns.Key, xmlns.Value);
				}
			}

			if (injectables.Any())
			{
				var injection = " " + string.Join(" ", injectables
					.Select(x => $"xmlns{(string.IsNullOrEmpty(x.Key) ? "" : $":{x.Key}")}=\"{x.Value}\"")
				);

				xaml = EndOfTagRegex.Replace(xaml, injection.TrimEnd(), 1);
			}

			return xaml;
		}

		/// <summary>
		/// Load partial xaml with omittable closing tags.
		/// </summary>
		/// <param name="xaml">Xaml with single or double quotes</param>
		/// <param name="xmlnses">Optional; xmlns that may be needed. <see cref="KnownXmlnses"/> will be used if null.</param>
		/// <param name="complementaryXmlnses">Completary xmlnses that adds to <paramref name="xmlnses"/></param>
		/// <returns></returns>
		public static T LoadPartialXaml<T>(string xaml, IDictionary<string, string>? xmlnses = null, IDictionary<string, string>? complementaryXmlnses = null)
			where T : class
		{
			xaml = XamlAutoFill(xaml);
			xaml = InjectXmlns(xaml, xmlnses, complementaryXmlnses);

			return LoadXaml<T>(xaml);
		}

		/// <summary>
		/// XamlReader.Load the xaml and type-check result.
		/// </summary>
		/// <param name="xaml">Xaml with single or double quotes</param>
		/// <param name="xmlnses">Optional; xmlns that may be needed. <see cref="KnownXmlnses"/> will be used if null.</param>
		/// <param name="complementaryXmlnses">Completary xmlnses that adds to <paramref name="xmlnses"/></param>
		public static T LoadXaml<T>(string xaml, IDictionary<string, string>? xmlnses = null, IDictionary<string, string>? complementaryXmlnses = null)
			where T : class
		{
			xaml = InjectXmlns(xaml, xmlnses, complementaryXmlnses);

			return LoadXaml<T>(xaml, xmlnses);
		}

		/// <summary>
		/// XamlReader.Load the xaml and type-check result.
		/// </summary>
		private static T LoadXaml<T>(string xaml) where T : class
		{
			var result = XamlReader.Load(xaml);
			Assert.IsNotNull(result, "XamlReader.Load returned null");
			Assert.IsInstanceOfType(result, typeof(T), "XamlReader.Load did not return the expected type");

			return (T)result;
		}
	}
}
