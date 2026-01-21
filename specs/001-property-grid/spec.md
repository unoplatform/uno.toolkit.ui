# Feature Specification: PropertyGrid Control

**Feature ID:** 001
**Status:** Draft
**Created:** 2026-01-21
**Last Updated:** 2026-01-21

---

## Overview

A cross-platform PropertyGrid control for Uno Platform that enables runtime inspection and editing of object properties with full support for data binding, validation, custom editors, and accessibility.

### Goals

- Provide a production-ready PropertyGrid control compatible with all Uno Platform targets (WebAssembly, Desktop, Mobile)
- Enable developers to inspect and modify object properties at runtime with minimal configuration
- Support both single and multi-object selection with mixed-value handling
- Deliver a modern, accessible UI consistent with Fluent/WinUI design language
- Ensure high performance through virtualization and metadata caching

### Non-Goals

- Visual designer or XAML editor functionality
- Database schema editing
- File system browser integration
- Advanced data binding expressions editor

---

## User Stories

### US-001: Basic Property Inspection
**As a** developer
**I want to** bind an object to PropertyGrid and see all its public properties
**So that** I can inspect object state at runtime

**Acceptance Criteria:**
- PropertyGrid displays all public instance properties by default
- Properties show name and current value
- Supports binding via `SelectedObject` property
- Read-only properties are displayed but not editable

### US-002: Property Editing
**As a** developer
**I want to** edit property values directly in the PropertyGrid
**So that** I can modify object state at runtime

**Acceptance Criteria:**
- String, numeric, bool, enum properties have built-in editors
- DateTime, TimeSpan have appropriate picker controls
- Color, Brush, Thickness, CornerRadius have specialized editors
- Point, Size, Rect have coordinate editors
- Uri, Guid have text-based editors
- Changes commit on LostFocus, Enter key, or PropertyChanged (configurable)
- Invalid values show validation errors

### US-003: Multi-Object Selection
**As a** developer
**I want to** select multiple objects and edit common properties
**So that** I can batch-modify multiple objects efficiently

**Acceptance Criteria:**
- Supports `SelectedObjects` collection binding
- Shows only properties common to all selected objects
- Displays "—" placeholder for properties with different values
- Editing a property updates all selected objects
- Validation considers all selected objects

### US-004: Property Organization
**As a** developer
**I want to** see properties organized by categories with descriptions
**So that** I can navigate complex objects easily

**Acceptance Criteria:**
- Respects `[Category]` attribute for grouping
- Respects `[DisplayName]` for custom property names
- Respects `[Description]` for property tooltips/descriptions
- Respects `[Browsable(false)]` to hide properties
- Respects `[ReadOnly]` to make properties read-only
- Supports `[DisplayOrder]` or similar for property ordering
- Categories are expandable/collapsible
- Optional description panel shows detailed property information

### US-005: Property Search and Filtering
**As a** developer
**I want to** search and filter properties by name, category, or description
**So that** I can quickly find specific properties in large object graphs

**Acceptance Criteria:**
- Search box filters properties in real-time
- Matches against property name, category name, and description
- Case-insensitive search
- Clears search with escape key or clear button
- Search persists across category expand/collapse

### US-006: Custom Editors
**As a** library author
**I want to** register custom editors for specific types or properties
**So that** I can provide specialized editing experiences

**Acceptance Criteria:**
- `EditorRegistry` allows type-to-editor registration
- `[Editor(typeof(MyEditor))]` attribute support
- `EditorPreparing` event allows dynamic editor assignment
- Custom editors implement `IPropertyEditor` interface
- Built-in editors are extensible/replaceable

### US-007: Validation
**As a** developer
**I want to** see validation errors inline with property editors
**So that** I can ensure data integrity during editing

**Acceptance Criteria:**
- Supports DataAnnotations attributes (`[Required]`, `[Range]`, `[StringLength]`, etc.)
- Supports `INotifyDataErrorInfo` interface
- Displays validation errors inline with editor
- Visual indication of validation state (error icon, red border)
- Error tooltip on hover
- Prevents navigation to invalid property values (optional)

### US-008: Accessibility
**As a** user with accessibility needs
**I want to** navigate and edit properties using keyboard and screen readers
**So that** I can use the PropertyGrid effectively

**Acceptance Criteria:**
- Full keyboard navigation (Tab, Arrow keys, Enter, Escape)
- Screen reader announces property name, value, description, and validation state
- High-contrast theme support
- Respects system text scaling
- Focus indicators visible in all themes

