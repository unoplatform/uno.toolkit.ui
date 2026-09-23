// Vendored from microsoft/microsoft-ui-reactor @ v0.1.0-preview.13 (c9191b97c40a2e4d6bcbc72df7714184862b4d36), imported 2026-09-04.
// Source: tests/Reactor.Tests/YogaGenerated/YogaAbsolutePositionTest.cs -- xUnit->MSTest; DO NOT HAND-EDIT; see Tests/Yoga/README.md.
// SPDX-License-Identifier: MIT -- (c) Microsoft Corporation (C# port); (c) Facebook, Inc. and its affiliates (Yoga).
// Full license text: THIRD-PARTY-NOTICES.md

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Toolkit.UI;
using Uno.Toolkit.UI.Yoga;

namespace Uno.Toolkit.RuntimeTests.Tests.Yoga;

/// <summary>
/// Ported from yoga/tests/generated/YGAbsolutePositionTest.cpp
/// </summary>
[TestClass]
public class YogaAbsolutePositionTest
{
    [TestMethod]
    public void Absolute_Layout_Width_Height_Start_Top()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Point(10f);
        root_child0.Height = YogaValue.Point(10f);
        root_child0.PositionType = FlexPositionType.Absolute;
        root_child0.SetPosition(YogaEdge.Start, YogaValue.Point(10f));
        root_child0.SetPosition(YogaEdge.Top, YogaValue.Point(10f));
        root.InsertChild(root_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(10f, root_child0.LayoutX);
        YogaAssert.Equal(10f, root_child0.LayoutY);
        YogaAssert.Equal(10f, root_child0.LayoutWidth);
        YogaAssert.Equal(10f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(80f, root_child0.LayoutX);
        YogaAssert.Equal(10f, root_child0.LayoutY);
        YogaAssert.Equal(10f, root_child0.LayoutWidth);
        YogaAssert.Equal(10f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Absolute_Layout_Width_Height_Left_Auto_Right()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Point(10f);
        root_child0.Height = YogaValue.Point(10f);
        root_child0.PositionType = FlexPositionType.Absolute;
        root_child0.SetPosition(YogaEdge.Left, YogaValue.Auto);
        root_child0.SetPosition(YogaEdge.Right, YogaValue.Point(10f));
        root.InsertChild(root_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(80f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(10f, root_child0.LayoutWidth);
        YogaAssert.Equal(10f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(80f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(10f, root_child0.LayoutWidth);
        YogaAssert.Equal(10f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Absolute_Layout_Width_Height_Left_Right_Auto()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Point(10f);
        root_child0.Height = YogaValue.Point(10f);
        root_child0.PositionType = FlexPositionType.Absolute;
        root_child0.SetPosition(YogaEdge.Left, YogaValue.Point(10f));
        root_child0.SetPosition(YogaEdge.Right, YogaValue.Auto);
        root.InsertChild(root_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(10f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(10f, root_child0.LayoutWidth);
        YogaAssert.Equal(10f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(10f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(10f, root_child0.LayoutWidth);
        YogaAssert.Equal(10f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Absolute_Layout_Width_Height_Left_Auto_Right_Auto()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Point(10f);
        root_child0.Height = YogaValue.Point(10f);
        root_child0.PositionType = FlexPositionType.Absolute;
        root_child0.SetPosition(YogaEdge.Left, YogaValue.Auto);
        root_child0.SetPosition(YogaEdge.Right, YogaValue.Auto);
        root.InsertChild(root_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(10f, root_child0.LayoutWidth);
        YogaAssert.Equal(10f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(90f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(10f, root_child0.LayoutWidth);
        YogaAssert.Equal(10f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Absolute_Layout_Width_Height_End_Bottom()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Point(10f);
        root_child0.Height = YogaValue.Point(10f);
        root_child0.PositionType = FlexPositionType.Absolute;
        root_child0.SetPosition(YogaEdge.End, YogaValue.Point(10f));
        root_child0.SetPosition(YogaEdge.Bottom, YogaValue.Point(10f));
        root.InsertChild(root_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(80f, root_child0.LayoutX);
        YogaAssert.Equal(80f, root_child0.LayoutY);
        YogaAssert.Equal(10f, root_child0.LayoutWidth);
        YogaAssert.Equal(10f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(10f, root_child0.LayoutX);
        YogaAssert.Equal(80f, root_child0.LayoutY);
        YogaAssert.Equal(10f, root_child0.LayoutWidth);
        YogaAssert.Equal(10f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Absolute_Layout_Start_Top_End_Bottom()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        var root_child0 = new YogaNode(config);
        root_child0.PositionType = FlexPositionType.Absolute;
        root_child0.SetPosition(YogaEdge.Start, YogaValue.Point(10f));
        root_child0.SetPosition(YogaEdge.Top, YogaValue.Point(10f));
        root_child0.SetPosition(YogaEdge.End, YogaValue.Point(10f));
        root_child0.SetPosition(YogaEdge.Bottom, YogaValue.Point(10f));
        root.InsertChild(root_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(10f, root_child0.LayoutX);
        YogaAssert.Equal(10f, root_child0.LayoutY);
        YogaAssert.Equal(80f, root_child0.LayoutWidth);
        YogaAssert.Equal(80f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(10f, root_child0.LayoutX);
        YogaAssert.Equal(10f, root_child0.LayoutY);
        YogaAssert.Equal(80f, root_child0.LayoutWidth);
        YogaAssert.Equal(80f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Absolute_Layout_Width_Height_Start_Top_End_Bottom()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Point(10f);
        root_child0.Height = YogaValue.Point(10f);
        root_child0.PositionType = FlexPositionType.Absolute;
        root_child0.SetPosition(YogaEdge.Start, YogaValue.Point(10f));
        root_child0.SetPosition(YogaEdge.Top, YogaValue.Point(10f));
        root_child0.SetPosition(YogaEdge.End, YogaValue.Point(10f));
        root_child0.SetPosition(YogaEdge.Bottom, YogaValue.Point(10f));
        root.InsertChild(root_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(10f, root_child0.LayoutX);
        YogaAssert.Equal(10f, root_child0.LayoutY);
        YogaAssert.Equal(10f, root_child0.LayoutWidth);
        YogaAssert.Equal(10f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(80f, root_child0.LayoutX);
        YogaAssert.Equal(10f, root_child0.LayoutY);
        YogaAssert.Equal(10f, root_child0.LayoutWidth);
        YogaAssert.Equal(10f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Do_Not_Clamp_Height_Of_Absolute_Node_To_Height_Of_Its_Overflow_Hidden_Parent()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Height = YogaValue.Point(50f);
        root.Width = YogaValue.Point(50f);
        root.Overflow = YogaOverflow.Hidden;
        root.FlexDirection = FlexDirection.Row;
        var root_child0 = new YogaNode(config);
        root_child0.PositionType = FlexPositionType.Absolute;
        root_child0.SetPosition(YogaEdge.Start, YogaValue.Point(0f));
        root_child0.SetPosition(YogaEdge.Top, YogaValue.Point(0f));
        root.InsertChild(root_child0, 0);
        var root_child0_child0 = new YogaNode(config);
        root_child0_child0.Width = YogaValue.Point(100f);
        root_child0_child0.Height = YogaValue.Point(100f);
        root_child0.InsertChild(root_child0_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(50f, root.LayoutWidth);
        YogaAssert.Equal(50f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(100f, root_child0.LayoutWidth);
        YogaAssert.Equal(100f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(100f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(100f, root_child0_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(50f, root.LayoutWidth);
        YogaAssert.Equal(50f, root.LayoutHeight);
        YogaAssert.Equal(-50f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(100f, root_child0.LayoutWidth);
        YogaAssert.Equal(100f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(100f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(100f, root_child0_child0.LayoutHeight);
    }

    [TestMethod]
    public void Absolute_Layout_Within_Border()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Height = YogaValue.Point(100f);
        root.Width = YogaValue.Point(100f);
        root.SetBorder(YogaEdge.All, 10f);
        root.SetMargin(YogaEdge.All, YogaValue.Point(10f));
        root.SetPadding(YogaEdge.All, YogaValue.Point(10f));
        var root_child0 = new YogaNode(config);
        root_child0.PositionType = FlexPositionType.Absolute;
        root_child0.Width = YogaValue.Point(50f);
        root_child0.Height = YogaValue.Point(50f);
        root_child0.SetPosition(YogaEdge.Left, YogaValue.Point(0f));
        root_child0.SetPosition(YogaEdge.Top, YogaValue.Point(0f));
        root.InsertChild(root_child0, 0);
        var root_child1 = new YogaNode(config);
        root_child1.PositionType = FlexPositionType.Absolute;
        root_child1.Width = YogaValue.Point(50f);
        root_child1.Height = YogaValue.Point(50f);
        root_child1.SetPosition(YogaEdge.Right, YogaValue.Point(0f));
        root_child1.SetPosition(YogaEdge.Bottom, YogaValue.Point(0f));
        root.InsertChild(root_child1, 1);
        var root_child2 = new YogaNode(config);
        root_child2.PositionType = FlexPositionType.Absolute;
        root_child2.Width = YogaValue.Point(50f);
        root_child2.Height = YogaValue.Point(50f);
        root_child2.SetPosition(YogaEdge.Left, YogaValue.Point(0f));
        root_child2.SetPosition(YogaEdge.Top, YogaValue.Point(0f));
        root_child2.SetMargin(YogaEdge.All, YogaValue.Point(10f));
        root.InsertChild(root_child2, 2);
        var root_child3 = new YogaNode(config);
        root_child3.PositionType = FlexPositionType.Absolute;
        root_child3.Width = YogaValue.Point(50f);
        root_child3.Height = YogaValue.Point(50f);
        root_child3.SetPosition(YogaEdge.Right, YogaValue.Point(0f));
        root_child3.SetPosition(YogaEdge.Bottom, YogaValue.Point(0f));
        root_child3.SetMargin(YogaEdge.All, YogaValue.Point(10f));
        root.InsertChild(root_child3, 3);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(10f, root.LayoutX);
        YogaAssert.Equal(10f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(10f, root_child0.LayoutX);
        YogaAssert.Equal(10f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0.LayoutHeight);
        YogaAssert.Equal(40f, root_child1.LayoutX);
        YogaAssert.Equal(40f, root_child1.LayoutY);
        YogaAssert.Equal(50f, root_child1.LayoutWidth);
        YogaAssert.Equal(50f, root_child1.LayoutHeight);
        YogaAssert.Equal(20f, root_child2.LayoutX);
        YogaAssert.Equal(20f, root_child2.LayoutY);
        YogaAssert.Equal(50f, root_child2.LayoutWidth);
        YogaAssert.Equal(50f, root_child2.LayoutHeight);
        YogaAssert.Equal(30f, root_child3.LayoutX);
        YogaAssert.Equal(30f, root_child3.LayoutY);
        YogaAssert.Equal(50f, root_child3.LayoutWidth);
        YogaAssert.Equal(50f, root_child3.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(10f, root.LayoutX);
        YogaAssert.Equal(10f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(10f, root_child0.LayoutX);
        YogaAssert.Equal(10f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0.LayoutHeight);
        YogaAssert.Equal(40f, root_child1.LayoutX);
        YogaAssert.Equal(40f, root_child1.LayoutY);
        YogaAssert.Equal(50f, root_child1.LayoutWidth);
        YogaAssert.Equal(50f, root_child1.LayoutHeight);
        YogaAssert.Equal(20f, root_child2.LayoutX);
        YogaAssert.Equal(20f, root_child2.LayoutY);
        YogaAssert.Equal(50f, root_child2.LayoutWidth);
        YogaAssert.Equal(50f, root_child2.LayoutHeight);
        YogaAssert.Equal(30f, root_child3.LayoutX);
        YogaAssert.Equal(30f, root_child3.LayoutY);
        YogaAssert.Equal(50f, root_child3.LayoutWidth);
        YogaAssert.Equal(50f, root_child3.LayoutHeight);
    }

    [TestMethod]
    public void Absolute_Layout_Align_Items_And_Justify_Content_Center()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Height = YogaValue.Point(100f);
        root.Width = YogaValue.Point(110f);
        root.FlexGrow = 1f;
        root.AlignItems = FlexAlign.Center;
        root.JustifyContent = FlexJustify.Center;
        var root_child0 = new YogaNode(config);
        root_child0.PositionType = FlexPositionType.Absolute;
        root_child0.Width = YogaValue.Point(60f);
        root_child0.Height = YogaValue.Point(40f);
        root.InsertChild(root_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(110f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(25f, root_child0.LayoutX);
        YogaAssert.Equal(30f, root_child0.LayoutY);
        YogaAssert.Equal(60f, root_child0.LayoutWidth);
        YogaAssert.Equal(40f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(110f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(25f, root_child0.LayoutX);
        YogaAssert.Equal(30f, root_child0.LayoutY);
        YogaAssert.Equal(60f, root_child0.LayoutWidth);
        YogaAssert.Equal(40f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Absolute_Layout_Align_Items_And_Justify_Content_Flex_End()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Height = YogaValue.Point(100f);
        root.Width = YogaValue.Point(110f);
        root.FlexGrow = 1f;
        root.AlignItems = FlexAlign.FlexEnd;
        root.JustifyContent = FlexJustify.FlexEnd;
        var root_child0 = new YogaNode(config);
        root_child0.PositionType = FlexPositionType.Absolute;
        root_child0.Width = YogaValue.Point(60f);
        root_child0.Height = YogaValue.Point(40f);
        root.InsertChild(root_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(110f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(50f, root_child0.LayoutX);
        YogaAssert.Equal(60f, root_child0.LayoutY);
        YogaAssert.Equal(60f, root_child0.LayoutWidth);
        YogaAssert.Equal(40f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(110f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(60f, root_child0.LayoutY);
        YogaAssert.Equal(60f, root_child0.LayoutWidth);
        YogaAssert.Equal(40f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Absolute_Layout_Justify_Content_Center()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Height = YogaValue.Point(100f);
        root.Width = YogaValue.Point(110f);
        root.FlexGrow = 1f;
        root.JustifyContent = FlexJustify.Center;
        var root_child0 = new YogaNode(config);
        root_child0.PositionType = FlexPositionType.Absolute;
        root_child0.Width = YogaValue.Point(60f);
        root_child0.Height = YogaValue.Point(40f);
        root.InsertChild(root_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(110f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(30f, root_child0.LayoutY);
        YogaAssert.Equal(60f, root_child0.LayoutWidth);
        YogaAssert.Equal(40f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(110f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(50f, root_child0.LayoutX);
        YogaAssert.Equal(30f, root_child0.LayoutY);
        YogaAssert.Equal(60f, root_child0.LayoutWidth);
        YogaAssert.Equal(40f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Absolute_Layout_Align_Items_Center()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Height = YogaValue.Point(100f);
        root.Width = YogaValue.Point(110f);
        root.FlexGrow = 1f;
        root.AlignItems = FlexAlign.Center;
        var root_child0 = new YogaNode(config);
        root_child0.PositionType = FlexPositionType.Absolute;
        root_child0.Width = YogaValue.Point(60f);
        root_child0.Height = YogaValue.Point(40f);
        root.InsertChild(root_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(110f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(25f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(60f, root_child0.LayoutWidth);
        YogaAssert.Equal(40f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(110f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(25f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(60f, root_child0.LayoutWidth);
        YogaAssert.Equal(40f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Absolute_Layout_Align_Items_Center_On_Child_Only()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Height = YogaValue.Point(100f);
        root.Width = YogaValue.Point(110f);
        root.FlexGrow = 1f;
        var root_child0 = new YogaNode(config);
        root_child0.PositionType = FlexPositionType.Absolute;
        root_child0.Width = YogaValue.Point(60f);
        root_child0.Height = YogaValue.Point(40f);
        root_child0.AlignSelf = FlexAlign.Center;
        root.InsertChild(root_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(110f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(25f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(60f, root_child0.LayoutWidth);
        YogaAssert.Equal(40f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(110f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(25f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(60f, root_child0.LayoutWidth);
        YogaAssert.Equal(40f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Absolute_Layout_Align_Items_And_Justify_Content_Center_And_Top_Position()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Height = YogaValue.Point(100f);
        root.Width = YogaValue.Point(110f);
        root.FlexGrow = 1f;
        root.AlignItems = FlexAlign.Center;
        root.JustifyContent = FlexJustify.Center;
        var root_child0 = new YogaNode(config);
        root_child0.PositionType = FlexPositionType.Absolute;
        root_child0.Width = YogaValue.Point(60f);
        root_child0.Height = YogaValue.Point(40f);
        root_child0.SetPosition(YogaEdge.Top, YogaValue.Point(10f));
        root.InsertChild(root_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(110f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(25f, root_child0.LayoutX);
        YogaAssert.Equal(10f, root_child0.LayoutY);
        YogaAssert.Equal(60f, root_child0.LayoutWidth);
        YogaAssert.Equal(40f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(110f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(25f, root_child0.LayoutX);
        YogaAssert.Equal(10f, root_child0.LayoutY);
        YogaAssert.Equal(60f, root_child0.LayoutWidth);
        YogaAssert.Equal(40f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Absolute_Layout_Align_Items_And_Justify_Content_Center_And_Bottom_Position()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Height = YogaValue.Point(100f);
        root.Width = YogaValue.Point(110f);
        root.FlexGrow = 1f;
        root.AlignItems = FlexAlign.Center;
        root.JustifyContent = FlexJustify.Center;
        var root_child0 = new YogaNode(config);
        root_child0.PositionType = FlexPositionType.Absolute;
        root_child0.Width = YogaValue.Point(60f);
        root_child0.Height = YogaValue.Point(40f);
        root_child0.SetPosition(YogaEdge.Bottom, YogaValue.Point(10f));
        root.InsertChild(root_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(110f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(25f, root_child0.LayoutX);
        YogaAssert.Equal(50f, root_child0.LayoutY);
        YogaAssert.Equal(60f, root_child0.LayoutWidth);
        YogaAssert.Equal(40f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(110f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(25f, root_child0.LayoutX);
        YogaAssert.Equal(50f, root_child0.LayoutY);
        YogaAssert.Equal(60f, root_child0.LayoutWidth);
        YogaAssert.Equal(40f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Absolute_Layout_Align_Items_And_Justify_Content_Center_And_Left_Position()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Height = YogaValue.Point(100f);
        root.Width = YogaValue.Point(110f);
        root.FlexGrow = 1f;
        root.AlignItems = FlexAlign.Center;
        root.JustifyContent = FlexJustify.Center;
        var root_child0 = new YogaNode(config);
        root_child0.PositionType = FlexPositionType.Absolute;
        root_child0.Width = YogaValue.Point(60f);
        root_child0.Height = YogaValue.Point(40f);
        root_child0.SetPosition(YogaEdge.Left, YogaValue.Point(5f));
        root.InsertChild(root_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(110f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(5f, root_child0.LayoutX);
        YogaAssert.Equal(30f, root_child0.LayoutY);
        YogaAssert.Equal(60f, root_child0.LayoutWidth);
        YogaAssert.Equal(40f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(110f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(5f, root_child0.LayoutX);
        YogaAssert.Equal(30f, root_child0.LayoutY);
        YogaAssert.Equal(60f, root_child0.LayoutWidth);
        YogaAssert.Equal(40f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Absolute_Layout_Align_Items_And_Justify_Content_Center_And_Right_Position()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Height = YogaValue.Point(100f);
        root.Width = YogaValue.Point(110f);
        root.FlexGrow = 1f;
        root.AlignItems = FlexAlign.Center;
        root.JustifyContent = FlexJustify.Center;
        var root_child0 = new YogaNode(config);
        root_child0.PositionType = FlexPositionType.Absolute;
        root_child0.Width = YogaValue.Point(60f);
        root_child0.Height = YogaValue.Point(40f);
        root_child0.SetPosition(YogaEdge.Right, YogaValue.Point(5f));
        root.InsertChild(root_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(110f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(45f, root_child0.LayoutX);
        YogaAssert.Equal(30f, root_child0.LayoutY);
        YogaAssert.Equal(60f, root_child0.LayoutWidth);
        YogaAssert.Equal(40f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(110f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(45f, root_child0.LayoutX);
        YogaAssert.Equal(30f, root_child0.LayoutY);
        YogaAssert.Equal(60f, root_child0.LayoutWidth);
        YogaAssert.Equal(40f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Position_Root_With_Rtl_Should_Position_Withoutdirection()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Height = YogaValue.Point(52f);
        root.Width = YogaValue.Point(52f);
        root.SetPosition(YogaEdge.Left, YogaValue.Point(72f));
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(72f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(52f, root.LayoutWidth);
        YogaAssert.Equal(52f, root.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(72f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(52f, root.LayoutWidth);
        YogaAssert.Equal(52f, root.LayoutHeight);
    }

    [TestMethod]
    public void Absolute_Layout_Percentage_Bottom_Based_On_Parent_Height()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(200f);
        var root_child0 = new YogaNode(config);
        root_child0.PositionType = FlexPositionType.Absolute;
        root_child0.SetPosition(YogaEdge.Top, YogaValue.Percent(50f));
        root_child0.Width = YogaValue.Point(10f);
        root_child0.Height = YogaValue.Point(10f);
        root.InsertChild(root_child0, 0);
        var root_child1 = new YogaNode(config);
        root_child1.PositionType = FlexPositionType.Absolute;
        root_child1.SetPosition(YogaEdge.Bottom, YogaValue.Percent(50f));
        root_child1.Width = YogaValue.Point(10f);
        root_child1.Height = YogaValue.Point(10f);
        root.InsertChild(root_child1, 1);
        var root_child2 = new YogaNode(config);
        root_child2.PositionType = FlexPositionType.Absolute;
        root_child2.SetPosition(YogaEdge.Top, YogaValue.Percent(10f));
        root_child2.Width = YogaValue.Point(10f);
        root_child2.SetPosition(YogaEdge.Bottom, YogaValue.Percent(10f));
        root.InsertChild(root_child2, 2);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(200f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(100f, root_child0.LayoutY);
        YogaAssert.Equal(10f, root_child0.LayoutWidth);
        YogaAssert.Equal(10f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child1.LayoutX);
        YogaAssert.Equal(90f, root_child1.LayoutY);
        YogaAssert.Equal(10f, root_child1.LayoutWidth);
        YogaAssert.Equal(10f, root_child1.LayoutHeight);
        YogaAssert.Equal(0f, root_child2.LayoutX);
        YogaAssert.Equal(20f, root_child2.LayoutY);
        YogaAssert.Equal(10f, root_child2.LayoutWidth);
        YogaAssert.Equal(160f, root_child2.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(200f, root.LayoutHeight);
        YogaAssert.Equal(90f, root_child0.LayoutX);
        YogaAssert.Equal(100f, root_child0.LayoutY);
        YogaAssert.Equal(10f, root_child0.LayoutWidth);
        YogaAssert.Equal(10f, root_child0.LayoutHeight);
        YogaAssert.Equal(90f, root_child1.LayoutX);
        YogaAssert.Equal(90f, root_child1.LayoutY);
        YogaAssert.Equal(10f, root_child1.LayoutWidth);
        YogaAssert.Equal(10f, root_child1.LayoutHeight);
        YogaAssert.Equal(90f, root_child2.LayoutX);
        YogaAssert.Equal(20f, root_child2.LayoutY);
        YogaAssert.Equal(10f, root_child2.LayoutWidth);
        YogaAssert.Equal(160f, root_child2.LayoutHeight);
    }

    [TestMethod]
    public void Absolute_Layout_In_Wrap_Reverse_Column_Container()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        root.FlexWrap = FlexWrap.WrapReverse;
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Point(20f);
        root_child0.Height = YogaValue.Point(20f);
        root_child0.PositionType = FlexPositionType.Absolute;
        root.InsertChild(root_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(80f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(20f, root_child0.LayoutWidth);
        YogaAssert.Equal(20f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(20f, root_child0.LayoutWidth);
        YogaAssert.Equal(20f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Absolute_Layout_In_Wrap_Reverse_Row_Container()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.FlexDirection = FlexDirection.Row;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        root.FlexWrap = FlexWrap.WrapReverse;
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Point(20f);
        root_child0.Height = YogaValue.Point(20f);
        root_child0.PositionType = FlexPositionType.Absolute;
        root.InsertChild(root_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(80f, root_child0.LayoutY);
        YogaAssert.Equal(20f, root_child0.LayoutWidth);
        YogaAssert.Equal(20f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(80f, root_child0.LayoutX);
        YogaAssert.Equal(80f, root_child0.LayoutY);
        YogaAssert.Equal(20f, root_child0.LayoutWidth);
        YogaAssert.Equal(20f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Absolute_Layout_In_Wrap_Reverse_Column_Container_Flex_End()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        root.FlexWrap = FlexWrap.WrapReverse;
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Point(20f);
        root_child0.Height = YogaValue.Point(20f);
        root_child0.PositionType = FlexPositionType.Absolute;
        root_child0.AlignSelf = FlexAlign.FlexEnd;
        root.InsertChild(root_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(20f, root_child0.LayoutWidth);
        YogaAssert.Equal(20f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(80f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(20f, root_child0.LayoutWidth);
        YogaAssert.Equal(20f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Absolute_Layout_In_Wrap_Reverse_Row_Container_Flex_End()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.FlexDirection = FlexDirection.Row;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        root.FlexWrap = FlexWrap.WrapReverse;
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Point(20f);
        root_child0.Height = YogaValue.Point(20f);
        root_child0.PositionType = FlexPositionType.Absolute;
        root_child0.AlignSelf = FlexAlign.FlexEnd;
        root.InsertChild(root_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(20f, root_child0.LayoutWidth);
        YogaAssert.Equal(20f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(80f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(20f, root_child0.LayoutWidth);
        YogaAssert.Equal(20f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Percent_Absolute_Position_Infinite_Height()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(300f);
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Point(300f);
        root.InsertChild(root_child0, 0);
        var root_child1 = new YogaNode(config);
        root_child1.Width = YogaValue.Percent(20f);
        root_child1.Height = YogaValue.Percent(20f);
        root_child1.SetPosition(YogaEdge.Left, YogaValue.Percent(20f));
        root_child1.SetPosition(YogaEdge.Top, YogaValue.Percent(20f));
        root_child1.PositionType = FlexPositionType.Absolute;
        root.InsertChild(root_child1, 1);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(300f, root.LayoutWidth);
        YogaAssert.Equal(0f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(300f, root_child0.LayoutWidth);
        YogaAssert.Equal(0f, root_child0.LayoutHeight);
        YogaAssert.Equal(60f, root_child1.LayoutX);
        YogaAssert.Equal(0f, root_child1.LayoutY);
        YogaAssert.Equal(60f, root_child1.LayoutWidth);
        YogaAssert.Equal(0f, root_child1.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(300f, root.LayoutWidth);
        YogaAssert.Equal(0f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(300f, root_child0.LayoutWidth);
        YogaAssert.Equal(0f, root_child0.LayoutHeight);
        YogaAssert.Equal(60f, root_child1.LayoutX);
        YogaAssert.Equal(0f, root_child1.LayoutY);
        YogaAssert.Equal(60f, root_child1.LayoutWidth);
        YogaAssert.Equal(0f, root_child1.LayoutHeight);
    }

    [TestMethod]
    public void Absolute_Layout_Percentage_Height_Based_On_Padded_Parent()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        root.SetPadding(YogaEdge.Top, YogaValue.Point(10f));
        root.SetBorder(YogaEdge.Top, 10f);
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Point(100f);
        root_child0.Height = YogaValue.Percent(50f);
        root_child0.PositionType = FlexPositionType.Absolute;
        root.InsertChild(root_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(20f, root_child0.LayoutY);
        YogaAssert.Equal(100f, root_child0.LayoutWidth);
        YogaAssert.Equal(45f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(20f, root_child0.LayoutY);
        YogaAssert.Equal(100f, root_child0.LayoutWidth);
        YogaAssert.Equal(45f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Absolute_Layout_Percentage_Height_Based_On_Padded_Parent_And_Align_Items_Center()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.AlignItems = FlexAlign.Center;
        root.JustifyContent = FlexJustify.Center;
        root.Width = YogaValue.Point(100f);
        root.Height = YogaValue.Point(100f);
        root.SetPadding(YogaEdge.Top, YogaValue.Point(20f));
        root.SetPadding(YogaEdge.Bottom, YogaValue.Point(20f));
        var root_child0 = new YogaNode(config);
        root_child0.PositionType = FlexPositionType.Absolute;
        root_child0.Width = YogaValue.Point(100f);
        root_child0.Height = YogaValue.Percent(50f);
        root.InsertChild(root_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(25f, root_child0.LayoutY);
        YogaAssert.Equal(100f, root_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(25f, root_child0.LayoutY);
        YogaAssert.Equal(100f, root_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Absolute_Layout_Padding_Left()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(200f);
        root.Height = YogaValue.Point(200f);
        root.SetPadding(YogaEdge.Left, YogaValue.Point(100f));
        var root_child0 = new YogaNode(config);
        root_child0.PositionType = FlexPositionType.Absolute;
        root_child0.Width = YogaValue.Point(50f);
        root_child0.Height = YogaValue.Point(50f);
        root.InsertChild(root_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(200f, root.LayoutWidth);
        YogaAssert.Equal(200f, root.LayoutHeight);
        YogaAssert.Equal(100f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(200f, root.LayoutWidth);
        YogaAssert.Equal(200f, root.LayoutHeight);
        YogaAssert.Equal(150f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Absolute_Layout_Padding_Right()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(200f);
        root.Height = YogaValue.Point(200f);
        root.SetPadding(YogaEdge.Right, YogaValue.Point(100f));
        var root_child0 = new YogaNode(config);
        root_child0.PositionType = FlexPositionType.Absolute;
        root_child0.Width = YogaValue.Point(50f);
        root_child0.Height = YogaValue.Point(50f);
        root.InsertChild(root_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(200f, root.LayoutWidth);
        YogaAssert.Equal(200f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(200f, root.LayoutWidth);
        YogaAssert.Equal(200f, root.LayoutHeight);
        YogaAssert.Equal(50f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Absolute_Layout_Padding_Top()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(200f);
        root.Height = YogaValue.Point(200f);
        root.SetPadding(YogaEdge.Top, YogaValue.Point(100f));
        var root_child0 = new YogaNode(config);
        root_child0.PositionType = FlexPositionType.Absolute;
        root_child0.Width = YogaValue.Point(50f);
        root_child0.Height = YogaValue.Point(50f);
        root.InsertChild(root_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(200f, root.LayoutWidth);
        YogaAssert.Equal(200f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(100f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(200f, root.LayoutWidth);
        YogaAssert.Equal(200f, root.LayoutHeight);
        YogaAssert.Equal(150f, root_child0.LayoutX);
        YogaAssert.Equal(100f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Absolute_Layout_Padding_Bottom()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(200f);
        root.Height = YogaValue.Point(200f);
        root.SetPadding(YogaEdge.Bottom, YogaValue.Point(100f));
        var root_child0 = new YogaNode(config);
        root_child0.PositionType = FlexPositionType.Absolute;
        root_child0.Width = YogaValue.Point(50f);
        root_child0.Height = YogaValue.Point(50f);
        root.InsertChild(root_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(200f, root.LayoutWidth);
        YogaAssert.Equal(200f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(200f, root.LayoutWidth);
        YogaAssert.Equal(200f, root.LayoutHeight);
        YogaAssert.Equal(150f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Absolute_Layout_Padding()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Point(200f);
        root_child0.Height = YogaValue.Point(200f);
        root_child0.SetMargin(YogaEdge.All, YogaValue.Point(10f));
        root.InsertChild(root_child0, 0);
        var root_child0_child0 = new YogaNode(config);
        root_child0_child0.PositionType = FlexPositionType.Static;
        root_child0_child0.Width = YogaValue.Point(200f);
        root_child0_child0.Height = YogaValue.Point(200f);
        root_child0_child0.SetPadding(YogaEdge.All, YogaValue.Point(50f));
        root_child0.InsertChild(root_child0_child0, 0);
        var root_child0_child0_child0 = new YogaNode(config);
        root_child0_child0_child0.PositionType = FlexPositionType.Absolute;
        root_child0_child0_child0.Width = YogaValue.Point(50f);
        root_child0_child0_child0.Height = YogaValue.Point(50f);
        root_child0_child0.InsertChild(root_child0_child0_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(220f, root.LayoutWidth);
        YogaAssert.Equal(220f, root.LayoutHeight);
        YogaAssert.Equal(10f, root_child0.LayoutX);
        YogaAssert.Equal(10f, root_child0.LayoutY);
        YogaAssert.Equal(200f, root_child0.LayoutWidth);
        YogaAssert.Equal(200f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(200f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(200f, root_child0_child0.LayoutHeight);
        YogaAssert.Equal(50f, root_child0_child0_child0.LayoutX);
        YogaAssert.Equal(50f, root_child0_child0_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0_child0_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child0_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(220f, root.LayoutWidth);
        YogaAssert.Equal(220f, root.LayoutHeight);
        YogaAssert.Equal(10f, root_child0.LayoutX);
        YogaAssert.Equal(10f, root_child0.LayoutY);
        YogaAssert.Equal(200f, root_child0.LayoutWidth);
        YogaAssert.Equal(200f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(200f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(200f, root_child0_child0.LayoutHeight);
        YogaAssert.Equal(100f, root_child0_child0_child0.LayoutX);
        YogaAssert.Equal(50f, root_child0_child0_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0_child0_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child0_child0.LayoutHeight);
    }

    [TestMethod]
    public void Absolute_Layout_Border()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Point(200f);
        root_child0.Height = YogaValue.Point(200f);
        root_child0.SetMargin(YogaEdge.All, YogaValue.Point(10f));
        root.InsertChild(root_child0, 0);
        var root_child0_child0 = new YogaNode(config);
        root_child0_child0.PositionType = FlexPositionType.Static;
        root_child0_child0.Width = YogaValue.Point(200f);
        root_child0_child0.Height = YogaValue.Point(200f);
        root_child0_child0.SetBorder(YogaEdge.All, 10f);
        root_child0.InsertChild(root_child0_child0, 0);
        var root_child0_child0_child0 = new YogaNode(config);
        root_child0_child0_child0.PositionType = FlexPositionType.Absolute;
        root_child0_child0_child0.Width = YogaValue.Point(50f);
        root_child0_child0_child0.Height = YogaValue.Point(50f);
        root_child0_child0.InsertChild(root_child0_child0_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(220f, root.LayoutWidth);
        YogaAssert.Equal(220f, root.LayoutHeight);
        YogaAssert.Equal(10f, root_child0.LayoutX);
        YogaAssert.Equal(10f, root_child0.LayoutY);
        YogaAssert.Equal(200f, root_child0.LayoutWidth);
        YogaAssert.Equal(200f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(200f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(200f, root_child0_child0.LayoutHeight);
        YogaAssert.Equal(10f, root_child0_child0_child0.LayoutX);
        YogaAssert.Equal(10f, root_child0_child0_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0_child0_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child0_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(220f, root.LayoutWidth);
        YogaAssert.Equal(220f, root.LayoutHeight);
        YogaAssert.Equal(10f, root_child0.LayoutX);
        YogaAssert.Equal(10f, root_child0.LayoutY);
        YogaAssert.Equal(200f, root_child0.LayoutWidth);
        YogaAssert.Equal(200f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(200f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(200f, root_child0_child0.LayoutHeight);
        YogaAssert.Equal(140f, root_child0_child0_child0.LayoutX);
        YogaAssert.Equal(10f, root_child0_child0_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0_child0_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child0_child0.LayoutHeight);
    }

    [TestMethod]
    public void Absolute_Layout_Column_Reverse_Margin_Border()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(200f);
        root.Height = YogaValue.Point(200f);
        root.FlexDirection = FlexDirection.ColumnReverse;
        var root_child0 = new YogaNode(config);
        root_child0.PositionType = FlexPositionType.Absolute;
        root_child0.Width = YogaValue.Point(50f);
        root_child0.Height = YogaValue.Point(50f);
        root_child0.SetPosition(YogaEdge.Left, YogaValue.Point(5f));
        root_child0.SetPosition(YogaEdge.Right, YogaValue.Point(3f));
        root_child0.SetMargin(YogaEdge.Right, YogaValue.Point(4f));
        root_child0.SetMargin(YogaEdge.Left, YogaValue.Point(3f));
        root_child0.SetBorder(YogaEdge.Right, 7f);
        root_child0.SetBorder(YogaEdge.Left, 1f);
        root.InsertChild(root_child0, 0);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(200f, root.LayoutWidth);
        YogaAssert.Equal(200f, root.LayoutHeight);
        YogaAssert.Equal(8f, root_child0.LayoutX);
        YogaAssert.Equal(150f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(200f, root.LayoutWidth);
        YogaAssert.Equal(200f, root.LayoutHeight);
        YogaAssert.Equal(143f, root_child0.LayoutX);
        YogaAssert.Equal(150f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0.LayoutHeight);
    }

}
