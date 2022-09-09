using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Uno.Extensions;
using Uno.Logging;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
#endif

namespace Uno.Toolkit.UI
{
	public static partial class TabBarItemExtensions
	{
		/// <summary>
		/// Specifies the on-click behaviors of <see cref="TabBarItem"/>.
		/// </summary>
		[Flags]
		public enum TBIOnClickBehaviors
		{
			None = 0,

			/// <summary>
			/// Find the first <see cref="NavigationView"/> with back stack to back navigate.
			/// </summary>
			BackNavigation = 1 << 0,

			/// <summary>
			/// Find the first <see cref="ListView"/> or <see cref="ScrollViewer"/> to reset scroll position.
			/// </summary>
			ScrollToTop = 1 << 1,

			/// <summary>
			/// All of above.
			/// </summary>
			Auto = BackNavigation | ScrollToTop,
		}
	}

	public static partial class TabBarItemExtensions
	{
		private static readonly ILogger Logger = typeof(TabBarItemExtensions).Log();

		#region DependencyProperty: OnClickBehaviors

		/// <summary>
		/// Backing property for the <see cref="TabBarItem"/> on-click behaviors when already selected.
		/// </summary>
		public static DependencyProperty OnClickBehaviorsProperty { get; } = DependencyProperty.RegisterAttached(
			"OnClickBehaviors",
			typeof(TBIOnClickBehaviors),
			typeof(TabBarItemExtensions),
			new PropertyMetadata(default(TBIOnClickBehaviors), OnOnClickBehaviorsChanged));

		public static TBIOnClickBehaviors GetOnClickBehaviors(TabBarItem obj) => (TBIOnClickBehaviors)obj.GetValue(OnClickBehaviorsProperty);
		public static void SetOnClickBehaviors(TabBarItem obj, TBIOnClickBehaviors value) => obj.SetValue(OnClickBehaviorsProperty, value);

		#endregion
		#region DependencyProperty: OnClickBehaviorsTarget

		/// <summary>
		/// Optional. Backing property for the target of <see cref="OnClickBehaviorsProperty"/>.
		/// </summary>
		/// <remarks>
		/// The content host which the on-click behavior is applied is either
		/// the target itself or one of its descendants (via deep first search) suitable for the behavior.
		/// When omitted, the parent of <see cref="TabBar"/> will serve as the target.
		/// </remarks>
		public static DependencyProperty OnClickBehaviorsTargetProperty { get; } = DependencyProperty.RegisterAttached(
			"OnClickBehaviorsTarget",
			typeof(UIElement),
			typeof(TabBarItemExtensions),
			new PropertyMetadata(default(UIElement)));

		public static UIElement GetOnClickBehaviorsTarget(TabBarItem obj) => (UIElement)obj.GetValue(OnClickBehaviorsTargetProperty);
		public static void SetOnClickBehaviorsTarget(TabBarItem obj, UIElement value) => obj.SetValue(OnClickBehaviorsTargetProperty, value);

		#endregion
		#region DependencyProperty: OnClickBehaviorsSubscription [private]

		private static DependencyProperty OnClickBehaviorsSubscriptionProperty { get; } = DependencyProperty.RegisterAttached(
			"OnClickBehaviorsSubscription",
			typeof(IDisposable),
			typeof(TabBarItemExtensions),
			new PropertyMetadata(default(IDisposable)));

		private static IDisposable GetOnClickBehaviorsSubscription(TabBarItem obj) => (IDisposable)obj.GetValue(OnClickBehaviorsSubscriptionProperty);
		private static void SetOnClickBehaviorsSubscription(TabBarItem obj, IDisposable value) => obj.SetValue(OnClickBehaviorsSubscriptionProperty, value);

		#endregion

		private static void OnOnClickBehaviorsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not TabBarItem tbi) return;

			// We are indirectly hooking to TBI.Click when TBI.IsSelected changes to true,
			// because TBI.IsSelected cannot be reliably used to confirm navigation had occurred.
			// When clicking on an unselected TBI, the value is still false for uwp, but on uno that is already true.

			GetOnClickBehaviorsSubscription(tbi)?.Dispose();
			tbi.Click -= OnTabBarItemClick;

			if (e.NewValue is TBIOnClickBehaviors and not TBIOnClickBehaviors.None)
			{
				OnTabBarItemIsSelectedChanged(tbi, TabBarItem.IsSelectableProperty);
				SetOnClickBehaviorsSubscription(tbi, tbi.RegisterDisposablePropertyChangedCallback(TabBarItem.IsSelectedProperty, OnTabBarItemIsSelectedChanged));
			}
		}

		private static void OnTabBarItemIsSelectedChanged(DependencyObject sender, DependencyProperty dp)
		{
			if (sender is not TabBarItem tbi) return;

			if (tbi.IsSelected)
			{
				tbi.Click += OnTabBarItemClick;
			}
			else
			{
				tbi.Click -= OnTabBarItemClick;
			}
		}

		private static void OnTabBarItemClick(object sender, RoutedEventArgs e)
		{
			if (sender is not TabBarItem tbi) return;

			bool IsValidContentHost(object x) =>
				x is ListView ||
				x is ScrollViewer ||
				x is Frame { CanGoBack: true };

			// defaults to the parent of TabBar if not provided
			var target = GetOnClickBehaviorsTarget(tbi);
			var context = target ?? VisualTreeHelperEx.GetAncestors(tbi)
				.OfType<UIElement>()
				.SkipWhile(x => x is not TabBar).Skip(1)
				.FirstOrDefault();
			if (context is null) return;

			var contentHost = IsValidContentHost(context) ? context
#if __IOS__ || __ANDROID__
				// native view may not be fully crawlable using VisualTreeHelper.GetChild
				: VisualTreeHelperEx.Native.GetFirstDescendant<DependencyObject>(
#else
				: VisualTreeHelperEx.GetFirstDescendant<DependencyObject>(
#endif
					context,
					hierarchyPredicate: x => (x as UIElement)?.Visibility != Visibility.Collapsed,
					predicate: IsValidContentHost
				);

			switch (contentHost)
			{
				case ListView lv: ScrollableHelper.SmoothScrollTop(lv); break;
				case ScrollViewer sv: sv.ChangeView(0, 0, zoomFactor: default, disableAnimation: false); break;
				case Frame f: if (f.CanGoBack) f.GoBack(); break;

				default:
					Logger.WarnIfEnabled(() => "No suitable content host found in the visual tree.");
					break;
			}
		}
	}
}
