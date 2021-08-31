#if __ANDROID__
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Graphics.Drawables;
using AndroidX.AppCompat.Widget;
using Microsoft.Extensions.Logging;
using Android.Views;
using Uno.Disposables;
using Uno.Extensions;
using Uno.Logging;
using Uno.UI.ToolkitLib.Extensions;
using Windows.Foundation.Collections;
using AndroidX.Core.Graphics.Drawable;
using Windows.UI.Core;
using Android.Views.InputMethods;
using Android.Content;
using Uno.UI.Extensions;
using Uno.UI.ToolkitLib.Helpers;
using Windows.UI;
using ColorHelper = Uno.UI.ToolkitLib.Helpers.ColorHelper;


#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Automation.Peers;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Automation.Peers;
#endif

namespace Uno.UI.ToolkitLib
{
	internal partial class NavigationBarRenderer : Renderer<NavigationBar, Toolbar>
	{
		private static string? _actionBarUpDescription;
		private static string? ActionBarUpDescription
		{
			get
			{
				if (_actionBarUpDescription == null)
				{
					if (ContextHelper.Current is Activity activity
						&& activity.Resources?.GetIdentifier("action_bar_up_description", "string", "android") is { } resourceId)
					{
						_actionBarUpDescription = activity.Resources.GetString(resourceId);
					}
					else
					{
						if (typeof(NavigationBarRenderer).Log().IsEnabled(LogLevel.Error))
						{
							typeof(NavigationBarRenderer).Log().Error("Couldn't resolve resource 'action_bar_up_description'.");
						}
					}
				}

				return _actionBarUpDescription;
			}
		}

		private Android.Graphics.Color? _originalTitleTextColor;
		private Android.Graphics.Drawables.Drawable? _originalBackground;
		private Border? _contentContainer;

		public NavigationBarRenderer(NavigationBar element) : base(element) { }

		protected override Toolbar CreateNativeInstance() => new Toolbar(ContextHelper.Current);

		protected override IEnumerable<IDisposable> Initialize()
		{
			var native = Native;
			_originalBackground = native.Background;
			_originalTitleTextColor = native.GetTitleTextColor();

			// Content
			// This allows custom Content to be properly laid out inside the native Toolbar.
			_contentContainer = new Border()
			{
				Visibility = Visibility.Collapsed,
				// This container requires a fixed height to be properly laid out by its native parent.
				// According to Google's Material Design Guidelines, the Toolbar must have a minimum height of 48.
				// https://material.io/guidelines/layout/structure.html
				Height = 48,
				Name = "NavigationBarRendererContentHolder",
				
				// Set the alignment so that the measured sized
				// returned is size of the child, not the available
				// size provided to the ToolBar view.
				VerticalAlignment = VerticalAlignment.Top,
				HorizontalAlignment = HorizontalAlignment.Left,
			};

			native.AddView(_contentContainer);
			yield return Disposable.Create(() => native.RemoveView(_contentContainer));

			// Commands.Click
			native.MenuItemClick += Native_MenuItemClick;
			yield return Disposable.Create(() => native.MenuItemClick -= Native_MenuItemClick);

			// NavigationCommand.Click
			native.NavigationClick += Native_NavigationClick;
			yield return Disposable.Create(() => native.NavigationClick -= Native_NavigationClick);

			// Commands
			VectorChangedEventHandler<ICommandBarElement> OnVectorChanged = (s, e) => Invalidate();
			if (Element is { } element)
			{
				element.PrimaryCommands.VectorChanged += OnVectorChanged;
				element.SecondaryCommands.VectorChanged += OnVectorChanged;
				yield return Disposable.Create(() => element.PrimaryCommands.VectorChanged -= OnVectorChanged);
				yield return Disposable.Create(() => element.SecondaryCommands.VectorChanged -= OnVectorChanged);


				// Properties
				yield return element.RegisterDisposableNestedPropertyChangedCallback(
					(s, e) => Invalidate(),
					new[] { CommandBar.PrimaryCommandsProperty },
					new[] { CommandBar.SecondaryCommandsProperty },
					new[] { CommandBar.ContentProperty },
					new[] { CommandBar.ForegroundProperty },
					new[] { CommandBar.ForegroundProperty, SolidColorBrush.ColorProperty },
					new[] { CommandBar.ForegroundProperty, SolidColorBrush.OpacityProperty },
					new[] { CommandBar.BackgroundProperty },
					new[] { CommandBar.BackgroundProperty, SolidColorBrush.ColorProperty },
					new[] { CommandBar.BackgroundProperty, SolidColorBrush.OpacityProperty },
					new[] { CommandBar.VisibilityProperty },
					new[] { CommandBar.PaddingProperty },
					new[] { CommandBar.OpacityProperty },
					new[] { CommandBar.HorizontalContentAlignmentProperty },
					new[] { CommandBar.VerticalContentAlignmentProperty },
					new[] { CommandBar.OpacityProperty },
					new[] { NavigationBar.SubtitleProperty },
					new[] { NavigationBar.MainCommandProperty },
					new[] { NavigationBar.MainCommandProperty, AppBarButton.VisibilityProperty },
					new[] { NavigationBar.MainCommandProperty, AppBarButton.ForegroundProperty },
					new[] { NavigationBar.MainCommandProperty, AppBarButton.IconProperty }
				);
			}
		}