### US-009: Performance with Large Objects
**As a** developer
**I want to** inspect objects with hundreds of properties without UI lag
**So that** I can debug complex objects efficiently

**Acceptance Criteria:**
- Property list is virtualized (only visible rows rendered)
- Property metadata cached per type
- `DeferRefresh()` method batches multiple updates
- Smooth scrolling with 100+ properties
- Category expand/collapse is instant

### US-010: Theming
**As a** developer
**I want to** PropertyGrid to match my app's Fluent/WinUI theme
**So that** I maintain consistent visual design

**Acceptance Criteria:**
- Light and dark theme support
- High-contrast theme compliance
- Compact and comfortable density modes
- Customizable via standard Uno/WinUI styling
- Matches WinUI PropertyGrid appearance where applicable

---

## Functional Requirements

### FR-001: Object Binding
- **MUST** support `SelectedObject` property (single object)
- **MUST** support `SelectedObjects` property (collection)
- **MUST** refresh automatically when bound object's properties change (via INotifyPropertyChanged)
- **SHOULD** support `ICustomTypeDescriptor` for dynamic property lists

### FR-002: Property Reflection
- **MUST** use .NET reflection to discover public instance properties
- **MUST** respect `[Browsable(false)]` to hide properties
- **MUST** respect `[ReadOnly(true)]` to disable editing
- **MUST** respect `[Category]` for grouping
- **MUST** respect `[DisplayName]` for custom labels
- **MUST** respect `[Description]` for tooltips
- **MUST** sort properties by category first, then alphabetically by display name within each category
- **SHOULD** respect `[DisplayOrder]` or `[PropertyOrder]` for custom ordering override within categories

### FR-003: Built-in Editors
- **MUST** provide editors for:
  - String (`TextBox`)
  - Numeric types (`NumberBox` or `TextBox` with numeric validation)
  - Boolean (`CheckBox` or `ToggleSwitch`)
  - Enum (`ComboBox`)
  - DateTime (`DatePicker` + `TimePicker`)
  - TimeSpan (duration editor)
  - Color (`ColorPicker`)
  - Brush (color + gradient editor)
  - Thickness (4-value editor)
  - CornerRadius (4-value editor)
  - Point (X, Y editor)
  - Size (Width, Height editor)
  - Rect (X, Y, Width, Height editor)
  - Uri (`TextBox` with validation)
  - Guid (`TextBox` with validation)
  - Complex objects (expandable inline with nested PropertyGrid rendering)
  - Collections (displays count and "Edit..." button that opens modal dialog with add/remove/reorder capabilities)

### FR-004: Custom Editors
- **MUST** support `[Editor(typeof(TEditor))]` attribute
- **MUST** provide `IPropertyEditor` interface for custom implementations
- **MUST** expose `EditorRegistry` for type-to-editor registration
- **MUST** raise `EditorPreparing` event for dynamic editor assignment
- **MUST** raise `EditorLoaded` event after editor instantiation

### FR-005: Multi-Select Editing
- **MUST** show only common properties when multiple objects selected
- **MUST** display "—" when property values differ across selected objects
- **MUST** update all selected objects when a property is edited
- **MUST** validate changes against all selected objects

### FR-006: Validation
- **MUST** support DataAnnotations attributes
- **MUST** support `INotifyDataErrorInfo`
- **MUST** display validation errors inline with editor
- **MUST** show error icon and tooltip
- **MUST** aggregate validation errors at category level (error indicator on category header with count)
- **MUST** prevent commit of invalid values (configurable)
- **SHOULD** validate on PropertyChanged, LostFocus, or manual trigger

### FR-007: Layout
- **MUST** use two-column layout (property name | editor)
- **MUST** support expandable/collapsible categories with error indicators (show error icon and count on category header when validation errors exist)
- **MUST** support expandable/collapsible complex properties (nested PropertyGrid rendering)
- **MUST** virtualize property list for performance
- **MUST** detect and display circular object references with appropriate UI (e.g., "[Circular Reference]" message)
- **SHOULD** support splitter between name and editor columns
- **SHOULD** support optional description panel (bottom or side)

### FR-008: Search and Filtering
- **MUST** provide search box for filtering properties
- **MUST** match against property name, category, and description
- **MUST** update results in real-time
- **MUST** support case-insensitive matching
- **SHOULD** highlight matching text

