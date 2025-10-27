#!/usr/bin/env python3
"""
Generate concise, searchable documentation from full Toolkit control documentation.

This script processes all markdown documentation files for controls and helpers,
creating condensed versions optimized for search and retrieval systems.
"""

import os
import re
from pathlib import Path
from typing import Dict, List, Tuple


class ConciseDocGenerator:
    """Generates concise documentation from markdown files."""
    
    def __init__(self, doc_dir: str, output_dir: str):
        self.doc_dir = Path(doc_dir)
        self.output_dir = Path(output_dir)
        self.output_dir.mkdir(exist_ok=True)
    
    def extract_yaml_front_matter(self, content: str) -> Tuple[Dict[str, str], str]:
        """Extract YAML front matter and return it with remaining content."""
        front_matter = {}
        remaining_content = content
        
        if content.startswith('---'):
            parts = content.split('---', 2)
            if len(parts) >= 3:
                yaml_content = parts[1].strip()
                remaining_content = parts[2].strip()
                
                # Parse simple YAML (uid only for our case)
                for line in yaml_content.split('\n'):
                    if ':' in line:
                        key, value = line.split(':', 1)
                        front_matter[key.strip()] = value.strip()
        
        return front_matter, remaining_content
    
    def extract_title(self, content: str) -> Tuple[str, str]:
        """Extract the main title and return it with remaining content."""
        lines = content.split('\n')
        title = ""
        start_idx = 0
        
        for i, line in enumerate(lines):
            if line.startswith('# '):
                title = line[2:].strip()
                start_idx = i + 1
                break
        
        remaining = '\n'.join(lines[start_idx:]).strip()
        return title, remaining
    
    def extract_summary(self, content: str) -> Tuple[str, str]:
        """Extract summary/description section."""
        lines = content.split('\n')
        summary_lines = []
        remaining_lines = []
        in_summary = True
        
        for i, line in enumerate(lines):
            # Stop at first section header or specific markers
            if line.startswith('##') or line.startswith('###'):
                in_summary = False
                remaining_lines.extend(lines[i:])
                break
            
            if in_summary:
                # Skip TIP/NOTE blocks in summary
                if line.strip().startswith('> [!'):
                    continue
                # Skip video embeds in summary
                if 'Video' in line and 'youtube' in line.lower():
                    continue
                if line.strip():
                    summary_lines.append(line)
            else:
                remaining_lines.append(line)
        
        if not remaining_lines and in_summary:
            # All lines were summary
            remaining_lines = []
        
        summary = '\n'.join(summary_lines).strip()
        # Limit summary to first 3 paragraphs
        paragraphs = [p for p in summary.split('\n\n') if p.strip()]
        summary = '\n\n'.join(paragraphs[:3])
        
        remaining = '\n'.join(remaining_lines).strip()
        return summary, remaining
    
    def extract_section(self, content: str, section_name: str, level: int = 2) -> str:
        """Extract a specific section by name."""
        header_marker = '#' * level
        pattern = f'^{header_marker} {re.escape(section_name)}\\s*$'
        
        lines = content.split('\n')
        section_lines = []
        in_section = False
        
        for i, line in enumerate(lines):
            if re.match(pattern, line, re.IGNORECASE):
                in_section = True
                continue
            
            if in_section:
                # Stop at next section of same or higher level
                if line.startswith('#' * level + ' ') and not line.startswith('#' * (level + 1)):
                    break
                section_lines.append(line)
        
        return '\n'.join(section_lines).strip()
    
    def extract_properties_table(self, content: str) -> str:
        """Extract properties/attached properties table."""
        properties = self.extract_section(content, "Properties", level=2)
        if not properties:
            properties = self.extract_section(content, "Attached Properties", level=2)
        if not properties:
            # Try level 3 for nested structures
            properties = self.extract_section(content, "Properties", level=3)
        if not properties:
            properties = self.extract_section(content, "Attached Properties", level=3)
        
        # Clean up and limit table size
        if properties:
            lines = properties.split('\n')
            # Keep table header and up to 15 rows
            table_lines = [l for l in lines if '|' in l]
            if len(table_lines) > 17:  # header + separator + 15 rows
                table_lines = table_lines[:17] + ['| ... | ... | ... |']
            properties = '\n'.join(table_lines)
        
        return properties
    
    def extract_usage_examples(self, content: str) -> str:
        """Extract usage examples section."""
        usage = self.extract_section(content, "Usage")
        
        if usage:
            # Keep only code blocks, limit to 2-3 examples
            code_blocks = re.findall(r'```[\s\S]*?```', usage)
            if code_blocks:
                # Take first 2 code blocks
                usage = '\n\n'.join(code_blocks[:2])
        
        return usage
    
    def extract_constructors(self, content: str) -> str:
        """Extract constructors section."""
        constructors = self.extract_section(content, "Constructors", level=3)
        return constructors
    
    def extract_events(self, content: str) -> str:
        """Extract events section."""
        events = self.extract_section(content, "Events", level=3)
        return events
    
    def clean_markdown(self, text: str) -> str:
        """Clean up markdown text by removing excessive whitespace."""
        # Remove multiple blank lines
        text = re.sub(r'\n{3,}', '\n\n', text)
        return text.strip()
    
    def generate_concise_doc(self, input_file: Path) -> str:
        """Generate concise documentation from a markdown file."""
        with open(input_file, 'r', encoding='utf-8') as f:
            content = f.read()
        
        # Extract components
        front_matter, content = self.extract_yaml_front_matter(content)
        title, content = self.extract_title(content)
        summary, content = self.extract_summary(content)
        
        # Extract specific sections
        properties = self.extract_properties_table(content)
        constructors = self.extract_constructors(content)
        events = self.extract_events(content)
        usage = self.extract_usage_examples(content)
        
        # Build concise document
        concise_parts = []
        
        # Front matter
        if front_matter:
            concise_parts.append('---')
            for key, value in front_matter.items():
                concise_parts.append(f'{key}: {value}')
            concise_parts.append('---')
            concise_parts.append('')
        
        # Title
        concise_parts.append(f'# {title} (Concise Reference)')
        concise_parts.append('')
        
        # Summary
        if summary:
            concise_parts.append('## Summary')
            concise_parts.append('')
            concise_parts.append(summary)
            concise_parts.append('')
        
        # Constructors
        if constructors:
            concise_parts.append('## Constructors')
            concise_parts.append('')
            concise_parts.append(constructors)
            concise_parts.append('')
        
        # Properties
        if properties:
            concise_parts.append('## Properties')
            concise_parts.append('')
            concise_parts.append(properties)
            concise_parts.append('')
        
        # Events
        if events:
            concise_parts.append('## Events')
            concise_parts.append('')
            concise_parts.append(events)
            concise_parts.append('')
        
        # Usage
        if usage:
            concise_parts.append('## Usage Examples')
            concise_parts.append('')
            concise_parts.append(usage)
            concise_parts.append('')
        
        # Footer
        concise_parts.append('---')
        concise_parts.append('')
        concise_parts.append(f'**Note**: This is a concise reference. ')
        concise_parts.append(f'For complete documentation, see [{input_file.name}]({input_file.name})')
        
        return self.clean_markdown('\n'.join(concise_parts))
    
    def process_directory(self, subdir: str):
        """Process all markdown files in a subdirectory."""
        source_dir = self.doc_dir / subdir
        output_subdir = self.output_dir / subdir
        output_subdir.mkdir(exist_ok=True)
        
        if not source_dir.exists():
            print(f"Warning: {source_dir} does not exist")
            return
        
        md_files = list(source_dir.glob('*.md'))
        print(f"\nProcessing {len(md_files)} files in {subdir}/")
        
        for md_file in md_files:
            try:
                concise_content = self.generate_concise_doc(md_file)
                
                # Write to output with .concise.md suffix
                output_file = output_subdir / f'{md_file.stem}.concise.md'
                with open(output_file, 'w', encoding='utf-8') as f:
                    f.write(concise_content)
                
                print(f"  ✓ Generated {output_file.relative_to(self.output_dir)}")
            except Exception as e:
                print(f"  ✗ Error processing {md_file.name}: {e}")
    
    def generate_index(self):
        """Generate an index file for all concise documentation."""
        index_parts = []
        index_parts.append('# Concise Documentation Index')
        index_parts.append('')
        index_parts.append('This directory contains concise, searchable versions of the Uno Toolkit documentation.')
        index_parts.append('')
        index_parts.append('## Purpose')
        index_parts.append('')
        index_parts.append('These concise documents are optimized for:')
        index_parts.append('- Quick reference and lookup')
        index_parts.append('- Search and retrieval systems')
        index_parts.append('- Documentation embedding and indexing')
        index_parts.append('- Rapid comprehension of control capabilities')
        index_parts.append('')
        index_parts.append('## Structure')
        index_parts.append('')
        index_parts.append('Each concise document includes:')
        index_parts.append('- **Summary**: Brief description of the control/helper')
        index_parts.append('- **Properties**: Key properties and their descriptions')
        index_parts.append('- **Constructors**: Available constructors (when applicable)')
        index_parts.append('- **Events**: Available events (when applicable)')
        index_parts.append('- **Usage Examples**: Essential code examples')
        index_parts.append('')
        index_parts.append('## Controls')
        index_parts.append('')
        
        # List control files
        controls_dir = self.output_dir / 'controls'
        if controls_dir.exists():
            for md_file in sorted(controls_dir.glob('*.concise.md')):
                name = md_file.stem.replace('.concise', '')
                index_parts.append(f'- [{name}](controls/{md_file.name})')
        
        index_parts.append('')
        index_parts.append('## Helpers')
        index_parts.append('')
        
        # List helper files
        helpers_dir = self.output_dir / 'helpers'
        if helpers_dir.exists():
            for md_file in sorted(helpers_dir.glob('*.concise.md')):
                name = md_file.stem.replace('.concise', '')
                index_parts.append(f'- [{name}](helpers/{md_file.name})')
        
        index_parts.append('')
        index_parts.append('---')
        index_parts.append('')
        index_parts.append('Generated from the full documentation at `doc/controls/` and `doc/helpers/`.')
        
        # Write index
        index_file = self.output_dir / 'README.md'
        with open(index_file, 'w', encoding='utf-8') as f:
            f.write('\n'.join(index_parts))
        
        print(f"\n✓ Generated {index_file.relative_to(self.doc_dir.parent)}")


def main():
    """Main entry point."""
    script_dir = Path(__file__).parent
    doc_dir = script_dir
    output_dir = script_dir / 'concise'
    
    print("=" * 60)
    print("Generating Concise Documentation")
    print("=" * 60)
    
    generator = ConciseDocGenerator(doc_dir, output_dir)
    
    # Process controls and helpers
    generator.process_directory('controls')
    generator.process_directory('helpers')
    
    # Generate index
    generator.generate_index()
    
    print("\n" + "=" * 60)
    print("✓ Concise documentation generation complete!")
    print(f"  Output directory: {output_dir}")
    print("=" * 60)


if __name__ == '__main__':
    main()
