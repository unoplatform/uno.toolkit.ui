---
uid: Toolkit.Controls.SafeArea.HowTo
tags: [safe-area, visible-bounds-padding, safearea, visible-bounds, safe-area-control]
---

# Keep content out of notches and system bars

> `SafeArea` keeps interactive content inside the unobscured screen area (status bars, notches, rounded corners) and can adapt to the on-screen keyboard. It works by aligning content to `ApplicationView.VisibleBounds`. ([Uno Platform][1])

**UnoFeature:** `Toolkit` (add to `<UnoFeatures>` in your `.csproj`)

**Outcome:** your page content never hides under the status bar / rounded corners.

```xml
<Page
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:utu="using:Uno.Toolkit.UI">

    <utu:SafeArea>
        <Grid>
            <!-- Place interactive content here -->
            <TextBlock Text="Hello safe world" />
        </Grid>
    </utu:SafeArea>
</Page>
```

**Why this works:** `SafeArea` overrides padding/margin so content stays within `VisibleBounds`. Prefer it over the older `VisibleBoundsPadding` behavior. ([Uno Platform][1])

---

## Add safe padding to a single edge only

**Outcome:** only the top gets inset (e.g., account for status bar), other edges stay flush.

```xml
<Grid xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
      xmlns:utu="using:Uno.Toolkit.UI">

    <utu:SafeArea Mode="Padding" Insets="Top">
        <CommandBar>
            <AppBarButton Icon="Back" Label="Back" />
        </CommandBar>
    </utu:SafeArea>
</Grid>
```

**Notes:** Restricting alignment lets you protect specific sides while keeping others edge-to-edge. (SafeArea aligns to the visible bounds rectangle exposed by the platform.) ([Uno Platform][1])

---

## Respect the on-screen keyboard

**Outcome:** inputs aren't hidden by the soft keyboard; the layout shifts safely.

```xml
<Page
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:utu="using:Uno.Toolkit.UI">

    <!-- Treat the keyboard as an unsafe area and adjust layout -->
    <utu:SafeArea Insets="SoftInput">
        <ScrollViewer>
            <StackPanel Spacing="12">
                <TextBox Header="Email" />
                <PasswordBox Header="Password" />
                <Button Content="Sign in" />
            </StackPanel>
        </ScrollViewer>
    </utu:SafeArea>
</Page>
```

**Why this works:** `SafeArea` can observe keyboard state and include its occupied area as "unsafe," preventing overlap. ([Uno Platform][1])

## Pad a scrollable content area safely

**Outcome:** scrollers keep last items tappable (e.g., bottom buttons) without being obscured.

```xml
<Page
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:utu="using:Uno.Toolkit.UI">

    <utu:SafeArea Insets="SoftInput">
        <ScrollViewer>
            <StackPanel Spacing="8">
                <!-- Lots of content -->
                <Button Content="Continue" />
            </StackPanel>
        </ScrollViewer>
    </utu:SafeArea>
</Page>
```

**Why:** Safe area ensures bottom padding adapts for bars, corners, and keyboard. ([Uno Platform][1])

---

## Layer safe areas (header + footer)

**Outcome:** top and bottom chrome each get their own safe insets.

```xml
<Grid
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:utu="using:Uno.Toolkit.UI">

    <!-- Top header -->
      <Grid Height="56" utu:SafeArea.Insets="Top">
          <TextBlock Text="Header" VerticalAlignment="Center" Margin="16,0"/>
      </Grid>

    <!-- Bottom footer -->
      <Grid Height="72" utu:SafeArea.Insets="Bottom">
          <Button Content="Primary action" HorizontalAlignment="Stretch" Margin="16,8"/>
      </Grid>
</Grid>
```

**Note:** SafeArea supports layered usage; recent toolkit updates improved performance of layered scenarios. ([GitHub][5])

---

## FAQ (quick answers)

* **Does SafeArea change `Padding` or `Margin`?** It overrides padding/margin of its child/attached target to keep content inside `VisibleBounds`. ([Uno Platform][1])
* **Can it react to the soft keyboard?** Yes—enable `ConsiderSoftInput="True"`. ([Uno Platform][1])
* **Do I need a design system package (Material/Cupertino)?** SafeArea is in the base Toolkit package; many Toolkit controls are styled via design systems, but SafeArea itself doesn't require them. ([Uno Platform][2])

[1]: https://platform.uno/docs/articles/external/uno.toolkit.ui/doc/controls/SafeArea.html?utm_source=chatgpt.com "SafeArea"
[2]: https://platform.uno/docs/articles/external/uno.toolkit.ui/doc/getting-started.html?utm_source=chatgpt.com "Getting Started with Uno Toolkit"
[5]: https://github.com/unoplatform/uno.toolkit.ui/releases?utm_source=chatgpt.com "Releases · unoplatform/uno.toolkit.ui"
