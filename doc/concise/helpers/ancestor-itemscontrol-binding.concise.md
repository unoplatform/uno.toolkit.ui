---
uid: Toolkit.Helpers.Bindings
---

# AncestorBinding & ItemsControlBinding (Concise Reference)

## Summary

These markup extensions provide relative binding based on ancestor type. If you are familiar with WPF, they are very similar to `{RelativeSource Mode=FindAncestor}`.
They are typically used from inside a `DataTemplate` to access elements outside of said data-template which is not normally accessible (eg: through `ElementName` binding). The common usage is to access the parent data-context from inside the `ItemsControl.ItemTemplate`.

---

**Note**: This is a concise reference. 
For complete documentation, see [ancestor-itemscontrol-binding.md](ancestor-itemscontrol-binding.md)