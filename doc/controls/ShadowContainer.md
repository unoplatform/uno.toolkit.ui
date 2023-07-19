---
uid: Toolkit.Controls.ShadowContainer
---

## Summary
The `ShadowContainer` provides the possibility to add many-colored shadows to its content.

## Remarks
For now, the control simply adapts its corner radius to the content's corner radius. More complicated shapes like texts or pictures with alpha, are not supported.

### XAML
```xml
xmlns:utu="using:Uno.Toolkit.UI"
...

<utu:ShadowContainer>
    <utu:ShadowContainer.Shadows>
        <utu:ShadowCollection>
            <utu:Shadow ... />
            <utu:Shadow ... />
        </utu:ShadowCollection>
    </utu:ShadowContainer.Shadows>

    <SomeControl />

</utu:ShadowContainer>
```

### Inheritance
Object &#8594; DependencyObject &#8594; UIElement &#8594; FrameworkElement &#8594; Control &#8594; ContentControl

### Constructors
| Constructor       | Description                                                |
| ----------------- | ---------------------------------------------------------- |
| ShadowContainer() | Initializes a new instance of the `ShadowContainer` class. |

### Properties
| Property | Type             | Description |
| -------- | ---------------- | ----------- |
Shadows  | ShadowCollection |  The collection of shadows that will be displayed under your control. A `ShadowCollection` can be stored in a resource dictionary to have a consistent style through your app. The `ShadowCollection` implements `INotifyCollectionChanged`.

## Shadow

Dependency object representing a single shadow.
Public properties are all dependency properties.

### Shadow Properties
| Property | Type | Description |
| -------- | ---- | ----------- |
IsInner | bool | True is this shadow is an inner shadow (like `inset` in css box-shadow).
OffsetX | double | The X offset of the shadow.
OffsetY | double | The Y offset of the shadow.
Color | Color | The color of the shadow. It will be multiplied by the `Opacity` property before rendering.
Opacity | double | The opacity of the shadow.
BlurRadius | double | The radius of the blur that will be applied to the shadow **[0..100]**.
Spread | double | The spread will inflate or deflate (if negative) the control shadow size **before** applying the blur.

## Usage

```xml
xmlns:utu="using:Uno.Toolkit.UI"
...

<Page.Resources>
    <Color x:Key="UnoColor">#7a67f8</Color>
    <Color x:Key="UnoPink">#f85977</Color>

    <!-- You can define your shadows in the resource dictionary -->
    <ui:ShadowCollection x:Name="ButtonShadows">
        <ui:Shadow BlurRadius="15"
                   OffsetY="8"
				   Opacity="0.5"
				   Color="{StaticResource UnoColor}" />
    </ui:ShadowCollection>
</Page.Resources>

<ui:ShadowContainer>
    <ui:ShadowContainer.Shadows>
        <!-- You can define your shadows directly -->
        <ui:ShadowCollection x:Name="Shadows">
            <ui:Shadow BlurRadius="20"
					   OffsetX="10"
					   OffsetY="10"
					   Opacity="0.5"
					   Spread="-5"
					   Color="{StaticResource UnoColor}" />
            <ui:Shadow BlurRadius="20"
					   OffsetX="-10"
					   OffsetY="-10"
					   Opacity="0.5"
					   Spread="-5"
					   Color="{StaticResource UnoPink}" />
        </ui:ShadowCollection>
    </ui:ShadowContainer.Shadows>
    <StackPanel Width="300"
                Padding="16"
                Background="White"
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
            <ui:ShadowContainer Shadows="{StaticResource ButtonShadows}">
                <Button Background="{StaticResource UnoColor}"
                        BorderThickness="1"
                        Content="Add Shadow"
                        Foreground="White" />
            </ui:ShadowContainer>

            <!-- Reference to the dictionary button shadows -->
            <ui:ShadowContainer Shadows="{StaticResource ButtonShadows}">
                <Button Background="{StaticResource UnoColor}"
                        BorderThickness="1"
                        Content="Remove Shadow"
                        Foreground="White" />
            </ui:ShadowContainer>
        </StackPanel>
    </StackPanel>
</ui:ShadowContainer>
```

![2 colored shadows with 2 buttons with single shadows](../assets/shadows-colors.png)

### Neumorphism

