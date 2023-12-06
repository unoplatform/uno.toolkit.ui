//#define TREEGRAPH_VERBOSE_LAYOUT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Shapes;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;
#endif
#if __IOS__
using UIKit;
using _View = UIKit.UIView;
#elif __MACOS__
using AppKit;
using _View = AppKit.NSView;
#elif __ANDROID__
using _View = Android.Views.View;
#else
#if IS_WINUI
using _View = Microsoft.UI.Xaml.DependencyObject;
#else
using _View = Windows.UI.Xaml.DependencyObject;
#endif
#endif

using static System.Reflection.BindingFlags;
using static Uno.Toolkit.UI.PrettyPrint;

namespace Uno.Toolkit.UI
{
	/// <summary>
	/// Utility class to traverse the visual tree.
	/// </summary>
	/// <remarks>
	/// This class is implemented with <see cref="VisualTreeHelper.GetChild(DependencyObject, int)"/> which doesn't cross uwp/native barrier.
	/// Use the <see cref="Native"/> counterpart if the uwp/native barrier needs to be crossed.
	/// </remarks>
	internal static partial class VisualTreeHelperEx
	{
		/// <summary>
		/// Produces a text representation of the visual tree.
		/// </summary>
		/// <param name="reference">Any node of the visual tree</param>
		public static string TreeGraph(this DependencyObject reference) => TreeGraph(reference, DebugVTNode);
		
		/// <summary>
		/// Produces a text representation of the visual tree, using the provided method of description.
		/// </summary>
		/// <param name="reference">Any node of the visual tree</param>
		/// <param name="describeProperties">A function to describe the properties of interest of a visual tree node.</param>
		/// <returns></returns>
		public static string TreeGraph(this DependencyObject reference, Func<object, IEnumerable<string>> describeProperties) =>
			TreeGraph(reference, x => DebugVTNode(x, describeProperties));

		/// <summary>
		/// Produces a text representation of the visual tree, using the provided method of description.
		/// </summary>
		/// <param name="reference">Any node of the visual tree</param>
		/// <param name="describe">A function to describe a visual tree node in a single line.</param>
		/// <returns></returns>
		public static string TreeGraph(this DependencyObject reference, Func<object, string> describe)
		{
			var buffer = new StringBuilder();
			Walk(reference);
			return buffer.ToString();

			void Walk(DependencyObject o, int depth = 0)
			{
				Print(o, depth);
				foreach (var child in o.GetChildren())
				{
					Walk(child, depth + 1);
				}
			}
			void Print(object o, int depth)
			{
				buffer
					.Append(new string(' ', depth * 4))
					.Append(describe(o))
					.AppendLine();
			}
		}

		/// <summary>
		/// Returns the first ancestor of a specified type.
		/// </summary>
		/// <typeparam name="T">The type of ancestor to find.</typeparam>
		/// <param name="reference">Any node of the visual tree</param>
		/// <remarks>First Counting from the <paramref name="reference"/> and not from the root of tree.</remarks>
		public static T? GetFirstAncestor<T>(this DependencyObject reference) => GetAncestors(reference)
			.OfType<T>()
			.FirstOrDefault();

		/// <summary>
		/// Returns the first ancestor of a specified type that satisfies the <paramref name="predicate"/>.
		/// </summary>
		/// <typeparam name="T">The type of ancestor to find.</typeparam>
		/// <param name="reference">Any node of the visual tree</param>
		/// <param name="predicate">A function to test each node for a condition.</param>
		/// <remarks>First Counting from the <paramref name="reference"/> and not from the root of tree.</remarks>
		public static T? GetFirstAncestor<T>(this DependencyObject reference, Func<T, bool> predicate) => GetAncestors(reference)
			.OfType<T>()
			.FirstOrDefault(predicate);
		
