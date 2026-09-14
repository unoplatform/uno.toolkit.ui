// Vendored from microsoft/microsoft-ui-reactor @ v0.1.0-preview.13 (c9191b97c40a2e4d6bcbc72df7714184862b4d36), imported 2026-09-04.
// Source: tests/Reactor.Tests/YogaGenerated/YogaJustifyContentTest.cs -- xUnit->MSTest; DO NOT HAND-EDIT; see Tests/Yoga/README.md.
// SPDX-License-Identifier: MIT -- (c) Microsoft Corporation (C# port); (c) Facebook, Inc. and its affiliates (Yoga).
// Full license text: THIRD-PARTY-NOTICES.md

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Toolkit.UI;
using Uno.Toolkit.UI.Yoga;

namespace Uno.Toolkit.RuntimeTests.Tests.Yoga;

/// <summary>
/// Ported from yoga/tests/generated/YGJustifyContentTest.cpp
/// </summary>
[TestClass]
public class FlexJustifyContentTest
{
    [TestMethod]
    public void Justify_Content_Row_Flex_Start()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(102f);
        root.Height = YogaValue.Point(102f);
        root.FlexDirection = FlexDirection.Row;
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Point(10f);
        root.InsertChild(root_child0, 0);
        var root_child1 = new YogaNode(config);
        root_child1.Width = YogaValue.Point(10f);
        root.InsertChild(root_child1, 1);
        var root_child2 = new YogaNode(config);
        root_child2.Width = YogaValue.Point(10f);
        root.InsertChild(root_child2, 2);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(102f, root.LayoutWidth);
        YogaAssert.Equal(102f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(10f, root_child0.LayoutWidth);
        YogaAssert.Equal(102f, root_child0.LayoutHeight);
        YogaAssert.Equal(10f, root_child1.LayoutX);
        YogaAssert.Equal(0f, root_child1.LayoutY);
        YogaAssert.Equal(10f, root_child1.LayoutWidth);
        YogaAssert.Equal(102f, root_child1.LayoutHeight);
        YogaAssert.Equal(20f, root_child2.LayoutX);
        YogaAssert.Equal(0f, root_child2.LayoutY);
        YogaAssert.Equal(10f, root_child2.LayoutWidth);
        YogaAssert.Equal(102f, root_child2.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(102f, root.LayoutWidth);
        YogaAssert.Equal(102f, root.LayoutHeight);
        YogaAssert.Equal(92f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(10f, root_child0.LayoutWidth);
        YogaAssert.Equal(102f, root_child0.LayoutHeight);
        YogaAssert.Equal(82f, root_child1.LayoutX);
        YogaAssert.Equal(0f, root_child1.LayoutY);
        YogaAssert.Equal(10f, root_child1.LayoutWidth);
        YogaAssert.Equal(102f, root_child1.LayoutHeight);
        YogaAssert.Equal(72f, root_child2.LayoutX);
        YogaAssert.Equal(0f, root_child2.LayoutY);
        YogaAssert.Equal(10f, root_child2.LayoutWidth);
        YogaAssert.Equal(102f, root_child2.LayoutHeight);
    }

    [TestMethod]
    public void Justify_Content_Row_Flex_End()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(102f);
        root.Height = YogaValue.Point(102f);
        root.FlexDirection = FlexDirection.Row;
        root.JustifyContent = FlexJustify.FlexEnd;
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Point(10f);
        root.InsertChild(root_child0, 0);
        var root_child1 = new YogaNode(config);
        root_child1.Width = YogaValue.Point(10f);
        root.InsertChild(root_child1, 1);
        var root_child2 = new YogaNode(config);
        root_child2.Width = YogaValue.Point(10f);
        root.InsertChild(root_child2, 2);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(102f, root.LayoutWidth);
        YogaAssert.Equal(102f, root.LayoutHeight);
        YogaAssert.Equal(72f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(10f, root_child0.LayoutWidth);
        YogaAssert.Equal(102f, root_child0.LayoutHeight);
        YogaAssert.Equal(82f, root_child1.LayoutX);
        YogaAssert.Equal(0f, root_child1.LayoutY);
        YogaAssert.Equal(10f, root_child1.LayoutWidth);
        YogaAssert.Equal(102f, root_child1.LayoutHeight);
        YogaAssert.Equal(92f, root_child2.LayoutX);
        YogaAssert.Equal(0f, root_child2.LayoutY);
        YogaAssert.Equal(10f, root_child2.LayoutWidth);
        YogaAssert.Equal(102f, root_child2.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(102f, root.LayoutWidth);
        YogaAssert.Equal(102f, root.LayoutHeight);
        YogaAssert.Equal(20f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(10f, root_child0.LayoutWidth);
        YogaAssert.Equal(102f, root_child0.LayoutHeight);
        YogaAssert.Equal(10f, root_child1.LayoutX);
        YogaAssert.Equal(0f, root_child1.LayoutY);
        YogaAssert.Equal(10f, root_child1.LayoutWidth);
        YogaAssert.Equal(102f, root_child1.LayoutHeight);
        YogaAssert.Equal(0f, root_child2.LayoutX);
        YogaAssert.Equal(0f, root_child2.LayoutY);
        YogaAssert.Equal(10f, root_child2.LayoutWidth);
        YogaAssert.Equal(102f, root_child2.LayoutHeight);
    }

    [TestMethod]
    public void Justify_Content_Row_Center()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(102f);
        root.Height = YogaValue.Point(102f);
        root.FlexDirection = FlexDirection.Row;
        root.JustifyContent = FlexJustify.Center;
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Point(10f);
        root.InsertChild(root_child0, 0);
        var root_child1 = new YogaNode(config);
        root_child1.Width = YogaValue.Point(10f);
        root.InsertChild(root_child1, 1);
        var root_child2 = new YogaNode(config);
        root_child2.Width = YogaValue.Point(10f);
        root.InsertChild(root_child2, 2);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(102f, root.LayoutWidth);
        YogaAssert.Equal(102f, root.LayoutHeight);
        YogaAssert.Equal(36f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(10f, root_child0.LayoutWidth);
        YogaAssert.Equal(102f, root_child0.LayoutHeight);
        YogaAssert.Equal(46f, root_child1.LayoutX);
        YogaAssert.Equal(0f, root_child1.LayoutY);
        YogaAssert.Equal(10f, root_child1.LayoutWidth);
        YogaAssert.Equal(102f, root_child1.LayoutHeight);
        YogaAssert.Equal(56f, root_child2.LayoutX);
        YogaAssert.Equal(0f, root_child2.LayoutY);
        YogaAssert.Equal(10f, root_child2.LayoutWidth);
        YogaAssert.Equal(102f, root_child2.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(102f, root.LayoutWidth);
        YogaAssert.Equal(102f, root.LayoutHeight);
        YogaAssert.Equal(56f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(10f, root_child0.LayoutWidth);
        YogaAssert.Equal(102f, root_child0.LayoutHeight);
        YogaAssert.Equal(46f, root_child1.LayoutX);
        YogaAssert.Equal(0f, root_child1.LayoutY);
        YogaAssert.Equal(10f, root_child1.LayoutWidth);
        YogaAssert.Equal(102f, root_child1.LayoutHeight);
        YogaAssert.Equal(36f, root_child2.LayoutX);
        YogaAssert.Equal(0f, root_child2.LayoutY);
        YogaAssert.Equal(10f, root_child2.LayoutWidth);
        YogaAssert.Equal(102f, root_child2.LayoutHeight);
    }

    [TestMethod]
    public void Justify_Content_Row_Space_Between()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(102f);
        root.Height = YogaValue.Point(102f);
        root.FlexDirection = FlexDirection.Row;
        root.JustifyContent = FlexJustify.SpaceBetween;
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Point(10f);
        root.InsertChild(root_child0, 0);
        var root_child1 = new YogaNode(config);
        root_child1.Width = YogaValue.Point(10f);
        root.InsertChild(root_child1, 1);
        var root_child2 = new YogaNode(config);
        root_child2.Width = YogaValue.Point(10f);
        root.InsertChild(root_child2, 2);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(102f, root.LayoutWidth);
        YogaAssert.Equal(102f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(10f, root_child0.LayoutWidth);
        YogaAssert.Equal(102f, root_child0.LayoutHeight);
        YogaAssert.Equal(46f, root_child1.LayoutX);
        YogaAssert.Equal(0f, root_child1.LayoutY);
        YogaAssert.Equal(10f, root_child1.LayoutWidth);
        YogaAssert.Equal(102f, root_child1.LayoutHeight);
        YogaAssert.Equal(92f, root_child2.LayoutX);
        YogaAssert.Equal(0f, root_child2.LayoutY);
        YogaAssert.Equal(10f, root_child2.LayoutWidth);
        YogaAssert.Equal(102f, root_child2.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(102f, root.LayoutWidth);
        YogaAssert.Equal(102f, root.LayoutHeight);
        YogaAssert.Equal(92f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(10f, root_child0.LayoutWidth);
        YogaAssert.Equal(102f, root_child0.LayoutHeight);
        YogaAssert.Equal(46f, root_child1.LayoutX);
        YogaAssert.Equal(0f, root_child1.LayoutY);
        YogaAssert.Equal(10f, root_child1.LayoutWidth);
        YogaAssert.Equal(102f, root_child1.LayoutHeight);
        YogaAssert.Equal(0f, root_child2.LayoutX);
        YogaAssert.Equal(0f, root_child2.LayoutY);
        YogaAssert.Equal(10f, root_child2.LayoutWidth);
        YogaAssert.Equal(102f, root_child2.LayoutHeight);
    }

    [TestMethod]
    public void Justify_Content_Row_Space_Around()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(102f);
        root.Height = YogaValue.Point(102f);
        root.FlexDirection = FlexDirection.Row;
        root.JustifyContent = FlexJustify.SpaceAround;
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Point(10f);
        root.InsertChild(root_child0, 0);
        var root_child1 = new YogaNode(config);
        root_child1.Width = YogaValue.Point(10f);
        root.InsertChild(root_child1, 1);
        var root_child2 = new YogaNode(config);
        root_child2.Width = YogaValue.Point(10f);
        root.InsertChild(root_child2, 2);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(102f, root.LayoutWidth);
        YogaAssert.Equal(102f, root.LayoutHeight);
        YogaAssert.Equal(12f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(10f, root_child0.LayoutWidth);
        YogaAssert.Equal(102f, root_child0.LayoutHeight);
        YogaAssert.Equal(46f, root_child1.LayoutX);
        YogaAssert.Equal(0f, root_child1.LayoutY);
        YogaAssert.Equal(10f, root_child1.LayoutWidth);
        YogaAssert.Equal(102f, root_child1.LayoutHeight);
        YogaAssert.Equal(80f, root_child2.LayoutX);
        YogaAssert.Equal(0f, root_child2.LayoutY);
        YogaAssert.Equal(10f, root_child2.LayoutWidth);
        YogaAssert.Equal(102f, root_child2.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(102f, root.LayoutWidth);
        YogaAssert.Equal(102f, root.LayoutHeight);
        YogaAssert.Equal(80f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(10f, root_child0.LayoutWidth);
        YogaAssert.Equal(102f, root_child0.LayoutHeight);
        YogaAssert.Equal(46f, root_child1.LayoutX);
        YogaAssert.Equal(0f, root_child1.LayoutY);
        YogaAssert.Equal(10f, root_child1.LayoutWidth);
        YogaAssert.Equal(102f, root_child1.LayoutHeight);
        YogaAssert.Equal(12f, root_child2.LayoutX);
        YogaAssert.Equal(0f, root_child2.LayoutY);
        YogaAssert.Equal(10f, root_child2.LayoutWidth);
        YogaAssert.Equal(102f, root_child2.LayoutHeight);
    }

    [TestMethod]
    public void Justify_Content_Column_Flex_Start()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(102f);
        root.Height = YogaValue.Point(102f);
        var root_child0 = new YogaNode(config);
        root_child0.Height = YogaValue.Point(10f);
        root.InsertChild(root_child0, 0);
        var root_child1 = new YogaNode(config);
        root_child1.Height = YogaValue.Point(10f);
        root.InsertChild(root_child1, 1);
        var root_child2 = new YogaNode(config);
        root_child2.Height = YogaValue.Point(10f);
        root.InsertChild(root_child2, 2);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(102f, root.LayoutWidth);
        YogaAssert.Equal(102f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(102f, root_child0.LayoutWidth);
        YogaAssert.Equal(10f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child1.LayoutX);
        YogaAssert.Equal(10f, root_child1.LayoutY);
        YogaAssert.Equal(102f, root_child1.LayoutWidth);
        YogaAssert.Equal(10f, root_child1.LayoutHeight);
        YogaAssert.Equal(0f, root_child2.LayoutX);
        YogaAssert.Equal(20f, root_child2.LayoutY);
        YogaAssert.Equal(102f, root_child2.LayoutWidth);
        YogaAssert.Equal(10f, root_child2.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(102f, root.LayoutWidth);
        YogaAssert.Equal(102f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(102f, root_child0.LayoutWidth);
        YogaAssert.Equal(10f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child1.LayoutX);
        YogaAssert.Equal(10f, root_child1.LayoutY);
        YogaAssert.Equal(102f, root_child1.LayoutWidth);
        YogaAssert.Equal(10f, root_child1.LayoutHeight);
        YogaAssert.Equal(0f, root_child2.LayoutX);
        YogaAssert.Equal(20f, root_child2.LayoutY);
        YogaAssert.Equal(102f, root_child2.LayoutWidth);
        YogaAssert.Equal(10f, root_child2.LayoutHeight);
    }

    [TestMethod]
    public void Justify_Content_Column_Flex_End()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(102f);
        root.Height = YogaValue.Point(102f);
        root.JustifyContent = FlexJustify.FlexEnd;
        var root_child0 = new YogaNode(config);
        root_child0.Height = YogaValue.Point(10f);
        root.InsertChild(root_child0, 0);
        var root_child1 = new YogaNode(config);
        root_child1.Height = YogaValue.Point(10f);
        root.InsertChild(root_child1, 1);
        var root_child2 = new YogaNode(config);
        root_child2.Height = YogaValue.Point(10f);
        root.InsertChild(root_child2, 2);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(102f, root.LayoutWidth);
        YogaAssert.Equal(102f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(72f, root_child0.LayoutY);
        YogaAssert.Equal(102f, root_child0.LayoutWidth);
        YogaAssert.Equal(10f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child1.LayoutX);
        YogaAssert.Equal(82f, root_child1.LayoutY);
        YogaAssert.Equal(102f, root_child1.LayoutWidth);
        YogaAssert.Equal(10f, root_child1.LayoutHeight);
        YogaAssert.Equal(0f, root_child2.LayoutX);
        YogaAssert.Equal(92f, root_child2.LayoutY);
        YogaAssert.Equal(102f, root_child2.LayoutWidth);
        YogaAssert.Equal(10f, root_child2.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(102f, root.LayoutWidth);
        YogaAssert.Equal(102f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(72f, root_child0.LayoutY);
        YogaAssert.Equal(102f, root_child0.LayoutWidth);
        YogaAssert.Equal(10f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child1.LayoutX);
        YogaAssert.Equal(82f, root_child1.LayoutY);
        YogaAssert.Equal(102f, root_child1.LayoutWidth);
        YogaAssert.Equal(10f, root_child1.LayoutHeight);
        YogaAssert.Equal(0f, root_child2.LayoutX);
        YogaAssert.Equal(92f, root_child2.LayoutY);
        YogaAssert.Equal(102f, root_child2.LayoutWidth);
        YogaAssert.Equal(10f, root_child2.LayoutHeight);
    }

    [TestMethod]
    public void Justify_Content_Column_Center()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(102f);
        root.Height = YogaValue.Point(102f);
        root.JustifyContent = FlexJustify.Center;
        var root_child0 = new YogaNode(config);
        root_child0.Height = YogaValue.Point(10f);
        root.InsertChild(root_child0, 0);
        var root_child1 = new YogaNode(config);
        root_child1.Height = YogaValue.Point(10f);
        root.InsertChild(root_child1, 1);
        var root_child2 = new YogaNode(config);
        root_child2.Height = YogaValue.Point(10f);
        root.InsertChild(root_child2, 2);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(102f, root.LayoutWidth);
        YogaAssert.Equal(102f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(36f, root_child0.LayoutY);
        YogaAssert.Equal(102f, root_child0.LayoutWidth);
        YogaAssert.Equal(10f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child1.LayoutX);
        YogaAssert.Equal(46f, root_child1.LayoutY);
        YogaAssert.Equal(102f, root_child1.LayoutWidth);
        YogaAssert.Equal(10f, root_child1.LayoutHeight);
        YogaAssert.Equal(0f, root_child2.LayoutX);
        YogaAssert.Equal(56f, root_child2.LayoutY);
        YogaAssert.Equal(102f, root_child2.LayoutWidth);
        YogaAssert.Equal(10f, root_child2.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(102f, root.LayoutWidth);
        YogaAssert.Equal(102f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(36f, root_child0.LayoutY);
        YogaAssert.Equal(102f, root_child0.LayoutWidth);
        YogaAssert.Equal(10f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child1.LayoutX);
        YogaAssert.Equal(46f, root_child1.LayoutY);
        YogaAssert.Equal(102f, root_child1.LayoutWidth);
        YogaAssert.Equal(10f, root_child1.LayoutHeight);
        YogaAssert.Equal(0f, root_child2.LayoutX);
        YogaAssert.Equal(56f, root_child2.LayoutY);
        YogaAssert.Equal(102f, root_child2.LayoutWidth);
        YogaAssert.Equal(10f, root_child2.LayoutHeight);
    }

    [TestMethod]
    public void Justify_Content_Column_Space_Between()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(102f);
        root.Height = YogaValue.Point(102f);
        root.JustifyContent = FlexJustify.SpaceBetween;
        var root_child0 = new YogaNode(config);
        root_child0.Height = YogaValue.Point(10f);
        root.InsertChild(root_child0, 0);
        var root_child1 = new YogaNode(config);
        root_child1.Height = YogaValue.Point(10f);
        root.InsertChild(root_child1, 1);
        var root_child2 = new YogaNode(config);
        root_child2.Height = YogaValue.Point(10f);
        root.InsertChild(root_child2, 2);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(102f, root.LayoutWidth);
        YogaAssert.Equal(102f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(102f, root_child0.LayoutWidth);
        YogaAssert.Equal(10f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child1.LayoutX);
        YogaAssert.Equal(46f, root_child1.LayoutY);
        YogaAssert.Equal(102f, root_child1.LayoutWidth);
        YogaAssert.Equal(10f, root_child1.LayoutHeight);
        YogaAssert.Equal(0f, root_child2.LayoutX);
        YogaAssert.Equal(92f, root_child2.LayoutY);
        YogaAssert.Equal(102f, root_child2.LayoutWidth);
        YogaAssert.Equal(10f, root_child2.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(102f, root.LayoutWidth);
        YogaAssert.Equal(102f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(102f, root_child0.LayoutWidth);
        YogaAssert.Equal(10f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child1.LayoutX);
        YogaAssert.Equal(46f, root_child1.LayoutY);
        YogaAssert.Equal(102f, root_child1.LayoutWidth);
        YogaAssert.Equal(10f, root_child1.LayoutHeight);
        YogaAssert.Equal(0f, root_child2.LayoutX);
        YogaAssert.Equal(92f, root_child2.LayoutY);
        YogaAssert.Equal(102f, root_child2.LayoutWidth);
        YogaAssert.Equal(10f, root_child2.LayoutHeight);
    }

    [TestMethod]
    public void Justify_Content_Column_Space_Around()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(102f);
        root.Height = YogaValue.Point(102f);
        root.JustifyContent = FlexJustify.SpaceAround;
        var root_child0 = new YogaNode(config);
        root_child0.Height = YogaValue.Point(10f);
        root.InsertChild(root_child0, 0);
        var root_child1 = new YogaNode(config);
        root_child1.Height = YogaValue.Point(10f);
        root.InsertChild(root_child1, 1);
        var root_child2 = new YogaNode(config);
        root_child2.Height = YogaValue.Point(10f);
        root.InsertChild(root_child2, 2);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(102f, root.LayoutWidth);
        YogaAssert.Equal(102f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(12f, root_child0.LayoutY);
        YogaAssert.Equal(102f, root_child0.LayoutWidth);
        YogaAssert.Equal(10f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child1.LayoutX);
        YogaAssert.Equal(46f, root_child1.LayoutY);
        YogaAssert.Equal(102f, root_child1.LayoutWidth);
        YogaAssert.Equal(10f, root_child1.LayoutHeight);
        YogaAssert.Equal(0f, root_child2.LayoutX);
        YogaAssert.Equal(80f, root_child2.LayoutY);
        YogaAssert.Equal(102f, root_child2.LayoutWidth);
        YogaAssert.Equal(10f, root_child2.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(102f, root.LayoutWidth);
        YogaAssert.Equal(102f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(12f, root_child0.LayoutY);
        YogaAssert.Equal(102f, root_child0.LayoutWidth);
        YogaAssert.Equal(10f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child1.LayoutX);
        YogaAssert.Equal(46f, root_child1.LayoutY);
        YogaAssert.Equal(102f, root_child1.LayoutWidth);
        YogaAssert.Equal(10f, root_child1.LayoutHeight);
        YogaAssert.Equal(0f, root_child2.LayoutX);
        YogaAssert.Equal(80f, root_child2.LayoutY);
        YogaAssert.Equal(102f, root_child2.LayoutWidth);
        YogaAssert.Equal(10f, root_child2.LayoutHeight);
    }

    [TestMethod]
    public void Justify_Content_Row_Min_Width_And_Margin()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.MinWidth = YogaValue.Point(50f);
        root.SetMargin(YogaEdge.Left, YogaValue.Point(100f));
        root.JustifyContent = FlexJustify.Center;
        root.FlexDirection = FlexDirection.Row;
        var root_child0 = new YogaNode(config);
        root_child0.Height = YogaValue.Point(20f);
        root_child0.Width = YogaValue.Point(20f);
        root.InsertChild(root_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(100f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(50f, root.LayoutWidth);
        YogaAssert.Equal(20f, root.LayoutHeight);
        YogaAssert.Equal(15f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(20f, root_child0.LayoutWidth);
        YogaAssert.Equal(20f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(100f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(50f, root.LayoutWidth);
        YogaAssert.Equal(20f, root.LayoutHeight);
        YogaAssert.Equal(15f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(20f, root_child0.LayoutWidth);
        YogaAssert.Equal(20f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Justify_Content_Row_Max_Width_And_Margin()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.MaxWidth = YogaValue.Point(80f);
        root.SetMargin(YogaEdge.Left, YogaValue.Point(100f));
        root.JustifyContent = FlexJustify.Center;
        root.FlexDirection = FlexDirection.Row;
        var root_child0 = new YogaNode(config);
        root_child0.Height = YogaValue.Point(20f);
        root_child0.Width = YogaValue.Point(20f);
        root.InsertChild(root_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(100f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(80f, root.LayoutWidth);
        YogaAssert.Equal(20f, root.LayoutHeight);
        YogaAssert.Equal(30f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(20f, root_child0.LayoutWidth);
        YogaAssert.Equal(20f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(100f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(80f, root.LayoutWidth);
        YogaAssert.Equal(20f, root.LayoutHeight);
        YogaAssert.Equal(30f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(20f, root_child0.LayoutWidth);
        YogaAssert.Equal(20f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Justify_Content_Column_Min_Height_And_Margin()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.MinHeight = YogaValue.Point(50f);
        root.SetMargin(YogaEdge.Top, YogaValue.Point(100f));
        root.JustifyContent = FlexJustify.Center;
        var root_child0 = new YogaNode(config);
        root_child0.Height = YogaValue.Point(20f);
        root_child0.Width = YogaValue.Point(20f);
        root.InsertChild(root_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(100f, root.LayoutY);
        YogaAssert.Equal(20f, root.LayoutWidth);
        YogaAssert.Equal(50f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(15f, root_child0.LayoutY);
        YogaAssert.Equal(20f, root_child0.LayoutWidth);
        YogaAssert.Equal(20f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(100f, root.LayoutY);
        YogaAssert.Equal(20f, root.LayoutWidth);
        YogaAssert.Equal(50f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(15f, root_child0.LayoutY);
        YogaAssert.Equal(20f, root_child0.LayoutWidth);
        YogaAssert.Equal(20f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Justify_Content_Column_Max_Height_And_Margin()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Height = YogaValue.Point(100f);
        root.MaxHeight = YogaValue.Point(80f);
        root.SetMargin(YogaEdge.Top, YogaValue.Point(100f));
        root.JustifyContent = FlexJustify.Center;
        var root_child0 = new YogaNode(config);
        root_child0.Height = YogaValue.Point(20f);
        root_child0.Width = YogaValue.Point(20f);
        root.InsertChild(root_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(100f, root.LayoutY);
        YogaAssert.Equal(20f, root.LayoutWidth);
        YogaAssert.Equal(80f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(30f, root_child0.LayoutY);
        YogaAssert.Equal(20f, root_child0.LayoutWidth);
        YogaAssert.Equal(20f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(100f, root.LayoutY);
        YogaAssert.Equal(20f, root.LayoutWidth);
        YogaAssert.Equal(80f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(30f, root_child0.LayoutY);
        YogaAssert.Equal(20f, root_child0.LayoutWidth);
        YogaAssert.Equal(20f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Justify_Content_Column_Space_Evenly()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(102f);
        root.Height = YogaValue.Point(102f);
        root.JustifyContent = FlexJustify.SpaceEvenly;
        var root_child0 = new YogaNode(config);
        root_child0.Height = YogaValue.Point(10f);
        root.InsertChild(root_child0, 0);
        var root_child1 = new YogaNode(config);
        root_child1.Height = YogaValue.Point(10f);
        root.InsertChild(root_child1, 1);
        var root_child2 = new YogaNode(config);
        root_child2.Height = YogaValue.Point(10f);
        root.InsertChild(root_child2, 2);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(102f, root.LayoutWidth);
        YogaAssert.Equal(102f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(18f, root_child0.LayoutY);
        YogaAssert.Equal(102f, root_child0.LayoutWidth);
        YogaAssert.Equal(10f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child1.LayoutX);
        YogaAssert.Equal(46f, root_child1.LayoutY);
        YogaAssert.Equal(102f, root_child1.LayoutWidth);
        YogaAssert.Equal(10f, root_child1.LayoutHeight);
        YogaAssert.Equal(0f, root_child2.LayoutX);
        YogaAssert.Equal(74f, root_child2.LayoutY);
        YogaAssert.Equal(102f, root_child2.LayoutWidth);
        YogaAssert.Equal(10f, root_child2.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(102f, root.LayoutWidth);
        YogaAssert.Equal(102f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(18f, root_child0.LayoutY);
        YogaAssert.Equal(102f, root_child0.LayoutWidth);
        YogaAssert.Equal(10f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child1.LayoutX);
        YogaAssert.Equal(46f, root_child1.LayoutY);
        YogaAssert.Equal(102f, root_child1.LayoutWidth);
        YogaAssert.Equal(10f, root_child1.LayoutHeight);
        YogaAssert.Equal(0f, root_child2.LayoutX);
        YogaAssert.Equal(74f, root_child2.LayoutY);
        YogaAssert.Equal(102f, root_child2.LayoutWidth);
        YogaAssert.Equal(10f, root_child2.LayoutHeight);
    }

    [TestMethod]
    public void Justify_Content_Row_Space_Evenly()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(102f);
        root.Height = YogaValue.Point(102f);
        root.JustifyContent = FlexJustify.SpaceEvenly;
        root.FlexDirection = FlexDirection.Row;
        var root_child0 = new YogaNode(config);
        root_child0.Height = YogaValue.Point(10f);
        root.InsertChild(root_child0, 0);
        var root_child1 = new YogaNode(config);
        root_child1.Height = YogaValue.Point(10f);
        root.InsertChild(root_child1, 1);
        var root_child2 = new YogaNode(config);
        root_child2.Height = YogaValue.Point(10f);
        root.InsertChild(root_child2, 2);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(102f, root.LayoutWidth);
        YogaAssert.Equal(102f, root.LayoutHeight);
        YogaAssert.Equal(26f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(0f, root_child0.LayoutWidth);
        YogaAssert.Equal(10f, root_child0.LayoutHeight);
        YogaAssert.Equal(51f, root_child1.LayoutX);
        YogaAssert.Equal(0f, root_child1.LayoutY);
        YogaAssert.Equal(0f, root_child1.LayoutWidth);
        YogaAssert.Equal(10f, root_child1.LayoutHeight);
        YogaAssert.Equal(77f, root_child2.LayoutX);
        YogaAssert.Equal(0f, root_child2.LayoutY);
        YogaAssert.Equal(0f, root_child2.LayoutWidth);
        YogaAssert.Equal(10f, root_child2.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(102f, root.LayoutWidth);
        YogaAssert.Equal(102f, root.LayoutHeight);
        YogaAssert.Equal(77f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(0f, root_child0.LayoutWidth);
        YogaAssert.Equal(10f, root_child0.LayoutHeight);
        YogaAssert.Equal(51f, root_child1.LayoutX);
        YogaAssert.Equal(0f, root_child1.LayoutY);
        YogaAssert.Equal(0f, root_child1.LayoutWidth);
        YogaAssert.Equal(10f, root_child1.LayoutHeight);
        YogaAssert.Equal(26f, root_child2.LayoutX);
        YogaAssert.Equal(0f, root_child2.LayoutY);
        YogaAssert.Equal(0f, root_child2.LayoutWidth);
        YogaAssert.Equal(10f, root_child2.LayoutHeight);
    }

    [TestMethod]
    public void Justify_Content_Min_Width_With_Padding_Child_Width_Greater_Than_Parent()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(1000f);
        root.Height = YogaValue.Point(1584f);
        root.AlignContent = FlexAlign.Stretch;
        var root_child0 = new YogaNode(config);
        root_child0.FlexDirection = FlexDirection.Row;
        root_child0.AlignContent = FlexAlign.Stretch;
        root.InsertChild(root_child0, 0);
        var root_child0_child0 = new YogaNode(config);
        root_child0_child0.FlexDirection = FlexDirection.Row;
        root_child0_child0.JustifyContent = FlexJustify.Center;
        root_child0_child0.AlignContent = FlexAlign.Stretch;
        root_child0_child0.MinWidth = YogaValue.Point(400f);
        root_child0_child0.SetPadding(YogaEdge.Left, YogaValue.Point(100f));
        root_child0_child0.SetPadding(YogaEdge.Right, YogaValue.Point(100f));
        root_child0.InsertChild(root_child0_child0, 0);
        var root_child0_child0_child0 = new YogaNode(config);
        root_child0_child0_child0.Height = YogaValue.Point(100f);
        root_child0_child0_child0.Width = YogaValue.Point(300f);
        root_child0_child0_child0.AlignContent = FlexAlign.Stretch;
        root_child0_child0_child0.FlexDirection = FlexDirection.Row;
        root_child0_child0.InsertChild(root_child0_child0_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(1000f, root.LayoutWidth);
        YogaAssert.Equal(1584f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(1000f, root_child0.LayoutWidth);
        YogaAssert.Equal(100f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(500f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(100f, root_child0_child0.LayoutHeight);
        YogaAssert.Equal(100f, root_child0_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0_child0.LayoutY);
        YogaAssert.Equal(300f, root_child0_child0_child0.LayoutWidth);
        YogaAssert.Equal(100f, root_child0_child0_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(1000f, root.LayoutWidth);
        YogaAssert.Equal(1584f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(1000f, root_child0.LayoutWidth);
        YogaAssert.Equal(100f, root_child0.LayoutHeight);
        YogaAssert.Equal(500f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(500f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(100f, root_child0_child0.LayoutHeight);
        YogaAssert.Equal(100f, root_child0_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0_child0.LayoutY);
        YogaAssert.Equal(300f, root_child0_child0_child0.LayoutWidth);
        YogaAssert.Equal(100f, root_child0_child0_child0.LayoutHeight);
    }

    [TestMethod]
    public void Justify_Content_Min_Width_With_Padding_Child_Width_Lower_Than_Parent()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(1080f);
        root.Height = YogaValue.Point(1584f);
        root.AlignContent = FlexAlign.Stretch;
        var root_child0 = new YogaNode(config);
        root_child0.FlexDirection = FlexDirection.Row;
        root_child0.AlignContent = FlexAlign.Stretch;
        root.InsertChild(root_child0, 0);
        var root_child0_child0 = new YogaNode(config);
        root_child0_child0.FlexDirection = FlexDirection.Row;
        root_child0_child0.JustifyContent = FlexJustify.Center;
        root_child0_child0.AlignContent = FlexAlign.Stretch;
        root_child0_child0.MinWidth = YogaValue.Point(400f);
        root_child0_child0.SetPadding(YogaEdge.Left, YogaValue.Point(100f));
        root_child0_child0.SetPadding(YogaEdge.Right, YogaValue.Point(100f));
        root_child0.InsertChild(root_child0_child0, 0);
        var root_child0_child0_child0 = new YogaNode(config);
        root_child0_child0_child0.Height = YogaValue.Point(100f);
        root_child0_child0_child0.Width = YogaValue.Point(199f);
        root_child0_child0_child0.AlignContent = FlexAlign.Stretch;
        root_child0_child0_child0.FlexDirection = FlexDirection.Row;
        root_child0_child0.InsertChild(root_child0_child0_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(1080f, root.LayoutWidth);
        YogaAssert.Equal(1584f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(1080f, root_child0.LayoutWidth);
        YogaAssert.Equal(100f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(400f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(100f, root_child0_child0.LayoutHeight);
        YogaAssert.Equal(101f, root_child0_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0_child0.LayoutY);
        YogaAssert.Equal(199f, root_child0_child0_child0.LayoutWidth);
        YogaAssert.Equal(100f, root_child0_child0_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(1080f, root.LayoutWidth);
        YogaAssert.Equal(1584f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(1080f, root_child0.LayoutWidth);
        YogaAssert.Equal(100f, root_child0.LayoutHeight);
        YogaAssert.Equal(680f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(400f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(100f, root_child0_child0.LayoutHeight);
        YogaAssert.Equal(101f, root_child0_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0_child0.LayoutY);
        YogaAssert.Equal(199f, root_child0_child0_child0.LayoutWidth);
        YogaAssert.Equal(100f, root_child0_child0_child0.LayoutHeight);
    }

    [TestMethod]
    public void Justify_Content_Space_Between_Indefinite_Container_Dim_With_Free_Space()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(300f);
        root.AlignItems = FlexAlign.Center;
        var root_child0 = new YogaNode(config);
        root_child0.FlexDirection = FlexDirection.Row;
        root_child0.MinWidth = YogaValue.Point(200f);
        root_child0.JustifyContent = FlexJustify.SpaceBetween;
        root.InsertChild(root_child0, 0);
        var root_child0_child0 = new YogaNode(config);
        root_child0_child0.Width = YogaValue.Point(50f);
        root_child0_child0.Height = YogaValue.Point(50f);
        root_child0.InsertChild(root_child0_child0, 0);
        var root_child0_child1 = new YogaNode(config);
        root_child0_child1.Width = YogaValue.Point(50f);
        root_child0_child1.Height = YogaValue.Point(50f);
        root_child0.InsertChild(root_child0_child1, 1);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(300f, root.LayoutWidth);
        YogaAssert.Equal(50f, root.LayoutHeight);
        YogaAssert.Equal(50f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(200f, root_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child0.LayoutHeight);
        YogaAssert.Equal(150f, root_child0_child1.LayoutX);
        YogaAssert.Equal(0f, root_child0_child1.LayoutY);
        YogaAssert.Equal(50f, root_child0_child1.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child1.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(300f, root.LayoutWidth);
        YogaAssert.Equal(50f, root.LayoutHeight);
        YogaAssert.Equal(50f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(200f, root_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0.LayoutHeight);
        YogaAssert.Equal(150f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child1.LayoutX);
        YogaAssert.Equal(0f, root_child0_child1.LayoutY);
        YogaAssert.Equal(50f, root_child0_child1.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child1.LayoutHeight);
    }

    [TestMethod]
    public void Justify_Content_Flex_Start_Row_Reverse()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        root.FlexDirection = FlexDirection.RowReverse;
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Point(20f);
        root.InsertChild(root_child0, 0);
        var root_child1 = new YogaNode(config);
        root_child1.Width = YogaValue.Point(20f);
        root.InsertChild(root_child1, 1);
        var root_child2 = new YogaNode(config);
        root_child2.Width = YogaValue.Point(20f);
        root.InsertChild(root_child2, 2);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(80f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(20f, root_child0.LayoutWidth);
        YogaAssert.Equal(100f, root_child0.LayoutHeight);
        YogaAssert.Equal(60f, root_child1.LayoutX);
        YogaAssert.Equal(0f, root_child1.LayoutY);
        YogaAssert.Equal(20f, root_child1.LayoutWidth);
        YogaAssert.Equal(100f, root_child1.LayoutHeight);
        YogaAssert.Equal(40f, root_child2.LayoutX);
        YogaAssert.Equal(0f, root_child2.LayoutY);
        YogaAssert.Equal(20f, root_child2.LayoutWidth);
        YogaAssert.Equal(100f, root_child2.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(20f, root_child0.LayoutWidth);
        YogaAssert.Equal(100f, root_child0.LayoutHeight);
        YogaAssert.Equal(20f, root_child1.LayoutX);
        YogaAssert.Equal(0f, root_child1.LayoutY);
        YogaAssert.Equal(20f, root_child1.LayoutWidth);
        YogaAssert.Equal(100f, root_child1.LayoutHeight);
        YogaAssert.Equal(40f, root_child2.LayoutX);
        YogaAssert.Equal(0f, root_child2.LayoutY);
        YogaAssert.Equal(20f, root_child2.LayoutWidth);
        YogaAssert.Equal(100f, root_child2.LayoutHeight);
    }

    [TestMethod]
    public void Justify_Content_Flex_End_Row_Reverse()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        root.FlexDirection = FlexDirection.RowReverse;
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Point(20f);
        root.InsertChild(root_child0, 0);
        var root_child1 = new YogaNode(config);
        root_child1.Width = YogaValue.Point(20f);
        root.InsertChild(root_child1, 1);
        var root_child2 = new YogaNode(config);
        root_child2.Width = YogaValue.Point(20f);
        root.InsertChild(root_child2, 2);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(80f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(20f, root_child0.LayoutWidth);
        YogaAssert.Equal(100f, root_child0.LayoutHeight);
        YogaAssert.Equal(60f, root_child1.LayoutX);
        YogaAssert.Equal(0f, root_child1.LayoutY);
        YogaAssert.Equal(20f, root_child1.LayoutWidth);
        YogaAssert.Equal(100f, root_child1.LayoutHeight);
        YogaAssert.Equal(40f, root_child2.LayoutX);
        YogaAssert.Equal(0f, root_child2.LayoutY);
        YogaAssert.Equal(20f, root_child2.LayoutWidth);
        YogaAssert.Equal(100f, root_child2.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(20f, root_child0.LayoutWidth);
        YogaAssert.Equal(100f, root_child0.LayoutHeight);
        YogaAssert.Equal(20f, root_child1.LayoutX);
        YogaAssert.Equal(0f, root_child1.LayoutY);
        YogaAssert.Equal(20f, root_child1.LayoutWidth);
        YogaAssert.Equal(100f, root_child1.LayoutHeight);
        YogaAssert.Equal(40f, root_child2.LayoutX);
        YogaAssert.Equal(0f, root_child2.LayoutY);
        YogaAssert.Equal(20f, root_child2.LayoutWidth);
        YogaAssert.Equal(100f, root_child2.LayoutHeight);
    }

    [TestMethod]
    public void Justify_Content_Overflow_Row_Flex_Start()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(102f);
        root.Height = YogaValue.Point(102f);
        root.FlexDirection = FlexDirection.Row;
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Point(40f);
        root.InsertChild(root_child0, 0);
        var root_child1 = new YogaNode(config);
        root_child1.Width = YogaValue.Point(40f);
        root.InsertChild(root_child1, 1);
        var root_child2 = new YogaNode(config);
        root_child2.Width = YogaValue.Point(40f);
        root.InsertChild(root_child2, 2);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(102f, root.LayoutWidth);
        YogaAssert.Equal(102f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(40f, root_child0.LayoutWidth);
        YogaAssert.Equal(102f, root_child0.LayoutHeight);
        YogaAssert.Equal(40f, root_child1.LayoutX);
        YogaAssert.Equal(0f, root_child1.LayoutY);
        YogaAssert.Equal(40f, root_child1.LayoutWidth);
        YogaAssert.Equal(102f, root_child1.LayoutHeight);
        YogaAssert.Equal(80f, root_child2.LayoutX);
        YogaAssert.Equal(0f, root_child2.LayoutY);
        YogaAssert.Equal(40f, root_child2.LayoutWidth);
        YogaAssert.Equal(102f, root_child2.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(102f, root.LayoutWidth);
        YogaAssert.Equal(102f, root.LayoutHeight);
        YogaAssert.Equal(62f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(40f, root_child0.LayoutWidth);
        YogaAssert.Equal(102f, root_child0.LayoutHeight);
        YogaAssert.Equal(22f, root_child1.LayoutX);
        YogaAssert.Equal(0f, root_child1.LayoutY);
        YogaAssert.Equal(40f, root_child1.LayoutWidth);
        YogaAssert.Equal(102f, root_child1.LayoutHeight);
        YogaAssert.Equal(-18f, root_child2.LayoutX);
        YogaAssert.Equal(0f, root_child2.LayoutY);
        YogaAssert.Equal(40f, root_child2.LayoutWidth);
        YogaAssert.Equal(102f, root_child2.LayoutHeight);
    }

    [TestMethod]
    public void Justify_Content_Overflow_Row_Flex_End()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(102f);
        root.Height = YogaValue.Point(102f);
        root.FlexDirection = FlexDirection.Row;
        root.JustifyContent = FlexJustify.FlexEnd;
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Point(40f);
        root.InsertChild(root_child0, 0);
        var root_child1 = new YogaNode(config);
        root_child1.Width = YogaValue.Point(40f);
        root.InsertChild(root_child1, 1);
        var root_child2 = new YogaNode(config);
        root_child2.Width = YogaValue.Point(40f);
        root.InsertChild(root_child2, 2);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(102f, root.LayoutWidth);
        YogaAssert.Equal(102f, root.LayoutHeight);
        YogaAssert.Equal(-18f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(40f, root_child0.LayoutWidth);
        YogaAssert.Equal(102f, root_child0.LayoutHeight);
        YogaAssert.Equal(22f, root_child1.LayoutX);
        YogaAssert.Equal(0f, root_child1.LayoutY);
        YogaAssert.Equal(40f, root_child1.LayoutWidth);
        YogaAssert.Equal(102f, root_child1.LayoutHeight);
        YogaAssert.Equal(62f, root_child2.LayoutX);
        YogaAssert.Equal(0f, root_child2.LayoutY);
        YogaAssert.Equal(40f, root_child2.LayoutWidth);
        YogaAssert.Equal(102f, root_child2.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(102f, root.LayoutWidth);
        YogaAssert.Equal(102f, root.LayoutHeight);
        YogaAssert.Equal(80f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(40f, root_child0.LayoutWidth);
        YogaAssert.Equal(102f, root_child0.LayoutHeight);
        YogaAssert.Equal(40f, root_child1.LayoutX);
        YogaAssert.Equal(0f, root_child1.LayoutY);
        YogaAssert.Equal(40f, root_child1.LayoutWidth);
        YogaAssert.Equal(102f, root_child1.LayoutHeight);
        YogaAssert.Equal(0f, root_child2.LayoutX);
        YogaAssert.Equal(0f, root_child2.LayoutY);
        YogaAssert.Equal(40f, root_child2.LayoutWidth);
        YogaAssert.Equal(102f, root_child2.LayoutHeight);
    }

    [TestMethod]
    public void Justify_Content_Overflow_Row_Center()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(102f);
        root.Height = YogaValue.Point(102f);
        root.FlexDirection = FlexDirection.Row;
        root.JustifyContent = FlexJustify.Center;
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Point(40f);
        root.InsertChild(root_child0, 0);
        var root_child1 = new YogaNode(config);
        root_child1.Width = YogaValue.Point(40f);
        root.InsertChild(root_child1, 1);
        var root_child2 = new YogaNode(config);
        root_child2.Width = YogaValue.Point(40f);
        root.InsertChild(root_child2, 2);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(102f, root.LayoutWidth);
        YogaAssert.Equal(102f, root.LayoutHeight);
        YogaAssert.Equal(-9f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(40f, root_child0.LayoutWidth);
        YogaAssert.Equal(102f, root_child0.LayoutHeight);
        YogaAssert.Equal(31f, root_child1.LayoutX);
        YogaAssert.Equal(0f, root_child1.LayoutY);
        YogaAssert.Equal(40f, root_child1.LayoutWidth);
        YogaAssert.Equal(102f, root_child1.LayoutHeight);
        YogaAssert.Equal(71f, root_child2.LayoutX);
        YogaAssert.Equal(0f, root_child2.LayoutY);
        YogaAssert.Equal(40f, root_child2.LayoutWidth);
        YogaAssert.Equal(102f, root_child2.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(102f, root.LayoutWidth);
        YogaAssert.Equal(102f, root.LayoutHeight);
        YogaAssert.Equal(71f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(40f, root_child0.LayoutWidth);
        YogaAssert.Equal(102f, root_child0.LayoutHeight);
        YogaAssert.Equal(31f, root_child1.LayoutX);
        YogaAssert.Equal(0f, root_child1.LayoutY);
        YogaAssert.Equal(40f, root_child1.LayoutWidth);
        YogaAssert.Equal(102f, root_child1.LayoutHeight);
        YogaAssert.Equal(-9f, root_child2.LayoutX);
        YogaAssert.Equal(0f, root_child2.LayoutY);
        YogaAssert.Equal(40f, root_child2.LayoutWidth);
        YogaAssert.Equal(102f, root_child2.LayoutHeight);
    }

    [TestMethod]
    public void Justify_Content_Overflow_Row_Space_Between()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(102f);
        root.Height = YogaValue.Point(102f);
        root.FlexDirection = FlexDirection.Row;
        root.JustifyContent = FlexJustify.SpaceBetween;
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Point(40f);
        root.InsertChild(root_child0, 0);
        var root_child1 = new YogaNode(config);
        root_child1.Width = YogaValue.Point(40f);
        root.InsertChild(root_child1, 1);
        var root_child2 = new YogaNode(config);
        root_child2.Width = YogaValue.Point(40f);
        root.InsertChild(root_child2, 2);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(102f, root.LayoutWidth);
        YogaAssert.Equal(102f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(40f, root_child0.LayoutWidth);
        YogaAssert.Equal(102f, root_child0.LayoutHeight);
        YogaAssert.Equal(40f, root_child1.LayoutX);
        YogaAssert.Equal(0f, root_child1.LayoutY);
        YogaAssert.Equal(40f, root_child1.LayoutWidth);
        YogaAssert.Equal(102f, root_child1.LayoutHeight);
        YogaAssert.Equal(80f, root_child2.LayoutX);
        YogaAssert.Equal(0f, root_child2.LayoutY);
        YogaAssert.Equal(40f, root_child2.LayoutWidth);
        YogaAssert.Equal(102f, root_child2.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(102f, root.LayoutWidth);
        YogaAssert.Equal(102f, root.LayoutHeight);
        YogaAssert.Equal(62f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(40f, root_child0.LayoutWidth);
        YogaAssert.Equal(102f, root_child0.LayoutHeight);
        YogaAssert.Equal(22f, root_child1.LayoutX);
        YogaAssert.Equal(0f, root_child1.LayoutY);
        YogaAssert.Equal(40f, root_child1.LayoutWidth);
        YogaAssert.Equal(102f, root_child1.LayoutHeight);
        YogaAssert.Equal(-18f, root_child2.LayoutX);
        YogaAssert.Equal(0f, root_child2.LayoutY);
        YogaAssert.Equal(40f, root_child2.LayoutWidth);
        YogaAssert.Equal(102f, root_child2.LayoutHeight);
    }

    [TestMethod]
    public void Justify_Content_Overflow_Row_Space_Around()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(102f);
        root.Height = YogaValue.Point(102f);
        root.FlexDirection = FlexDirection.Row;
        root.JustifyContent = FlexJustify.SpaceAround;
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Point(40f);
        root.InsertChild(root_child0, 0);
        var root_child1 = new YogaNode(config);
        root_child1.Width = YogaValue.Point(40f);
        root.InsertChild(root_child1, 1);
        var root_child2 = new YogaNode(config);
        root_child2.Width = YogaValue.Point(40f);
        root.InsertChild(root_child2, 2);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(102f, root.LayoutWidth);
        YogaAssert.Equal(102f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(40f, root_child0.LayoutWidth);
        YogaAssert.Equal(102f, root_child0.LayoutHeight);
        YogaAssert.Equal(40f, root_child1.LayoutX);
        YogaAssert.Equal(0f, root_child1.LayoutY);
        YogaAssert.Equal(40f, root_child1.LayoutWidth);
        YogaAssert.Equal(102f, root_child1.LayoutHeight);
        YogaAssert.Equal(80f, root_child2.LayoutX);
        YogaAssert.Equal(0f, root_child2.LayoutY);
        YogaAssert.Equal(40f, root_child2.LayoutWidth);
        YogaAssert.Equal(102f, root_child2.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(102f, root.LayoutWidth);
        YogaAssert.Equal(102f, root.LayoutHeight);
        YogaAssert.Equal(62f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(40f, root_child0.LayoutWidth);
        YogaAssert.Equal(102f, root_child0.LayoutHeight);
        YogaAssert.Equal(22f, root_child1.LayoutX);
        YogaAssert.Equal(0f, root_child1.LayoutY);
        YogaAssert.Equal(40f, root_child1.LayoutWidth);
        YogaAssert.Equal(102f, root_child1.LayoutHeight);
        YogaAssert.Equal(-18f, root_child2.LayoutX);
        YogaAssert.Equal(0f, root_child2.LayoutY);
        YogaAssert.Equal(40f, root_child2.LayoutWidth);
        YogaAssert.Equal(102f, root_child2.LayoutHeight);
    }

    [TestMethod]
    public void Justify_Content_Overflow_Row_Space_Evenly()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(102f);
        root.Height = YogaValue.Point(102f);
        root.FlexDirection = FlexDirection.Row;
        root.JustifyContent = FlexJustify.SpaceEvenly;
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Point(40f);
        root.InsertChild(root_child0, 0);
        var root_child1 = new YogaNode(config);
        root_child1.Width = YogaValue.Point(40f);
        root.InsertChild(root_child1, 1);
        var root_child2 = new YogaNode(config);
        root_child2.Width = YogaValue.Point(40f);
        root.InsertChild(root_child2, 2);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(102f, root.LayoutWidth);
        YogaAssert.Equal(102f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(40f, root_child0.LayoutWidth);
        YogaAssert.Equal(102f, root_child0.LayoutHeight);
        YogaAssert.Equal(40f, root_child1.LayoutX);
        YogaAssert.Equal(0f, root_child1.LayoutY);
        YogaAssert.Equal(40f, root_child1.LayoutWidth);
        YogaAssert.Equal(102f, root_child1.LayoutHeight);
        YogaAssert.Equal(80f, root_child2.LayoutX);
        YogaAssert.Equal(0f, root_child2.LayoutY);
        YogaAssert.Equal(40f, root_child2.LayoutWidth);
        YogaAssert.Equal(102f, root_child2.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(102f, root.LayoutWidth);
        YogaAssert.Equal(102f, root.LayoutHeight);
        YogaAssert.Equal(62f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(40f, root_child0.LayoutWidth);
        YogaAssert.Equal(102f, root_child0.LayoutHeight);
        YogaAssert.Equal(22f, root_child1.LayoutX);
        YogaAssert.Equal(0f, root_child1.LayoutY);
        YogaAssert.Equal(40f, root_child1.LayoutWidth);
        YogaAssert.Equal(102f, root_child1.LayoutHeight);
        YogaAssert.Equal(-18f, root_child2.LayoutX);
        YogaAssert.Equal(0f, root_child2.LayoutY);
        YogaAssert.Equal(40f, root_child2.LayoutWidth);
        YogaAssert.Equal(102f, root_child2.LayoutHeight);
    }

    [TestMethod]
    [Ignore("Skipped in upstream Yoga (GTEST_SKIP)")]
    public void Justify_Content_Overflow_Row_Reverse_Space_Around()
    {
        // TODO: GTEST_SKIP();
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(102f);
        root.Height = YogaValue.Point(102f);
        root.FlexDirection = FlexDirection.RowReverse;
        root.JustifyContent = FlexJustify.SpaceAround;
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Point(40f);
        root.InsertChild(root_child0, 0);
        var root_child1 = new YogaNode(config);
        root_child1.Width = YogaValue.Point(40f);
        root.InsertChild(root_child1, 1);
        var root_child2 = new YogaNode(config);
        root_child2.Width = YogaValue.Point(40f);
        root.InsertChild(root_child2, 2);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(102f, root.LayoutWidth);
        YogaAssert.Equal(102f, root.LayoutHeight);
        YogaAssert.Equal(80f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(40f, root_child0.LayoutWidth);
        YogaAssert.Equal(102f, root_child0.LayoutHeight);
        YogaAssert.Equal(40f, root_child1.LayoutX);
        YogaAssert.Equal(0f, root_child1.LayoutY);
        YogaAssert.Equal(40f, root_child1.LayoutWidth);
        YogaAssert.Equal(102f, root_child1.LayoutHeight);
        YogaAssert.Equal(0f, root_child2.LayoutX);
        YogaAssert.Equal(0f, root_child2.LayoutY);
        YogaAssert.Equal(40f, root_child2.LayoutWidth);
        YogaAssert.Equal(102f, root_child2.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(102f, root.LayoutWidth);
        YogaAssert.Equal(102f, root.LayoutHeight);
        YogaAssert.Equal(-18f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(40f, root_child0.LayoutWidth);
        YogaAssert.Equal(102f, root_child0.LayoutHeight);
        YogaAssert.Equal(22f, root_child1.LayoutX);
        YogaAssert.Equal(0f, root_child1.LayoutY);
        YogaAssert.Equal(40f, root_child1.LayoutWidth);
        YogaAssert.Equal(102f, root_child1.LayoutHeight);
        YogaAssert.Equal(62f, root_child2.LayoutX);
        YogaAssert.Equal(0f, root_child2.LayoutY);
        YogaAssert.Equal(40f, root_child2.LayoutWidth);
        YogaAssert.Equal(102f, root_child2.LayoutHeight);
    }

    [TestMethod]
    [Ignore("Skipped in upstream Yoga (GTEST_SKIP)")]
    public void Justify_Content_Overflow_Row_Reverse_Space_Evenly()
    {
        // TODO: GTEST_SKIP();
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(102f);
        root.Height = YogaValue.Point(102f);
        root.FlexDirection = FlexDirection.RowReverse;
        root.JustifyContent = FlexJustify.SpaceEvenly;
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Point(40f);
        root.InsertChild(root_child0, 0);
        var root_child1 = new YogaNode(config);
        root_child1.Width = YogaValue.Point(40f);
        root.InsertChild(root_child1, 1);
        var root_child2 = new YogaNode(config);
        root_child2.Width = YogaValue.Point(40f);
        root.InsertChild(root_child2, 2);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(102f, root.LayoutWidth);
        YogaAssert.Equal(102f, root.LayoutHeight);
        YogaAssert.Equal(80f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(40f, root_child0.LayoutWidth);
        YogaAssert.Equal(102f, root_child0.LayoutHeight);
        YogaAssert.Equal(40f, root_child1.LayoutX);
        YogaAssert.Equal(0f, root_child1.LayoutY);
        YogaAssert.Equal(40f, root_child1.LayoutWidth);
        YogaAssert.Equal(102f, root_child1.LayoutHeight);
        YogaAssert.Equal(0f, root_child2.LayoutX);
        YogaAssert.Equal(0f, root_child2.LayoutY);
        YogaAssert.Equal(40f, root_child2.LayoutWidth);
        YogaAssert.Equal(102f, root_child2.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(102f, root.LayoutWidth);
        YogaAssert.Equal(102f, root.LayoutHeight);
        YogaAssert.Equal(-18f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(40f, root_child0.LayoutWidth);
        YogaAssert.Equal(102f, root_child0.LayoutHeight);
        YogaAssert.Equal(22f, root_child1.LayoutX);
        YogaAssert.Equal(0f, root_child1.LayoutY);
        YogaAssert.Equal(40f, root_child1.LayoutWidth);
        YogaAssert.Equal(102f, root_child1.LayoutHeight);
        YogaAssert.Equal(62f, root_child2.LayoutX);
        YogaAssert.Equal(0f, root_child2.LayoutY);
        YogaAssert.Equal(40f, root_child2.LayoutWidth);
        YogaAssert.Equal(102f, root_child2.LayoutHeight);
    }

    [TestMethod]
    public void Justify_Content_Overflow_Row_Space_Evenly_Auto_Margin()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(102f);
        root.Height = YogaValue.Point(102f);
        root.FlexDirection = FlexDirection.Row;
        root.JustifyContent = FlexJustify.SpaceEvenly;
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Point(40f);
        root_child0.SetMargin(YogaEdge.Right, YogaValue.Auto);
        root.InsertChild(root_child0, 0);
        var root_child1 = new YogaNode(config);
        root_child1.Width = YogaValue.Point(40f);
        root.InsertChild(root_child1, 1);
        var root_child2 = new YogaNode(config);
        root_child2.Width = YogaValue.Point(40f);
        root.InsertChild(root_child2, 2);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(102f, root.LayoutWidth);
        YogaAssert.Equal(102f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(40f, root_child0.LayoutWidth);
        YogaAssert.Equal(102f, root_child0.LayoutHeight);
        YogaAssert.Equal(40f, root_child1.LayoutX);
        YogaAssert.Equal(0f, root_child1.LayoutY);
        YogaAssert.Equal(40f, root_child1.LayoutWidth);
        YogaAssert.Equal(102f, root_child1.LayoutHeight);
        YogaAssert.Equal(80f, root_child2.LayoutX);
        YogaAssert.Equal(0f, root_child2.LayoutY);
        YogaAssert.Equal(40f, root_child2.LayoutWidth);
        YogaAssert.Equal(102f, root_child2.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(102f, root.LayoutWidth);
        YogaAssert.Equal(102f, root.LayoutHeight);
        YogaAssert.Equal(62f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(40f, root_child0.LayoutWidth);
        YogaAssert.Equal(102f, root_child0.LayoutHeight);
        YogaAssert.Equal(22f, root_child1.LayoutX);
        YogaAssert.Equal(0f, root_child1.LayoutY);
        YogaAssert.Equal(40f, root_child1.LayoutWidth);
        YogaAssert.Equal(102f, root_child1.LayoutHeight);
        YogaAssert.Equal(-18f, root_child2.LayoutX);
        YogaAssert.Equal(0f, root_child2.LayoutY);
        YogaAssert.Equal(40f, root_child2.LayoutWidth);
        YogaAssert.Equal(102f, root_child2.LayoutHeight);
    }

}
