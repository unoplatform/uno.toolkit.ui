---
uid: Toolkit.Controls.DrawerFlyoutPresenter.HowTo
tags: [drawer, flyout, bottom-sheet, slide-up, gesture, swipe, modal]
---

# Show a drawer flyout with gesture support

**What you get:** A full-screen flyout that slides up from the bottom.

**Packages:** `Uno.Toolkit.UI`

```xml
<Button Content="Open drawer"
        xmlns:toolkit="using:Uno.UI.Toolkit">
    <Button.Flyout>
        <Flyout Placement="Full"
                FlyoutPresenterStyle="{StaticResource BottomDrawerFlyoutPresenterStyle}">
            <StackPanel toolkit:VisibleBoundsPadding.PaddingMask="All"
                        Background="SkyBlue"
                        MinHeight="200">
                <TextBlock Text="Hello from the drawer" />
                <Button Content="Action" />
            </StackPanel>
        </Flyout>
    </Button.Flyout>
</Button>
```

* Use `Placement="Full"` and a `*DrawerFlyoutPresenterStyle` resource (e.g., `BottomDrawerFlyoutPresenterStyle`). ([Uno Platform][1])
* `VisibleBoundsPadding.PaddingMask="All"` keeps content inside safe areas on mobile. ([Uno Platform][1])

---

## Open from left / right / top / bottom

**What you get:** A drawer that slides in from a specific edge.

**Packages:** `Uno.Toolkit.UI`

Choose one built-in presenter style:

```xml
<!-- Left origin (opens toward Right) -->
<Flyout Placement="Full" FlyoutPresenterStyle="{StaticResource LeftDrawerFlyoutPresenterStyle}" />

<!-- Top origin (opens toward Down) -->
<Flyout Placement="Full" FlyoutPresenterStyle="{StaticResource TopDrawerFlyoutPresenterStyle}" />

<!-- Right origin (opens toward Left) -->
<Flyout Placement="Full" FlyoutPresenterStyle="{StaticResource RightDrawerFlyoutPresenterStyle}" />

<!-- Bottom origin (opens toward Up) -->
<Flyout Placement="Full" FlyoutPresenterStyle="{StaticResource BottomDrawerFlyoutPresenterStyle}" />
```

> Tip: The name indicates where the drawer **starts**; the `OpenDirection` is the opposite (e.g., Bottom → opens **Up**). ([Uno Platform][1])

---

## Change drawer size (auto / ratio / fixed)

**What you get:** Control over the drawer's width/height.

**Packages:** `Uno.Toolkit.UI`

```xml
<!-- Fit to content -->
<Style TargetType="FlyoutPresenter">
    <Setter Property="utu:DrawerFlyoutPresenter.DrawerLength" Value="Auto" />
</Style>

<!-- Ratio of screen (0*..1*, exclusive of 0*) -->
<Style TargetType="FlyoutPresenter">
    <Setter Property="utu:DrawerFlyoutPresenter.DrawerLength" Value="0.66*" />
</Style>

<!-- Fixed pixels -->
<Style TargetType="FlyoutPresenter">
    <Setter Property="utu:DrawerFlyoutPresenter.DrawerLength" Value="300" />
</Style>
```

* `DrawerLength` accepts `Auto`, `Star` (e.g., `0.66*`), or `Pixel` values. Star range is (0*, 1*]. ([Uno Platform][1])

---

## Enable swipe gestures and light dismiss

**What you get:** Users can drag the drawer and tap outside to close.

**Packages:** `Uno.Toolkit.UI`

```xml
<Style TargetType="FlyoutPresenter">
    <Setter Property="utu:DrawerFlyoutPresenter.IsGestureEnabled" Value="True" />
    <Setter Property="utu:DrawerFlyoutPresenter.IsLightDismissEnabled" Value="True" />
</Style>
```

* `IsGestureEnabled` allows touch/drag interactions.
* `IsLightDismissEnabled` allows tapping the overlay to close. ([Uno Platform][1])

---

## Change the overlay color

**What you get:** A custom scrim behind the drawer.

**Packages:** `Uno.Toolkit.UI`

```xml
<Style BasedOn="{StaticResource BottomDrawerFlyoutPresenterStyle}"
       TargetType="FlyoutPresenter">
    <Setter Property="utu:DrawerFlyoutPresenter.LightDismissOverlayBackground" Value="#80808080" />
</Style>
```

* Set `LightDismissOverlayBackground` on the presenter style, not on content. ([Uno Platform][1])

---

## Set properties inline (no style)

**What you get:** Quick, per-instance configuration.

**Packages:** `Uno.Toolkit.UI`

```xml
<!-- In a ControlTemplate or when directly templating a presenter -->
<utu:DrawerFlyoutPresenter
    OpenDirection="Up"
    DrawerLength="0.66*"
    LightDismissOverlayBackground="#80808080"
    IsGestureEnabled="True"
    IsLightDismissEnabled="True" />
```

* All properties can be used as dependency properties **or** attached properties. ([Uno Platform][1])

---

## Property reference (quick scan)

* **OpenDirection** (`DrawerOpenDirection`, default **Up**) — Direction the drawer opens **toward** (drawer appears on the opposite side). ([Uno Platform][1])
* **DrawerLength** (`GridLength`, default **0.66***): `Auto` (fit content), `Star` (ratio of screen; (0*, 1*]), or `Pixel` (fixed). ([Uno Platform][1])
* **LightDismissOverlayBackground** (`Brush`) — Overlay brush (default `#80808080`). ([Uno Platform][1])
* **IsGestureEnabled** (`bool`, default **true**) — Enable drag gestures. ([Uno Platform][1])
* **IsLightDismissEnabled** (`bool`, default **true**) — Tap outside to dismiss. ([Uno Platform][1])

---

[1]: https://platform.uno/docs/articles/external/uno.toolkit.ui/doc/controls/DrawerFlyoutPresenter.html "DrawerFlyoutPresenter "