		/// <summary>
		/// Returns the first ancestor of a specified type that satisfies the <paramref name="predicate"/>.
		/// </summary>
		/// <typeparam name="T">The type of ancestor to find.</typeparam>
		/// <param name="reference">Any node of the visual tree</param>
		/// <param name="hierarchyPredicate">A function to test each ancestor for a condition.</param>
		/// <param name="predicate">A function to test each node for a condition.</param>
		/// <remarks>First Counting from the <paramref name="reference"/> and not from the root of tree.</remarks>
		public static T? GetFirstAncestor<T>(this DependencyObject reference, Func<DependencyObject, bool> hierarchyPredicate, Func<T, bool> predicate) => GetAncestors(reference, hierarchyPredicate)
			.OfType<T>()
			.FirstOrDefault(predicate);

		/// <summary>
		/// Returns the first descendant of a specified type.
		/// </summary>
		/// <typeparam name="T">The type of descendant to find.</typeparam>
		/// <param name="reference">Any node of the visual tree</param>
		/// <remarks>The crawling is done depth first.</remarks>
		public static T? GetFirstDescendant<T>(this DependencyObject reference) => GetDescendants(reference)
			.OfType<T>()
			.FirstOrDefault();

		/// <summary>
		/// Returns the first descendant of a specified type that satisfies the <paramref name="predicate"/>.
		/// </summary>
		/// <typeparam name="T">The type of descendant to find.</typeparam>
		/// <param name="reference">Any node of the visual tree</param>
		/// <param name="predicate">A function to test each node for a condition.</param>
		/// <remarks>The crawling is done depth first.</remarks>
		public static T? GetFirstDescendant<T>(this DependencyObject reference, Func<T, bool> predicate) => GetDescendants(reference)
			.OfType<T>()
			.FirstOrDefault(predicate);

		/// <summary>
		/// Returns the first descendant of a specified type that satisfies the <paramref name="predicate"/> whose ancestors (up to <paramref name="reference"/>) satisfy the <paramref name="hierarchyPredicate"/>.
		/// </summary>
		/// <typeparam name="T">The type of descendant to find.</typeparam>
		/// <param name="reference">Any node of the visual tree</param>
		/// <param name="hierarchyPredicate">A function to test each ancestor for a condition.</param>
		/// <param name="predicate">A function to test each descendant for a condition.</param>
		/// <remarks>The crawling is done depth first.</remarks>
		public static T? GetFirstDescendant<T>(this DependencyObject reference, Func<DependencyObject, bool> hierarchyPredicate, Func<T, bool> predicate) => GetDescendants(reference, hierarchyPredicate)
			.OfType<T>()
			.FirstOrDefault(predicate);

		public static IEnumerable<DependencyObject> GetAncestors(this DependencyObject o) => GetAncestors(o, x => true);

		public static IEnumerable<DependencyObject> GetAncestors(this DependencyObject o, Func<DependencyObject, bool> hierarchyPredicate)
		{
			if (o is null) yield break;
			while (VisualTreeHelper.GetParent(o) is { } parent && hierarchyPredicate(parent))
			{
				yield return o = parent;
			}
		}

		public static IEnumerable<DependencyObject> GetDescendants(this DependencyObject reference) => GetDescendants(reference, x => true);

		public static IEnumerable<DependencyObject> GetDescendants(this DependencyObject reference, Func<DependencyObject, bool> hierarchyPredicate)
		{
			foreach (var child in reference.GetChildren().Where(hierarchyPredicate))
			{
				yield return child;
				foreach (var grandchild in child.GetDescendants(hierarchyPredicate))
				{
					yield return grandchild;
				}
			}
		}

		public static IEnumerable<DependencyObject> GetChildren(this DependencyObject reference)
		{
			return Enumerable
				.Range(0, VisualTreeHelper.GetChildrenCount(reference))
				.Select(x => VisualTreeHelper.GetChild(reference, x));
		}

