// Vendored from microsoft/microsoft-ui-reactor @ v0.1.0-preview.13 (c9191b97c40a2e4d6bcbc72df7714184862b4d36), imported 2026-09-04.
// Source: tests/Reactor.Tests/YogaGenerated/YogaFlexBasisFitContentTest.cs -- xUnit->MSTest; DO NOT HAND-EDIT; see Tests/Yoga/README.md.
// SPDX-License-Identifier: MIT -- (c) Microsoft Corporation (C# port); (c) Facebook, Inc. and its affiliates (Yoga).
// Full license text: THIRD-PARTY-NOTICES.md

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Toolkit.UI;
using Uno.Toolkit.UI.Yoga;

namespace Uno.Toolkit.RuntimeTests.Tests.Yoga;

/// <summary>
/// Ported from yoga/tests/generated/YGFlexBasisFitContentTest.cpp
/// </summary>
[TestClass]
public class YogaFlexBasisFitContentTest
{
    [TestMethod]
    public void Container_Child_Overflows_Definite_Parent_Column()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(200f);
        root.Height = YogaValue.Point(300f);
        var root_child0 = new YogaNode(config);
        root.InsertChild(root_child0, 0);
        var root_child0_child0 = new YogaNode(config);
        root_child0_child0.Height = YogaValue.Point(500f);
        root_child0_child0.Width = YogaValue.Point(50f);
        root_child0.InsertChild(root_child0_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(200f, root.LayoutWidth);
        YogaAssert.Equal(300f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(200f, root_child0.LayoutWidth);
        YogaAssert.Equal(500f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(500f, root_child0_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(200f, root.LayoutWidth);
        YogaAssert.Equal(300f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(200f, root_child0.LayoutWidth);
        YogaAssert.Equal(500f, root_child0.LayoutHeight);
        YogaAssert.Equal(150f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(500f, root_child0_child0.LayoutHeight);
    }

    [TestMethod]
    public void Container_Child_Overflows_Definite_Parent_Row()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(300f);
        root.Height = YogaValue.Point(200f);
        root.FlexDirection = FlexDirection.Row;
        var root_child0 = new YogaNode(config);
        root.InsertChild(root_child0, 0);
        var root_child0_child0 = new YogaNode(config);
        root_child0_child0.Width = YogaValue.Point(500f);
        root_child0_child0.Height = YogaValue.Point(50f);
        root_child0.InsertChild(root_child0_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(300f, root.LayoutWidth);
        YogaAssert.Equal(200f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(500f, root_child0.LayoutWidth);
        YogaAssert.Equal(200f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(500f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(300f, root.LayoutWidth);
        YogaAssert.Equal(200f, root.LayoutHeight);
        YogaAssert.Equal(-200f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(500f, root_child0.LayoutWidth);
        YogaAssert.Equal(200f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(500f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child0.LayoutHeight);
    }

    [TestMethod]
    public void Container_Child_Within_Bounds_Column()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(200f);
        root.Height = YogaValue.Point(300f);
        var root_child0 = new YogaNode(config);
        root.InsertChild(root_child0, 0);
        var root_child0_child0 = new YogaNode(config);
        root_child0_child0.Height = YogaValue.Point(100f);
        root_child0_child0.Width = YogaValue.Point(50f);
        root_child0.InsertChild(root_child0_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(200f, root.LayoutWidth);
        YogaAssert.Equal(300f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(200f, root_child0.LayoutWidth);
        YogaAssert.Equal(100f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(100f, root_child0_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(200f, root.LayoutWidth);
        YogaAssert.Equal(300f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(200f, root_child0.LayoutWidth);
        YogaAssert.Equal(100f, root_child0.LayoutHeight);
        YogaAssert.Equal(150f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(100f, root_child0_child0.LayoutHeight);
    }

    [TestMethod]
    public void Multiple_Container_Children_Overflow_Column()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(200f);
        root.Height = YogaValue.Point(300f);
        var root_child0 = new YogaNode(config);
        root.InsertChild(root_child0, 0);
        var root_child0_child0 = new YogaNode(config);
        root_child0_child0.Height = YogaValue.Point(400f);
        root_child0.InsertChild(root_child0_child0, 0);
        var root_child1 = new YogaNode(config);
        root.InsertChild(root_child1, 1);
        var root_child1_child0 = new YogaNode(config);
        root_child1_child0.Height = YogaValue.Point(500f);
        root_child1.InsertChild(root_child1_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(200f, root.LayoutWidth);
        YogaAssert.Equal(300f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(200f, root_child0.LayoutWidth);
        YogaAssert.Equal(400f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(200f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(400f, root_child0_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child1.LayoutX);
        YogaAssert.Equal(400f, root_child1.LayoutY);
        YogaAssert.Equal(200f, root_child1.LayoutWidth);
        YogaAssert.Equal(500f, root_child1.LayoutHeight);
        YogaAssert.Equal(0f, root_child1_child0.LayoutX);
        YogaAssert.Equal(0f, root_child1_child0.LayoutY);
        YogaAssert.Equal(200f, root_child1_child0.LayoutWidth);
        YogaAssert.Equal(500f, root_child1_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(200f, root.LayoutWidth);
        YogaAssert.Equal(300f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(200f, root_child0.LayoutWidth);
        YogaAssert.Equal(400f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(200f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(400f, root_child0_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child1.LayoutX);
        YogaAssert.Equal(400f, root_child1.LayoutY);
        YogaAssert.Equal(200f, root_child1.LayoutWidth);
        YogaAssert.Equal(500f, root_child1.LayoutHeight);
        YogaAssert.Equal(0f, root_child1_child0.LayoutX);
        YogaAssert.Equal(0f, root_child1_child0.LayoutY);
        YogaAssert.Equal(200f, root_child1_child0.LayoutWidth);
        YogaAssert.Equal(500f, root_child1_child0.LayoutHeight);
    }

    [TestMethod]
    public void Scroll_Container_Column()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(200f);
        root.Height = YogaValue.Point(300f);
        root.Overflow = YogaOverflow.Scroll;
        var root_child0 = new YogaNode(config);
        root.InsertChild(root_child0, 0);
        var root_child0_child0 = new YogaNode(config);
        root_child0_child0.Height = YogaValue.Point(500f);
        root_child0.InsertChild(root_child0_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(200f, root.LayoutWidth);
        YogaAssert.Equal(300f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(200f, root_child0.LayoutWidth);
        YogaAssert.Equal(500f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(200f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(500f, root_child0_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(200f, root.LayoutWidth);
        YogaAssert.Equal(300f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(200f, root_child0.LayoutWidth);
        YogaAssert.Equal(500f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(200f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(500f, root_child0_child0.LayoutHeight);
    }

    [TestMethod]
    public void Explicit_And_Container_Children_Column()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(200f);
        root.Height = YogaValue.Point(300f);
        var root_child0 = new YogaNode(config);
        root_child0.Height = YogaValue.Point(100f);
        root.InsertChild(root_child0, 0);
        var root_child1 = new YogaNode(config);
        root.InsertChild(root_child1, 1);
        var root_child1_child0 = new YogaNode(config);
        root_child1_child0.Height = YogaValue.Point(500f);
        root_child1.InsertChild(root_child1_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(200f, root.LayoutWidth);
        YogaAssert.Equal(300f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(200f, root_child0.LayoutWidth);
        YogaAssert.Equal(100f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child1.LayoutX);
        YogaAssert.Equal(100f, root_child1.LayoutY);
        YogaAssert.Equal(200f, root_child1.LayoutWidth);
        YogaAssert.Equal(500f, root_child1.LayoutHeight);
        YogaAssert.Equal(0f, root_child1_child0.LayoutX);
        YogaAssert.Equal(0f, root_child1_child0.LayoutY);
        YogaAssert.Equal(200f, root_child1_child0.LayoutWidth);
        YogaAssert.Equal(500f, root_child1_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(200f, root.LayoutWidth);
        YogaAssert.Equal(300f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(200f, root_child0.LayoutWidth);
        YogaAssert.Equal(100f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child1.LayoutX);
        YogaAssert.Equal(100f, root_child1.LayoutY);
        YogaAssert.Equal(200f, root_child1.LayoutWidth);
        YogaAssert.Equal(500f, root_child1.LayoutHeight);
        YogaAssert.Equal(0f, root_child1_child0.LayoutX);
        YogaAssert.Equal(0f, root_child1_child0.LayoutY);
        YogaAssert.Equal(200f, root_child1_child0.LayoutWidth);
        YogaAssert.Equal(500f, root_child1_child0.LayoutHeight);
    }

    [TestMethod]
    public void Flex_Basis_In_Scroll_Content_Container()
    {
        var config = new YogaConfig();
        config.SetExperimentalFeatureEnabled(YogaExperimentalFeature.FixFlexBasisFitContent, true);
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(200f);
        root.Height = YogaValue.Point(300f);
        root.Overflow = YogaOverflow.Scroll;
        var root_child0 = new YogaNode(config);
        root.InsertChild(root_child0, 0);
        var root_child0_child0 = new YogaNode(config);
        root_child0_child0.FlexBasis = YogaValue.Point(200f);
        root_child0.InsertChild(root_child0_child0, 0);
        var root_child0_child1 = new YogaNode(config);
        root_child0_child1.FlexBasis = YogaValue.Point(300f);
        root_child0.InsertChild(root_child0_child1, 1);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(200f, root.LayoutWidth);
        YogaAssert.Equal(300f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(200f, root_child0.LayoutWidth);
        YogaAssert.Equal(500f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(200f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(200f, root_child0_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child1.LayoutX);
        YogaAssert.Equal(200f, root_child0_child1.LayoutY);
        YogaAssert.Equal(200f, root_child0_child1.LayoutWidth);
        YogaAssert.Equal(300f, root_child0_child1.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(200f, root.LayoutWidth);
        YogaAssert.Equal(300f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(200f, root_child0.LayoutWidth);
        YogaAssert.Equal(500f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(200f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(200f, root_child0_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child1.LayoutX);
        YogaAssert.Equal(200f, root_child0_child1.LayoutY);
        YogaAssert.Equal(200f, root_child0_child1.LayoutWidth);
        YogaAssert.Equal(300f, root_child0_child1.LayoutHeight);
    }

}
