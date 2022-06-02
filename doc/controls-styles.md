# Uno Toolkit Controls
The `Uno.Toolkit.UI` library adds the following controls:
- `AutoLayout`: A custom panel used by the [Figma plugin](https://platform.uno/unofigma/) to bridge the gap between Figma and UWP layout implementation.
- [`Card` and `CardContentControl`](controls\CardAndCardContentControl.md): \[Material control\] Cards contain content and actions that relate information about a subject.
- [`Chip` and `ChipGroup`](controls\ChipAndChipGroup.md): \[Material control\] Chips are compact elements that represent an input, attribute, or action.
- `Divider`: \[Material control\] A divider is a thin line that groups content in lists and layouts.
- [`DrawerControl`](controls\DrawerControl.md): A container to display additional content, in a hidden pane that can be revealed using a swipe gesture, like a drawer.
- [`TabBar` and `TabBarItem`](controls\TabBarAndTabBarItem.md): A list of selectable items that can be used to facilitate lateral navigation within an application.
- [`NavigationBar`](controls\NavigationBar.md): A custom control that helps implement navigation logic for your application.

## Helpers
The `Uno.Toolkit.UI` library adds the following helper classes:
- `SystemThemeHelper`: Provides a set of helper methods to check the current operating system theme, and manipulate the application dark/light theme.
- [`AncestorBinding` and `ItemsControlBinding`](helpers\ancestor-itemscontrol-binding.md): These markup extensions provides relative binding based on ancestor type. If you are familiar with WPF, they are very similar to `{RelativeSource Mode=FindAncestor}`.

## Control Styles
Control|Style Key|IsDefaultStyle*|
-|-|-
AppBarButton|MainCommandStyle|True|
AppBarButton|ModalMainCommandStyle|
AppBarButton|PrimaryMainCommandStyle|
AppBarButton|PrimaryModalMainCommandStyle|
AppBarButton|PrimaryAppBarButtonStyle|
utu:Card|FilledCardStyle|
utu:Card|OutlinedCardStyle|
utu:Card|ElevatedCardStyle|
utu:Card|AvatarFilledCardStyle|
utu:Card|AvatarOutlinedCardStyle|
utu:Card|AvatarElevatedCardStyle|
utu:Card|SmallMediaFilledCardStyle|
utu:Card|SmallMediaOutlinedCardStyle|
utu:Card|SmallMediaElevatedCardStyle|
utu:CardContentControl|FilledCardContentControlStyle|
utu:CardContentControl|OutlinedCardContentControlStyle|
utu:CardContentControl|ElevatedCardContentControlStyle|
utu:Chip|AssistChipStyle|
utu:Chip|ElevatedAssistChipStyle|
utu:Chip|InputChipStyle|
utu:Chip|FilterChipStyle|
utu:Chip|ElevatedFilterChipStyle|
utu:Chip|SuggestionChipStyle|
utu:Chip|ElevatedSuggestionChipStyle|
utu:ChipGroup|InputChipGroupStyle|
utu:ChipGroup|ElevatedSuggestionChipGroupStyle|
utu:ChipGroup|SuggestionChipGroupStyle|
utu:ChipGroup|ElevatedFilterChipGroupStyle|
utu:ChipGroup|FilterChipGroupStyle|
utu:ChipGroup|ElevatedAssistChipGroupStyle|
utu:ChipGroup|AssistChipGroupStyle|
utu:Divider|DividerStyle|True|
utu:NavigationBar|NavigationBarStyle|True|
utu:NavigationBar|ModalNavigationBarStyle|
utu:NavigationBar|PrimaryNavigationBarStyle|
utu:NavigationBar|PrimaryModalNavigationBarStyle|
utu:TabBar|BottomTabBarStyle|
utu:TabBar|TopTabBarStyle|
utu:TabBar|ColoredTopTabBarStyle|
utu:TabBarItem|BottomFabTabBarItemStyle|
utu:TabBarItem|BottomTabBarItemStyle|

IsDefaultStyle*: Styles in this column will be set as the default implicit style for the matching control
