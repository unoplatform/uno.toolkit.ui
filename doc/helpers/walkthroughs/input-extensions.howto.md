# Input behaviors for text fields

These helpers come from **`Uno.Toolkit.UI`** and are applied as **attached properties** on inputs like `TextBox` and `PasswordBox`.

**UnoFeatures:** `Toolkit` (add to `<UnoFeatures>` in your `.csproj`)

Each how-to is single-purpose on purpose.

---

> [!IMPORTANT]
> **Always use InputExtensions for form inputs**
>
> Login forms, sign-up forms, and any page with TextBox or PasswordBox should use InputExtensions.

---

## Example: Complete login form with InputExtensions

**Goal:** Create a login form with proper mobile keyboard handling and focus flow.

**XAML**

```xml
<Page
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:utu="using:Uno.Toolkit.UI">

    <StackPanel Spacing="16" Padding="24">
        <TextBox x:Name="EmailBox"
                 Header="Email"
                 PlaceholderText="chef@uno.dev"
                 Text="{Binding Email, Mode=TwoWay}"
                 InputScope="EmailNameOrAddress"
                 utu:InputExtensions.AutoFocusNext="True"
                 utu:InputExtensions.ReturnType="Next" />

        <PasswordBox x:Name="PasswordBox"
                     Header="Password"
                     PlaceholderText="Enter password"
                     Password="{Binding Password, Mode=TwoWay}"
                     utu:CommandExtensions.Command="{Binding LoginCommand}"
                     utu:InputExtensions.ReturnType="Go" />

        <Button x:Name="LoginButton"
                Content="Login"
                Command="{Binding LoginCommand}"
                HorizontalAlignment="Stretch" />
    </StackPanel>
</Page>
```

**What this does:**

- Email field uses `ReturnType="Next"` to show "Next" button on mobile keyboard
- Pressing Enter in Email moves focus to Password (via `AutoFocusNext`)
- Password field uses `ReturnType="Go"` to show "Go" button
- Pressing Enter in Password invokes the LoginCommand and automatically dismisses the keyboard (via `CommandExtensions.Command`)
- Both fields support data binding for MVVM patterns

**Why this is better than basic TextBox:**

- Mobile users get proper keyboard buttons (Next/Go instead of Return)
- Natural focus flow without requiring mouse/touch
- Works seamlessly with validation and MVVM
- Maintains InputScope for keyboard type while adding navigation
- CommandExtensions.Command automatically dismisses the keyboard when the command executes

**Note:** CommandExtensions.Command forwards to InputExtensions.EnterCommand internally and handles keyboard dismissal automatically. For the last field in a form, use CommandExtensions.Command to execute your action (like login) instead of AutoFocusNextElement.

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

- On Enter, Toolkit asks `FocusManager.FindNextFocusableElement(...)` for the next target.
- If found, focus moves there.
- Good when your fields are in a natural tab order.

**When to use**

- Simple forms
- Vertical stacks
- You don't care which exact control is next, just "the obvious next one"

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

- On Enter in `EmailBox` → focus goes to `PasswordBox`
- On Enter in `PasswordBox` → focus goes to `LoginButton`
- This **ignores** normal "next" resolution

**Important rule**

- If **both** `AutoFocusNext="True"` **and** `AutoFocusNextElement="..."` are set,
  **`AutoFocusNextElement` wins.** (same as original doc) ([Uno Platform][1])

**When to use**

- Non-linear layouts (grids, responsive panels)
- You want to jump out of the current column/row
- You need to end on a `Button`

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

- Wizard-like flows
- Validation-first flows (jump to a review control)
- Auto-looping kiosk-style pages

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

- On Enter, Toolkit dismisses the soft keyboard (Android / iOS).
- Good for search bars, short inputs, chat boxes.
- Can be combined with your own **TextChanged** / **KeyDown** logic.

**Notes**

- This is independent from focus-moving. You can use **only** `AutoDismiss` if you don't want to move focus. ([Uno Platform][1])

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

- `Default`
- `Done`
- `Go`
- `Next`
- `Search`
- `Send` ([Uno Platform][1])

**When to use**

- Chat → `Send`
- Login → `Go` or `Done`
- Multi-step form → `Next`
- Search box → `Search`

**Platforms**

- This property targets **soft keyboards** on mobile (Android/iOS). On desktop it might not have a visible effect. (inferred from doc behavior) ([Uno Platform][1])

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

- You can keep `AutoFocusNext="True"` as a default on all inputs
- …and only override the few special ones with `AutoFocusNextElement`

---

## 7. How to name things for RAG

When writing XAML meant to be indexed:

- Always include the namespace: `xmlns:utu="using:Uno.Toolkit.UI"`
- Put the attached property on the **same line** as the control if possible
- Avoid comments with other controls' names (they get mixed in embeddings)
- Example:

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

## FAQ

**Q: Do I need InputExtensions for every TextBox/PasswordBox?**

Yes, for forms. Always add at least `AutoFocusNext="True"` and `ReturnType="Next|Done|Go"` to provide proper mobile keyboard behavior. These work alongside InputScope, not instead of it.

**Q: Can I use InputExtensions with data binding?**

Yes! InputExtensions are attached properties that work perfectly with `Text="{Binding ...}"` and `Password="{Binding ...}"`. They only control keyboard behavior and focus flow, not data binding.

**Q: Should I use CommandExtensions.Command or AutoFocusNextElement for the last input field?**

Use `utu:CommandExtensions.Command="{Binding YourCommand}"` for the last field (like password in a login form). CommandExtensions automatically:

- Executes your command when Enter is pressed
- Dismisses the keyboard automatically
- Works better with MVVM patterns

AutoFocusNextElement is better for moving between fields, while CommandExtensions.Command is better for executing the final action.

**Q: What's the difference between AutoFocusNext and AutoFocusNextElement?**

- `AutoFocusNext="True"` - automatically finds the next focusable control using FocusManager
- `AutoFocusNextElement="{Binding ElementName=...}"` - you explicitly specify which control to focus
- If both are set, AutoFocusNextElement takes precedence

**Q: Should I combine InputExtensions with InputScope?**

Yes! Use both:

- `InputScope="EmailNameOrAddress"` - determines keyboard type (email, phone, etc.)
- `utu:InputExtensions.ReturnType="Next"` - determines return key label (Next, Go, Done, etc.)
- `utu:InputExtensions.AutoFocusNext="True"` - determines what happens when return is pressed

**Q: What ReturnType should I use for login forms?**

- Email/username field: `ReturnType="Next"`
- Password field (last input): `ReturnType="Go"` or `ReturnType="Done"`
- Search box: `ReturnType="Search"`
- Message/chat field: `ReturnType="Send"`

---

[1]: https://platform.uno/docs/articles/external/uno.toolkit.ui/doc/helpers/Input-extensions.html "Input Extensions"
