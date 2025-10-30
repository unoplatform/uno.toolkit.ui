# Input behaviors for text fields

These helpers come from **`Uno.Toolkit.UI`** and are applied as **attached properties** on inputs like `TextBox` and `PasswordBox`.

NuGet needed for all snippets: **Uno.Toolkit.UI** (or the Toolkit meta-package your app already uses).

Each how-to is single-purpose on purpose.

---

## 1. How to move focus to the next field

**Goal:** when the user presses **Enter / Return** in a `TextBox`, go to the next focusable control.

**XAML**

```xml
<Page
    x:Class="SampleApp.MainPage"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:utu="using:Uno.Toolkit.UI">

    <StackPanel Spacing="12">
        <TextBox
            Header="First name"
            utu:InputExtensions.AutoFocusNext="True" />

        <TextBox
            Header="Last name"
            utu:InputExtensions.AutoFocusNext="True" />

        <PasswordBox
            utu:InputExtensions.AutoFocusNext="True" />

        <Button Content="Submit" />
    </StackPanel>
</Page>
```

**What it does**

* On Enter, Toolkit asks `FocusManager.FindNextFocusableElement(...)` for the next target.
* If found, focus moves there.
* Good when your fields are in a natural tab order.

**When to use**

* Simple forms
* Vertical stacks
* You don’t care which exact control is next, just “the obvious next one”

---

## 2. How to move focus to a specific field

**Goal:** when Enter is pressed, **always** focus a particular control (not just the “next” one).

**XAML**

```xml
<Page
    x:Class="SampleApp.MainPage"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:utu="using:Uno.Toolkit.UI">

    <StackPanel Spacing="12">
        <TextBox
            x:Name="EmailBox"
            Header="Email"
            utu:InputExtensions.AutoFocusNextElement="{Binding ElementName=PasswordBox}" />

        <PasswordBox
            x:Name="PasswordBox"
            utu:InputExtensions.AutoFocusNextElement="{Binding ElementName=LoginButton}" />

        <Button
            x:Name="LoginButton"
            Content="Login" />
    </StackPanel>
</Page>
```

**What it does**

* On Enter in `EmailBox` → focus goes to `PasswordBox`
* On Enter in `PasswordBox` → focus goes to `LoginButton`
* This **ignores** normal “next” resolution

**Important rule**

* If **both** `AutoFocusNext="True"` **and** `AutoFocusNextElement="..."` are set,
  **`AutoFocusNextElement` wins.** (same as original doc) ([Uno Platform][1])

**When to use**

* Non-linear layouts (grids, responsive panels)
* You want to jump out of the current column/row
* You need to end on a `Button`

---

## 3. How to chain inputs in a custom order

**Goal:** press Enter repeatedly and visit fields in **your** order (1 → 2 → 4 → 3, etc.)

**XAML**

```xml
<StackPanel
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:utu="using:Uno.Toolkit.UI">

    <!-- 1 -->
    <TextBox
        x:Name="Input1"
        Header="1"
        utu:InputExtensions.AutoFocusNext="True" />

    <!-- 2 -->
    <TextBox
        x:Name="Input2"
        Header="2"
        utu:InputExtensions.AutoFocusNextElement="{Binding ElementName=Input4}" />

    <!-- 3 -->
    <TextBox
        x:Name="Input3"
        Header="3"
        utu:InputExtensions.AutoFocusNextElement="{Binding ElementName=Input1}" />

    <!-- 4 -->
    <TextBox
        x:Name="Input4"
        Header="4"
        utu:InputExtensions.AutoFocusNextElement="{Binding ElementName=Input3}" />
</StackPanel>
```

**Result** (same pattern as the official sample) 👆
Order when pressing Enter: **1 → 2 → 4 → 3 → 1 ...** ([Uno Platform][1])

**Why this is useful**

* Wizard-like flows
* Validation-first flows (jump to a review control)
* Auto-looping kiosk-style pages

---

## 4. How to dismiss the soft keyboard on Enter

**Goal:** mobile user finishes typing, presses Enter, the on-screen keyboard closes.

**XAML**

```xml
<TextBox
    Header="Search"
    xmlns:utu="using:Uno.Toolkit.UI"
    utu:InputExtensions.AutoDismiss="True" />
```

**What it does**

* On Enter, Toolkit dismisses the soft keyboard (Android / iOS).
* Good for search bars, short inputs, chat boxes.
* Can be combined with your own **TextChanged** / **KeyDown** logic.

**Notes**

* This is independent from focus-moving. You can use **only** `AutoDismiss` if you don’t want to move focus. ([Uno Platform][1])

---

## 5. How to show a specific return button on mobile

**Goal:** change the icon/label of the return key on Android/iOS to match the task:
Done, Go, Next, Search, Send.

**XAML**

```xml
<TextBox
    Header="Message"
    xmlns:utu="using:Uno.Toolkit.UI"
    utu:InputExtensions.ReturnType="Send" />
```

**Supported values** (from the source page) 📱

* `Default`
* `Done`
* `Go`
* `Next`
* `Search`
* `Send` ([Uno Platform][1])

**When to use**

* Chat → `Send`
* Login → `Go` or `Done`
* Multi-step form → `Next`
* Search box → `Search`

**Platforms**

* This property targets **soft keyboards** on mobile (Android/iOS). On desktop it might not have a visible effect. (inferred from doc behavior) ([Uno Platform][1])

---

## 6. How to prefer a specific target over “next”

**Scenario:** you set **both**:

```xml
utu:InputExtensions.AutoFocusNext="True"
utu:InputExtensions.AutoFocusNextElement="{Binding ElementName=Target}"
```

**Result:** the **element** wins. This is exactly what the original page says:

> “`AutoFocusNextElement` will take precedence over `AutoFocusNext` when both are set.” ([Uno Platform][1])

**Why it matters**

* You can keep `AutoFocusNext="True"` as a default on all inputs
* …and only override the few special ones with `AutoFocusNextElement`

---

## 7. How to name things for RAG

When writing XAML meant to be indexed:

* Always include the namespace: `xmlns:utu="using:Uno.Toolkit.UI"`
* Put the attached property on the **same line** as the control if possible
* Avoid comments with other controls’ names (they get mixed in embeddings)
* Example:

```xml
<TextBox x:Name="EmailBox"
         utu:InputExtensions.AutoFocusNextElement="{Binding ElementName=PasswordBox}" />
```

---

## 9. Quick reference

| Outcome                    | Property                                   | Value / Type                |      |    |      |        |       |
| -------------------------- | ------------------------------------------ | --------------------------- | ---- | -- | ---- | ------ | ----- |
| Move to next control       | `utu:InputExtensions.AutoFocusNext`        | `True`                      |      |    |      |        |       |
| Move to a specific control | `utu:InputExtensions.AutoFocusNextElement` | `{Binding ElementName=...}` |      |    |      |        |       |
| Hide keyboard on Enter     | `utu:InputExtensions.AutoDismiss`          | `True`                      |      |    |      |        |       |
| Change mobile return key   | `utu:InputExtensions.ReturnType`           | `Default                    | Done | Go | Next | Search | Send` |

(Precedence: **AutoFocusNextElement > AutoFocusNext**) ([Uno Platform][1])

---

[1]: https://platform.uno/docs/articles/external/uno.toolkit.ui/doc/helpers/Input-extensions.html?utm_source=chatgpt.com "Input Extensions"
