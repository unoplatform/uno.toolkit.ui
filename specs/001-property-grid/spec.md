## Clarifications

### Existing Clarifications

(Existing content in the file)

### Session 2026-01-21 (Clarification Phase)

**Architecture & Design:**
- Q: Should PropertyGrid adopt a ViewModel-per-Property architecture pattern? → A: Yes (Option B) - Use specialized ViewModels per property type following Uno.HotDesign pattern
- Q: Should property changes use a message-based architecture? → A: Yes (Option B) - Implement IPropertyUpdater interface for decoupled property updates
- Q: Should editors support inline vs. flyout modes? → A: Yes (Option B) - Simple editors inline, complex editors with optional flyout for advanced editing
- Q: Should PropertyGrid support XAML value source tracking? → A: No (Option A) - Not required for v1, simpler implementation without value source indicators

**Performance & Optimization:**
- Q: What should be the metadata caching strategy? → A: Type-level caching with optional pre-caching for known types
- Q: How should search/filter be debounced? → A: 300ms debounce after last keystroke

**User Experience:**
- Q: Should the description panel be fixed or flyout-based? → A: Flyout-based (Option B) - Property-level flyout on "..." button click
- Q: Should PropertyGrid support "active state" visual feedback? → A: Yes - Show visual feedback for pointer hover, keyboard focus, and active editing state
- Q: Should circular reference detection use depth limit or reference tracking? → A: Both (Option C) - Reference tracking with HashSet<object> plus depth limit (max 10 levels) as failsafe

**Scope Decisions:**
- Q: Should we support property grouping beyond categories? → A: No for v1 - Add to Future Enhancements
- Q: Should we provide a simplified compact single-column mode? → A: No for v1 - Two-column layout is standard
- Q: What level of customization for description panel? → A: Not applicable (using flyout pattern instead)

**Testing & Quality:**
- Q: Should test coverage target specific categories? → A: Yes - Define categories: Unit tests (reflection, caching, validation), Integration tests (ViewModel↔Model sync), Runtime tests (UI rendering), UI tests (keyboard, accessibility)
- Q: Should analytics/telemetry hooks be exposed? → A: Yes (Option B) - Expose events for property changes, consumers implement their own tracking

## Open Questions

4. Should we support property grouping beyond categories (e.g., tags)? - DECISION: No for v1, added to Future Enhancements
6. Should we provide a simplified "compact" mode with single-column layout? - DECISION: No for v1, two-column is standard  
7. What level of customization should be allowed for the description panel? - DECISION: Using flyout pattern, not applicable

## Future Enhancements

11. **Property Grouping Beyond Categories**: Support tags or custom grouping mechanisms in addition to categories
12. **Compact Single-Column Mode**: Simplified layout option for constrained screen sizes
13. **XAML Value Source Tracking**: Track and display whether property values come from literals, bindings, resources, or are inherited

## Feature Requirements (FR)

FR-002: Value Source Tracking is deferred to future version.
FR-007: Description panel will use flyout pattern (property-level flyout on "..." button), not fixed panel.
FR-008: Search input will be debounced with 300ms delay after last keystroke.
FR-010: Detailed metadata caching strategy: Type-level caching with ConcurrentDictionary<Type, PropertyDescriptor[]>, optional pre-caching support for known types.
FR-013: Circular reference detection algorithm: Use HashSet<object> for reference tracking combined with depth limit (max 10 levels) as failsafe.
FR-014: Message-Based Property Updates - MUST implement IPropertyUpdater interface with methods: ChangePropertyValue(PropertyDetails, PropertyValueBase), ClearProperty(PropertyDetails). MUST support debouncing for high-frequency updates. SHOULD expose PropertyValueChanging and PropertyValueChanged events.
FR-015: Editor Modes - MUST support inline editing mode for simple editors (TextBox, CheckBox, ComboBox). MUST support flyout mode for complex editors (ColorPicker, Binding editor, Resource picker). Flyout triggered by "..." button on property row.
FR-016: Active State Visual Feedback - MUST provide visual feedback for pointer hover on property rows. MUST show focus indicators for keyboard navigation. SHOULD show distinct visual state for active editing.
