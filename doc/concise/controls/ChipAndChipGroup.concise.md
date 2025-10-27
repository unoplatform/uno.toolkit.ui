---
uid: Toolkit.Controls.Chip
---

# Chip & ChipGroup (Concise Reference)

## Summary

> This guide covers details for `Chip` and `ChipGroup` specifically. If you are just getting started with the Uno Material Toolkit Library, please see our [general getting started](../getting-started.md) page to make sure you have the correct setup in place.

## Constructors

| Constructor | Description                                     |
|-------------|-------------------------------------------------|
| `Chip()`    | Initializes a new instance of the `Chip` class. |

## Properties

| Property                  | Type           | Description                                                                    |
|---------------------------|----------------|--------------------------------------------------------------------------------|
| `CanRemove`               | `bool`         | Gets or sets whether the remove button is visible.                             |
| `Elevation`               | `double`       | Gets or sets the elevation of the `Chip`.                                      |
| `Icon`                    | `object`       | Gets or sets the icon of the `Chip`.                                           |
| `IconTemplate`            | `DataTemplate` | Gets or sets the data template that is used to display the icon of the `Chip`. |
| `RemovedCommand`          | `ICommand`     | Gets or sets the command to invoke when the remove button is pressed.          |
| `RemovedCommandParameter` | `object`       | Gets or sets the parameter to pass to the RemovedCommand property.             |

## Events

| Event      | Type                       | Description                                                                                         |
|------------|----------------------------|-----------------------------------------------------------------------------------------------------|
| `Removed`  | `RoutedEventHandler`       | Occurs when the remove button is pressed.                                                           |
| `Removing` | `ChipRemovingEventHandler` | Occurs when the remove button is pressed, but before the `Removed` event allowing for cancellation. |

> [!NOTE]
> When used outside of a `ChipGroup`, the `Removed` event does not cause itself to be removed from the view.

```csharp
delegate void ChipRemovingEventHandler(object sender, ChipRemovingEventArgs e);
sealed class ChipRemovingEventArgs : EventArgs
{
    // Gets or sets whether the Chip.Removed" event should be canceled.
    public bool Cancel { get; set; }
}
```

---

**Note**: This is a concise reference. 
For complete documentation, see [ChipAndChipGroup.md](ChipAndChipGroup.md)