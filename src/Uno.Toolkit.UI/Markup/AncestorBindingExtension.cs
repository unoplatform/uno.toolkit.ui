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
			// until we can reach/resolve 'property', we have no option but to return null.
			// while, this would crash if this is set on a dp that doesn't accept null,
			// there is really nothing else we can salvage from here.
			// but usually that is also indicative of another problem: using this on an invalid target/property.
			if (serviceProvider.GetService(typeof(IProvideValueTarget)) is not IProvideValueTarget pvt) return null;
			if (pvt.TargetObject is not FrameworkElement owner) return null;
			if (pvt.TargetProperty is not ProvideValueTargetProperty { DeclaringType: { } ownerType, Name: { } propertyName }) return null;
			if (ownerType.FindDependencyProperty(propertyName) is not { } property) return null;

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

			// return current value, until the binding comes online.
			return owner.GetValue(property);
		}
#endif

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
