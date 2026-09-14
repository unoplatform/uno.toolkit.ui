// Vendored from microsoft/microsoft-ui-reactor @ v0.1.0-preview.13 (c9191b97c40a2e4d6bcbc72df7714184862b4d36), imported 2026-09-04.
// Source: tests/Reactor.Tests/YogaGenerated/YogaBoxSizingTest.cs -- xUnit->MSTest; DO NOT HAND-EDIT; see Tests/Yoga/README.md.
// SPDX-License-Identifier: MIT -- (c) Microsoft Corporation (C# port); (c) Facebook, Inc. and its affiliates (Yoga).
// Full license text: THIRD-PARTY-NOTICES.md

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Toolkit.UI;
using Uno.Toolkit.UI.Yoga;

namespace Uno.Toolkit.RuntimeTests.Tests.Yoga;

/// <summary>
/// Ported from yoga/tests/generated/YGBoxSizingTest.cpp
/// </summary>
[TestClass]
public class YogaBoxSizingTest
{
    [TestMethod]
    public void Box_Sizing_Content_Box_Simple()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        root.SetPadding(YogaEdge.All, YogaValue.Point(5f));
        root.SetBorder(YogaEdge.All, 10f);
        root.Style.BoxSizing = YogaBoxSizing.ContentBox;
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(130f, root.LayoutWidth);
        YogaAssert.Equal(130f, root.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(130f, root.LayoutWidth);
        YogaAssert.Equal(130f, root.LayoutHeight);
    }

