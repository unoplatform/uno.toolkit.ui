# Uno.Toolkit
A set of custom controls for the UWP, WinUI and the Uno Platform not offered out of the box by WinUI, such as Card for example.

### Material Styles for custom controls
| **Controls**              | **StyleNames**                                                                |
|---------------------------|-------------------------------------------------------------------------------|
| Card                      | MaterialOutlinedCardStyle <br> MaterialElevatedCardStyle <br> MaterialAvatarOutlinedCardStyle <br> MaterialAvatarElevatedCardStyle <br> MaterialSmallMediaOutlinedCardStyle <br> MaterialSmallMediaElevatedCardStyle |

#### Start using the styles in your pages!
To use styles, just find the name of the style from our documentation or sample app and use it like this:

Here is how to use our custom controls like a Card
```xaml
xmlns:utu="using:Uno.Toolkit.UI"

[...]

<material:Card Header="Outlined card"
	      SubHeader="With title and subitle"
	      Style="{StaticResource MaterialOutlinedCardStyle}" />
```