### FR-009: Events
- **MUST** raise `PropertyValueChanging` (cancellable, before change)
- **MUST** raise `PropertyValueChanged` (after change)
- **MUST** raise `EditorPreparing` (before editor creation)
- **MUST** raise `EditorLoaded` (after editor instantiation)
- **SHOULD** raise `CategoryExpanding`/`CategoryExpanded`

### FR-010: Performance
- **MUST** virtualize property list (UI virtualization)
- **MUST** cache property metadata per type
- **MUST** provide `DeferRefresh()` / `ResumeRefresh()` for batch updates
- **MUST** avoid reflection on every render cycle
- **SHOULD** support incremental property discovery

### FR-011: Theming
- **MUST** support Fluent light and dark themes
- **MUST** support high-contrast themes
- **MUST** respond to system theme changes
- **SHOULD** support compact/comfortable density modes
- **SHOULD** allow custom styling via standard WinUI/Uno mechanisms

### FR-012: Accessibility
- **MUST** support keyboard navigation (Tab, Arrow keys, Enter, Escape)
- **MUST** support screen readers (Name, Value, Description, State)
- **MUST** provide visible focus indicators
- **MUST** support high-contrast mode
- **MUST** respect system text scaling
- **SHOULD** support shortcut keys (Ctrl+F for search, etc.)

### FR-013: Error Handling
- **MUST** handle missing property getters gracefully (show read-only)
- **MUST** handle missing property setters gracefully (show read-only)
- **MUST** catch and display exceptions from property setters
- **MUST** detect circular object references and display "[Circular Reference]" message in property value (prevents infinite loops during nested expansion)
- **MUST** handle null values in editors

---

## Non-Functional Requirements

### NFR-001: Performance
- Property list with 100 items renders in < 100ms
- Property list with 1000 items scrolls smoothly (60fps)
- Metadata caching reduces reflection overhead by 90%+
- Search/filter responds within 50ms for 100 properties

### NFR-002: Compatibility
- Works on all Uno Platform targets: WebAssembly, Skia (Desktop), iOS, Android, macOS Catalyst
- Compatible with .NET 9.0 and .NET 10.0
- Compatible with Uno Platform 5.x and 6.x
- Works with WinUI 3 on Windows (if Uno.Toolkit.UI targets WinUI)

### NFR-003: Accessibility
- WCAG 2.1 Level AA compliance
- Keyboard navigation covers 100% of functionality
- Screen reader support tested on NVDA (Windows), VoiceOver (macOS/iOS), TalkBack (Android)
- High-contrast mode tested on Windows and macOS

### NFR-004: Maintainability
- Code coverage > 80% (unit tests)
- XML documentation on all public APIs
- Sample app demonstrates all major features
- Follows Uno.Toolkit.UI coding standards and architecture patterns

### NFR-005: Extensibility
- Custom editors can be implemented without modifying source
- Editor registry allows global or scoped registration
- Events allow interception at all key points
- Metadata provider is swappable (support `ICustomTypeDescriptor`)

---

## Technical Approach

### Architecture

**Core Components:**
1. **PropertyGrid** (main control)
   - User-facing XAML control
   - Exposes `SelectedObject`, `SelectedObjects`, `PropertySource`, search, events
   - Hosts `PropertyGridView` internally

2. **PropertyGridView** (internal)
   - Virtualized `ItemsRepeater` or `ListView`
   - Renders categories and property rows
   - Handles layout and scrolling

3. **PropertyDescriptor** (metadata)
   - Wraps `System.ComponentModel.PropertyDescriptor` or `System.Reflection.PropertyInfo`
   - Cached per type via `PropertyDescriptorProvider`
   - Includes: Name, DisplayName, Description, Category, Type, Attributes, ReadOnly, Browsable

4. **PropertyItem** (view model)
   - Represents a single property in the grid
   - Binds to editor control
   - Handles validation state and mixed-value state

5. **IPropertyEditor** (interface)
   - Contract for custom editors
   - Methods: `SetValue()`, `GetValue()`, `OnAttached()`, `OnDetached()`

6. **EditorRegistry**
   - Maps `Type` → `IPropertyEditor` or `DataTemplate`
   - Singleton or per-grid instance
   - Allows global and local overrides

7. **PropertyGridViewModel** (optional)
   - Manages property list, filtering, categorization
   - Handles multi-select logic and mixed-value calculation

