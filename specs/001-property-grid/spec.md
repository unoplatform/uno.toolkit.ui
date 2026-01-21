# Property Grid Specification

<!-- Content of the existing spec file before the specified updates -->

## Last Updated
2026-01-21

## Architecture

### ViewModel-per-Property Pattern
After each property in the grid, it is essential to utilize a ViewModel-per-Property architecture. This approach ensures that each property can manage its state and behavior independently, promoting better encapsulation and separation of concerns.

## Functional Requirements

### FR-002: Value Source Tracking
The property grid must implement Value Source Tracking to determine the origin of property values. This allows for informed decisions based on property origins, improving data handling.

### FR-003: Built-in Editors
The property grid will support various built-in editors. Editors can be either inline or presented in flyout menus, allowing for flexible user interaction.

...

### FR-007: Property Flyout Pattern
A flyout pattern should be employed for properties that require additional configuration. This ensures a clean and user-friendly interface.

### FR-010: Metadata Caching Strategy
Implement a caching strategy for metadata retrieval, along with search debouncing to optimize the property grid's responsiveness.

### FR-013: Circular Reference Detection Algorithm
To prevent potential issues with property references, a circular reference detection algorithm will be in place, enhancing the reliability of the property grid.

### FR-014: Message-Based Property Updates
Newly introduced: The property grid will support message-based updates via the IPropertyUpdater interface, which facilitates dynamic updates across properties.

### FR-015: Value Source Tracking (Optional)
To accommodate optional tracking scenarios, support for value source tracking at a configurable level will be provided.

### FR-016: Responsive Extension Integration
To ensure that extensions can respond appropriately to property changes, integration points for responsive extensions will be part of the property grid architecture.

## Key Entities
- **PropertyDescriptor**: Contains information about a property in the grid.
- **PropertyDetails**: A detailed record of property characteristics after PropertyDescriptor.
- **PropertyValueBase**: A hierarchy representing the possible values for properties post PropertyDetails.
- **IPropertyUpdater**: An interface for message-based updates to properties.
- **Event Argument Types**: Specifies various event types pertinent to the property grid operations.

## Test Categories
### SC-005: Detailed Test Categories
Comprehensive test categories should be implemented from unit tests to integration tests to cover all aspects of property grid functionality.

## Appendix
### GridDefinitionsEditor Example
An example of GridDefinitionsEditor is included to demonstrate its complexity and capabilities.

## Open Questions
- How will the active state of a property be tracked?
- What requirements are there for design-time data support?
- How should properties be ordered in the grid?
- What analytics should be collected regarding property usage?

End of Specification