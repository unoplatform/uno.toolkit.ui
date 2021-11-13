
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.Disposables;
using Uno.Extensions;

#if IS_WINUI
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#endif

namespace Uno.Toolkit.UI.Extensions
{
	internal static class DependencyObjectExtensions
	{
#if HAS_UNO
		/// <summary>
		/// Registers a notification function for listening to changes to a tree of DependencyProperties relative to this DependencyObject instance.
		/// </summary>
		/// <param name="instance">The DependencyObject instance the dependency property tree starts at.</param>
		/// <param name="callback">A callback based on the PropertyChangedCallback delegate, which the system invokes when the value of any of the specified properties changes.</param>
		/// <param name="properties">The tree of dependency property to register for property-changed notification.</param>
		/// <returns>A disposable that will unregister the callback when disposed.</returns>
		/// <remarks>
		/// <para>Each node of the dependency property tree is represented by an array describing the path of the dependency property relative to the given dependency object instance.</para>
		/// <para>For example, to register for notifications to changes to the Color of a TextBlock's Foreground:</para>
		/// <code>var disposable = myTextBlock.RegisterDisposableNestedPropertyChangedCallback(callback, new [] { TextBlock.ForegroundProperty, SolidColorBrush.ColorProperty });</code>
		/// </remarks>
		internal static IDisposable RegisterDisposableNestedPropertyChangedCallback(this DependencyObject instance, PropertyChangedCallback callback, params DependencyProperty[][] properties)
		{
			if (instance == null)
			{
				return Disposable.Empty;
			}

			return properties
				.Where(Enumerable.Any)
				.GroupBy(Enumerable.First, propertyPath => propertyPath.Skip(1).ToArray())
				.Where(Enumerable.Any)
				.Where(group => group.Key != null)
				.Select(group =>
				{
					var property = group.Key;
					var subProperties = group.ToArray();

					var childDisposable = new SerialDisposable();

					childDisposable.Disposable = (instance.GetValue(property) as DependencyObject)?.RegisterDisposableNestedPropertyChangedCallback(callback, subProperties);

					var disposable = instance.RegisterDisposablePropertyChangedCallback(property, (s, e) =>
					{
						callback(s, e);

						childDisposable.Disposable = s?.RegisterDisposableNestedPropertyChangedCallback(callback, subProperties);
					});

					return new CompositeDisposable(disposable, childDisposable);
				})
				.Apply(disposables => new CompositeDisposable(disposables));
		}
#endif
		internal static IDisposable RegisterDisposablePropertyChangedCallback(this DependencyObject? instance, DependencyProperty property, DependencyPropertyChangedCallback callback)
		{
			if (instance == null)
			{
				return Disposable.Empty;
			}

			var token = instance.RegisterPropertyChangedCallback(property, callback);
			return Disposable.Create(() => instance.UnregisterPropertyChangedCallback(property, token));
		}

		public static T? FindChild<T>(this DependencyObject depObj)
		where T :
#if HAS_UNO
			class,
#endif
			DependencyObject
		{
			if (depObj == null) return default(T);

			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
			{
				var child = VisualTreeHelper.GetChild(depObj, i);


				var result = (child is T t) ? t : FindChild<T>(child);
				if (result != null) return result;
			}
			return default(T);
		}
		public static T? GetFirstParent<T>(this DependencyObject element, bool includeCurrent = true)
		where T :
#if HAS_UNO
			class,
#endif
			DependencyObject
		{
			var c = element.GetAncestors(includeCurrent);
			return c.OfType<T>().FirstOrDefault();
		}

		public static IEnumerable<DependencyObject> GetAncestors(this DependencyObject element, bool includeCurrent = true)
		{
			if (includeCurrent)
			{
				yield return element;
			}

			if (element is null) yield break;
			while (VisualTreeHelper.GetParent(element) is { } parent)
			{
				yield return element = parent;
			}
		}
	}
}