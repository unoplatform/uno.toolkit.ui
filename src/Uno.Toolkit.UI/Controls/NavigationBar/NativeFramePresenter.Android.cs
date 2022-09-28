#if __ANDROID__
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Uno.Extensions;
using Uno.Logging;

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Android.App;
using Android.Views.Animations;
using Android.Views;
using Uno.Extensions.Specialized;
using System.Threading;
using Windows.UI.Core;
using Uno.Disposables;
using Uno.UI.Helpers;
using Uno.UI;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Media.Animation;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Animation;
#endif

namespace Uno.Toolkit.UI
{
	public partial class NativeFramePresenter : Grid // Inheriting from Grid is a hack to remove 1 visual layer (Android 4.4 stack size limits)
	{
		private readonly Grid? _pageStack;
		private Frame? _frame;
		private bool _isUpdatingStack;
		private PageStackEntry? _currentEntry;
		private Queue<(PageStackEntry? pageEntry, NavigationEventArgs args)> _stackUpdates = new Queue<(PageStackEntry?, NavigationEventArgs)>();

		public NativeFramePresenter()
		{
			_pageStack = this;
		}

		protected override void OnTemplatedParentChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnTemplatedParentChanged(e);
			Initialize(TemplatedParent as Frame);
		}

		private void Initialize(Frame? frame)
		{
			if (frame == null || _frame == frame)
			{
				return;
			}

			_frame = frame;
			_frame.Navigated += OnNavigated;
			if (_frame.BackStack is ObservableCollection<PageStackEntry> backStack)
			{
				backStack.CollectionChanged += OnBackStackChanged;
			}

			if (_frame.Content is Page startPage)
			{
				_stackUpdates.Enqueue((FrameNavigationHelper.GetCurrentEntry(_frame), FrameNavigationHelper.CreateNavigationEventArgs(_frame.Content, NavigationMode.New, null, null, null!, null)));
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
				InvalidateStack();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
			}
		}

		private void OnBackStackChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			UpdateBackButtonVisibility();
		}

		private void UpdateBackButtonVisibility()
		{
			var pageEntry = FrameNavigationHelper.GetCurrentEntry(_frame);
			var page = FrameNavigationHelper.GetInstance(pageEntry);

			var navBar = page?.FindFirstChild<NavigationBar>();
			navBar?.UpdateMainCommandVisibility();
		}

		private void OnNavigated(object sender, NavigationEventArgs e)
		{
			_stackUpdates.Enqueue((FrameNavigationHelper.GetCurrentEntry(_frame), e));

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
			InvalidateStack();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
		}

		private async Task InvalidateStack()
		{
			if (_isUpdatingStack)
			{
				return;
			}

			_isUpdatingStack = true;

			while (_stackUpdates.Any())
			{
				var navigation = _stackUpdates.Dequeue();
				await UpdateStack(navigation.pageEntry, navigation.args);
			}

			_isUpdatingStack = false;
		}

		private async Task UpdateStack(PageStackEntry? entry, NavigationEventArgs e)
		{
			var oldEntry = _currentEntry;
			var newEntry = entry;

			var newPage = FrameNavigationHelper.GetInstance(newEntry);
			var oldPage = FrameNavigationHelper.GetInstance(oldEntry);

			if (newPage == null || newPage == oldPage)
			{
				return;
			}

			switch (e.NavigationMode)
			{
				case NavigationMode.Forward:
				case NavigationMode.New:
				case NavigationMode.Refresh:
					_pageStack?.Children.Add(newPage);
					if (GetIsAnimated(newEntry))
					{
						await newPage.AnimateAsync(GetEnterAnimation());
						newPage.ClearAnimation();
					}
					if (FeatureConfiguration.NativeFramePresenter.AndroidUnloadInactivePages && oldPage != null)
					{
						_pageStack?.Children.Remove(oldPage);
					}
					break;
				case NavigationMode.Back:
					if (FeatureConfiguration.NativeFramePresenter.AndroidUnloadInactivePages)
					{
						_pageStack?.Children.Insert(0, newPage);
					}
					if (GetIsAnimated(oldEntry))
					{
						if (oldPage != null)
						{
							await oldPage.AnimateAsync(GetExitAnimation());
							oldPage.ClearAnimation();
						}
					}


					if (oldPage != null)
					{
						_pageStack?.Children.Remove(oldPage);
					}

					if (!FeatureConfiguration.NativeFramePresenter.AndroidUnloadInactivePages)
					{
						// Remove pages from the grid that may have been removed from the BackStack list
						// Those items are not removed on BackStack list changes to avoid interfering with the GoBack method's behavior.
						for (var pageIndex = _pageStack!.Children.Count - 1; pageIndex >= 0; pageIndex--)
						{
							var page = _pageStack.Children[pageIndex];
							if (page == newPage)
							{
								break;
							}

							_pageStack.Children.Remove(page);
						}

						//In case we cleared the whole stack. This should never happen
						if (_pageStack.Children.Count == 0)
						{
							_pageStack.Children.Insert(0, newPage);
						}
					}
				break;
			}

			_currentEntry = newEntry;
		}


		private static bool GetIsAnimated(PageStackEntry? entry)
		{
			return !(entry?.NavigationTransitionInfo is SuppressNavigationTransitionInfo);
		}

		private static Animation GetEnterAnimation()
		{
			// Source:
			// https://android.googlesource.com/platform/frameworks/base/+/android-cts-7.1_r5/core/res/res/anim/activity_open_enter.xml

			var enterAnimation = new AnimationSet(false)
			{
				ZAdjustment = ContentZorder.Top,
				FillAfter = true
			};

			enterAnimation.AddAnimation(new AlphaAnimation(0, 1)
			{
				Interpolator = new DecelerateInterpolator(2), // DecelerateQuart
				FillEnabled = true,
				FillBefore = false,
				FillAfter = true,
				Duration = 200,
			});

			enterAnimation.AddAnimation(new TranslateAnimation(
				Dimension.Absolute, 0,
				Dimension.Absolute, 0,
				Dimension.RelativeToSelf, 0.08f,
				Dimension.Absolute, 0
			)
			{
				FillEnabled = true,
				FillBefore = true,
				FillAfter = true,
				Interpolator = new DecelerateInterpolator(2.5f), // DecelerateQuint
				Duration = 350,
			});

			return enterAnimation;
		}

		private static Animation GetExitAnimation()
		{
			// Source:
			// https://android.googlesource.com/platform/frameworks/base/+/android-cts-7.1_r5/core/res/res/anim/activity_close_exit.xml

			var exitAnimation = new AnimationSet(false)
			{
				ZAdjustment = ContentZorder.Top,
				FillAfter = true
			};

			exitAnimation.AddAnimation(new AlphaAnimation(1, 0)
			{
				Interpolator = new LinearInterpolator(),
				FillEnabled = true,
				FillBefore = false,
				FillAfter = true,
				StartOffset = 100,
				Duration = 150,
			});

			exitAnimation.AddAnimation(new TranslateAnimation(
				Dimension.Absolute, 0,
				Dimension.Absolute, 0,
				Dimension.RelativeToSelf, 0.0f,
				Dimension.RelativeToSelf, 0.08f
			)
			{
				FillEnabled = true,
				FillBefore = true,
				FillAfter = true,
				Interpolator = new AccelerateInterpolator(2), // AccelerateQuart
				Duration = 250,
			});

			return exitAnimation;
		}
	}
}
#endif
