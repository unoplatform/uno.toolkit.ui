# Uno Toolkit Controls

The `Uno.Toolkit.UI` library adds the following controls:

- `AutoLayout`: A custom panel used by the [Figma plugin](https://platform.uno/unofigma/) to bridge the gap between Figma and UWP layout implementation.
- [`Card` and `CardContentControl`](CardAndCardContentControl.md): \[Material control\] Cards contain content and actions that relate information about a subject.
- [`Chip` and `ChipGroup`](ChipAndChipGroup.md): \[Material control\] Chips are compact elements that represent an input, attribute, or action.
- `Divider`: \[Material control\] A divider is a thin line that groups content in lists and layouts.
- [`DrawerControl`](DrawerControl.md): A container to display additional content, in a hidden pane that can be revealed using a swipe gesture, like a drawer.
- [`DrawerFlyoutPresenter`](DrawerFlyoutPresenter.md): A specialized `ContentPresenter` to be used in the template of a `FlyoutPresenter` to enable gesture support.
- [`LoadingView`](LoadingView.md): A control that indicates that the UI is waiting on a task to complete.
- [`TabBar` and `TabBarItem`](TabBarAndTabBarItem.md): A list of selectable items that can be used to facilitate lateral navigation within an application.
- [`NavigationBar`](NavigationBar.md): A custom control that helps implement navigation logic for your application.

## Helpers

The `Uno.Toolkit.UI` library adds the following helper classes:

- `SystemThemeHelper`: Provides a set of helper methods to check the current operating system theme, and manipulate the application dark/light theme.
- [`AncestorBinding` and `ItemsControlBinding`](helpers\ancestor-itemscontrol-binding.md): These markup extensions provides relative binding based on ancestor type. If you are familiar with WPF, they are very similar to `{RelativeSource Mode=FindAncestor}`.
- [`CommandExtensions`](helpers\command-extensions.md): Provides Command/CommandParameter attached properties for common scenarios.
- [`InputExtensions`](helpers\input-extensions.md): Provides various attached properties for _input controls_, such as `TextBox` and `PasswordBox`.
- [`StatusBar`](helpers\StatusBar-extensions.md): Provides two attached properties on `Page` to controls the visual of the status bar on mobile platforms.
- [`TabBarItemExtensions`](helpers\TabBarItem-extensions.md): Provides additional features for `TabBarItem`.
- [`VisualStateManagerExtensions`](helpers\VisualStateManager-extensions.md): Provides a way of manipulating the visual states of Control with attached property. Exposes visual states as attachable properties that lets you bind a string on a `Control` to set its `VisualState`.