    [TestMethod]
    public void Box_Sizing_Border_Box_Simple()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        root.SetPadding(YogaEdge.All, YogaValue.Point(5f));
        root.SetBorder(YogaEdge.All, 10f);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
    }

    [TestMethod]
    public void Box_Sizing_Content_Box_Percent()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Percent(50f);
        root_child0.Height = YogaValue.Percent(25f);
        root_child0.SetPadding(YogaEdge.All, YogaValue.Point(4f));
        root_child0.SetBorder(YogaEdge.All, 16f);
        root_child0.Style.BoxSizing = YogaBoxSizing.ContentBox;
        root.InsertChild(root_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(90f, root_child0.LayoutWidth);
        YogaAssert.Equal(65f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(10f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(90f, root_child0.LayoutWidth);
        YogaAssert.Equal(65f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Box_Sizing_Border_Box_Percent()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Percent(50f);
        root_child0.Height = YogaValue.Percent(25f);
        root_child0.SetPadding(YogaEdge.All, YogaValue.Point(4f));
        root_child0.SetBorder(YogaEdge.All, 16f);
        root.InsertChild(root_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(40f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(50f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(40f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Box_Sizing_Content_Box_Absolute()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        var root_child0 = new YogaNode(config);
        root_child0.Height = YogaValue.Percent(25f);
        root_child0.SetPadding(YogaEdge.All, YogaValue.Point(12f));
        root_child0.SetBorder(YogaEdge.All, 8f);
        root_child0.Style.BoxSizing = YogaBoxSizing.ContentBox;
        root_child0.PositionType = FlexPositionType.Absolute;
        root.InsertChild(root_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(40f, root_child0.LayoutWidth);
        YogaAssert.Equal(65f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(60f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(40f, root_child0.LayoutWidth);
        YogaAssert.Equal(65f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Box_Sizing_Border_Box_Absolute()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        var root_child0 = new YogaNode(config);
        root_child0.Height = YogaValue.Percent(25f);
        root_child0.SetPadding(YogaEdge.All, YogaValue.Point(12f));
        root_child0.SetBorder(YogaEdge.All, 8f);
        root_child0.PositionType = FlexPositionType.Absolute;
        root.InsertChild(root_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(40f, root_child0.LayoutWidth);
        YogaAssert.Equal(40f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(60f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(40f, root_child0.LayoutWidth);
        YogaAssert.Equal(40f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Box_Sizing_Content_Box_Comtaining_Block()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        root.SetPadding(YogaEdge.All, YogaValue.Point(12f));
        root.SetBorder(YogaEdge.All, 8f);
        root.Style.BoxSizing = YogaBoxSizing.ContentBox;
        var root_child0 = new YogaNode(config);
        root_child0.PositionType = FlexPositionType.Static;
        root.InsertChild(root_child0, 0);
        var root_child0_child0 = new YogaNode(config);
        root_child0_child0.Width = YogaValue.Point(50f);
        root_child0_child0.Height = YogaValue.Percent(25f);
        root_child0_child0.PositionType = FlexPositionType.Absolute;
        root_child0.InsertChild(root_child0_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(140f, root.LayoutWidth);
        YogaAssert.Equal(140f, root.LayoutHeight);
        YogaAssert.Equal(20f, root_child0.LayoutX);
        YogaAssert.Equal(20f, root_child0.LayoutY);
        YogaAssert.Equal(100f, root_child0.LayoutWidth);
        YogaAssert.Equal(0f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(31f, root_child0_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(140f, root.LayoutWidth);
        YogaAssert.Equal(140f, root.LayoutHeight);
        YogaAssert.Equal(20f, root_child0.LayoutX);
        YogaAssert.Equal(20f, root_child0.LayoutY);
        YogaAssert.Equal(100f, root_child0.LayoutWidth);
        YogaAssert.Equal(0f, root_child0.LayoutHeight);
        YogaAssert.Equal(50f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(31f, root_child0_child0.LayoutHeight);
    }

    [TestMethod]
    public void Box_Sizing_Border_Box_Comtaining_Block()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        root.SetPadding(YogaEdge.All, YogaValue.Point(12f));
        root.SetBorder(YogaEdge.All, 8f);
        var root_child0 = new YogaNode(config);
        root_child0.PositionType = FlexPositionType.Static;
        root.InsertChild(root_child0, 0);
        var root_child0_child0 = new YogaNode(config);
        root_child0_child0.Width = YogaValue.Point(50f);
        root_child0_child0.Height = YogaValue.Percent(25f);
        root_child0_child0.PositionType = FlexPositionType.Absolute;
        root_child0.InsertChild(root_child0_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(20f, root_child0.LayoutX);
        YogaAssert.Equal(20f, root_child0.LayoutY);
        YogaAssert.Equal(60f, root_child0.LayoutWidth);
        YogaAssert.Equal(0f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(21f, root_child0_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(20f, root_child0.LayoutX);
        YogaAssert.Equal(20f, root_child0.LayoutY);
        YogaAssert.Equal(60f, root_child0.LayoutWidth);
        YogaAssert.Equal(0f, root_child0.LayoutHeight);
        YogaAssert.Equal(10f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(21f, root_child0_child0.LayoutHeight);
    }

    [TestMethod]
    public void Box_Sizing_Content_Box_Padding_Only()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        root.SetPadding(YogaEdge.All, YogaValue.Point(5f));
        root.Style.BoxSizing = YogaBoxSizing.ContentBox;
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(110f, root.LayoutWidth);
        YogaAssert.Equal(110f, root.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(110f, root.LayoutWidth);
        YogaAssert.Equal(110f, root.LayoutHeight);
    }

    [TestMethod]
    public void Box_Sizing_Content_Box_Padding_Only_Percent()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(150f);
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Point(50f);
        root_child0.Height = YogaValue.Point(75f);
        root_child0.SetPadding(YogaEdge.All, YogaValue.Percent(10f));
        root_child0.Style.BoxSizing = YogaBoxSizing.ContentBox;
        root.InsertChild(root_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(150f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(70f, root_child0.LayoutWidth);
        YogaAssert.Equal(95f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(150f, root.LayoutHeight);
        YogaAssert.Equal(30f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(70f, root_child0.LayoutWidth);
        YogaAssert.Equal(95f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Box_Sizing_Border_Box_Padding_Only()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        root.SetPadding(YogaEdge.All, YogaValue.Point(5f));
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
    }

    [TestMethod]
    public void Box_Sizing_Border_Box_Padding_Only_Percent()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(150f);
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Point(50f);
        root_child0.Height = YogaValue.Point(75f);
        root_child0.SetPadding(YogaEdge.All, YogaValue.Percent(10f));
        root.InsertChild(root_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(150f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(75f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(150f, root.LayoutHeight);
        YogaAssert.Equal(50f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(75f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Box_Sizing_Content_Box_Border_Only()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        root.SetBorder(YogaEdge.All, 10f);
        root.Style.BoxSizing = YogaBoxSizing.ContentBox;
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(120f, root.LayoutWidth);
        YogaAssert.Equal(120f, root.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(120f, root.LayoutWidth);
        YogaAssert.Equal(120f, root.LayoutHeight);
    }

    [TestMethod]
    public void Box_Sizing_Content_Box_Border_Only_Percent()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Percent(50f);
        root_child0.Style.BoxSizing = YogaBoxSizing.ContentBox;
        root.InsertChild(root_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(0f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(50f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(0f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Box_Sizing_Border_Box_Border_Only()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        root.SetBorder(YogaEdge.All, 10f);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
    }

    [TestMethod]
    public void Box_Sizing_Border_Box_Border_Only_Percent()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Percent(50f);
        root.InsertChild(root_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(0f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(50f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(0f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Box_Sizing_Content_Box_No_Padding_No_Border()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        root.Style.BoxSizing = YogaBoxSizing.ContentBox;
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
    }

    [TestMethod]
    public void Box_Sizing_Border_Box_No_Padding_No_Border()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
    }

    [TestMethod]
    public void Box_Sizing_Content_Box_Children()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        root.SetPadding(YogaEdge.All, YogaValue.Point(5f));
        root.SetBorder(YogaEdge.All, 10f);
        root.Style.BoxSizing = YogaBoxSizing.ContentBox;
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Point(25f);
        root_child0.Height = YogaValue.Point(25f);
        root.InsertChild(root_child0, 0);
        var root_child1 = new YogaNode(config);
        root_child1.Width = YogaValue.Point(25f);
        root_child1.Height = YogaValue.Point(25f);
        root.InsertChild(root_child1, 1);
        var root_child2 = new YogaNode(config);
        root_child2.Width = YogaValue.Point(25f);
        root_child2.Height = YogaValue.Point(25f);
        root.InsertChild(root_child2, 2);
        var root_child3 = new YogaNode(config);
        root_child3.Width = YogaValue.Point(25f);
        root_child3.Height = YogaValue.Point(25f);
        root.InsertChild(root_child3, 3);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(130f, root.LayoutWidth);
        YogaAssert.Equal(130f, root.LayoutHeight);
        YogaAssert.Equal(15f, root_child0.LayoutX);
        YogaAssert.Equal(15f, root_child0.LayoutY);
        YogaAssert.Equal(25f, root_child0.LayoutWidth);
        YogaAssert.Equal(25f, root_child0.LayoutHeight);
        YogaAssert.Equal(15f, root_child1.LayoutX);
        YogaAssert.Equal(40f, root_child1.LayoutY);
        YogaAssert.Equal(25f, root_child1.LayoutWidth);
        YogaAssert.Equal(25f, root_child1.LayoutHeight);
        YogaAssert.Equal(15f, root_child2.LayoutX);
        YogaAssert.Equal(65f, root_child2.LayoutY);
        YogaAssert.Equal(25f, root_child2.LayoutWidth);
        YogaAssert.Equal(25f, root_child2.LayoutHeight);
        YogaAssert.Equal(15f, root_child3.LayoutX);
        YogaAssert.Equal(90f, root_child3.LayoutY);
        YogaAssert.Equal(25f, root_child3.LayoutWidth);
        YogaAssert.Equal(25f, root_child3.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(130f, root.LayoutWidth);
        YogaAssert.Equal(130f, root.LayoutHeight);
        YogaAssert.Equal(90f, root_child0.LayoutX);
        YogaAssert.Equal(15f, root_child0.LayoutY);
        YogaAssert.Equal(25f, root_child0.LayoutWidth);
        YogaAssert.Equal(25f, root_child0.LayoutHeight);
        YogaAssert.Equal(90f, root_child1.LayoutX);
        YogaAssert.Equal(40f, root_child1.LayoutY);
        YogaAssert.Equal(25f, root_child1.LayoutWidth);
        YogaAssert.Equal(25f, root_child1.LayoutHeight);
        YogaAssert.Equal(90f, root_child2.LayoutX);
        YogaAssert.Equal(65f, root_child2.LayoutY);
        YogaAssert.Equal(25f, root_child2.LayoutWidth);
        YogaAssert.Equal(25f, root_child2.LayoutHeight);
        YogaAssert.Equal(90f, root_child3.LayoutX);
        YogaAssert.Equal(90f, root_child3.LayoutY);
        YogaAssert.Equal(25f, root_child3.LayoutWidth);
        YogaAssert.Equal(25f, root_child3.LayoutHeight);
    }

    [TestMethod]
    public void Box_Sizing_Border_Box_Children()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        root.SetPadding(YogaEdge.All, YogaValue.Point(5f));
        root.SetBorder(YogaEdge.All, 10f);
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Point(25f);
        root_child0.Height = YogaValue.Point(25f);
        root.InsertChild(root_child0, 0);
        var root_child1 = new YogaNode(config);
        root_child1.Width = YogaValue.Point(25f);
        root_child1.Height = YogaValue.Point(25f);
        root.InsertChild(root_child1, 1);
        var root_child2 = new YogaNode(config);
        root_child2.Width = YogaValue.Point(25f);
        root_child2.Height = YogaValue.Point(25f);
        root.InsertChild(root_child2, 2);
        var root_child3 = new YogaNode(config);
        root_child3.Width = YogaValue.Point(25f);
        root_child3.Height = YogaValue.Point(25f);
        root.InsertChild(root_child3, 3);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(15f, root_child0.LayoutX);
        YogaAssert.Equal(15f, root_child0.LayoutY);
        YogaAssert.Equal(25f, root_child0.LayoutWidth);
        YogaAssert.Equal(25f, root_child0.LayoutHeight);
        YogaAssert.Equal(15f, root_child1.LayoutX);
        YogaAssert.Equal(40f, root_child1.LayoutY);
        YogaAssert.Equal(25f, root_child1.LayoutWidth);
        YogaAssert.Equal(25f, root_child1.LayoutHeight);
        YogaAssert.Equal(15f, root_child2.LayoutX);
        YogaAssert.Equal(65f, root_child2.LayoutY);
        YogaAssert.Equal(25f, root_child2.LayoutWidth);
        YogaAssert.Equal(25f, root_child2.LayoutHeight);
        YogaAssert.Equal(15f, root_child3.LayoutX);
        YogaAssert.Equal(90f, root_child3.LayoutY);
        YogaAssert.Equal(25f, root_child3.LayoutWidth);
        YogaAssert.Equal(25f, root_child3.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(60f, root_child0.LayoutX);
        YogaAssert.Equal(15f, root_child0.LayoutY);
        YogaAssert.Equal(25f, root_child0.LayoutWidth);
        YogaAssert.Equal(25f, root_child0.LayoutHeight);
        YogaAssert.Equal(60f, root_child1.LayoutX);
        YogaAssert.Equal(40f, root_child1.LayoutY);
        YogaAssert.Equal(25f, root_child1.LayoutWidth);
        YogaAssert.Equal(25f, root_child1.LayoutHeight);
        YogaAssert.Equal(60f, root_child2.LayoutX);
        YogaAssert.Equal(65f, root_child2.LayoutY);
        YogaAssert.Equal(25f, root_child2.LayoutWidth);
        YogaAssert.Equal(25f, root_child2.LayoutHeight);
        YogaAssert.Equal(60f, root_child3.LayoutX);
        YogaAssert.Equal(90f, root_child3.LayoutY);
        YogaAssert.Equal(25f, root_child3.LayoutWidth);
        YogaAssert.Equal(25f, root_child3.LayoutHeight);
    }

    [TestMethod]
    public void Box_Sizing_Content_Box_Siblings()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Point(25f);
        root_child0.Height = YogaValue.Point(25f);
        root.InsertChild(root_child0, 0);
        var root_child1 = new YogaNode(config);
        root_child1.Width = YogaValue.Point(25f);
        root_child1.Height = YogaValue.Point(25f);
        root_child1.Style.BoxSizing = YogaBoxSizing.ContentBox;
        root_child1.SetPadding(YogaEdge.All, YogaValue.Point(10f));
        root_child1.SetBorder(YogaEdge.All, 10f);
        root.InsertChild(root_child1, 1);
        var root_child2 = new YogaNode(config);
        root_child2.Width = YogaValue.Point(25f);
        root_child2.Height = YogaValue.Point(25f);
        root.InsertChild(root_child2, 2);
        var root_child3 = new YogaNode(config);
        root_child3.Width = YogaValue.Point(25f);
        root_child3.Height = YogaValue.Point(25f);
        root.InsertChild(root_child3, 3);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(25f, root_child0.LayoutWidth);
        YogaAssert.Equal(25f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child1.LayoutX);
        YogaAssert.Equal(25f, root_child1.LayoutY);
        YogaAssert.Equal(65f, root_child1.LayoutWidth);
        YogaAssert.Equal(65f, root_child1.LayoutHeight);
        YogaAssert.Equal(0f, root_child2.LayoutX);
        YogaAssert.Equal(90f, root_child2.LayoutY);
        YogaAssert.Equal(25f, root_child2.LayoutWidth);
        YogaAssert.Equal(25f, root_child2.LayoutHeight);
        YogaAssert.Equal(0f, root_child3.LayoutX);
        YogaAssert.Equal(115f, root_child3.LayoutY);
        YogaAssert.Equal(25f, root_child3.LayoutWidth);
        YogaAssert.Equal(25f, root_child3.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(75f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(25f, root_child0.LayoutWidth);
        YogaAssert.Equal(25f, root_child0.LayoutHeight);
        YogaAssert.Equal(35f, root_child1.LayoutX);
        YogaAssert.Equal(25f, root_child1.LayoutY);
        YogaAssert.Equal(65f, root_child1.LayoutWidth);
        YogaAssert.Equal(65f, root_child1.LayoutHeight);
        YogaAssert.Equal(75f, root_child2.LayoutX);
        YogaAssert.Equal(90f, root_child2.LayoutY);
        YogaAssert.Equal(25f, root_child2.LayoutWidth);
        YogaAssert.Equal(25f, root_child2.LayoutHeight);
        YogaAssert.Equal(75f, root_child3.LayoutX);
        YogaAssert.Equal(115f, root_child3.LayoutY);
        YogaAssert.Equal(25f, root_child3.LayoutWidth);
        YogaAssert.Equal(25f, root_child3.LayoutHeight);
    }

    [TestMethod]
    public void Box_Sizing_Border_Box_Siblings()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Point(25f);
        root_child0.Height = YogaValue.Point(25f);
        root.InsertChild(root_child0, 0);
        var root_child1 = new YogaNode(config);
        root_child1.Width = YogaValue.Point(25f);
        root_child1.Height = YogaValue.Point(25f);
        root_child1.SetPadding(YogaEdge.All, YogaValue.Point(10f));
        root_child1.SetBorder(YogaEdge.All, 10f);
        root.InsertChild(root_child1, 1);
        var root_child2 = new YogaNode(config);
        root_child2.Width = YogaValue.Point(25f);
        root_child2.Height = YogaValue.Point(25f);
        root.InsertChild(root_child2, 2);
        var root_child3 = new YogaNode(config);
        root_child3.Width = YogaValue.Point(25f);
        root_child3.Height = YogaValue.Point(25f);
        root.InsertChild(root_child3, 3);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(25f, root_child0.LayoutWidth);
        YogaAssert.Equal(25f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child1.LayoutX);
        YogaAssert.Equal(25f, root_child1.LayoutY);
        YogaAssert.Equal(40f, root_child1.LayoutWidth);
        YogaAssert.Equal(40f, root_child1.LayoutHeight);
        YogaAssert.Equal(0f, root_child2.LayoutX);
        YogaAssert.Equal(65f, root_child2.LayoutY);
        YogaAssert.Equal(25f, root_child2.LayoutWidth);
        YogaAssert.Equal(25f, root_child2.LayoutHeight);
        YogaAssert.Equal(0f, root_child3.LayoutX);
        YogaAssert.Equal(90f, root_child3.LayoutY);
        YogaAssert.Equal(25f, root_child3.LayoutWidth);
        YogaAssert.Equal(25f, root_child3.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(75f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(25f, root_child0.LayoutWidth);
        YogaAssert.Equal(25f, root_child0.LayoutHeight);
        YogaAssert.Equal(60f, root_child1.LayoutX);
        YogaAssert.Equal(25f, root_child1.LayoutY);
        YogaAssert.Equal(40f, root_child1.LayoutWidth);
        YogaAssert.Equal(40f, root_child1.LayoutHeight);
        YogaAssert.Equal(75f, root_child2.LayoutX);
        YogaAssert.Equal(65f, root_child2.LayoutY);
        YogaAssert.Equal(25f, root_child2.LayoutWidth);
        YogaAssert.Equal(25f, root_child2.LayoutHeight);
        YogaAssert.Equal(75f, root_child3.LayoutX);
        YogaAssert.Equal(90f, root_child3.LayoutY);
        YogaAssert.Equal(25f, root_child3.LayoutWidth);
        YogaAssert.Equal(25f, root_child3.LayoutHeight);
    }

    [TestMethod]
    public void Box_Sizing_Content_Box_Max_Width()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        var root_child0 = new YogaNode(config);
        root_child0.MaxWidth = YogaValue.Point(50f);
        root_child0.Height = YogaValue.Point(25f);
        root_child0.Style.BoxSizing = YogaBoxSizing.ContentBox;
        root_child0.SetPadding(YogaEdge.All, YogaValue.Point(5f));
        root_child0.SetBorder(YogaEdge.All, 15f);
        root.InsertChild(root_child0, 0);
        var root_child1 = new YogaNode(config);
        root_child1.Width = YogaValue.Point(25f);
        root_child1.Height = YogaValue.Point(25f);
        root.InsertChild(root_child1, 1);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(90f, root_child0.LayoutWidth);
        YogaAssert.Equal(65f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child1.LayoutX);
        YogaAssert.Equal(65f, root_child1.LayoutY);
        YogaAssert.Equal(25f, root_child1.LayoutWidth);
        YogaAssert.Equal(25f, root_child1.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(10f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(90f, root_child0.LayoutWidth);
        YogaAssert.Equal(65f, root_child0.LayoutHeight);
        YogaAssert.Equal(75f, root_child1.LayoutX);
        YogaAssert.Equal(65f, root_child1.LayoutY);
        YogaAssert.Equal(25f, root_child1.LayoutWidth);
        YogaAssert.Equal(25f, root_child1.LayoutHeight);
    }

    [TestMethod]
    public void Box_Sizing_Border_Box_Max_Width()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        var root_child0 = new YogaNode(config);
        root_child0.MaxWidth = YogaValue.Point(50f);
        root_child0.Height = YogaValue.Point(25f);
        root_child0.SetPadding(YogaEdge.All, YogaValue.Point(5f));
        root_child0.SetBorder(YogaEdge.All, 15f);
        root.InsertChild(root_child0, 0);
        var root_child1 = new YogaNode(config);
        root_child1.Width = YogaValue.Point(25f);
        root_child1.Height = YogaValue.Point(25f);
        root.InsertChild(root_child1, 1);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(40f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child1.LayoutX);
        YogaAssert.Equal(40f, root_child1.LayoutY);
        YogaAssert.Equal(25f, root_child1.LayoutWidth);
        YogaAssert.Equal(25f, root_child1.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(50f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(40f, root_child0.LayoutHeight);
        YogaAssert.Equal(75f, root_child1.LayoutX);
        YogaAssert.Equal(40f, root_child1.LayoutY);
        YogaAssert.Equal(25f, root_child1.LayoutWidth);
        YogaAssert.Equal(25f, root_child1.LayoutHeight);
    }

    [TestMethod]
    public void Box_Sizing_Content_Box_Max_Height()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Point(50f);
        root_child0.MaxHeight = YogaValue.Point(50f);
        root_child0.Style.BoxSizing = YogaBoxSizing.ContentBox;
        root_child0.SetPadding(YogaEdge.All, YogaValue.Point(5f));
        root_child0.SetBorder(YogaEdge.All, 15f);
        root.InsertChild(root_child0, 0);
        var root_child1 = new YogaNode(config);
        root_child1.Width = YogaValue.Point(25f);
        root_child1.Height = YogaValue.Point(25f);
        root.InsertChild(root_child1, 1);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(90f, root_child0.LayoutWidth);
        YogaAssert.Equal(40f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child1.LayoutX);
        YogaAssert.Equal(40f, root_child1.LayoutY);
        YogaAssert.Equal(25f, root_child1.LayoutWidth);
        YogaAssert.Equal(25f, root_child1.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(10f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(90f, root_child0.LayoutWidth);
        YogaAssert.Equal(40f, root_child0.LayoutHeight);
        YogaAssert.Equal(75f, root_child1.LayoutX);
        YogaAssert.Equal(40f, root_child1.LayoutY);
        YogaAssert.Equal(25f, root_child1.LayoutWidth);
        YogaAssert.Equal(25f, root_child1.LayoutHeight);
    }

    [TestMethod]
    public void Box_Sizing_Border_Box_Max_Height()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Point(50f);
        root_child0.MaxHeight = YogaValue.Point(50f);
        root_child0.SetPadding(YogaEdge.All, YogaValue.Point(5f));
        root_child0.SetBorder(YogaEdge.All, 15f);
        root.InsertChild(root_child0, 0);
        var root_child1 = new YogaNode(config);
        root_child1.Width = YogaValue.Point(25f);
        root_child1.Height = YogaValue.Point(25f);
        root.InsertChild(root_child1, 1);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(40f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child1.LayoutX);
        YogaAssert.Equal(40f, root_child1.LayoutY);
        YogaAssert.Equal(25f, root_child1.LayoutWidth);
        YogaAssert.Equal(25f, root_child1.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(50f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(40f, root_child0.LayoutHeight);
        YogaAssert.Equal(75f, root_child1.LayoutX);
        YogaAssert.Equal(40f, root_child1.LayoutY);
        YogaAssert.Equal(25f, root_child1.LayoutWidth);
        YogaAssert.Equal(25f, root_child1.LayoutHeight);
    }

    [TestMethod]
    public void Box_Sizing_Content_Box_Min_Width()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        var root_child0 = new YogaNode(config);
        root_child0.MinWidth = YogaValue.Point(50f);
        root_child0.Height = YogaValue.Point(25f);
        root_child0.Style.BoxSizing = YogaBoxSizing.ContentBox;
        root_child0.SetPadding(YogaEdge.All, YogaValue.Point(5f));
        root_child0.SetBorder(YogaEdge.All, 15f);
        root.InsertChild(root_child0, 0);
        var root_child1 = new YogaNode(config);
        root_child1.Width = YogaValue.Point(25f);
        root_child1.Height = YogaValue.Point(25f);
        root.InsertChild(root_child1, 1);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(100f, root_child0.LayoutWidth);
        YogaAssert.Equal(65f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child1.LayoutX);
        YogaAssert.Equal(65f, root_child1.LayoutY);
        YogaAssert.Equal(25f, root_child1.LayoutWidth);
        YogaAssert.Equal(25f, root_child1.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(100f, root_child0.LayoutWidth);
        YogaAssert.Equal(65f, root_child0.LayoutHeight);
        YogaAssert.Equal(75f, root_child1.LayoutX);
        YogaAssert.Equal(65f, root_child1.LayoutY);
        YogaAssert.Equal(25f, root_child1.LayoutWidth);
        YogaAssert.Equal(25f, root_child1.LayoutHeight);
    }

    [TestMethod]
    public void Box_Sizing_Border_Box_Min_Width()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        var root_child0 = new YogaNode(config);
        root_child0.MinWidth = YogaValue.Point(50f);
        root_child0.Height = YogaValue.Point(25f);
        root_child0.SetPadding(YogaEdge.All, YogaValue.Point(5f));
        root_child0.SetBorder(YogaEdge.All, 15f);
        root.InsertChild(root_child0, 0);
        var root_child1 = new YogaNode(config);
        root_child1.Width = YogaValue.Point(25f);
        root_child1.Height = YogaValue.Point(25f);
        root.InsertChild(root_child1, 1);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(100f, root_child0.LayoutWidth);
        YogaAssert.Equal(40f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child1.LayoutX);
        YogaAssert.Equal(40f, root_child1.LayoutY);
        YogaAssert.Equal(25f, root_child1.LayoutWidth);
        YogaAssert.Equal(25f, root_child1.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(100f, root_child0.LayoutWidth);
        YogaAssert.Equal(40f, root_child0.LayoutHeight);
        YogaAssert.Equal(75f, root_child1.LayoutX);
        YogaAssert.Equal(40f, root_child1.LayoutY);
        YogaAssert.Equal(25f, root_child1.LayoutWidth);
        YogaAssert.Equal(25f, root_child1.LayoutHeight);
    }

    [TestMethod]
    public void Box_Sizing_Content_Box_Min_Height()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Point(50f);
        root_child0.MinHeight = YogaValue.Point(50f);
        root_child0.Style.BoxSizing = YogaBoxSizing.ContentBox;
        root_child0.SetPadding(YogaEdge.All, YogaValue.Point(5f));
        root_child0.SetBorder(YogaEdge.All, 15f);
        root.InsertChild(root_child0, 0);
        var root_child1 = new YogaNode(config);
        root_child1.Width = YogaValue.Point(25f);
        root_child1.Height = YogaValue.Point(25f);
        root.InsertChild(root_child1, 1);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(90f, root_child0.LayoutWidth);
        YogaAssert.Equal(90f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child1.LayoutX);
        YogaAssert.Equal(90f, root_child1.LayoutY);
        YogaAssert.Equal(25f, root_child1.LayoutWidth);
        YogaAssert.Equal(25f, root_child1.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(10f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(90f, root_child0.LayoutWidth);
        YogaAssert.Equal(90f, root_child0.LayoutHeight);
        YogaAssert.Equal(75f, root_child1.LayoutX);
        YogaAssert.Equal(90f, root_child1.LayoutY);
        YogaAssert.Equal(25f, root_child1.LayoutWidth);
        YogaAssert.Equal(25f, root_child1.LayoutHeight);
    }

    [TestMethod]
    public void Box_Sizing_Border_Box_Min_Height()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Point(50f);
        root_child0.MinHeight = YogaValue.Point(50f);
        root_child0.SetPadding(YogaEdge.All, YogaValue.Point(5f));
        root_child0.SetBorder(YogaEdge.All, 15f);
        root.InsertChild(root_child0, 0);
        var root_child1 = new YogaNode(config);
        root_child1.Width = YogaValue.Point(25f);
        root_child1.Height = YogaValue.Point(25f);
        root.InsertChild(root_child1, 1);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child1.LayoutX);
        YogaAssert.Equal(50f, root_child1.LayoutY);
        YogaAssert.Equal(25f, root_child1.LayoutWidth);
        YogaAssert.Equal(25f, root_child1.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(50f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0.LayoutHeight);
        YogaAssert.Equal(75f, root_child1.LayoutX);
        YogaAssert.Equal(50f, root_child1.LayoutY);
        YogaAssert.Equal(25f, root_child1.LayoutWidth);
        YogaAssert.Equal(25f, root_child1.LayoutHeight);
    }

    [TestMethod]
    public void Box_Sizing_Content_Box_No_Height_No_Width()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        var root_child0 = new YogaNode(config);
        root_child0.Style.BoxSizing = YogaBoxSizing.ContentBox;
        root_child0.SetPadding(YogaEdge.All, YogaValue.Point(2f));
        root_child0.SetBorder(YogaEdge.All, 7f);
        root.InsertChild(root_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(100f, root_child0.LayoutWidth);
        YogaAssert.Equal(18f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(100f, root_child0.LayoutWidth);
        YogaAssert.Equal(18f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Box_Sizing_Border_Box_No_Height_No_Width()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        var root_child0 = new YogaNode(config);
        root_child0.SetPadding(YogaEdge.All, YogaValue.Point(2f));
        root_child0.SetBorder(YogaEdge.All, 7f);
        root.InsertChild(root_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(100f, root_child0.LayoutWidth);
        YogaAssert.Equal(18f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(100f, root_child0.LayoutWidth);
        YogaAssert.Equal(18f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Box_Sizing_Content_Box_Nested()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        root.Style.BoxSizing = YogaBoxSizing.ContentBox;
        root.SetPadding(YogaEdge.All, YogaValue.Point(15f));
        root.SetBorder(YogaEdge.All, 3f);
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Point(20f);
        root_child0.Height = YogaValue.Point(20f);
        root_child0.Style.BoxSizing = YogaBoxSizing.ContentBox;
        root_child0.SetPadding(YogaEdge.All, YogaValue.Point(2f));
        root_child0.SetBorder(YogaEdge.All, 7f);
        root.InsertChild(root_child0, 0);
        var root_child0_child0 = new YogaNode(config);
        root_child0_child0.Width = YogaValue.Point(10f);
        root_child0_child0.Height = YogaValue.Point(5f);
        root_child0_child0.Style.BoxSizing = YogaBoxSizing.ContentBox;
        root_child0_child0.SetPadding(YogaEdge.All, YogaValue.Point(1f));
        root_child0_child0.SetBorder(YogaEdge.All, 2f);
        root_child0.InsertChild(root_child0_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(136f, root.LayoutWidth);
        YogaAssert.Equal(136f, root.LayoutHeight);
        YogaAssert.Equal(18f, root_child0.LayoutX);
        YogaAssert.Equal(18f, root_child0.LayoutY);
        YogaAssert.Equal(38f, root_child0.LayoutWidth);
        YogaAssert.Equal(38f, root_child0.LayoutHeight);
        YogaAssert.Equal(9f, root_child0_child0.LayoutX);
        YogaAssert.Equal(9f, root_child0_child0.LayoutY);
        YogaAssert.Equal(16f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(11f, root_child0_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(136f, root.LayoutWidth);
        YogaAssert.Equal(136f, root.LayoutHeight);
        YogaAssert.Equal(80f, root_child0.LayoutX);
        YogaAssert.Equal(18f, root_child0.LayoutY);
        YogaAssert.Equal(38f, root_child0.LayoutWidth);
        YogaAssert.Equal(38f, root_child0.LayoutHeight);
        YogaAssert.Equal(13f, root_child0_child0.LayoutX);
        YogaAssert.Equal(9f, root_child0_child0.LayoutY);
        YogaAssert.Equal(16f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(11f, root_child0_child0.LayoutHeight);
    }

    [TestMethod]
    public void Box_Sizing_Border_Box_Nested()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        root.SetPadding(YogaEdge.All, YogaValue.Point(15f));
        root.SetBorder(YogaEdge.All, 3f);
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Point(20f);
        root_child0.Height = YogaValue.Point(20f);
        root_child0.SetPadding(YogaEdge.All, YogaValue.Point(2f));
        root_child0.SetBorder(YogaEdge.All, 7f);
        root.InsertChild(root_child0, 0);
        var root_child0_child0 = new YogaNode(config);
        root_child0_child0.Width = YogaValue.Point(10f);
        root_child0_child0.Height = YogaValue.Point(5f);
        root_child0_child0.SetPadding(YogaEdge.All, YogaValue.Point(1f));
        root_child0_child0.SetBorder(YogaEdge.All, 2f);
        root_child0.InsertChild(root_child0_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(18f, root_child0.LayoutX);
        YogaAssert.Equal(18f, root_child0.LayoutY);
        YogaAssert.Equal(20f, root_child0.LayoutWidth);
        YogaAssert.Equal(20f, root_child0.LayoutHeight);
        YogaAssert.Equal(9f, root_child0_child0.LayoutX);
        YogaAssert.Equal(9f, root_child0_child0.LayoutY);
        YogaAssert.Equal(10f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(6f, root_child0_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(62f, root_child0.LayoutX);
        YogaAssert.Equal(18f, root_child0.LayoutY);
        YogaAssert.Equal(20f, root_child0.LayoutWidth);
        YogaAssert.Equal(20f, root_child0.LayoutHeight);
        YogaAssert.Equal(1f, root_child0_child0.LayoutX);
        YogaAssert.Equal(9f, root_child0_child0.LayoutY);
        YogaAssert.Equal(10f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(6f, root_child0_child0.LayoutHeight);
    }

    [TestMethod]
    public void Box_Sizing_Content_Box_Nested_Alternating()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        root.Style.BoxSizing = YogaBoxSizing.ContentBox;
        root.SetPadding(YogaEdge.All, YogaValue.Point(3f));
        root.SetBorder(YogaEdge.All, 2f);
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Point(40f);
        root_child0.Height = YogaValue.Point(40f);
        root_child0.SetPadding(YogaEdge.All, YogaValue.Point(8f));
        root_child0.SetBorder(YogaEdge.All, 2f);
        root.InsertChild(root_child0, 0);
        var root_child0_child0 = new YogaNode(config);
        root_child0_child0.Width = YogaValue.Point(20f);
        root_child0_child0.Height = YogaValue.Point(25f);
        root_child0_child0.Style.BoxSizing = YogaBoxSizing.ContentBox;
        root_child0_child0.SetPadding(YogaEdge.All, YogaValue.Point(3f));
        root_child0_child0.SetBorder(YogaEdge.All, 6f);
        root_child0.InsertChild(root_child0_child0, 0);
        var root_child0_child0_child0 = new YogaNode(config);
        root_child0_child0_child0.Width = YogaValue.Point(10f);
        root_child0_child0_child0.Height = YogaValue.Point(5f);
        root_child0_child0_child0.SetPadding(YogaEdge.All, YogaValue.Point(1f));
        root_child0_child0_child0.SetBorder(YogaEdge.All, 1f);
        root_child0_child0.InsertChild(root_child0_child0_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(110f, root.LayoutWidth);
        YogaAssert.Equal(110f, root.LayoutHeight);
        YogaAssert.Equal(5f, root_child0.LayoutX);
        YogaAssert.Equal(5f, root_child0.LayoutY);
        YogaAssert.Equal(40f, root_child0.LayoutWidth);
        YogaAssert.Equal(40f, root_child0.LayoutHeight);
        YogaAssert.Equal(10f, root_child0_child0.LayoutX);
        YogaAssert.Equal(10f, root_child0_child0.LayoutY);
        YogaAssert.Equal(38f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(43f, root_child0_child0.LayoutHeight);
        YogaAssert.Equal(9f, root_child0_child0_child0.LayoutX);
        YogaAssert.Equal(9f, root_child0_child0_child0.LayoutY);
        YogaAssert.Equal(10f, root_child0_child0_child0.LayoutWidth);
        YogaAssert.Equal(5f, root_child0_child0_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(110f, root.LayoutWidth);
        YogaAssert.Equal(110f, root.LayoutHeight);
        YogaAssert.Equal(65f, root_child0.LayoutX);
        YogaAssert.Equal(5f, root_child0.LayoutY);
        YogaAssert.Equal(40f, root_child0.LayoutWidth);
        YogaAssert.Equal(40f, root_child0.LayoutHeight);
        YogaAssert.Equal(-8f, root_child0_child0.LayoutX);
        YogaAssert.Equal(10f, root_child0_child0.LayoutY);
        YogaAssert.Equal(38f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(43f, root_child0_child0.LayoutHeight);
        YogaAssert.Equal(19f, root_child0_child0_child0.LayoutX);
        YogaAssert.Equal(9f, root_child0_child0_child0.LayoutY);
        YogaAssert.Equal(10f, root_child0_child0_child0.LayoutWidth);
        YogaAssert.Equal(5f, root_child0_child0_child0.LayoutHeight);
    }

    [TestMethod]
    public void Box_Sizing_Border_Box_Nested_Alternating()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        root.SetPadding(YogaEdge.All, YogaValue.Point(3f));
        root.SetBorder(YogaEdge.All, 2f);
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Point(40f);
        root_child0.Height = YogaValue.Point(40f);
        root_child0.Style.BoxSizing = YogaBoxSizing.ContentBox;
        root_child0.SetPadding(YogaEdge.All, YogaValue.Point(8f));
        root_child0.SetBorder(YogaEdge.All, 2f);
        root.InsertChild(root_child0, 0);
        var root_child0_child0 = new YogaNode(config);
        root_child0_child0.Width = YogaValue.Point(20f);
        root_child0_child0.Height = YogaValue.Point(25f);
        root_child0_child0.SetPadding(YogaEdge.All, YogaValue.Point(3f));
        root_child0_child0.SetBorder(YogaEdge.All, 6f);
        root_child0.InsertChild(root_child0_child0, 0);
        var root_child0_child0_child0 = new YogaNode(config);
        root_child0_child0_child0.Width = YogaValue.Point(10f);
        root_child0_child0_child0.Height = YogaValue.Point(5f);
        root_child0_child0_child0.Style.BoxSizing = YogaBoxSizing.ContentBox;
        root_child0_child0_child0.SetPadding(YogaEdge.All, YogaValue.Point(1f));
        root_child0_child0_child0.SetBorder(YogaEdge.All, 1f);
        root_child0_child0.InsertChild(root_child0_child0_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(5f, root_child0.LayoutX);
        YogaAssert.Equal(5f, root_child0.LayoutY);
        YogaAssert.Equal(60f, root_child0.LayoutWidth);
        YogaAssert.Equal(60f, root_child0.LayoutHeight);
        YogaAssert.Equal(10f, root_child0_child0.LayoutX);
        YogaAssert.Equal(10f, root_child0_child0.LayoutY);
        YogaAssert.Equal(20f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(25f, root_child0_child0.LayoutHeight);
        YogaAssert.Equal(9f, root_child0_child0_child0.LayoutX);
        YogaAssert.Equal(9f, root_child0_child0_child0.LayoutY);
        YogaAssert.Equal(14f, root_child0_child0_child0.LayoutWidth);
        YogaAssert.Equal(9f, root_child0_child0_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(35f, root_child0.LayoutX);
        YogaAssert.Equal(5f, root_child0.LayoutY);
        YogaAssert.Equal(60f, root_child0.LayoutWidth);
        YogaAssert.Equal(60f, root_child0.LayoutHeight);
        YogaAssert.Equal(30f, root_child0_child0.LayoutX);
        YogaAssert.Equal(10f, root_child0_child0.LayoutY);
        YogaAssert.Equal(20f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(25f, root_child0_child0.LayoutHeight);
        YogaAssert.Equal(-3f, root_child0_child0_child0.LayoutX);
        YogaAssert.Equal(9f, root_child0_child0_child0.LayoutY);
        YogaAssert.Equal(14f, root_child0_child0_child0.LayoutWidth);
        YogaAssert.Equal(9f, root_child0_child0_child0.LayoutHeight);
    }

    [TestMethod]
    [Ignore("Skipped in upstream Yoga (GTEST_SKIP)")]
    public void Box_Sizing_Content_Box_Flex_Basis_Row()
    {
        // TODO: GTEST_SKIP();
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        root.FlexDirection = FlexDirection.Row;
        var root_child0 = new YogaNode(config);
        root_child0.FlexBasis = YogaValue.Point(50f);
        root_child0.Height = YogaValue.Point(25f);
        root_child0.SetPadding(YogaEdge.All, YogaValue.Point(5f));
        root_child0.SetBorder(YogaEdge.All, 10f);
        root_child0.Style.BoxSizing = YogaBoxSizing.ContentBox;
        root.InsertChild(root_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(80f, root_child0.LayoutWidth);
        YogaAssert.Equal(55f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(20f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(80f, root_child0.LayoutWidth);
        YogaAssert.Equal(55f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Box_Sizing_Border_Box_Flex_Basis_Row()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        root.FlexDirection = FlexDirection.Row;
        var root_child0 = new YogaNode(config);
        root_child0.FlexBasis = YogaValue.Point(50f);
        root_child0.Height = YogaValue.Point(25f);
        root_child0.SetPadding(YogaEdge.All, YogaValue.Point(5f));
        root_child0.SetBorder(YogaEdge.All, 10f);
        root.InsertChild(root_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(30f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(50f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(30f, root_child0.LayoutHeight);
    }

    [TestMethod]
    [Ignore("Skipped in upstream Yoga (GTEST_SKIP)")]
    public void Box_Sizing_Content_Box_Flex_Basis_Column()
    {
        // TODO: GTEST_SKIP();
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        var root_child0 = new YogaNode(config);
        root_child0.FlexBasis = YogaValue.Point(50f);
        root_child0.Height = YogaValue.Point(25f);
        root_child0.SetPadding(YogaEdge.All, YogaValue.Point(5f));
        root_child0.SetBorder(YogaEdge.All, 10f);
        root_child0.Style.BoxSizing = YogaBoxSizing.ContentBox;
        root.InsertChild(root_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(100f, root_child0.LayoutWidth);
        YogaAssert.Equal(80f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(100f, root_child0.LayoutWidth);
        YogaAssert.Equal(80f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Box_Sizing_Border_Box_Flex_Basis_Column()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        var root_child0 = new YogaNode(config);
        root_child0.FlexBasis = YogaValue.Point(50f);
        root_child0.Height = YogaValue.Point(25f);
        root_child0.SetPadding(YogaEdge.All, YogaValue.Point(5f));
        root_child0.SetBorder(YogaEdge.All, 10f);
        root.InsertChild(root_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(100f, root_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(100f, root_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Box_Sizing_Content_Box_Padding_Start()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        root.SetPadding(YogaEdge.Start, YogaValue.Point(5f));
        root.Style.BoxSizing = YogaBoxSizing.ContentBox;
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(105f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(105f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
    }

    [TestMethod]
    public void Box_Sizing_Border_Box_Padding_Start()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        root.SetPadding(YogaEdge.Start, YogaValue.Point(5f));
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
    }

    [TestMethod]
    public void Box_Sizing_Content_Box_Padding_End()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        root.SetPadding(YogaEdge.End, YogaValue.Point(5f));
        root.Style.BoxSizing = YogaBoxSizing.ContentBox;
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(105f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(105f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
    }

    [TestMethod]
    public void Box_Sizing_Border_Box_Padding_End()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        root.SetPadding(YogaEdge.End, YogaValue.Point(5f));
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
    }

    [TestMethod]
    public void Box_Sizing_Content_Box_Border_Start()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        root.SetBorder(YogaEdge.Start, 5f);
        root.Style.BoxSizing = YogaBoxSizing.ContentBox;
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(105f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(105f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
    }

    [TestMethod]
    public void Box_Sizing_Border_Box_Border_Start()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        root.SetBorder(YogaEdge.Start, 5f);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
    }

    [TestMethod]
    public void Box_Sizing_Content_Box_Border_End()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        root.SetBorder(YogaEdge.End, 5f);
        root.Style.BoxSizing = YogaBoxSizing.ContentBox;
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(105f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(105f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
    }

    [TestMethod]
    public void Box_Sizing_Border_Box_Border_End()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        root.SetBorder(YogaEdge.End, 5f);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
    }

}
