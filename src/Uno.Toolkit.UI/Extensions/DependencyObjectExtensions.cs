﻿using System;
using System.Collections.Generic;
using System.Linq;
using Uno.Disposables;
using Uno.Extensions;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Uno.Logging;

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

namespace Uno.Toolkit.UI
{
	internal static class DependencyObjectExtensions
	{
		private static Dictionary<(Type type, string property), DependencyProperty?> _dependencyPropertyReflectionCache = new Dictionary<(Type, string), DependencyProperty?>(2);

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

			var disposables = properties
				.Where(Enumerable.Any)
				.GroupBy(Enumerable.First, propertyPath => propertyPath.Skip(1).ToArray())
				.Where(Enumerable.Any)
				.Where(group => group.Key != null)
				.Select(group =>
				{
					var property = group.Key;

					if (instance.TryGetValue(property, out var dpValue))
					{
						var childDisposable = new SerialDisposable();
						var subProperties = group.ToArray();
						childDisposable.Disposable = dpValue?.RegisterDisposableNestedPropertyChangedCallback(callback, subProperties);

						var disposable = instance.RegisterDisposablePropertyChangedCallback(property, (s, e) =>
						{
							callback(s, e);
							if (s is { } && s.TryGetValue(property, out var dpValue))
							{
								childDisposable.Disposable = dpValue?.RegisterDisposableNestedPropertyChangedCallback(callback, subProperties);
							}
						});

						return new CompositeDisposable(disposable, childDisposable);
					}

					return Disposable.Empty;
				});

			return new CompositeDisposable(disposables);
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

		/// <summary>
		/// ** Recursively ** gets an enumerable sequence of all the parent objects of a given element.
		/// Parents are ordered from bottom to the top, i.e. from direct parent to the root of the window.
		/// </summary>
		/// <param name="element">The element to search from</param>
		/// <param name="includeCurrent">Determines if the current <paramref name="element"/> should be included or not.</param>
		public static IEnumerable<DependencyObject> GetAllParents(this DependencyObject element, bool includeCurrent = true)
		{
			if (includeCurrent)
			{
				yield return element;
			}

			for (var parent = (element as FrameworkElement)?.Parent ?? VisualTreeHelper.GetParent(element);
				parent != null;
				parent = (parent as FrameworkElement)?.Parent ?? VisualTreeHelper.GetParent(parent))
			{
				yield return parent;
			}
		}

		/// <summary>
		/// Search for the first parent of the given type.
		/// </summary>
		/// <typeparam name="T">The type of child we are looking for</typeparam>
		/// <param name="element">The element to search from</param>
		/// <param name="includeCurrent">Determines if the current <paramref name="element"/> should be tested or not.</param>
		/// <returns>The first found parent that is of the given type.</returns>
		public static T? FindFirstParent<T>(this DependencyObject element, bool includeCurrent = true)
			where T : DependencyObject
			=> element.GetAllParents(includeCurrent).OfType<T>().FirstOrDefault();

#if HAS_UNO
		/// <summary>
		/// Set the parent of the specified dependency object
		/// </summary>
		internal static void SetParent(this DependencyObject dependencyObject, object? parent)
		{
			if (parent != null
				&& dependencyObject is IDependencyObjectStoreProvider storeProvider
				&& (!ReferenceEquals(storeProvider.Store.Parent, parent)))
			{
				storeProvider.Store.Parent = parent;
			}
		}
#endif

		public static DependencyProperty? FindDependencyPropertyUsingReflection<TProperty>(this DependencyObject dependencyObject, string propertyName)
		{
			var type = dependencyObject.GetType();
			var propertyType = typeof(TProperty);
			var key = (ownerType: type, propertyName);

			if (_dependencyPropertyReflectionCache.TryGetValue(key, out var property))
			{
				return property;
			}

			property = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Static)?.GetValue(null) as DependencyProperty
				?? type.GetField(propertyName, BindingFlags.Public | BindingFlags.Static)?.GetValue(null) as DependencyProperty;

			if (property == null)
			{
#if HAS_UNO
				dependencyObject.Log().LogWarning($"The {propertyName} dependency property does not exist on {type}");
#endif
			}
#if HAS_UNO
			else if (property.GetType() != propertyType)
			{
				dependencyObject.Log().LogWarning($"The {propertyName} dependency property {type} is not of the {propertyType} Type.");
				property = null;
			}
#endif

			_dependencyPropertyReflectionCache[key] = property;

			return property;
		}

		public static bool TryGetValue(this DependencyObject dependencyObject, DependencyProperty dependencyProperty, out DependencyObject? value)
		{
			value = default;

			try
			{
				value = dependencyObject.GetValue(dependencyProperty) as DependencyObject;
			}
			catch (Exception e)
			{
				typeof(DependencyObjectExtensions).Log().WarnIfEnabled(() => $"Error attempting to read property:", e);
				return false;
			}

			return true;
		}
	}
}
