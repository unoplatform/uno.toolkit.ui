using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
#endif

namespace Uno.Toolkit.UI
{
	/// <summary>
	/// Provides a mean to bind to an ancestor of a specific type.
	/// </summary>
	public class AncestorBindingExtension : MarkupExtension
	{
#if WINDOWS_UWP
		/// <summary>
		/// Define whether <see cref="ProvideValue()" /> should throw or just result null.
		/// </summary>
		public static bool ShouldThrow = true;
#endif

		private static readonly Dictionary<(Type, string), PropertyInfo?> _targetPropertyLookupCache = new();

		/// <summary>
		/// Binding path from the ancestor.
		/// </summary>
		public string? Path { get; set; }

		// note: Type literal are not recognized by Uno's XamlSourceGenerator for Nullable<Type>.
		/// <summary>
		/// Type of ancestor to bind from.
		/// </summary>
		public Type AncestorType { get; set; } = typeof(object);

		public AncestorBindingExtension()
		{
		}

#if WINDOWS_UWP
		protected override object? ProvideValue()
		{
			const string Message = "This feature is not supported on UWP for windows as it depends on WinUI3 api. It still works on all non-Windows UWP platforms and all WinUI 3 platforms.";
			return ShouldThrow ? throw new PlatformNotSupportedException(Message) : null;
		}
#else
		protected override object? ProvideValue(IXamlServiceProvider serviceProvider)
		{
			if (serviceProvider.GetService(typeof(IProvideValueTarget)) is not IProvideValueTarget pvt) return null;
			if (pvt.TargetObject is not FrameworkElement owner) return null;
			if (pvt.TargetProperty is not ProvideValueTargetProperty { DeclaringType: { } ownerType, Name: { } propertyName }) return null;
			if (FindTargetDependencyProperty(ownerType, propertyName)?.GetValue(pvt.TargetObject) is not DependencyProperty property) return null;

			owner.Loaded += OnTargetLoaded;
			void OnTargetLoaded(object s, RoutedEventArgs e)
			{
				if (s is FrameworkElement fe)
				{
					fe.Loaded -= OnTargetLoaded;

					if (GetAncestors(fe).FirstOrDefault(x => AncestorType?.IsAssignableFrom(x.GetType()) == true) is { } source)
					{
						var binding = new Binding
						{
							Path = new PropertyPath(Path),
							Source = source,
							Mode = BindingMode.OneWay,
						};
						fe.SetBinding(property, binding);
					}
				}
			}

			return null;
		}
#endif

		private static PropertyInfo? FindTargetDependencyProperty(Type ownerType, string propertyName)
		{
			var key = (ownerType, propertyName);
			if (!_targetPropertyLookupCache.TryGetValue(key, out var value))
			{
				value = ownerType.GetProperty(propertyName + "Property", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
				_targetPropertyLookupCache[key] = value;
			}

			return value;
		}

		private static IEnumerable<DependencyObject> GetAncestors(DependencyObject x)
		{
			if (x is null) yield break;
			while (VisualTreeHelper.GetParent(x) is { } parent)
			{
				yield return x = parent;
			}
		}
	}
}
