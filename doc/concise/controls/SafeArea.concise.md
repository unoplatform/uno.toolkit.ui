---
uid: Toolkit.Controls.SafeArea
---

# SafeArea (Concise Reference)

## Summary

`SafeArea` is a specialized control that overrides the `Padding` or `Margin` properties of its child/attached control to ensure that its inner content is always within the [`ApplicationView.VisibleBounds`](https://learn.microsoft.com/uwp/api/windows.ui.viewmanagement.applicationview.visiblebounds) rectangle.
The `ApplicationView.VisibleBounds` is the rectangular area of the screen which is completely unobscured by any window decoration, such as the status bar, rounded screen corners, or any type of screen notch.
`SafeArea` can also be used to specify certain areas of the UI that should adapt its layout in order to avoid being covered by any sort of soft-input panel, such as the on-screen keyboard. This is done by observing the state of the keyboard and treating the area that it occupies when open as part of the "unsafe" area of the screen.
In some cases, it is acceptable for visible content to be partially obscured (a page background for example) and it should extend to fill the entire window. Other types of content should be restricted to the visible bounds (for instance: readable text, or interactive controls). `SafeArea` enables this kind of fine-grained control over responsiveness to the safe and "unsafe" areas of the screen.

## Properties

| Property | Type        | Description                                                                                                                                                                 |
|----------|-------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `Insets` | `InsetMask` | Gets or sets the specific bound(s) of the "safe" area that you want to be considered when `SafeArea` attempts to apply the Padding or Margin. Defaults to `InsetMask.None`. |
| `Mode`   | `InsetMode` | Gets or sets whether the `SafeArea` insets will be applied to the control's `Margin` or its `Padding`. Defaults to `InsetMode.Padding`.                                     |

## Usage Examples

```xml
<Page xmlns:utu="using:Uno.Toolkit.UI">
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto" />
            <RowDefinition Height="*" />
            <RowDefinition Height="Auto" />
        </Grid.RowDefinitions>
        <utu:TabBar Background="Purple">
            <utu:TabBar.Items>
                <utu:TabBarItem Foreground="White"
                                Content="Home" />
                <utu:TabBarItem Foreground="White"
                                Content="Search" />
                <utu:TabBarItem Foreground="White"
                                Content="Support" />
                <utu:TabBarItem Foreground="White"
                                Content="About" />
            </utu:TabBar.Items>
        </utu:TabBar>

        <TextBlock Text="Page Content"
                    FontSize="30"
                    Grid.Row="1"
                    VerticalAlignment="Center"
                    HorizontalAlignment="Center" />

        <utu:TabBar Grid.Row="2"
                    Background="Purple">
            <utu:TabBar.Items>
                <utu:TabBarItem Foreground="White"
                                Content="Home">
                    <utu:TabBarItem.Icon>
                        <FontIcon Foreground="White"
                                    Glyph="&#xE80F;" />
                    </utu:TabBarItem.Icon>
                </utu:TabBarItem>
                <utu:TabBarItem Foreground="White"
                                Content="Search">
                    <utu:TabBarItem.Icon>
                        <FontIcon Foreground="White"
                                    Glyph="&#xe721;" />
                    </utu:TabBarItem.Icon>
                </utu:TabBarItem>
                <utu:TabBarItem Foreground="White"
                                Content="Support">
                    <utu:TabBarItem.Icon>
                        <FontIcon Foreground="White"
                                    Glyph="&#xE8F2;" />
                    </utu:TabBarItem.Icon>
                </utu:TabBarItem>
                <utu:TabBarItem Foreground="White"
                                Content="About">
                    <utu:TabBarItem.Icon>
                        <FontIcon Foreground="White"
                                    Glyph="&#xE946;" />
                    </utu:TabBarItem.Icon>
                </utu:TabBarItem>
            </utu:TabBar.Items>
        </utu:TabBar>
    </Grid>
</Page>
```

```diff
<utu:TabBar Background="Purple"
+           utu:SafeArea.Insets="Top">
```

```diff
<utu:TabBar Grid.Row="2"
+           utu:SafeArea.Insets="Bottom"
            Background="Purple">
```

---

**Note**: This is a concise reference. 
For complete documentation, see [SafeArea.md](SafeArea.md)