# PropertyGrid Clarifications

**Date:** 2026-01-21
**Status:** Completed

This document records all clarification decisions made during the specification review process.

---

## Architecture & Design Decisions

### Q1: ViewModel-per-Property Architecture
**Decision:** Yes (Specialized ViewModels per property type)
**Rationale:** Provides better separation of concerns, testability, and is proven in Uno HotDesign
**Impact:**
- Create `PropertyGridCellViewModel` base class
- Implement specialized subclasses: `StringEditorViewModel`, `NumericEditorViewModel`, `BooleanEditorViewModel`, `ColorEditorViewModel`, etc.

### Q2: Message-Based Property Updates
**Decision:** Yes (IPropertyUpdater interface)
**Rationale:** Enables future undo/redo, change tracking, and remote debugging
**Impact:**
- Implement `IPropertyUpdater` interface with `ChangePropertyValue()` and `ClearProperty()` methods
- Add FR-014 to specification

### Q3: Editor Modes (Inline vs. Flyout)
**Decision:** Both (Simple inline, complex with flyout)
**Rationale:** Better UX for complex editing (Color, Binding, Resource)
**Impact:**
- Inline editors: String, Numeric, Boolean, Enum
- Flyout editors: Color, Brush, Thickness, CornerRadius, DateTime, Binding, Resource
- Add FR-015 to specification

### Q4: XAML Value Source Tracking
**Decision:** No (Deferred to v2)
**Rationale:** Adds complexity; not required for v1 scenarios
**Impact:**
- Add to Future Enhancements (#13)
- Note deferral in FR-002

---

## Performance & Optimization Decisions

### Q5: Metadata Caching Strategy
**Decision:** Type-level caching with optional pre-caching
**Details:**
- Cache `PropertyDescriptor[]` per `Type` using `ConcurrentDictionary<Type, PropertyDescriptor[]>`
- Cache on first access, reuse for all instances
- Support opt-in pre-caching for known types
**Impact:** Update FR-010 with detailed caching strategy

### Q6: Search/Filter Debouncing
**Decision:** 300ms debounce after last keystroke
**Rationale:** Balances responsiveness with performance
**Impact:** Update FR-008 with debounce specification

---

## User Experience Decisions

### Q7: Description Panel Pattern
**Decision:** Flyout-based (not fixed panel)
**Details:** Property-level flyout triggered by "..." button
**Rationale:** More flexible, saves screen space
**Impact:** Update FR-007

### Q8: Active State Visual Feedback
**Decision:** Yes
**Details:** Show visual feedback for pointer hover, keyboard focus, active editing
**Rationale:** Improves discoverability and accessibility
**Impact:** Add FR-016

### Q9: Circular Reference Detection
**Decision:** Both (Reference tracking + depth limit)
**Details:**
- Use `HashSet<object>` for reference tracking
- Add depth limit (default: 10 levels) as failsafe
- Display "[Circular Reference to {Type}]" message
**Rationale:** Most robust approach
**Impact:** Update FR-013 with detailed algorithm

---

## Scope Decisions

### Q10: Property Grouping Beyond Categories
**Decision:** No for v1
**Impact:** Add to Future Enhancements (#11)

### Q11: Compact Single-Column Mode
**Decision:** No for v1
**Rationale:** Two-column layout is standard
**Impact:** Add to Future Enhancements (#12)

### Q12: Description Panel Customization
**Decision:** Not applicable (using flyout pattern)

---

## Testing & Quality Decisions

### Q13: Test Coverage Categories
**Decision:** Define specific categories
**Categories:**
- **Unit Tests**:  Property reflection, metadata caching, validation logic, circular reference detection, search/filter algorithms
- **Integration Tests**: Property value changes, editor selection, event firing, ViewModel↔Model sync
- **Runtime Tests**:  UI rendering on all platforms, keyboard navigation, theme changes, virtualization performance
- **UI Tests**: Visual regression, accessibility (keyboard, screen reader), touch interaction, flyout behavior
**Impact:** Update SC-005 with detailed breakdown

### Q14: Analytics/Telemetry Hooks
**Decision:** Expose events (Option B)
**Details:** Consumers implement tracking via `PropertyValueChanged` event
**Impact:** No built-in telemetry; events provide extensibility

---

## Implementation Impact Summary

### New Functional Requirements
- **FR-014**:  Message-Based Property Updates (IPropertyUpdater)
- **FR-015**: Editor Modes (Inline vs.  Flyout)
- **FR-016**: Active State Visual Feedback

### Updated Functional Requirements
- **FR-002**: Note XAML value source tracking deferral
- **FR-007**: Replace fixed description panel with flyout pattern
- **FR-008**: Add 300ms debounce specification
- **FR-010**: Detail metadata caching strategy
- **FR-013**: Specify circular reference detection algorithm

### Architecture Additions
- PropertyGridCellViewModel (base class + specialized subclasses)
- IPropertyUpdater interface
- PropertyDetails immutable record

### Future Enhancements Added
- #11: Property Grouping Beyond Categories
- #12: Compact Single-Column Mode
- #13: XAML Value Source Tracking

### Test Categories Defined
- Unit, Integration, Runtime, UI test breakdown with specific coverage requirements

---

## Open Questions Resolution

All open questions from the original specification have been resolved:
- Q4 (Property grouping) → Deferred to v2 (#11)
- Q6 (Compact mode) → Deferred to v2 (#12)
- Q7 (Description panel customization) → N/A (using flyout)

---

**Status:** All clarifications complete.  Ready for implementation planning phase.