		protected override void Render()
		{
			if (_contentContainer == null)
			{
				throw new InvalidOperationException();
			}
			var native = Native;
			if (Element is not { } element)
			{
				return;
			}

			// Content
			var content = element.Content;
			native.Title = content as string;
			_contentContainer.Child = content as UIElement;
			_contentContainer.VerticalAlignment = element.VerticalContentAlignment;
			_contentContainer.HorizontalAlignment = element.HorizontalContentAlignment;
			_contentContainer.Visibility = content is UIElement
				? Visibility.Visible
				: Visibility.Collapsed;

			// CommandBarExtensions.Subtitle
			native.Subtitle = element.GetValue(NavigationBar.SubtitleProperty) as string;

			// Background
			if (ColorHelper.TryGetColorWithOpacity(element.Background, out var backgroundColor))
			{
				native.SetBackgroundColor((Android.Graphics.Color)backgroundColor);
			}
			else
			{
				native.Background = _originalBackground ?? new ColorDrawable(Color.FromArgb(255, 250, 250, 250));
			}

			// Foreground
			if (ColorHelper.TryGetColorWithOpacity(element.Foreground, out var foregroundColor))
			{
				native.SetTitleTextColor((Android.Graphics.Color)foregroundColor);
			}
			else if (_originalTitleTextColor != null)
			{
				native.SetTitleTextColor(_originalTitleTextColor.Value);
			}

			// PrimaryCommands & SecondaryCommands
			var currentMenuItemIds = GetMenuItems(native.Menu)
				.Select(i => i!.ItemId);
			var intendedMenuItemIds = element.PrimaryCommands
				.Concat(element.SecondaryCommands)
				.OfType<AppBarButton>()
				.Select(i => i.GetHashCode());

			if (!currentMenuItemIds.SequenceEqual(intendedMenuItemIds))
			{
				native.Menu.Clear();
				foreach (var command in element.PrimaryCommands.Concat(element.SecondaryCommands).OfType<AppBarButton>())
				{
#pragma warning disable 618
					var menuItem = native.Menu.Add(0, command.GetHashCode(), Menu.None, null);
#pragma warning restore 618
					if (menuItem is { })
					{
						var renderer = command.GetRenderer(() => new AppBarButtonRenderer(command));
						renderer.Native = menuItem;
					}
				}
			}

			var mainCommand = element.GetValue(NavigationBar.MainCommandProperty) as AppBarButton;
			// CommandBarExtensions.NavigationCommand
			if (mainCommand is AppBarButton navigationCommand)
			{
				var renderer = navigationCommand.GetRenderer(() => new NavigationAppBarButtonRenderer(navigationCommand));
				renderer.Native = native;

				if (navigationCommand.Icon is BitmapIcon bitmapIcon)
				{
					native.NavigationIcon = DrawableHelper.FromUri(bitmapIcon.UriSource);
				}
				else
				{
					native.NavigationIcon = new AndroidX.AppCompat.Graphics.Drawable.DrawerArrowDrawable(ContextHelper.Current)
					{
						// 0 = menu icon
						// 1 = back icon
						Progress = 1,
					};
				}

				if (ColorHelper.TryGetColorWithOpacity(navigationCommand.Foreground, out var backButtonForeground))
				{
					switch (native.NavigationIcon)
					{
						case AndroidX.AppCompat.Graphics.Drawable.DrawerArrowDrawable drawerArrowDrawable:
							drawerArrowDrawable.Color = (Android.Graphics.Color)backButtonForeground;
							break;
						case Drawable drawable:
							DrawableCompat.SetTint(drawable, (Android.Graphics.Color)backButtonForeground);
							break;
					}
				}

				native.NavigationContentDescription = ActionBarUpDescription;
			}
			else
			{
				native.NavigationIcon = null;
				native.NavigationContentDescription = null;
			}

			// Padding
			var physicalPadding = element.Padding.LogicalToPhysicalPixels();
			native.SetPadding(
				(int)physicalPadding.Left,
				(int)physicalPadding.Top,
				(int)physicalPadding.Right,
				(int)physicalPadding.Bottom
			);

			// Opacity
			native.Alpha = (float)element.Opacity;
		}

		private IEnumerable<IMenuItem?> GetMenuItems(Android.Views.IMenu menu)
		{
			for (int i = 0; i < menu.Size(); i++)
			{
				yield return menu.GetItem(i);
			}
		}

		private void Native_MenuItemClick(object? sender, Toolbar.MenuItemClickEventArgs e)
		{
			CloseKeyboard();

			var hashCode = e.Item.ItemId;
			var appBarButton = Element?.PrimaryCommands
				.Concat(Element?.SecondaryCommands)
				.OfType<AppBarButton>()
				.FirstOrDefault(c => hashCode == c.GetHashCode());

			appBarButton?.RaiseClick();
		}

		private void Native_NavigationClick(object? sender, Toolbar.NavigationClickEventArgs e)
		{
			CloseKeyboard();

			if (Element?.MainCommand is { } navigationCommand)
			{
				navigationCommand.RaiseClick();
			}
		}

		private void CloseKeyboard()
		{
			if ((ContextHelper.Current as Activity)?.CurrentFocus is { } focused)
			{
				var imm = ContextHelper.Current.GetSystemService(Context.InputMethodService) as InputMethodManager;
				imm?.HideSoftInputFromWindow(focused.WindowToken, HideSoftInputFlags.None);
			}
		}
	}
}
#endif