---
uid: Toolkit.Migration
---

# Upgrading Uno Toolkit

## Upgrading to Uno Toolkit 7.1

1. Bump to Uno 6.0

    Version 7.1 of Uno Toolkit now requires `Uno 6.0`.

2. Switch from `ReturnType` to `InputReturnType`

    The `InputExtensions` helper has been updated to use the built-in `InputReturnType` enum (provided by Uno v6+) instead of the legacy `ReturnType` enum.

    ```csharp
    // No code changes required in XAML, but under the hood
    // the attached property now uses InputReturnType:
    public static DependencyProperty ReturnTypeProperty =
        DependencyProperty.RegisterAttached(
            "ReturnType",
            typeof(InputReturnType),
            typeof(InputExtensions),
            new PropertyMetadata(InputReturnType.Default, OnReturnTypeChanged));
    ```
