
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.Collections;
using Uno.Disposables;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
#endif

namespace Uno.Toolkit.UI
{
	internal abstract class Renderer<TElement, TNative> : IDisposable
			where TElement :
#if HAS_UNO
			class,
#endif
			DependencyObject
			where TNative : class
	{
		private CompositeDisposable _subscriptions = new CompositeDisposable();
		private readonly WeakReference<TElement> _element;
		private TNative? _native;
		private bool _isRendering;

		public Renderer(TElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException(nameof(element));
			}

			if (element is FrameworkElement fe)
			{
				fe.Unloaded += (s, e) => Dispose();
			}

			_element = new WeakReference<TElement>(element);
		}

		public TElement? Element
		{
			get
			{
				if (_element.TryGetTarget(out var element) && element != null)
				{
					return element;
				}

				return default(TElement);
			}
		}

		// The property is annotated non-nullable because its getter cannot return null.
		// However, it can be set to null, so we add AllowNull attribute.
		[AllowNull]
		public TNative Native
		{
			get
			{
				if (_native == null)
				{
					// No Native instance was given.
					// We assume that the renderer knows how to create a new instance.
					_native = CreateNativeInstance();
					OnNativeChanged();
				}

				return _native;
			}
			set
			{
				if (!ReferenceEquals(_native, value))
				{
					_native = value;
					OnNativeChanged();
				}
			}
		}

		public bool HasNative => _native != null;

		private void OnNativeChanged()
		{
			// We remove subscriptions to the previous pair of element and native 
			_subscriptions.Dispose();

			if (HasNative)
			{
				_subscriptions = new CompositeDisposable(Initialize());
			}

			Invalidate();
		}

		protected abstract TNative CreateNativeInstance();

		public void Invalidate()
		{
			// We don't render anything if there's no rendering target
			if (HasNative
				// Prevent Render() being called reentrantly - this can happen when the Element's parent changes within the Render() method
				&& !_isRendering)
			{
				try
				{
					_isRendering = true;
					Render();
				}
				finally
				{
					_isRendering = false;
				}
			}
		}

		protected abstract IEnumerable<IDisposable> Initialize();

		protected abstract void Render();

		public void Dispose()
		{
			_subscriptions.Dispose();
		}
	}

	internal static class RendererHelper
	{
		private static readonly WeakAttachedDictionary<DependencyObject, Type> _renderers = new WeakAttachedDictionary<DependencyObject, Type>();

		public static TRenderer? TryGetRenderer<TElement, TRenderer>(this TElement element)
			where TElement : DependencyObject
			where TRenderer : class
		{
			TRenderer? renderer = null;
			if (_renderers.GetValue<TRenderer>(element, typeof(TRenderer)) is { } existingRenderer)
			{
				renderer = existingRenderer;
			}

			return renderer;
		}

		public static bool TryGetNative<TElement, TRenderer, TNative>(this TElement element, out TNative? native)
			where TElement :
#if HAS_UNO
			class,
#endif
			DependencyObject
			where TRenderer : Renderer<TElement, TNative>
			where TNative : class
		{
			native = null;
			if (TryGetRenderer<TElement, TRenderer>(element) is { } renderer && renderer.HasNative)
			{
				native = renderer.Native;
				return true;
			}

			return false;
		}

		public static void SetRenderer<TElement, TRenderer>(this TElement element, TRenderer? renderer)
			where TElement : DependencyObject
		{
			_renderers.SetValue(element, typeof(TRenderer), renderer);
		}

		public static void AddOrUpdateRenderer<TElement, TRenderer>(this TElement element, Func<TElement, TRenderer> onCreate, Action<TElement, TRenderer>? onUpdate = null)
			where TElement : DependencyObject
			where TRenderer : class
		{
			if (_renderers.GetValue<TRenderer>(element, typeof(TRenderer)) is { } renderer)
			{
				onUpdate?.Invoke(element, renderer);
			}
			else
			{
				element.SetRenderer(onCreate(element));
			}
		}

		public static TRenderer GetOrAddRenderer<TElement, TRenderer>(this TElement element, Func<TElement, TRenderer> rendererFactory)
			where TElement : DependencyObject
			where TRenderer : class
		{
			if (_renderers.GetValue<TRenderer>(element, typeof(TRenderer)) is not { } existingRenderer)
			{
				existingRenderer = rendererFactory(element);
				element.SetRenderer(existingRenderer);
			}

			return existingRenderer;
		}

#if __IOS__ || __ANDROID__
		public static AppBarButtonRenderer GetOrAddDefaultRenderer(this AppBarButton appBarButton)
			=> GetOrAddRenderer(appBarButton, elt => new AppBarButtonRenderer(elt));

		public static NavigationBarRenderer GetOrAddDefaultRenderer(this NavigationBar navBar)
			=> GetOrAddRenderer(navBar, elt => new NavigationBarRenderer(elt));
#endif
	}
}