### Data Flow

1. User sets `SelectedObject` or `SelectedObjects`
2. `PropertyDescriptorProvider` reflects properties and caches metadata
3. `PropertyItem` objects created for each visible property
4. `PropertyGridView` binds to `PropertyItem` collection
5. `EditorRegistry` assigns appropriate editor to each `PropertyItem`
6. User edits → `PropertyValueChanging` → validation → `SetValue()` → `PropertyValueChanged`

### Platform Considerations

- **WebAssembly**: Reflection may be slower; ensure metadata caching is aggressive
- **Mobile (iOS/Android)**: Touch-friendly editor controls; consider mobile-specific pickers
- **Desktop (Skia)**: Full keyboard support; splitter for column resizing
- **macOS Catalyst**: Native macOS controls where appropriate

### Dependencies

- Uno Platform SDK
- Uno.Toolkit.UI core libraries
- System.ComponentModel.DataAnnotations (for validation)
- System.ComponentModel.TypeConverter (for type conversion)

---

## Key Entities

### PropertyGrid
- **Properties:**
  - `SelectedObject` (object?)
  - `SelectedObjects` (IList?)
  - `PropertySource` (IPropertySource?) - abstraction for reflection provider
  - `SearchText` (string)
  - `ShowCategories` (bool)
  - `ShowDescriptionPanel` (bool)
  - `CommitMode` (PropertyChanged | LostFocus | Explicit)
  - `EditorRegistry` (IEditorRegistry)
- **Events:**
  - `PropertyValueChanging` (PropertyValueChangingEventArgs)
  - `PropertyValueChanged` (PropertyValueChangedEventArgs)
  - `EditorPreparing` (EditorPreparingEventArgs)
  - `EditorLoaded` (EditorLoadedEventArgs)
- **Methods:**
  - `DeferRefresh()` → IDisposable
  - `Refresh()`
  - `ExpandCategory(string categoryName)`
  - `CollapseCategory(string categoryName)`

### PropertyItem
- **Properties:**
  - `Name` (string)
  - `DisplayName` (string)
  - `Description` (string)
  - `Category` (string)
  - `PropertyType` (Type)
  - `Value` (object?)
  - `IsMixedValue` (bool)
  - `IsReadOnly` (bool)
  - `HasError` (bool)
  - `ErrorMessage` (string?)
  - `Editor` (IPropertyEditor)
- **Methods:**
  - `GetValue()` → object?
  - `SetValue(object? value)` → bool
  - `Validate()` → ValidationResult

### IPropertyEditor
- **Properties:**
  - `EditorControl` (FrameworkElement)
  - `Value` (object?)
- **Methods:**
  - `Initialize(PropertyItem item)`
  - `CommitValue()`
  - `CancelEdit()`

### PropertyDescriptor
- **Properties:**
  - `Name` (string)
  - `DisplayName` (string)
  - `Description` (string)
  - `Category` (string)
  - `PropertyType` (Type)
  - `IsBrowsable` (bool)
  - `IsReadOnly` (bool)
  - `Attributes` (AttributeCollection)
- **Methods:**
  - `GetValue(object component)` → object?
  - `SetValue(object component, object? value)`
  - `CanResetValue(object component)` → bool
  - `ResetValue(object component)`

---

## User Interface

### Layout Structure

```
┌─────────────────────────────────────────────┐
│ [🔍 Search...]                       [⚙️]  │ ← Toolbar
├─────────────────────────────────────────────┤
│ ▼ Appearance                                │ ← Category Header
│   ├─ BackgroundColor    [■ #FFFFFF ▼]      │ ← Property Row
│   ├─ Foreground         [■ #000000 ▼]      │
│   └─ FontSize           [14         ▲▼]    │
│ ▼ Layout                                    │
│   ├─ Width              [100        ▲▼]    │
│   ├─ Height             [—          ▲▼]    │ ← Mixed value
│   └─ Margin             [0,0,0,0    ...]   │
│ ▶ Advanced                                  │ ← Collapsed Category
├─────────────────────────────────────────────┤
│ Description Panel                           │
│ Width: The width of the element in device  │
│ independent pixels. Default: NaN (auto).    │
└─────────────────────────────────────────────┘
```

### Visual States

- **Normal**: Default appearance
- **PointerOver**: Hover effect on property row
- **Pressed**: Click feedback
- **Focused**: Keyboard focus indicator
- **ReadOnly**: Grayed out, non-editable appearance
- **Error**: Red border, error icon, tooltip with message
- **MixedValue**: "—" displayed in editor

