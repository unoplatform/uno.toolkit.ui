# Third-Party Notices

Uno Toolkit incorporates third-party material from the projects listed below. The original copyright
notices and the licenses under which that material was received are reproduced here in full.

Every vendored source file also carries a short provenance header naming its upstream path, the pinned
commit, the import date, and the SPDX license identifier — those headers point back at this file for the
full text.

---

## Yoga flexbox layout engine (C# port)

Used in three places in this repository:

| Path | Relationship |
|---|---|
| `src/Uno.Toolkit.UI/Layout/Yoga/` | vendored near-verbatim; the layout engine itself |
| `src/Uno.Toolkit.RuntimeTests/Tests/Yoga/` | vendored; the generated conformance corpus |
| `src/Uno.Toolkit.UI/Controls/FlexPanel/` | **adapted** from upstream's `src/Reactor/Yoga/FlexPanel.cs`, then diverged (this one is ours to maintain) |

The engine is compiled into the `Uno.Toolkit.WinUI` assembly, so this notice file also ships
inside that NuGet package — MIT requires the notice to travel with every copy, and a compiled
package is a copy.

The code is a C# line-port, authored by Microsoft as part of the Reactor project, of Meta's Yoga layout
engine. Both copyright holders are named below; both grants are MIT.

| | |
|---|---|
| Immediate upstream | <https://github.com/microsoft/microsoft-ui-reactor> |
| Tag | `v0.1.0-preview.13` |
| Commit | `c9191b97c40a2e4d6bcbc72df7714184862b4d36` |
| Upstream paths | `src/Reactor/Yoga/`, `tests/Reactor.Tests/YogaGenerated/` |
| Imported | 2026-09-04 |
| Original engine | <https://github.com/facebook/yoga> |

See [`src/Uno.Toolkit.UI/Layout/Yoga/README.md`](src/Uno.Toolkit.UI/Layout/Yoga/README.md) for the
adaptations applied on import and the procedure for re-syncing.

### MIT License — Reactor (the C# port)

```text
MIT License

Copyright (c) Microsoft Corporation.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

### MIT License — Yoga (the original engine)

```text
MIT License

Copyright (c) Facebook, Inc. and its affiliates.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```