[Following neumorphism rules](https://neumorphism.io), choose one background color, 2 shadow colors, and create a cool neumorphism style.
In order to achieve neumorphic depth effects (instead of having a raised element, it will feel as if it was hollow or bulging), set the `IsInner` property of a shadow to `true`. The shadow will then be displayed *inside* the element instead of beyond.
In css `box-shadow` it's equivalent to the `inset` property.

```xml
xmlns:utu="using:Uno.Toolkit.UI"
...

<Page.Resources>
    <Color x:Key="UnoColor">#7a67f8</Color>

    <ui:ShadowCollection x:Key="NeumorphismRaising">
        <!-- Bottom right darker violet -->
        <ui:Shadow BlurRadius="30"
                   OffsetX="10"
				   OffsetY="10"
				   Opacity="1"
				   Spread="-5"
				   Color="#6858d3" />
        <!-- Top left lighter violet -->
        <ui:Shadow BlurRadius="30"
				   OffsetX="-10"
				   OffsetY="-10"
				   Opacity="1"
				   Spread="-5"
				   Color="#8c76ff" />
    </ui:ShadowCollection>

    <ui:ShadowCollection x:Key="NeumorphismHollow">
		<!-- Inner top and left shadow -->
		<ui:Shadow BlurRadius="10"
				   IsInner="True"
				   OffsetX="5"
				   OffsetY="5"
				   Opacity="1"
				   Spread="0"
				   Color="#6858d3" />
		<!-- Inner bottom and right shadow -->
		<ui:Shadow BlurRadius="10"
				   IsInner="True"
				   OffsetX="-4"
				   OffsetY="-4"
				   Opacity="1"
				   Spread="0"
				   Color="#8c76ff" />
	</ui:ShadowCollection>

	<ui:ShadowCollection x:Key="NeumorphismBulging">
		<ui:Shadow BlurRadius="10"
				   IsInner="True"
				   OffsetX="-5"
				   OffsetY="-5"
				   Opacity="1"
				   Spread="0"
				   Color="#6858d3" />
		<ui:Shadow BlurRadius="10"
				   IsInner="True"
				   OffsetX="4"
				   OffsetY="4"
				   Opacity="1"
				   Spread="0"
				   Color="#8c76ff" />
		</ui:ShadowCollection>
</Page.Resources>

<StackPanel Width="400"
			Margin="0,32"
			Padding="32"
			Background="{StaticResource UnoColor}"
			CornerRadius="30">
	<ui:ShadowContainer Shadows="{StaticResource NeumorphismRaising}">
		<Grid Width="300"
				Padding="20"
				Background="{StaticResource UnoColor}"
				CornerRadius="20">
			<Grid.RowDefinitions>
				<RowDefinition Height="20" />
				<RowDefinition Height="20" />
			</Grid.RowDefinitions>

			<TextBlock FontSize="15"
					   Foreground="White"
					   Text="Neumorphism" />
			<TextBlock Grid.Row="1"
					   FontSize="12"
					   Foreground="White"
					   Text="Raising element" />
		</Grid>
	</ui:ShadowContainer>

	<ui:ShadowContainer Margin="0,60,0,0" Shadows="{StaticResource NeumorphismHollow}">
		<TextBox Width="200"
				 Height="40"
				 Padding="15,10,15,0"
				 VerticalAlignment="Center"
				 Background="{StaticResource UnoColor}"
				 BorderThickness="0"
				 CornerRadius="20"
				 Foreground="White"
				 PlaceholderForeground="LightGray"
				 PlaceholderText="Hollow element" />
	</ui:ShadowContainer>
	<ui:ShadowContainer Margin="0,15" Shadows="{StaticResource NeumorphismHollow}">
		<TextBox Width="200"
				 Height="40"
				 Padding="15,10,15,0"
				 VerticalContentAlignment="Center"
				 Background="{StaticResource UnoColor}"
				 BorderThickness="0"
				 CornerRadius="20"
				 Foreground="White"
				 PlaceholderForeground="LightGray"
				 PlaceholderText="Hollow element" />
	</ui:ShadowContainer>

	<ui:ShadowContainer Margin="0,30" Shadows="{StaticResource NeumorphismBulging}">
		<Button Width="200"
				Height="40"
				Background="{StaticResource UnoColor}"
				BorderBrush="{StaticResource UnoColor}"
				Content="Bulging element"
				CornerRadius="15"
				Foreground="White" />
	</ui:ShadowContainer>

	<StackPanel Margin="0,30,0,0"
				Padding="24"
				HorizontalAlignment="Center"
				Orientation="Horizontal"
				Spacing="16">

		<ui:ShadowContainer Shadows="{StaticResource NeumorphismRaising}">
			<Button Background="{StaticResource UnoColor}"
					BorderThickness="0"
					Content="Regular"
					Foreground="White" />
		</ui:ShadowContainer>

		<ui:ShadowContainer Shadows="{StaticResource NeumorphismRaising}">
			<Button Width="80"
					Height="80"
					Background="{StaticResource UnoColor}"
					BorderBrush="{StaticResource UnoColor}"
					Content="Circular"
					CornerRadius="40"
					Foreground="White" />
		</ui:ShadowContainer>

		<ui:ShadowContainer Shadows="{StaticResource NeumorphismRaising}">
			<Button Height="60"
					Background="{StaticResource UnoColor}"
					BorderThickness="0"
					Content="Bigger"
					CornerRadius="20"
					Foreground="White" />
		</ui:ShadowContainer>
	</StackPanel>
</StackPanel>
```

![neumorphism built with 2 shadows](../assets/shadows-neumorphism.png)
