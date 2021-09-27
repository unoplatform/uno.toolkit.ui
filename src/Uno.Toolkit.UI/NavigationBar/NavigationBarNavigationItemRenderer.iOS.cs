#if __IOS__
using CoreGraphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;
using Uno.Disposables;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.Foundation.Collections;
using Uno.UI.ToolkitLib.Extensions;
using Uno.Extensions;
using AppBarButton = Microsoft.UI.Xaml.Controls.AppBarButton;
using ICommandBarElement = Microsoft.UI.Xaml.Controls.ICommandBarElement;
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

namespace Uno.UI.ToolkitLib
{
	internal partial class NavigationBarNavigationItemRenderer : Renderer<NavigationBar, UINavigationItem>
	{
		private ContentPresenter? _contentPresenter;

		private readonly SerialDisposable _visibilitySubscriptions = new SerialDisposable();

		public NavigationBarNavigationItemRenderer(NavigationBar element) : base(element) { }

		protected override UINavigationItem CreateNativeInstance() => new UINavigationItem();

		protected override IEnumerable<IDisposable> Initialize()
		{
			if (Element is not { } element)
			{
				yield break;
			}

			// Content
			var titleView = new TitleView();
			_contentPresenter = new ContentPresenter();
			_contentPresenter.Content = titleView;
			//_titleView.SetParent(element);
			//_titleView.RegisterParentChangedCallback(this, OnTitleViewParentChanged);

			// Commands
			void OnVectorChanged(IObservableVector<ICommandBarElement> s, IVectorChangedEventArgs e)
			{
				RegisterCommandVisibilityAndInvalidate();
			}

			element.PrimaryCommands.VectorChanged += OnVectorChanged;
			element.SecondaryCommands.VectorChanged += OnVectorChanged;

			void Unregister()
			{
				_visibilitySubscriptions.Disposable = null;
				_contentPresenter = null;
				element.PrimaryCommands.VectorChanged -= OnVectorChanged;
				element.SecondaryCommands.VectorChanged -= OnVectorChanged;
			}

			yield return Disposable.Create(Unregister);

			// Properties
			yield return element.RegisterDisposableNestedPropertyChangedCallback(
				(s, e) => RegisterCommandVisibilityAndInvalidate(),
				new[] { NavigationBar.PrimaryCommandsProperty },
				new[] { NavigationBar.ContentProperty },
				new[] { NavigationBar.LeftCommandProperty },
				new[] { NavigationBar.LeftCommandProperty, AppBarButton.VisibilityProperty },
				new[] { NavigationBar.LeftCommandProperty, AppBarButton.ContentProperty }
			);

			RegisterCommandVisibilityAndInvalidate();
		}

		protected override void Render()
		{
			var native = Native;
			var element = Element;

			if (native == null)
			{
				throw new InvalidOperationException("Native should not be null.");
			}

			// Content
			var content = element?.Content;

			native.Title = content as string;
			native.TitleView = content is UIElement ? _contentPresenter : null;
			if (_contentPresenter?.Content is TitleView titleView)
			{
				titleView.Child = content as UIElement;
			}

			// PrimaryCommands
			native.RightBarButtonItems = element?
				.PrimaryCommands
				.OfType<AppBarButton>()
				.Where(btn => btn.Visibility == Visibility.Visible && (((btn.Content as FrameworkElement)?.Visibility ?? Visibility.Visible) == Visibility.Visible))
				.Select(appBarButton => appBarButton.GetRenderer(() => new AppBarButtonRenderer(appBarButton)).Native)
				.Reverse()
				.ToArray();

			// CommandBarExtensions.NavigationCommand
			var navigationCommand = element.GetValue(NavigationBar.LeftCommandProperty) as AppBarButton;
			if (navigationCommand?.Visibility == Visibility.Visible)
			{
				native.LeftBarButtonItem = navigationCommand.GetRenderer(() => new AppBarButtonRenderer(navigationCommand)).Native;
			}
			else
			{
				native.LeftBarButtonItem = null;
			}

			// CommandBarExtensions.BackButtonText
			if (navigationCommand?.Content is string backButtonText)
			{
				native.BackBarButtonItem = new UIBarButtonItem(backButtonText, UIBarButtonItemStyle.Plain, null);
			}
			else
			{
				native.BackBarButtonItem = null;
			}
		}

