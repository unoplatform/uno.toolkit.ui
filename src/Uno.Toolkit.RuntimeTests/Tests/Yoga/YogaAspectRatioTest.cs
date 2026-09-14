// Vendored from microsoft/microsoft-ui-reactor @ v0.1.0-preview.13 (c9191b97c40a2e4d6bcbc72df7714184862b4d36), imported 2026-09-04.
// Source: tests/Reactor.Tests/YogaGenerated/YogaAspectRatioTest.cs -- xUnit->MSTest; DO NOT HAND-EDIT; see Tests/Yoga/README.md.
// SPDX-License-Identifier: MIT -- (c) Microsoft Corporation (C# port); (c) Facebook, Inc. and its affiliates (Yoga).
// Full license text: THIRD-PARTY-NOTICES.md

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Toolkit.UI;
using Uno.Toolkit.UI.Yoga;

namespace Uno.Toolkit.RuntimeTests.Tests.Yoga;

/// <summary>
/// Ported from yoga/tests/generated/YGAspectRatioTest.cpp
/// </summary>
[TestClass]
public class YogaAspectRatioTest
{
    [TestMethod]
    [Ignore("Skipped in upstream Yoga (GTEST_SKIP)")]
    public void Aspect_Ratio_Does_Not_Stretch_Cross_Axis_Dim()
    {
        // TODO: GTEST_SKIP();
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(300f);
        root.Height = YogaValue.Point(300f);
        var root_child0 = new YogaNode(config);
        root_child0.FlexGrow = 1f;
        root_child0.FlexShrink = 1f;
        root_child0.FlexBasis = YogaValue.Percent(0f);
        root_child0.Overflow = YogaOverflow.Scroll;
        root.InsertChild(root_child0, 0);
        var root_child0_child0 = new YogaNode(config);
        root_child0_child0.FlexDirection = FlexDirection.Row;
        root_child0.InsertChild(root_child0_child0, 0);
        var root_child0_child0_child0 = new YogaNode(config);
        root_child0_child0_child0.FlexGrow = 2f;
        root_child0_child0_child0.FlexShrink = 1f;
        root_child0_child0_child0.FlexBasis = YogaValue.Percent(0f);
        root_child0_child0_child0.AspectRatio = 1f;
        root_child0_child0.InsertChild(root_child0_child0_child0, 0);
        var root_child0_child0_child1 = new YogaNode(config);
        root_child0_child0_child1.Width = YogaValue.Point(5f);
        root_child0_child0.InsertChild(root_child0_child0_child1, 1);
        var root_child0_child0_child2 = new YogaNode(config);
        root_child0_child0_child2.FlexGrow = 1f;
        root_child0_child0_child2.FlexShrink = 1f;
        root_child0_child0_child2.FlexBasis = YogaValue.Percent(0f);
        root_child0_child0.InsertChild(root_child0_child0_child2, 2);
        var root_child0_child0_child2_child0 = new YogaNode(config);
        root_child0_child0_child2_child0.FlexGrow = 1f;
        root_child0_child0_child2_child0.FlexShrink = 1f;
        root_child0_child0_child2_child0.FlexBasis = YogaValue.Percent(0f);
        root_child0_child0_child2_child0.AspectRatio = 1f;
        root_child0_child0_child2.InsertChild(root_child0_child0_child2_child0, 0);
        var root_child0_child0_child2_child0_child0 = new YogaNode(config);
        root_child0_child0_child2_child0_child0.Width = YogaValue.Point(5f);
        root_child0_child0_child2_child0.InsertChild(root_child0_child0_child2_child0_child0, 0);
        var root_child0_child0_child2_child0_child1 = new YogaNode(config);
        root_child0_child0_child2_child0_child1.FlexGrow = 1f;
        root_child0_child0_child2_child0_child1.FlexShrink = 1f;
        root_child0_child0_child2_child0_child1.FlexBasis = YogaValue.Percent(0f);
        root_child0_child0_child2_child0_child1.AspectRatio = 1f;
        root_child0_child0_child2_child0.InsertChild(root_child0_child0_child2_child0_child1, 1);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(300f, root.LayoutWidth);
        YogaAssert.Equal(300f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(300f, root_child0.LayoutWidth);
        YogaAssert.Equal(300f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(300f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(197f, root_child0_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0_child0.LayoutY);
        YogaAssert.Equal(197f, root_child0_child0_child0.LayoutWidth);
        YogaAssert.Equal(197f, root_child0_child0_child0.LayoutHeight);
        YogaAssert.Equal(197f, root_child0_child0_child1.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0_child1.LayoutY);
        YogaAssert.Equal(5f, root_child0_child0_child1.LayoutWidth);
        YogaAssert.Equal(197f, root_child0_child0_child1.LayoutHeight);
        YogaAssert.Equal(202f, root_child0_child0_child2.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0_child2.LayoutY);
        YogaAssert.Equal(98f, root_child0_child0_child2.LayoutWidth);
        YogaAssert.Equal(197f, root_child0_child0_child2.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0_child2_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0_child2_child0.LayoutY);
        YogaAssert.Equal(98f, root_child0_child0_child2_child0.LayoutWidth);
        YogaAssert.Equal(197f, root_child0_child0_child2_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0_child2_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0_child2_child0_child0.LayoutY);
        YogaAssert.Equal(5f, root_child0_child0_child2_child0_child0.LayoutWidth);
        YogaAssert.Equal(0f, root_child0_child0_child2_child0_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0_child2_child0_child1.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0_child2_child0_child1.LayoutY);
        YogaAssert.Equal(98f, root_child0_child0_child2_child0_child1.LayoutWidth);
        YogaAssert.Equal(197f, root_child0_child0_child2_child0_child1.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(300f, root.LayoutWidth);
        YogaAssert.Equal(300f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(300f, root_child0.LayoutWidth);
        YogaAssert.Equal(300f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(300f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(197f, root_child0_child0.LayoutHeight);
        YogaAssert.Equal(103f, root_child0_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0_child0.LayoutY);
        YogaAssert.Equal(197f, root_child0_child0_child0.LayoutWidth);
        YogaAssert.Equal(197f, root_child0_child0_child0.LayoutHeight);
        YogaAssert.Equal(98f, root_child0_child0_child1.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0_child1.LayoutY);
        YogaAssert.Equal(5f, root_child0_child0_child1.LayoutWidth);
        YogaAssert.Equal(197f, root_child0_child0_child1.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0_child2.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0_child2.LayoutY);
        YogaAssert.Equal(98f, root_child0_child0_child2.LayoutWidth);
        YogaAssert.Equal(197f, root_child0_child0_child2.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0_child2_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0_child2_child0.LayoutY);
        YogaAssert.Equal(98f, root_child0_child0_child2_child0.LayoutWidth);
        YogaAssert.Equal(197f, root_child0_child0_child2_child0.LayoutHeight);
        YogaAssert.Equal(93f, root_child0_child0_child2_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0_child2_child0_child0.LayoutY);
        YogaAssert.Equal(5f, root_child0_child0_child2_child0_child0.LayoutWidth);
        YogaAssert.Equal(0f, root_child0_child0_child2_child0_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0_child2_child0_child1.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0_child2_child0_child1.LayoutY);
        YogaAssert.Equal(98f, root_child0_child0_child2_child0_child1.LayoutWidth);
        YogaAssert.Equal(197f, root_child0_child0_child2_child0_child1.LayoutHeight);
    }

    [TestMethod]
    public void Zero_Aspect_Ratio_Behaves_Like_Auto()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(300f);
        root.Height = YogaValue.Point(300f);
        var root_child0 = new YogaNode(config);
        root_child0.AspectRatio = 0f;
        root_child0.Width = YogaValue.Point(50f);
        root.InsertChild(root_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(300f, root.LayoutWidth);
        YogaAssert.Equal(300f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(0f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(300f, root.LayoutWidth);
        YogaAssert.Equal(300f, root.LayoutHeight);
        YogaAssert.Equal(250f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(0f, root_child0.LayoutHeight);
    }

}
