# Lessons

## Theme sample hosts use the shared catalog

- User correction (2026-09-07): the Fluent sample must be constructed like Simple and Material and display every page with `FluentTemplate`.
- A standalone gallery demonstrated styles but bypassed the repository's sample navigation and coverage. Before adding a theme head, inspect the shared `.projitems`, partial `App`, theme constants, catalog filters, and `SamplePageLayout` selection.
- Reuse those components and add a regression that checks the real catalog and selected template. The repo-wide rule is recorded in `AGENTS.md`.