		private void RegisterCommandVisibilityAndInvalidate()
		{
			var disposables = Element
				?.PrimaryCommands
				.OfType<AppBarButton>()
				.Select(command => command.RegisterDisposableNestedPropertyChangedCallback(
					(s, e) => Invalidate(),
					new[] { AppBarButton.VisibilityProperty },
					new[] { AppBarButton.ContentProperty, FrameworkElement.VisibilityProperty }
				));

			if (disposables is { })
			{
				_visibilitySubscriptions.Disposable = new CompositeDisposable(disposables);
			}

			Invalidate();
		}
	}

	internal partial class TitleView : Border
	{
		private bool _blockReentrantMeasure;
		private Size _childSize;
		private Size? _lastAvailableSize;

		public override void SetSuperviewNeedsLayout()
		{
			// Skip the base invocation because the base fetches the native parent
			// view. This process creates a managed proxy during navigations, the
			// native counterpart is released. This causes the managed NSObject_Disposer:Drain
			// to fail to release an already released native reference.
			//
			// See https://github.com/unoplatform/uno/issues/7012 for more details.
		}

		internal TitleView()
		{
			if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
			{
				// Set the Frame to the full screen size so that the child can measure itself properly.
				// It will be constrained later on.
				var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
				Frame = new CGRect(new CGPoint(0, 0), new CGSize(bounds.Width, bounds.Height));
			}
			else
			{
				// For iOS 9 and 10, we need to do weird stuff with the initial frame.
				// The 0 width: Prevents flickers
				// The 44 height: Gives a valid default size that will be reused (god knows why) even after setting the height later on.
				Frame = new CGRect(new CGPoint(0, 0), new CGSize(0, 44));
			}
		}

		protected override void OnBeforeArrange()
		{
			//This is to ensure that the layouter gets the correct **finalRect**
			//LayoutSlotWithMarginsAndAlignments = RectFromUIRect(Frame);
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			if (_blockReentrantMeasure)
			{
				// In some cases setting the Frame below can prompt iOS to call SizeThatFits with 0 height, which would screw up the desired
				// size of children if we're within LayoutSubviews(). In this case simply return the existing measure.
				return _childSize;
			}

			//for the same reason expressed above we need to check if iOS sent a wrong availableSize which can be validated
			// by checking the availableSize.Height
			if (availableSize.Height == 0 && _lastAvailableSize?.Height != 0)
			{
				return _childSize;
			}

			try
			{
				_blockReentrantMeasure = true;

				_lastAvailableSize = availableSize;

				// By default, iOS will horizontally center the TitleView inside the UINavigationBar,
				// ignoring the size of the left and right buttons.

				_childSize = base.MeasureOverride(availableSize);

				if (Child is FrameworkElement frameworkElement
					&& frameworkElement.HorizontalAlignment == HorizontalAlignment.Stretch)
				{
					// To make the content stretch horizontally (instead of being centered),
					// we can set HorizontalAlignment.Stretch on it.
					// This will force the TitleView to take all available horizontal space.
					_childSize.Width = availableSize.Width;
				}
				else
				{
					if (!_childSize.Width.IsNaN()
						&& !_childSize.Height.IsNaN()
						&& _childSize.Height != 0
						&& _childSize.Width != 0
						&& ((Frame.Width != _childSize.Width && _childSize.Width < Frame.Width)
						|| (Frame.Height != _childSize.Height && _childSize.Height < Frame.Height)))
					{
						// Set the frame size to the child size so that the OS centers properly.
						// but only when the Frame is bigger than the Child preventing cases where
						// the Child Size is bigger making the Frame to overflow the Available Size.

						var width = _childSize.Width < Frame.Width ? _childSize.Width : Frame.Width;
						var height = _childSize.Height < Frame.Height ? _childSize.Height : Frame.Height;

						Frame = new CGRect(Frame.X, Frame.Y, width, height);
					}
				}

				return _childSize;
			}
			finally
			{
				_blockReentrantMeasure = false;
			}
		}

		public override CGRect Frame
		{
			get { return base.Frame; }
			set
			{
				if (!UIDevice.CurrentDevice.CheckSystemVersion(11, 0)
					// iOS likes to mix things up by calling SizeThatFits with zero Height for no apparent reason (eg when navigating back to a page). When
					// this happens, we need to remeasure with the correct size to ensure children are laid out correctly.
					|| (_lastAvailableSize?.Height == 0 && value.Height != 0))
				{
					// This allows text trimming when there are more AppBarButtons
					var availableSize = value.Size;
					base.MeasureOverride(availableSize);
				}

				base.Frame = value;
			}
		}
	}
}
#endif