### Responsive Behavior

- **Wide (> 600px)**: Two-column layout with optional description panel
- **Narrow (< 600px)**: Single column, description panel hidden or in flyout

---

## Success Criteria

### SC-001: Feature Completeness
- All built-in editors (string, numeric, bool, enum, DateTime, TimeSpan, Color, Brush, Thickness, CornerRadius, Point, Size, Rect, Uri, Guid) implemented and tested
- Single and multi-object selection working
- Search and filtering functional
- Categories expandable/collapsible

### SC-002: Performance
- 100 properties render in < 100ms
- Smooth scrolling with 1000 properties (60fps)
- Metadata cached (reflection only once per type)

### SC-003: Accessibility
- Keyboard navigation covers all functionality
- Screen reader tested on NVDA, VoiceOver, TalkBack
- High-contrast mode tested
- WCAG 2.1 Level AA compliant

### SC-004: Platform Coverage
- Tested on WebAssembly (Chrome, Edge, Safari)
- Tested on Desktop (Windows, macOS, Linux via Skia)
- Tested on Mobile (iOS, Android)

### SC-005: Code Quality
- Unit test coverage > 80%
- Zero high-priority static analysis warnings
- XML documentation complete
- Sample app included with all features demonstrated

### SC-006: Validation
- DataAnnotations validation working
- INotifyDataErrorInfo validation working
- Inline error display functional
- Invalid value commit blocked (when enabled)

### SC-007: Extensibility
- Custom editor can be registered and used
- `EditorPreparing` event allows dynamic editor assignment
- All events fire at correct times

---

## Dependencies

### Internal Dependencies
- Uno.Toolkit.UI core infrastructure
- Uno.Toolkit.UI theming system
- Uno.Toolkit.UI MVVM utilities (if used)

### External Dependencies
- Uno Platform SDK (5.x or 6.x)
- .NET 9.0 or .NET 10.0
- System.ComponentModel.DataAnnotations
- System.ComponentModel.TypeConverter

### Optional Dependencies
- ColorPicker control (if not using built-in)
- NumericUpDown control (NumberBox equivalent)

---

## Risks and Mitigations

### Risk 1: Reflection Performance on WebAssembly
**Severity:** High
**Mitigation:**
- Aggressive metadata caching (cache per type, not per instance)
- Lazy property discovery (only reflect visible properties)
- Consider source generators for known types (future enhancement)

### Risk 2: Nested PropertyGrid Rendering Complexity
**Severity:** High
**Mitigation:**
- Implement depth limit (e.g., max 5 levels of nesting) to prevent excessive recursion
- Circular reference detection at property discovery time
- Virtualization must account for nested expanded properties
- Performance testing with deeply nested object graphs
- Provide expansion depth configuration option

### Risk 3: Complex Custom Editors
**Severity:** Medium
**Mitigation:**
- Provide clear `IPropertyEditor` documentation with examples
- Include sample custom editors in sample app
- Support both code-based and XAML-based editors

### Risk 4: Multi-Select Mixed Value Calculation
**Severity:** Medium
**Mitigation:**
- Use efficient equality comparers
- Cache mixed-value state per property
- Provide `DeferRefresh()` for batch updates

### Risk 5: Accessibility Testing Coverage
**Severity:** Medium
**Mitigation:**
- Partner with accessibility experts for validation
- Automated accessibility testing in CI/CD
- Manual testing on all platforms with assistive technologies

### Risk 6: Theme Consistency Across Platforms
**Severity:** Low
**Mitigation:**
- Use Uno.Toolkit.UI theming primitives
- Test on all platforms in light, dark, and high-contrast modes
- Document any platform-specific styling differences

---

## Clarifications

### Session 2026-01-21

- Q: Should PropertyGrid support nested object editing (expandable complex properties)? → A: Support nested object editing with inline expandable properties (recursive PropertyGrid within cells)
- Q: Should collections be editable in-place or require a separate dialog? → A: Collections display count and "Edit..." button opening a modal dialog with add/remove/reorder
- Q: What is the default sort order for properties (alphabetical, definition order, or custom)? → A: Category-first grouping, then alphabetical within each category
- Q: How should circular object references be handled (detect and show message, or break cycle)? → A: Detect circular references and display "[Circular Reference]" message in property value
- Q: Should validation errors be aggregated at the category level? → A: Display error indicator on category header with error count, plus inline errors on individual properties

