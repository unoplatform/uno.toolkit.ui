---
uid: Toolkit.Controls.ShadowContainer
---

# ShadowContainer (Concise Reference)

## Properties

| Property   | Type               | Description                                                                                                                                                                                                                                    |
|------------|--------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `Shadows`  | `ShadowCollection` | The collection of shadows that will be displayed under your control. A `ShadowCollection` can be stored in a resource dictionary to have a consistent style throughout your app. The `ShadowCollection` implements `INotifyCollectionChanged`. |

## Usage Examples

```xml
xmlns:utu="using:Uno.Toolkit.UI"
...

<Page.Resources>
    <Color x:Key="UnoColor">#7a67f8</Color>
    <Color x:Key="UnoPink">#f85977</Color>

    <!-- You can define your shadows in the resource dictionary -->
    <utu:ShadowCollection x:Name="ButtonShadows">
        <utu:Shadow BlurRadius="15"
                    OffsetY="8"
                    Opacity="0.5"
                    Color="{StaticResource UnoColor}" />
    </utu:ShadowCollection>
</Page.Resources>

<utu:ShadowContainer Background="White">
    <utu:ShadowContainer.Shadows>
        <!-- You can define your shadows directly -->
        <utu:ShadowCollection x:Name="Shadows">
            <utu:Shadow BlurRadius="20"
                        OffsetX="10"
                        OffsetY="10"
                        Opacity="0.5"
                        Spread="-5"
                        Color="{StaticResource UnoColor}" />
            <utu:Shadow BlurRadius="20"
                        OffsetX="-10"
                        OffsetY="-10"
                        Opacity="0.5"
                        Spread="-5"
                        Color="{StaticResource UnoPink}" />
        </utu:ShadowCollection>
    </utu:ShadowContainer.Shadows>
    <StackPanel Width="300"
                Padding="16"
                BorderThickness="1"
                CornerRadius="20"
                Spacing="16">

        <TextBlock Style="{StaticResource TitleTextBlockStyle}" Text="Add many shadows" />
        <TextBlock Style="{StaticResource BodyTextBlockStyle}" Text="You can either declare shadows directly, or put your ShadowCollection in a resource dictionary." />

        <StackPanel Margin="0,16,0,0"
                    HorizontalAlignment="Center"
                    Orientation="Horizontal"
                    Spacing="16">

            <!-- Reference to the dictionary button shadows -->
            <utu:ShadowContainer Shadows="{StaticResource ButtonShadows}">
                <Button Background="{StaticResource UnoColor}"
                        BorderThickness="1"
                        Content="Add Shadow"
                        Foreground="White" />
            </utu:ShadowContainer>

            <!-- Reference to the dictionary button shadows -->
            <utu:ShadowContainer Shadows="{StaticResource ButtonShadows}">
                <Button Background="{StaticResource UnoColor}"
                        BorderThickness="1"
                        Content="Remove Shadow"
                        Foreground="White" />
            </utu:ShadowContainer>
        </StackPanel>
    </StackPanel>
</utu:ShadowContainer>
```

---

**Note**: This is a concise reference. 
For complete documentation, see [ShadowContainer.md](ShadowContainer.md)