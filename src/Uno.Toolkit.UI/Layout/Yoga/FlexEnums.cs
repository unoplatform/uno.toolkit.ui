// Vendored from microsoft/microsoft-ui-reactor @ v0.1.0-preview.13 (c9191b97c40a2e4d6bcbc72df7714184862b4d36), imported 2026-09-04.
// Source: src/Reactor/Yoga/FlexEnums.cs -- DO NOT HAND-EDIT; see Layout/Yoga/README.md.
// SPDX-License-Identifier: MIT -- (c) Microsoft Corporation (C# port); (c) Facebook, Inc. and its affiliates (Yoga).
// Full license text: THIRD-PARTY-NOTICES.md

// Public flex layout enums, originally from Yoga.
// These are the user-facing enum types for flex layout configuration.
// AI-HINT: Maps 1:1 to CSS Flexbox enum values. Used by FlexPanel and YogaStyle.

namespace Uno.Toolkit.UI;

public enum FlexAlign
{
    Auto = 0,
    FlexStart = 1,
    Center = 2,
    FlexEnd = 3,
    Stretch = 4,
    Baseline = 5,
    SpaceBetween = 6,
    SpaceAround = 7,
    SpaceEvenly = 8,
    Start = 9,
    End = 10,
}

public enum FlexDirection
{
    Column = 0,
    ColumnReverse = 1,
    Row = 2,
    RowReverse = 3,
}

public enum FlexJustify
{
    Auto = 0,
    FlexStart = 1,
    Center = 2,
    FlexEnd = 3,
    SpaceBetween = 4,
    SpaceAround = 5,
    SpaceEvenly = 6,
    Stretch = 7,
    Start = 8,
    End = 9,
}

public enum FlexLayoutDirection
{
    Inherit = 0,
    LeftToRight = 1,
    RightToLeft = 2,
}

public enum FlexPositionType
{
    Static = 0,
    Relative = 1,
    Absolute = 2,
}

public enum FlexWrap
{
    NoWrap = 0,
    Wrap = 1,
    WrapReverse = 2,
}
