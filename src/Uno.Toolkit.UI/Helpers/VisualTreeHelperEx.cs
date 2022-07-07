using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#endif

namespace Uno.Toolkit.UI
{
	internal static class VisualTreeHelperEx
	{
		public static string TreeGraph(this DependencyObject reference) => TreeGraph(reference, DebugVTNode);

		public static string TreeGraph(this DependencyObject reference, Func<DependencyObject, string> describe)
		{
			var buffer = new StringBuilder();
			Walk(reference);
			return buffer.ToString();

			void Walk(DependencyObject o, int depth = 0)
			{
				Print(o, depth);
				foreach (var child in GetChildren(o))
				{
					Walk(child, depth + 1);
				}
			}
			void Print(DependencyObject o, int depth)
			{
				buffer
					.Append(new string(' ', depth * 4))
					.Append(describe(o))
					.AppendLine();
			}
		}

		private static string DebugVTNode(DependencyObject x)
		{
			var fe = x as FrameworkElement;

			return new StringBuilder()
				.Append(x.GetType().Name)
				.Append(!string.IsNullOrEmpty(fe?.Name) ? $"#{fe?.Name}" : string.Empty)
				.ToString();
		}


		public static T GetFirstAncestor<T>(this DependencyObject reference) => GetAncestors(reference)
			.OfType<T>()
			.FirstOrDefault();

		public static T GetFirstAncestor<T>(this DependencyObject reference, Func<T, bool> predicate) => GetAncestors(reference)
			.OfType<T>()
			.FirstOrDefault(predicate);

		public static T GetFirstDescendant<T>(DependencyObject reference) => GetDescendants(reference)
			.OfType<T>()
			.FirstOrDefault();

		public static T GetFirstDescendant<T>(DependencyObject reference, Func<T, bool> predicate) => GetDescendants(reference)
			.OfType<T>()
			.FirstOrDefault(predicate);

		public static IEnumerable<DependencyObject> GetAncestors(this DependencyObject o)
		{
			if (o is null) yield break;
			while (VisualTreeHelper.GetParent(o) is { } parent)
			{
				yield return o = parent;
			}
		}

		public static IEnumerable<DependencyObject> GetDescendants(DependencyObject reference)
		{
			foreach (var child in GetChildren(reference))
			{
				yield return child;
				foreach (var grandchild in GetDescendants(child))
				{
					yield return grandchild;
				}
			}
		}

		public static IEnumerable<DependencyObject> GetChildren(DependencyObject reference)
		{
			return Enumerable
				.Range(0, VisualTreeHelper.GetChildrenCount(reference))
				.Select(x => VisualTreeHelper.GetChild(reference, x));
		}
	}
}