		public static DependencyObject? GetTemplateRoot(this DependencyObject o) => o?.GetChildren().FirstOrDefault();
	}
	internal static partial class VisualTreeHelperEx // TreeGraph helper methods
	{
		private static string DebugVTNode(object x)
		{
			return DebugVTNode(x, GetDetails);

			static IEnumerable<string> GetDetails(object x)
			{
				#region Common Details: Layout (high priority)
#if TREEGRAPH_VERBOSE_LAYOUT
#if __IOS__
				if (x is _View view && view.Superview is { })
				{
					var abs = view.Superview.ConvertPointToView(view.Frame.Location, toView: null);
					yield return $"Abs=[Rect {view.Frame.Width:0.#}x{view.Frame.Height:0.#}@{abs.X:0.#},{abs.Y:0.#}]";
				}
#elif __ANDROID__
				if (x is _View view)
				{
					yield return $"Rect={FormatViewRect(view)}";
				}
#endif
#endif
				if (x is FrameworkElement fe)
				{
					yield return $"Actual={fe.ActualWidth}x{fe.ActualHeight}";
#if TREEGRAPH_VERBOSE_LAYOUT
					yield return $"Constraints=[{fe.MinWidth},{fe.Width},{fe.MaxWidth}]x[{fe.MinHeight},{fe.Height},{fe.MaxHeight}]";
#endif
					yield return $"HV={fe.HorizontalAlignment}/{fe.VerticalAlignment}";
				}
				if (x is UIElement uie)
				{
					//yield return $"Desired={FormatSize(uie.DesiredSize)}";
					//yield return $"LAS={FormatSize(uie.LastAvailableSize)}";
				}
#if TREEGRAPH_VERBOSE_LAYOUT
				if (TryGetDpValue<HorizontalAlignment>(x, "HorizontalContentAlignment", out var hca) |
					TryGetDpValue<HorizontalAlignment>(x, "VerticalContentAlignment", out var vca))
				{
					yield return $"HVC={hca}/{vca}";
				}
#endif
				#endregion
				#region WinUI Control Details
				if (x is ScrollViewer sv)
				{
					yield return $"Offset={sv.HorizontalOffset:0.#},{sv.VerticalOffset:0.#}";
					yield return $"Viewport={sv.ViewportWidth:0.#}x{sv.ViewportHeight:0.#}";
					yield return $"Extent={sv.ExtentWidth:0.#}x{sv.ExtentHeight:0.#}";
				}
				if (x is ListViewItem lvi)
				{
					yield return $"Index={ItemsControl.ItemsControlFromItemContainer(lvi)?.IndexFromContainer(lvi) ?? -1}";
				}
				if (x is TextBlock txt && !string.IsNullOrEmpty(txt.Text))
				{
					yield return $"Text=\"{EscapeMultiline(txt.Text)}\"";
				}
				if (x is Shape shape)
				{
					if (shape.Fill is not null) yield return $"Fill={FormatBrush(shape.Fill)}";
					if (shape.Stroke is not null) yield return $"Stroke={FormatBrush(shape.Stroke)}*{shape.StrokeThickness}px";
				}
				#endregion
				#region Toolkit Control Details
				if (x is ResponsiveView rv)
				{
					if (rv.ResolvedLayout is { } resolved) yield return $"Resolved={resolved.Layout}";
					yield return $"Layout={rv.GetAppliedLayout()}";
				}
#if DEBUG
				if (ResponsiveExtension.TrackedInstances.Where(y => y.Owner.Target == x).ToArray() is { Length: > 0 } instances)
				{
					foreach (var item in instances)
					{
						if (item.Extension.Target is ResponsiveExtension re)
						{
							yield return $"{item.Property}@Responsive: {re.LastUsedLayout}->{re.ResolvedValue?.Layout}\\{re.ResolvedValue?.Value}";
						}
					}
				}
#endif
				#endregion
				#region Common Details: Layout,Misc (low priority)
				if (TryGetDpValue<CornerRadius>(x, "CornerRadius", out var cr)) yield return $"CornerRadius={FormatCornerRadius(cr)}";
				if (TryGetDpValue<Thickness>(x, "Margin", out var margin)) yield return $"Margin={FormatThickness(margin)}";
				if (TryGetDpValue<Thickness>(x, "Padding", out var padding)) yield return $"Padding={FormatThickness(padding)}";
				if (TryGetDpValue<double>(x, "Opacity", out var opacity)) yield return $"Opacity={opacity}";
				if (TryGetDpValue<Visibility>(x, "Visibility", out var visibility)) yield return $"Visibility={visibility}";
				if (GetActiveVisualStates(x as Control) is { } states) yield return $"VisualStates={states}";
				#endregion
			}
		}
		private static string DebugVTNode(object x, Func<object, IEnumerable<string>> describeProperties)
		{
			if (x is null) return "<null>";

			return new StringBuilder()
				.Append(x.GetType().Name)
				.Append((x as FrameworkElement)?.Name is string { Length: > 0 } xname ? $"#{xname}" : string.Empty)
				.Append(GetPropertiesDescriptionSafe())
				.ToString();

			string? GetPropertiesDescriptionSafe()
			{
				try
				{
					return string.Join(", ", describeProperties(x)) is { Length: > 0 } propertiesDescription
						? $" // {propertiesDescription}"
						: null;
				}
				catch (Exception e)
				{
					return $"// threw {e.GetType().Name}: {EscapeMultiline(e.Message, escapeTabs: true)}";
				}
			}
		}

