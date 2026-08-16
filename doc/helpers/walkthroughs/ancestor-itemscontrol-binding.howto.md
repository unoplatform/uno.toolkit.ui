# Bind to a parent from inside an item template

**Applies to:** `Uno.Toolkit.UI` (add the package to the project using the control)

**Goal:** make it easy for markup inside a `DataTemplate` (like an item in a `ListView`) to talk to a parent element or to the parent’s `DataContext`.

We do it with these markup extensions:

* `ItemsControlBinding` → “give me the closest items control”
* `AncestorBinding` → “give me the closest ancestor of this type”

---

## 1. Show parent view-model data inside each item

**Outcome:** each item can read something from the parent page/view-model.

```xaml
<Page
    x:Class="Samples.MainPage"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:utu="using:Uno.Toolkit.UI">

    <!-- DataContext = ExampleModel -->
    <ListView ItemsSource="{Binding Items}">
        <ListView.ItemTemplate>
            <DataTemplate>
                <StackPanel Spacing="4">
                    <!-- local item -->
                    <TextBlock Text="{Binding}" />

                    <!-- parent DataContext -->
                    <TextBlock
                        Text="{utu:ItemsControlBinding Path=DataContext.SomeText}" />
                </StackPanel>
            </DataTemplate>
        </ListView.ItemTemplate>
    </ListView>
</Page>
```

**Why it works:**
`ItemsControlBinding` walks up from the item to the nearest `ItemsControl` (`ListView` here), and then the `Path` says “now go to that control’s `DataContext.SomeText`”.

**Dependencies:** `Uno.Toolkit.UI`

---

## 2. Read a property from the parent items control

**Outcome:** item content can display a property of the `ListView` / `ItemsRepeater` / other items control.

```xaml
<ListView
    x:Name="MyList"
    Tag="From ListView"
    ItemsSource="{Binding Items}"
    xmlns:utu="using:Uno.Toolkit.UI">
    <ListView.ItemTemplate>
        <DataTemplate>
            <StackPanel Spacing="4">
                <TextBlock Text="Item:" />
                <TextBlock FontWeight="Bold" Text="{Binding}" />

                <!-- read the ListView.Tag -->
                <TextBlock
                    Text="{utu:ItemsControlBinding Path=Tag}" />
            </StackPanel>
        </DataTemplate>
    </ListView.ItemTemplate>
</ListView>
```

**Notes:**

* `ItemsControlBinding` is basically `{utu:AncestorBinding AncestorType=ItemsControl}` but shorter.
* Use it any time the parent control holds extra info (like `Tag`, `SelectedItem`, layout settings).

---

## 3. Read a property from a page (or another ancestor)

**Outcome:** item content can read from the enclosing `Page`, even though it’s deep inside an item template.

```xaml
<Page
    Tag="From Page"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:utu="using:Uno.Toolkit.UI">
    <ListView ItemsSource="{Binding Items}" Tag="From ListView">
        <ListView.ItemTemplate>
            <DataTemplate>
                <StackPanel Spacing="4">
                    <!-- page.Tag -->
                    <TextBlock
                        Text="{utu:AncestorBinding AncestorType=Page, Path=Tag}" />
                </StackPanel>
            </DataTemplate>
        </ListView.ItemTemplate>
    </ListView>
</Page>
```

**Why it works:**
`AncestorBinding` lets you target *any* ancestor type, not only items controls.

---

## 4. Reuse the parent DataContext in a template

**Outcome:** item UI can show both the item value *and* something from the parent view-model.

```csharp
public class ExampleModel
{
    public int[] Items { get; } = new[] { 1, 2, 3 };
    public string SomeText { get; } = "Lorem Ipsum";
}
```

```xaml
<Page
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:utu="using:Uno.Toolkit.UI">
    <!-- DataContext = ExampleModel -->
    <ListView ItemsSource="{Binding Items}">
        <ListView.ItemTemplate>
            <DataTemplate>
                <StackPanel Spacing="2">
                    <!-- item itself -->
                    <TextBlock Text="{Binding}" />

                    <!-- parent DataContext.SomeText -->
                    <TextBlock
                        Text="{utu:ItemsControlBinding Path=DataContext.SomeText}" />
                </StackPanel>
            </DataTemplate>
        </ListView.ItemTemplate>
    </ListView>
</Page>
```

This is the **most common** pattern: “I’m in an item, but I still need my page’s data.”

---

## 5. Pick the right helper

**Outcome:** you know when to use which.

* Use **`ItemsControlBinding`** when the thing you want is on the *nearest* items control or its `DataContext`.

  * “I’m in a list item → I want the list’s data context”
  * “I’m in a list item → I want the list’s property”
* Use **`AncestorBinding`** when you need *a specific ancestor type*, not just the nearest list.

  * “I’m in a list item → I want the page’s Tag”
  * “I’m in a list item → I want the parent Grid’s property”

---

## 6. Properties (for reference)

These are the properties you can set on the markup extension (same idea for both):

* **`AncestorType`** (`Type`) – required for `AncestorBinding`. Which ancestor to stop at.
* **`Path`** (`string`) – what to read from that ancestor.
* **`Converter`**, **`ConverterParameter`**, **`ConverterLanguage`** – same behavior as normal bindings.

Example with converter:

```xaml
<TextBlock
    Text="{utu:AncestorBinding AncestorType=Page,
           Path=Tag,
           Converter={StaticResource MyConverter}}" />
```

---

## 7. FAQ

**Q: Can I reach outside the item without naming elements?**
**A:** Yes. That’s the point of these helpers – no `x:Name` needed on the parent.

**Q: Is `ItemsControlBinding` only for `ListView`?**
**A:** No. It works with the closest `ItemsControl` (e.g. `ListView`, `ListBox`, `GridView`), same as saying `AncestorType=ItemsControl`.

**Q: Do I have to set the XAML namespace?**
**A:** Yes, once per XAML file:

```xaml
xmlns:utu="using:Uno.Toolkit.UI"
```

**Q: Does it work on WinUI 3 / non-Windows targets?**
**A:** Yes, these helpers are available on non-Windows UWP platforms and WinUI 3, same as the original doc.

---

That should give you straightforward, sliceable how-tos instead of one long conceptual page.
