---
uid: Toolkit.Helpers.ItemRepeaterExtensions
---

# ItemsRepeater Extensions (Concise Reference)

## Summary

Provides selection and incremental loading support for `ItemsRepeater`.

## Properties

| Property          | Type                 | Description                                                                                                                                                              |
|-------------------|----------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `SelectedItem`    | `object`             | Two-ways bindable property for the current/first(in Multiple mode) selected item.\*                                                                                      |
| `SelectedIndex`   | `int`                | Two-ways bindable property for the current/first(in Multiple mode) selected index.\*                                                                                     |
| `SelectedItems`   | `IList<object>`      | Two-ways bindable property for the current selected items.\*                                                                                                             |
| `SelectedIndexes` | `IList<int>`         | Two-ways bindable property for the current selected indexes.\*                                                                                                           |
| `SelectionMode`   | `ItemsSelectionMode` | Gets or sets the selection behavior: `None`, `SingleOrNone`, `Single`, `Multiple` <br/> note: Changing this value will cause the `Selected-`properties to be re-coerced. |

---

**Note**: This is a concise reference. 
For complete documentation, see [itemsrepeater-extensions.md](itemsrepeater-extensions.md)