		internal static bool TryGetDpValue<T>(object owner, string property, out T? value)
		{
			if (owner is DependencyObject @do &&
				owner.GetType().GetProperty($"{property}Property", Public | Static | FlattenHierarchy)
					?.GetValue(null, null) is DependencyProperty dp)
			{
				value = (T)@do.GetValue(dp);
				return true;
			}

			value = default;
			return false;
		}

		internal static string? GetActiveVisualStates(Control? control)
		{
			if (control?.GetTemplateRoot() is FrameworkElement root)
			{
				if (VisualStateManager.GetVisualStateGroups(root) is { Count: > 0 } vsgs)
				{
					return string.Join("|", vsgs.Select(x => x.CurrentState?.Name));
				}
			}

			return null;
		}
	}
	internal static partial class VisualTreeHelperEx // Native view impl
	{
#if __IOS__ || __ANDROID__
		internal static class Native
		{
			public static T? GetFirstDescendant<T>(_View reference) => GetDescendants(reference)
				.OfType<T>()
				.FirstOrDefault();

			public static T? GetFirstDescendant<T>(_View reference, Func<T, bool> predicate) => GetDescendants(reference)
				.OfType<T>()
				.FirstOrDefault(predicate);

			public static T? GetFirstDescendant<T>(_View reference, Func<_View, bool> hierarchyPredicate, Func<T, bool> predicate) => GetDescendants(reference, hierarchyPredicate)
				.OfType<T>()
				.FirstOrDefault(predicate);

			public static IEnumerable<_View> GetDescendants(_View reference) => GetDescendants(reference, x => true);

			public static IEnumerable<_View> GetDescendants(_View reference, Func<_View, bool> hierarchyPredicate)
			{
				foreach (var child in GetChildren(reference).Where(hierarchyPredicate))
				{
					yield return child;

					foreach (var grandchild in GetDescendants(child, hierarchyPredicate))
					{
						yield return grandchild;
					}
				}
			}

#if __IOS__
			private static IEnumerable<_View> GetChildren(_View reference)
			{
				return reference.Subviews;
			}
#elif __ANDROID__
			private static IEnumerable<_View> GetChildren(_View reference)
			{
				if (reference is Android.Views.ViewGroup vg)
				{
					return Enumerable
						.Range(0, vg.ChildCount)
						.Select(vg.GetChildAt)!;
				}

				return Enumerable.Empty<_View>();
			}
#endif
		}
#endif
	}
}
