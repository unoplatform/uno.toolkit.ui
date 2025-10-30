# Concise Documentation

## Overview

The `concise/` directory contains condensed, searchable versions of all Uno Toolkit control and helper documentation. These files are optimized for quick reference, search indexing, and documentation retrieval systems.

## Purpose

Concise documentation files serve multiple purposes:

- **Quick Reference**: Developers can quickly find essential information without reading full documentation
- **Search Optimization**: Reduced file sizes make searching and indexing more efficient
- **Documentation Systems**: Ideal for integration with documentation search tools and knowledge bases
- **Development Tools**: Can be used by IDE plugins, code completion tools, and documentation viewers

## What's Included

Each concise document contains:

- **Summary**: Brief description of the control or helper
- **Properties**: All public properties with types and descriptions
- **Constructors**: Available constructors (when applicable)
- **Events**: Available events (when applicable)
- **Usage Examples**: Essential code examples showing common usage patterns

## File Structure

```
doc/
├── concise/
│   ├── README.md               # Index of all concise documentation
│   ├── controls/               # Concise versions of control documentation
│   │   ├── ChipAndChipGroup.concise.md
│   │   ├── NavigationBar.concise.md
│   │   └── ...
│   └── helpers/                # Concise versions of helper documentation
│       ├── command-extensions.concise.md
│       ├── Input-extensions.concise.md
│       └── ...
└── generate_concise_docs.py    # Script to generate concise documentation
```

## Generating Concise Documentation

To regenerate all concise documentation files:

```bash
cd doc/
python3 generate_concise_docs.py
```

The script will:
1. Process all markdown files in `doc/controls/` and `doc/helpers/`
2. Extract key sections (summary, properties, events, usage)
3. Generate condensed `.concise.md` files in the `doc/concise/` directory
4. Create an index file at `doc/concise/README.md`

## When to Regenerate

Regenerate concise documentation when:

- New controls or helpers are added
- Existing documentation is significantly updated
- Properties, events, or usage examples change
- You want to ensure concise docs are in sync with full documentation

## Benefits

### Size Reduction
Concise files are typically 70-90% smaller than full documentation:
- Full doc: 278 lines → Concise: 50 lines (82% reduction)
- Full doc: 840 lines → Concise: 90 lines (89% reduction)

### Improved Searchability
- Removes verbose explanations and repetitive content
- Focuses on API surface and essential examples
- Maintains all critical technical information
- Preserves code examples for practical reference

### Easy Integration
- Standard markdown format
- Preserves YAML front matter for metadata
- Links back to full documentation
- Compatible with static site generators

## Full Documentation

For complete documentation with detailed explanations, platform-specific notes, advanced usage, and troubleshooting:

- **Controls**: See files in `doc/controls/`
- **Helpers**: See files in `doc/helpers/`
- **Online**: Visit the [Uno Toolkit documentation](https://platform.uno/docs/articles/external/uno.toolkit.ui/doc/getting-started.html)

## Maintenance

The generation script (`generate_concise_docs.py`) is designed to be:

- **Idempotent**: Running multiple times produces the same output
- **Self-contained**: No external dependencies beyond Python 3
- **Maintainable**: Clear structure and well-documented functions
- **Extensible**: Easy to modify extraction rules or add new sections

## Technical Details

### Extraction Algorithm

The script uses pattern matching and section detection to extract:

1. **YAML Front Matter**: Preserves metadata like `uid`
2. **Title**: Main heading of the document
3. **Summary**: First 3 paragraphs before any section headers
4. **Properties**: Tables from `## Properties` or `### Properties` sections
5. **Constructors**: Tables from `### Constructors` sections
6. **Events**: Tables from `### Events` sections
7. **Usage**: First 2 code blocks from `## Usage` sections

### Formatting Rules

- Preserves markdown tables
- Removes verbose notes and warnings
- Excludes video embeds
- Limits property tables to 15 rows
- Includes reference link to full documentation