---

## Open Questions
4. Should we support property grouping beyond categories (e.g., tags)?
6. Should we provide a simplified "compact" mode with single-column layout?
7. What level of customization should be allowed for the description panel (custom templates)?

---

## Assumptions

1. Target users are developers and power users, not end users
2. Objects will primarily use standard .NET types or types with TypeConverters
3. Most custom editors will be simple (single control, not complex dialogs)
4. Performance with 100-500 properties is more important than 10,000+
5. Light and dark themes are sufficient (custom themes use standard Uno/WinUI styling)
6. English is the primary language (localization is a future enhancement)
7. Property values fit in reasonable memory (not streaming large data)
8. Most properties will be simple values, not complex object graphs

---

## Out of Scope

- Visual XAML designer integration
- Code editor integration (Roslyn)
- Database schema editing
- File system browser
- Advanced expression editor (data binding expressions, formulas)
- Undo/redo stack (host application responsibility)
- Property value history/versioning
- Collaborative editing (real-time multi-user)
- Integration with specific third-party UI frameworks (DevExpress, Telerik, etc.)

---

## Future Enhancements

1. **Nested Object Editing**: Expand complex properties inline with drill-down UI
2. **Collection Editing**: In-place add/remove/reorder for collection properties
3. **Property Grouping**: Support tags or custom grouping beyond categories
4. **Localization**: Multi-language support for all UI strings
5. **Source Generators**: Pre-generate metadata for known types to improve WASM performance
6. **Property Value History**: Show recent values or value change timeline
7. **Favorites/Pinned Properties**: Allow users to pin frequently used properties to top
8. **Property Comparison**: Side-by-side comparison of two objects
9. **Advanced Search**: Regex, value-based search, type-based filtering
10. **Export/Import**: JSON or XML serialization of property values

---

## References

- WinUI PropertyGrid (Microsoft internal, limited public documentation)
- WPF PropertyGrid (various third-party implementations)
- Uno Platform Documentation: https://platform.uno/docs/
- Uno.Toolkit.UI: https://github.com/unoplatform/uno.toolkit.ui
- System.ComponentModel: https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel
- DataAnnotations: https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations

---

## Appendix

### Example Usage

```xaml
<Page xmlns:utu="using:Uno.Toolkit.UI">
    <utu:PropertyGrid
        SelectedObject="{x:Bind ViewModel.SelectedControl, Mode=OneWay}"
        ShowCategories="True"
        ShowDescriptionPanel="True"
        SearchText="{x:Bind SearchBox.Text, Mode=OneWay}"
        PropertyValueChanged="OnPropertyValueChanged" />
</Page>
```

```csharp
// Multi-select example
propertyGrid.SelectedObjects = new List<object> { control1, control2, control3 };

// Custom editor registration
PropertyGrid.EditorRegistry.RegisterEditor(typeof(MyCustomType), typeof(MyCustomEditor));

// Batch updates
using (propertyGrid.DeferRefresh())
{
    obj.Property1 = value1;
    obj.Property2 = value2;
    obj.Property3 = value3;
}
// Grid refreshes once when disposed

// Custom editor example
public class ColorEditor : IPropertyEditor
{
    public FrameworkElement EditorControl { get; }
    public object? Value { get; set; }

    public ColorEditor()
    {
        EditorControl = new ColorPicker();
    }

    public void Initialize(PropertyItem item)
    {
        // Bind ColorPicker to item.Value
    }

    public void CommitValue() { /* Commit logic */ }
    public void CancelEdit() { /* Cancel logic */ }
}
```

### Attribute Examples

```csharp
public class Person
{
    [Category("Identity")]
    [DisplayName("Full Name")]
    [Description("The person's full legal name")]
    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [Category("Identity")]
    [DisplayName("Date of Birth")]
    [Description("The person's date of birth")]
    public DateTime DateOfBirth { get; set; }

    [Category("Appearance")]
    [Description("Favorite color for theming")]
    [Editor(typeof(ColorEditor))]
    public Color FavoriteColor { get; set; }

    [Browsable(false)] // Hidden from PropertyGrid
    public string InternalId { get; set; }

    [ReadOnly(true)] // Displayed but not editable
    public int Age => DateTime.Now.Year - DateOfBirth.Year;
}
```

---

**End of Specification**
