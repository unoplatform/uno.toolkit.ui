# Feature Specifications

This directory contains feature specifications for Uno Toolkit UI development.

## Speckit Workflow

This repository uses the Speckit workflow for structured feature development:

### 1. Specify (`/speckit.specify`)
Create a detailed feature specification document. This establishes:
- Feature overview and goals
- User stories and acceptance criteria
- Functional and non-functional requirements
- Technical approach
- Success criteria

### 2. Clarify (`/speckit.clarify`)
Interactive session to resolve ambiguities and missing decision points in the specification.

### 3. Plan (`/speckit.plan`)
Generate an implementation plan with:
- Architecture and design decisions
- Component breakdown
- Integration points
- Risk analysis

### 4. Tasks (`/speckit.tasks`)
Create actionable GitHub issues from the implementation plan.

### 5. Implement (`/speckit.implement`)
Execute implementation tasks with traceability back to requirements.

## Directory Structure

```
specs/
├── README.md (this file)
└── NNN-feature-name/        # Feature directory (NNN = 3-digit number)
    ├── spec.md              # Feature specification
    ├── plan.md              # Implementation plan
    └── tasks.json           # Task definitions
```

## Creating a New Feature Specification

1. Create a feature branch: `git checkout -b NNN-feature-name`
2. Run `/speckit.specify` to create the initial specification
3. Run `/speckit.clarify` to resolve ambiguities
4. Run `/speckit.plan` to generate implementation plan
5. Run `/speckit.tasks` to create GitHub issues

## Prerequisites

- Git repository
- PowerShell 7+ (for script execution)
- Feature branch (format: `NNN-feature-name`)

## Prompt Files

Prompt instructions are stored in [.github/prompts/](.github/prompts/).
