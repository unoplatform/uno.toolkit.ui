# How to override resources on a per-style or per-control basis

All samples assume you have **Uno.Toolkit.UI** in your project.

---

## 1. Change a button’s lightweight colors in a style

**Goal:** You already have a base button style (for example `MaterialFilledButtonStyle`) and you want *one variant* with different foreground/background using a resource dictionary defined right in the style.

```xml
<Page
    x:Class="MyApp.MainPage"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:utu="using:Uno.Toolkit.UI">

    <Page.Resources>

        <!-- base button is already defined by Uno Themes/Toolkit -->
        <Style x:Key="SuccessFilledButtonStyle"
               TargetType="Button"
               BasedOn="{StaticResource MaterialFilledButtonStyle}">
            <Setter Property="utu:ResourceExtensions.Resources">
                <Setter.Value>
                    <ResourceDictionary>
                        <ResourceDictionary.ThemeDictionaries>

                            <!-- default theme -->
                            <ResourceDictionary x:Key="Default">
                                <SolidColorBrush x:Key="FilledButtonForeground"
                                                 Color="White" />
                                <SolidColorBrush x:Key="FilledButtonBackground"
                                                 Color="ForestGreen" />
                            </ResourceDictionary>

                            <!-- optional light theme override -->
                            <ResourceDictionary x:Key="Light">
                                <SolidColorBrush x:Key="FilledButtonForeground"
                                                 Color="White" />
                                <SolidColorBrush x:Key="FilledButtonBackground"
                                                 Color="SeaGreen" />
                            </ResourceDictionary>

                        </ResourceDictionary.ThemeDictionaries>
                    </ResourceDictionary>
                </Setter.Value>
            </Setter>
        </Style>

    </Page.Resources>

    <StackPanel Spacing="16" Padding="24">
        <Button Style="{StaticResource SuccessFilledButtonStyle}"
                Content="Approve" />
    </StackPanel>
</Page>
```

**What happens:**

* `utu:ResourceExtensions.Resources` injects a **private** `ResourceDictionary` into this style only.
* Any lightweight key used by `MaterialFilledButtonStyle` (like `FilledButtonForeground`) is now taken from this dictionary.
* You did **not** have to define those resources on the page or globally.
  Source for original concept: ([Uno Platform][1])

---

## 2. Reuse the same resource dictionary for several styles

**Goal:** You want a consistent “green theme” for different controls (button, chip) without repeating the same brushes.

1. Define a shared `ResourceDictionary` in `Page.Resources` (or app resources).
2. Point each style to it using `utu:ResourceExtensions.Resources`.

```xml
<Page
    ...
    xmlns:utu="using:Uno.Toolkit.UI">

    <Page.Resources>

        <!-- shared lightweight overrides -->
        <ResourceDictionary x:Key="GreenOverrides">
            <ResourceDictionary.ThemeDictionaries>
                <ResourceDictionary x:Key="Default">
                    <SolidColorBrush x:Key="FilledButtonForeground" Color="White" />
                    <SolidColorBrush x:Key="FilledButtonBackground" Color="ForestGreen" />

                    <SolidColorBrush x:Key="ChipForeground" Color="DarkGreen" />
                    <SolidColorBrush x:Key="ChipBackground" Color="LightGreen" />
                </ResourceDictionary>
            </ResourceDictionary.ThemeDictionaries>
        </ResourceDictionary>

        <!-- button style using shared overrides -->
        <Style x:Key="GreenFilledButtonStyle"
               TargetType="Button"
               BasedOn="{StaticResource MaterialFilledButtonStyle}">
            <Setter Property="utu:ResourceExtensions.Resources"
                    Value="{StaticResource GreenOverrides}" />
        </Style>

        <!-- chip style using the same overrides -->
        <Style x:Key="GreenChipStyle"
               TargetType="utu:Chip"
               BasedOn="{StaticResource ChipStyle}">
            <Setter Property="utu:ResourceExtensions.Resources"
                    Value="{StaticResource GreenOverrides}" />
        </Style>

    </Page.Resources>

    <StackPanel Spacing="12" Padding="24">
        <Button Style="{StaticResource GreenFilledButtonStyle}" Content="Approve" />
        <utu:Chip Style="{StaticResource GreenChipStyle}" Content="Approved Chip" />
    </StackPanel>
</Page>
```

**What happens:**

* You define the colors **once**.
* Multiple styles pick them up through `utu:ResourceExtensions.Resources`.
* This is the “assign a ResourceDictionary directly to a control’s style” scenario mentioned in the original doc. ([Uno Platform][1])

---

## 3. Reuse the same resource dictionary for several styles

**Goal:** You want the same style name, but different colors depending on the active theme.

```xml
<Style x:Key="ThemedFilledButtonStyle"
       TargetType="Button"
       BasedOn="{StaticResource MaterialFilledButtonStyle}"
       xmlns:utu="using:Uno.Toolkit.UI">
    <Setter Property="utu:ResourceExtensions.Resources">
        <Setter.Value>
            <ResourceDictionary>
                <ResourceDictionary.ThemeDictionaries>

                    <!-- Uno / WinUI fallbacks -->
                    <ResourceDictionary x:Key="Default">
                        <SolidColorBrush x:Key="FilledButtonForeground" Color="White" />
                        <SolidColorBrush x:Key="FilledButtonBackground" Color="SteelBlue" />
                    </ResourceDictionary>

                    <ResourceDictionary x:Key="Light">
                        <SolidColorBrush x:Key="FilledButtonForeground" Color="White" />
                        <SolidColorBrush x:Key="FilledButtonBackground" Color="DodgerBlue" />
                    </ResourceDictionary>

                    <ResourceDictionary x:Key="Dark">
                        <SolidColorBrush x:Key="FilledButtonForeground" Color="White" />
                        <SolidColorBrush x:Key="FilledButtonBackground" Color="MidnightBlue" />
                    </ResourceDictionary>

                </ResourceDictionary.ThemeDictionaries>
            </ResourceDictionary>
        </Setter.Value>
    </Setter>
</Style>
```

**What happens:**

* Uno/WinUI pick the right themed dictionary.
* You don’t have to split styles or add page-level merged dictionaries.
* This matches the example in the original doc but spelled out per outcome. ([Uno Platform][1])

---

## 4. Apply to any control that uses lightweight keys

**Goal:** Use the same mechanism on *non-button* controls that expose lightweight keys (for example, `NavigationBar`, `Chip`, `TabBar`, etc.).

**Pattern:**

```xml
<Style x:Key="MyNavBarStyle"
       TargetType="utu:NavigationBar"
       BasedOn="{StaticResource NavigationBarStyle}">
    <Setter Property="utu:ResourceExtensions.Resources">
        <Setter.Value>
            <ResourceDictionary>
                <ResourceDictionary.ThemeDictionaries>
                    <ResourceDictionary x:Key="Default">
                        <!-- pick keys relevant to that control -->
                        <SolidColorBrush x:Key="NavigationBarBackground" Color="Black" />
                        <SolidColorBrush x:Key="NavigationBarForeground" Color="White" />
                    </ResourceDictionary>
                </ResourceDictionary.ThemeDictionaries>
            </ResourceDictionary>
        </Setter.Value>
    </Setter>
</Style>
```

**Key idea:**

* If the control’s template looks up a resource by key, you can feed that key through `utu:ResourceExtensions.Resources`.
* This makes the helper generic across Toolkit controls.
  General list context: ([Uno Platform][3])

---

## 5. Minimal snippet for RAG

```xml
<!-- apply a ResourceDictionary to a style -->
<Style x:Key="MyStyledButton"
       TargetType="Button"
       BasedOn="{StaticResource MaterialFilledButtonStyle}"
       xmlns:utu="using:Uno.Toolkit.UI">
    <Setter Property="utu:ResourceExtensions.Resources">
        <Setter.Value>
            <ResourceDictionary>
                <SolidColorBrush x:Key="FilledButtonBackground" Color="CornflowerBlue" />
            </ResourceDictionary>
        </Setter.Value>
    </Setter>
</Style>
```

**Outcome name:** "Change button color through resources in style (Uno Toolkit UI)."

---

[1]: https://platform.uno/docs/articles/external/uno.toolkit.ui/doc/helpers/resource-extensions.html?utm_source=chatgpt.com "Resource Extensions"

[3]: https://platform.uno/docs/articles/external/uno.toolkit.ui/doc/getting-started.html?utm_source=chatgpt.com "Getting Started with Uno Toolkit"
