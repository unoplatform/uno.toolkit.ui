// Vendored from microsoft/microsoft-ui-reactor @ v0.1.0-preview.13 (c9191b97c40a2e4d6bcbc72df7714184862b4d36), imported 2026-09-04.
// Source: tests/Reactor.Tests/YogaGenerated/YogaIntrinsicSizeTest.cs -- xUnit->MSTest; DO NOT HAND-EDIT; see Tests/Yoga/README.md.
// SPDX-License-Identifier: MIT -- (c) Microsoft Corporation (C# port); (c) Facebook, Inc. and its affiliates (Yoga).
// Full license text: THIRD-PARTY-NOTICES.md

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Toolkit.UI;
using Uno.Toolkit.UI.Yoga;

namespace Uno.Toolkit.RuntimeTests.Tests.Yoga;

/// <summary>
/// Ported from yoga/tests/generated/YGIntrinsicSizeTest.cpp
/// </summary>
[TestClass]
public class YogaIntrinsicSizeTest
{
    [TestMethod]
    public void Contains_Inner_Text_Long_Word()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(2000f);
        root.Height = YogaValue.Point(2000f);
        root.AlignItems = FlexAlign.FlexStart;
        var root_child0 = new YogaNode(config);
        root_child0.FlexDirection = FlexDirection.Row;
        root.InsertChild(root_child0, 0);
        root_child0.Context = "LoremipsumdolorsitametconsecteturadipiscingelitSedeleifasdfettortoracauctorFuscerhoncusipsumtemporerosaliquamconsequatPraesentsoda";
        root_child0.MeasureFunction = IntrinsicSizeMeasureFunc;
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(2000f, root.LayoutWidth);
        YogaAssert.Equal(2000f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(1300f, root_child0.LayoutWidth);
        YogaAssert.Equal(10f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(2000f, root.LayoutWidth);
        YogaAssert.Equal(2000f, root.LayoutHeight);
        YogaAssert.Equal(700f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(1300f, root_child0.LayoutWidth);
        YogaAssert.Equal(10f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Contains_Inner_Text_No_Width_No_Height()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(2000f);
        root.Height = YogaValue.Point(2000f);
        root.AlignItems = FlexAlign.FlexStart;
        var root_child0 = new YogaNode(config);
        root_child0.FlexDirection = FlexDirection.Row;
        root.InsertChild(root_child0, 0);
        root_child0.Context = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed eleifasd et tortor ac auctor. Integer at volutpat libero, sed elementum dui interdum id. Aliquam consectetur massa vel neque aliquet, quis consequat risus fringilla. Fusce rhoncus ipsum tempor eros aliquam, vel tempus metus ullamcorper. Nam at nulla sed tellus vestibulum fringilla vel sit amet ligula. Proin velit lectus, euismod sit amet quam vel ultricies dolor, vitae finibus lorem ipsum. Pellentesque molestie at mi sit amet dictum. Donec vehicula lacinia felis sit amet consectetur. Praesent sodales enim sapien, sed varius ipsum pellentesque vel. Aenean eu mi eu justo tincidunt finibus vel sit amet ipsum. Sed bibasdum purus vel ipsum sagittis, quis fermentum dolor lobortis. Etiam vulputate eleifasd lectus vel varius. Phasellus imperdiet lectus sit amet ipsum egestas, ut bibasdum ipsum malesuada. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Sed mollis eros sit amet elit porttitor, vel venenatis turpis venenatis. Nulla tempus tortor at eros efficitur, sit amet dapibus ipsum malesuada. Ut at mauris sed nunc malesuada convallis. Duis id sem vel magna varius eleifasd vel at est. Donec eget orci a ipsum tempor lobortis. Sed at consectetur ipsum.";
        root_child0.MeasureFunction = IntrinsicSizeMeasureFunc;
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(2000f, root.LayoutWidth);
        YogaAssert.Equal(2000f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(2000f, root_child0.LayoutWidth);
        YogaAssert.Equal(70f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(2000f, root.LayoutWidth);
        YogaAssert.Equal(2000f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(2000f, root_child0.LayoutWidth);
        YogaAssert.Equal(70f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Contains_Inner_Text_No_Width_No_Height_Long_Word_In_Paragraph()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(2000f);
        root.Height = YogaValue.Point(2000f);
        root.AlignItems = FlexAlign.FlexStart;
        var root_child0 = new YogaNode(config);
        root_child0.FlexDirection = FlexDirection.Row;
        root.InsertChild(root_child0, 0);
        root_child0.Context = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed eleifasd et tortor ac auctor. Integer at volutpat libero, sed elementum dui interdum id. Aliquam consectetur massa vel neque aliquet, quis consequat risus fringilla. Fusce rhoncus ipsum tempor eros aliquam, vel tempus metus ullamcorper. Nam at nulla sed tellus vestibulum fringilla vel sit amet ligula. Proin velit lectus, euismod sit amet quam vel ultricies dolor, vitae finibus loremipsumloremipsumloremipsumloremipsumloremipsumloremipsumloremipsumloremipsumloremipsumloremipsumloremipsumloremipsumloremipsumlorem Pellentesque molestie at mi sit amet dictum. Donec vehicula lacinia felis sit amet consectetur. Praesent sodales enim sapien, sed varius ipsum pellentesque vel. Aenean eu mi eu justo tincidunt finibus vel sit amet ipsum. Sed bibasdum purus vel ipsum sagittis, quis fermentum dolor lobortis. Etiam vulputate eleifasd lectus vel varius. Phasellus imperdiet lectus sit amet ipsum egestas, ut bibasdum ipsum malesuada. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Sed mollis eros sit amet elit porttitor, vel venenatis turpis venenatis. Nulla tempus tortor at eros efficitur, sit amet dapibus ipsum malesuada. Ut at mauris sed nunc malesuada convallis. Duis id sem vel magna varius eleifasd vel at est. Donec eget orci a ipsum tempor lobortis. Sed at consectetur ipsum.";
        root_child0.MeasureFunction = IntrinsicSizeMeasureFunc;
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(2000f, root.LayoutWidth);
        YogaAssert.Equal(2000f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(2000f, root_child0.LayoutWidth);
        YogaAssert.Equal(70f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(2000f, root.LayoutWidth);
        YogaAssert.Equal(2000f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(2000f, root_child0.LayoutWidth);
        YogaAssert.Equal(70f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Contains_Inner_Text_Fixed_Width()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(2000f);
        root.Height = YogaValue.Point(2000f);
        root.AlignItems = FlexAlign.FlexStart;
        var root_child0 = new YogaNode(config);
        root_child0.FlexDirection = FlexDirection.Row;
        root_child0.Width = YogaValue.Point(100f);
        root.InsertChild(root_child0, 0);
        root_child0.Context = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed eleifasd et tortor ac auctor. Integer at volutpat libero, sed elementum dui interdum id. Aliquam consectetur massa vel neque aliquet, quis consequat risus fringilla. Fusce rhoncus ipsum tempor eros aliquam, vel tempus metus ullamcorper. Nam at nulla sed tellus vestibulum fringilla vel sit amet ligula. Proin velit lectus, euismod sit amet quam vel ultricies dolor, vitae finibus lorem ipsum. Pellentesque molestie at mi sit amet dictum. Donec vehicula lacinia felis sit amet consectetur. Praesent sodales enim sapien, sed varius ipsum pellentesque vel. Aenean eu mi eu justo tincidunt finibus vel sit amet ipsum. Sed bibasdum purus vel ipsum sagittis, quis fermentum dolor lobortis. Etiam vulputate eleifasd lectus vel varius. Phasellus imperdiet lectus sit amet ipsum egestas, ut bibasdum ipsum malesuada. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Sed mollis eros sit amet elit porttitor, vel venenatis turpis venenatis. Nulla tempus tortor at eros efficitur, sit amet dapibus ipsum malesuada. Ut at mauris sed nunc malesuada convallis. Duis id sem vel magna varius eleifasd vel at est. Donec eget orci a ipsum tempor lobortis. Sed at consectetur ipsum.";
        root_child0.MeasureFunction = IntrinsicSizeMeasureFunc;
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(2000f, root.LayoutWidth);
        YogaAssert.Equal(2000f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(100f, root_child0.LayoutWidth);
        YogaAssert.Equal(1290f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(2000f, root.LayoutWidth);
        YogaAssert.Equal(2000f, root.LayoutHeight);
        YogaAssert.Equal(1900f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(100f, root_child0.LayoutWidth);
        YogaAssert.Equal(1290f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Contains_Inner_Text_No_Width_Fixed_Height()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(2000f);
        root.Height = YogaValue.Point(2000f);
        root.AlignItems = FlexAlign.FlexStart;
        var root_child0 = new YogaNode(config);
        root_child0.FlexDirection = FlexDirection.Row;
        root_child0.Height = YogaValue.Point(20f);
        root.InsertChild(root_child0, 0);
        root_child0.Context = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed eleifasd et tortor ac auctor. Integer at volutpat libero, sed elementum dui interdum id. Aliquam consectetur massa vel neque aliquet, quis consequat risus fringilla. Fusce rhoncus ipsum tempor eros aliquam, vel tempus metus ullamcorper. Nam at nulla sed tellus vestibulum fringilla vel sit amet ligula. Proin velit lectus, euismod sit amet quam vel ultricies dolor, vitae finibus lorem ipsum. Pellentesque molestie at mi sit amet dictum. Donec vehicula lacinia felis sit amet consectetur. Praesent sodales enim sapien, sed varius ipsum pellentesque vel. Aenean eu mi eu justo tincidunt finibus vel sit amet ipsum. Sed bibasdum purus vel ipsum sagittis, quis fermentum dolor lobortis. Etiam vulputate eleifasd lectus vel varius. Phasellus imperdiet lectus sit amet ipsum egestas, ut bibasdum ipsum malesuada. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Sed mollis eros sit amet elit porttitor, vel venenatis turpis venenatis. Nulla tempus tortor at eros efficitur, sit amet dapibus ipsum malesuada. Ut at mauris sed nunc malesuada convallis. Duis id sem vel magna varius eleifasd vel at est. Donec eget orci a ipsum tempor lobortis. Sed at consectetur ipsum.";
        root_child0.MeasureFunction = IntrinsicSizeMeasureFunc;
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(2000f, root.LayoutWidth);
        YogaAssert.Equal(2000f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(2000f, root_child0.LayoutWidth);
        YogaAssert.Equal(20f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(2000f, root.LayoutWidth);
        YogaAssert.Equal(2000f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(2000f, root_child0.LayoutWidth);
        YogaAssert.Equal(20f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Contains_Inner_Text_Fixed_Width_Fixed_Height()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(2000f);
        root.Height = YogaValue.Point(2000f);
        root.AlignItems = FlexAlign.FlexStart;
        var root_child0 = new YogaNode(config);
        root_child0.FlexDirection = FlexDirection.Row;
        root_child0.Width = YogaValue.Point(50f);
        root_child0.Height = YogaValue.Point(20f);
        root.InsertChild(root_child0, 0);
        root_child0.Context = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed eleifasd et tortor ac auctor. Integer at volutpat libero, sed elementum dui interdum id. Aliquam consectetur massa vel neque aliquet, quis consequat risus fringilla. Fusce rhoncus ipsum tempor eros aliquam, vel tempus metus ullamcorper. Nam at nulla sed tellus vestibulum fringilla vel sit amet ligula. Proin velit lectus, euismod sit amet quam vel ultricies dolor, vitae finibus lorem ipsum. Pellentesque molestie at mi sit amet dictum. Donec vehicula lacinia felis sit amet consectetur. Praesent sodales enim sapien, sed varius ipsum pellentesque vel. Aenean eu mi eu justo tincidunt finibus vel sit amet ipsum. Sed bibasdum purus vel ipsum sagittis, quis fermentum dolor lobortis. Etiam vulputate eleifasd lectus vel varius. Phasellus imperdiet lectus sit amet ipsum egestas, ut bibasdum ipsum malesuada. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Sed mollis eros sit amet elit porttitor, vel venenatis turpis venenatis. Nulla tempus tortor at eros efficitur, sit amet dapibus ipsum malesuada. Ut at mauris sed nunc malesuada convallis. Duis id sem vel magna varius eleifasd vel at est. Donec eget orci a ipsum tempor lobortis. Sed at consectetur ipsum.";
        root_child0.MeasureFunction = IntrinsicSizeMeasureFunc;
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(2000f, root.LayoutWidth);
        YogaAssert.Equal(2000f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(20f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(2000f, root.LayoutWidth);
        YogaAssert.Equal(2000f, root.LayoutHeight);
        YogaAssert.Equal(1950f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(20f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Contains_Inner_Text_Max_Width_Max_Height()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(2000f);
        root.Height = YogaValue.Point(2000f);
        root.AlignItems = FlexAlign.FlexStart;
        var root_child0 = new YogaNode(config);
        root_child0.FlexDirection = FlexDirection.Row;
        root_child0.MaxWidth = YogaValue.Point(50f);
        root_child0.MaxHeight = YogaValue.Point(20f);
        root.InsertChild(root_child0, 0);
        root_child0.Context = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed eleifasd et tortor ac auctor. Integer at volutpat libero, sed elementum dui interdum id. Aliquam consectetur massa vel neque aliquet, quis consequat risus fringilla. Fusce rhoncus ipsum tempor eros aliquam, vel tempus metus ullamcorper. Nam at nulla sed tellus vestibulum fringilla vel sit amet ligula. Proin velit lectus, euismod sit amet quam vel ultricies dolor, vitae finibus lorem ipsum. Pellentesque molestie at mi sit amet dictum. Donec vehicula lacinia felis sit amet consectetur. Praesent sodales enim sapien, sed varius ipsum pellentesque vel. Aenean eu mi eu justo tincidunt finibus vel sit amet ipsum. Sed bibasdum purus vel ipsum sagittis, quis fermentum dolor lobortis. Etiam vulputate eleifasd lectus vel varius. Phasellus imperdiet lectus sit amet ipsum egestas, ut bibasdum ipsum malesuada. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Sed mollis eros sit amet elit porttitor, vel venenatis turpis venenatis. Nulla tempus tortor at eros efficitur, sit amet dapibus ipsum malesuada. Ut at mauris sed nunc malesuada convallis. Duis id sem vel magna varius eleifasd vel at est. Donec eget orci a ipsum tempor lobortis. Sed at consectetur ipsum.";
        root_child0.MeasureFunction = IntrinsicSizeMeasureFunc;
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(2000f, root.LayoutWidth);
        YogaAssert.Equal(2000f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(20f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(2000f, root.LayoutWidth);
        YogaAssert.Equal(2000f, root.LayoutHeight);
        YogaAssert.Equal(1950f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(20f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Contains_Inner_Text_Max_Width_Max_Height_Column()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(2000f);
        root.AlignItems = FlexAlign.FlexStart;
        var root_child0 = new YogaNode(config);
        root_child0.MaxWidth = YogaValue.Point(50f);
        root.InsertChild(root_child0, 0);
        root_child0.Context = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed eleifasd et tortor ac auctor. Integer at volutpat libero, sed elementum dui interdum id. Aliquam consectetur massa vel neque aliquet, quis consequat risus fringilla. Fusce rhoncus ipsum tempor eros aliquam, vel tempus metus ullamcorper. Nam at nulla sed tellus vestibulum fringilla vel sit amet ligula. Proin velit lectus, euismod sit amet quam vel ultricies dolor, vitae finibus lorem ipsum. Pellentesque molestie at mi sit amet dictum. Donec vehicula lacinia felis sit amet consectetur. Praesent sodales enim sapien, sed varius ipsum pellentesque vel. Aenean eu mi eu justo tincidunt finibus vel sit amet ipsum. Sed bibasdum purus vel ipsum sagittis, quis fermentum dolor lobortis. Etiam vulputate eleifasd lectus vel varius. Phasellus imperdiet lectus sit amet ipsum egestas, ut bibasdum ipsum malesuada. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Sed mollis eros sit amet elit porttitor, vel venenatis turpis venenatis. Nulla tempus tortor at eros efficitur, sit amet dapibus ipsum malesuada. Ut at mauris sed nunc malesuada convallis. Duis id sem vel magna varius eleifasd vel at est. Donec eget orci a ipsum tempor lobortis. Sed at consectetur ipsum.";
        root_child0.MeasureFunction = IntrinsicSizeMeasureFunc;
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(2000f, root.LayoutWidth);
        YogaAssert.Equal(1890f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(1890f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(2000f, root.LayoutWidth);
        YogaAssert.Equal(1890f, root.LayoutHeight);
        YogaAssert.Equal(1950f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(1890f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Contains_Inner_Text_Max_Width()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(2000f);
        root.Height = YogaValue.Point(2000f);
        root.AlignItems = FlexAlign.FlexStart;
        var root_child0 = new YogaNode(config);
        root_child0.FlexDirection = FlexDirection.Row;
        root_child0.MaxWidth = YogaValue.Point(100f);
        root.InsertChild(root_child0, 0);
        root_child0.Context = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed eleifasd et tortor ac auctor. Integer at volutpat libero, sed elementum dui interdum id. Aliquam consectetur massa vel neque aliquet, quis consequat risus fringilla. Fusce rhoncus ipsum tempor eros aliquam, vel tempus metus ullamcorper. Nam at nulla sed tellus vestibulum fringilla vel sit amet ligula. Proin velit lectus, euismod sit amet quam vel ultricies dolor, vitae finibus lorem ipsum. Pellentesque molestie at mi sit amet dictum. Donec vehicula lacinia felis sit amet consectetur. Praesent sodales enim sapien, sed varius ipsum pellentesque vel. Aenean eu mi eu justo tincidunt finibus vel sit amet ipsum. Sed bibasdum purus vel ipsum sagittis, quis fermentum dolor lobortis. Etiam vulputate eleifasd lectus vel varius. Phasellus imperdiet lectus sit amet ipsum egestas, ut bibasdum ipsum malesuada. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Sed mollis eros sit amet elit porttitor, vel venenatis turpis venenatis. Nulla tempus tortor at eros efficitur, sit amet dapibus ipsum malesuada. Ut at mauris sed nunc malesuada convallis. Duis id sem vel magna varius eleifasd vel at est. Donec eget orci a ipsum tempor lobortis. Sed at consectetur ipsum.";
        root_child0.MeasureFunction = IntrinsicSizeMeasureFunc;
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(2000f, root.LayoutWidth);
        YogaAssert.Equal(2000f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(100f, root_child0.LayoutWidth);
        YogaAssert.Equal(1290f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(2000f, root.LayoutWidth);
        YogaAssert.Equal(2000f, root.LayoutHeight);
        YogaAssert.Equal(1900f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(100f, root_child0.LayoutWidth);
        YogaAssert.Equal(1290f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Contains_Inner_Text_Fixed_Width_Shorter_Text()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(2000f);
        root.Height = YogaValue.Point(2000f);
        root.AlignItems = FlexAlign.FlexStart;
        var root_child0 = new YogaNode(config);
        root_child0.FlexDirection = FlexDirection.Row;
        root_child0.Width = YogaValue.Point(100f);
        root.InsertChild(root_child0, 0);
        root_child0.Context = "Lorem ipsum";
        root_child0.MeasureFunction = IntrinsicSizeMeasureFunc;
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(2000f, root.LayoutWidth);
        YogaAssert.Equal(2000f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(100f, root_child0.LayoutWidth);
        YogaAssert.Equal(20f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(2000f, root.LayoutWidth);
        YogaAssert.Equal(2000f, root.LayoutHeight);
        YogaAssert.Equal(1900f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(100f, root_child0.LayoutWidth);
        YogaAssert.Equal(20f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Contains_Inner_Text_Fixed_Height_Shorter_Text()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(2000f);
        root.Height = YogaValue.Point(2000f);
        root.AlignItems = FlexAlign.FlexStart;
        var root_child0 = new YogaNode(config);
        root_child0.FlexDirection = FlexDirection.Row;
        root_child0.Height = YogaValue.Point(100f);
        root.InsertChild(root_child0, 0);
        root_child0.Context = "Lorem ipsum";
        root_child0.MeasureFunction = IntrinsicSizeMeasureFunc;
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(2000f, root.LayoutWidth);
        YogaAssert.Equal(2000f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(110f, root_child0.LayoutWidth);
        YogaAssert.Equal(100f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(2000f, root.LayoutWidth);
        YogaAssert.Equal(2000f, root.LayoutHeight);
        YogaAssert.Equal(1890f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(110f, root_child0.LayoutWidth);
        YogaAssert.Equal(100f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Contains_Inner_Text_Max_Height()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(2000f);
        root.Height = YogaValue.Point(2000f);
        root.AlignItems = FlexAlign.FlexStart;
        var root_child0 = new YogaNode(config);
        root_child0.FlexDirection = FlexDirection.Row;
        root_child0.MaxHeight = YogaValue.Point(20f);
        root.InsertChild(root_child0, 0);
        root_child0.Context = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed eleifasd et tortor ac auctor. Integer at volutpat libero, sed elementum dui interdum id. Aliquam consectetur massa vel neque aliquet, quis consequat risus fringilla. Fusce rhoncus ipsum tempor eros aliquam, vel tempus metus ullamcorper. Nam at nulla sed tellus vestibulum fringilla vel sit amet ligula. Proin velit lectus, euismod sit amet quam vel ultricies dolor, vitae finibus lorem ipsum. Pellentesque molestie at mi sit amet dictum. Donec vehicula lacinia felis sit amet consectetur. Praesent sodales enim sapien, sed varius ipsum pellentesque vel. Aenean eu mi eu justo tincidunt finibus vel sit amet ipsum. Sed bibasdum purus vel ipsum sagittis, quis fermentum dolor lobortis. Etiam vulputate eleifasd lectus vel varius. Phasellus imperdiet lectus sit amet ipsum egestas, ut bibasdum ipsum malesuada. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Sed mollis eros sit amet elit porttitor, vel venenatis turpis venenatis. Nulla tempus tortor at eros efficitur, sit amet dapibus ipsum malesuada. Ut at mauris sed nunc malesuada convallis. Duis id sem vel magna varius eleifasd vel at est. Donec eget orci a ipsum tempor lobortis. Sed at consectetur ipsum.";
        root_child0.MeasureFunction = IntrinsicSizeMeasureFunc;
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(2000f, root.LayoutWidth);
        YogaAssert.Equal(2000f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(2000f, root_child0.LayoutWidth);
        YogaAssert.Equal(20f, root_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(2000f, root.LayoutWidth);
        YogaAssert.Equal(2000f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(2000f, root_child0.LayoutWidth);
        YogaAssert.Equal(20f, root_child0.LayoutHeight);
    }

    [TestMethod]
    public void Max_Content_Width()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.FlexDirection = FlexDirection.Row;
        root.Width = new YogaValue(0, YogaUnit.MaxContent);
        root.FlexWrap = FlexWrap.Wrap;
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Point(50f);
        root_child0.Height = YogaValue.Point(50f);
        root.InsertChild(root_child0, 0);
        var root_child1 = new YogaNode(config);
        root_child1.Width = YogaValue.Point(100f);
        root_child1.Height = YogaValue.Point(50f);
        root.InsertChild(root_child1, 1);
        var root_child2 = new YogaNode(config);
        root_child2.Width = YogaValue.Point(25f);
        root_child2.Height = YogaValue.Point(50f);
        root.InsertChild(root_child2, 2);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(175f, root.LayoutWidth);
        YogaAssert.Equal(50f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0.LayoutHeight);
        YogaAssert.Equal(50f, root_child1.LayoutX);
        YogaAssert.Equal(0f, root_child1.LayoutY);
        YogaAssert.Equal(100f, root_child1.LayoutWidth);
        YogaAssert.Equal(50f, root_child1.LayoutHeight);
        YogaAssert.Equal(150f, root_child2.LayoutX);
        YogaAssert.Equal(0f, root_child2.LayoutY);
        YogaAssert.Equal(25f, root_child2.LayoutWidth);
        YogaAssert.Equal(50f, root_child2.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(175f, root.LayoutWidth);
        YogaAssert.Equal(50f, root.LayoutHeight);
        YogaAssert.Equal(125f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0.LayoutHeight);
        YogaAssert.Equal(25f, root_child1.LayoutX);
        YogaAssert.Equal(0f, root_child1.LayoutY);
        YogaAssert.Equal(100f, root_child1.LayoutWidth);
        YogaAssert.Equal(50f, root_child1.LayoutHeight);
        YogaAssert.Equal(0f, root_child2.LayoutX);
        YogaAssert.Equal(0f, root_child2.LayoutY);
        YogaAssert.Equal(25f, root_child2.LayoutWidth);
        YogaAssert.Equal(50f, root_child2.LayoutHeight);
    }

    [TestMethod]
    [Ignore("Skipped in upstream Yoga (GTEST_SKIP)")]
    public void Fit_Content_Width()
    {
        // TODO: GTEST_SKIP();
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(90f);
        var root_child0 = new YogaNode(config);
        root_child0.FlexDirection = FlexDirection.Row;
        root_child0.Width = new YogaValue(0, YogaUnit.FitContent);
        root_child0.FlexWrap = FlexWrap.Wrap;
        root.InsertChild(root_child0, 0);
        var root_child0_child0 = new YogaNode(config);
        root_child0_child0.Width = YogaValue.Point(50f);
        root_child0_child0.Height = YogaValue.Point(50f);
        root_child0.InsertChild(root_child0_child0, 0);
        var root_child0_child1 = new YogaNode(config);
        root_child0_child1.Width = YogaValue.Point(100f);
        root_child0_child1.Height = YogaValue.Point(50f);
        root_child0.InsertChild(root_child0_child1, 1);
        var root_child0_child2 = new YogaNode(config);
        root_child0_child2.Width = YogaValue.Point(25f);
        root_child0_child2.Height = YogaValue.Point(50f);
        root_child0.InsertChild(root_child0_child2, 2);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(90f, root.LayoutWidth);
        YogaAssert.Equal(150f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(100f, root_child0.LayoutWidth);
        YogaAssert.Equal(150f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child1.LayoutX);
        YogaAssert.Equal(50f, root_child0_child1.LayoutY);
        YogaAssert.Equal(100f, root_child0_child1.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child1.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child2.LayoutX);
        YogaAssert.Equal(100f, root_child0_child2.LayoutY);
        YogaAssert.Equal(25f, root_child0_child2.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child2.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(90f, root.LayoutWidth);
        YogaAssert.Equal(150f, root.LayoutHeight);
        YogaAssert.Equal(-10f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(100f, root_child0.LayoutWidth);
        YogaAssert.Equal(150f, root_child0.LayoutHeight);
        YogaAssert.Equal(50f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child1.LayoutX);
        YogaAssert.Equal(50f, root_child0_child1.LayoutY);
        YogaAssert.Equal(100f, root_child0_child1.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child1.LayoutHeight);
        YogaAssert.Equal(75f, root_child0_child2.LayoutX);
        YogaAssert.Equal(100f, root_child0_child2.LayoutY);
        YogaAssert.Equal(25f, root_child0_child2.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child2.LayoutHeight);
    }

    [TestMethod]
    public void Stretch_Width()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(500f);
        var root_child0 = new YogaNode(config);
        root_child0.FlexDirection = FlexDirection.Row;
        root_child0.Width = new YogaValue(0, YogaUnit.Stretch);
        root_child0.FlexWrap = FlexWrap.Wrap;
        root.InsertChild(root_child0, 0);
        var root_child0_child0 = new YogaNode(config);
        root_child0_child0.Width = YogaValue.Point(50f);
        root_child0_child0.Height = YogaValue.Point(50f);
        root_child0.InsertChild(root_child0_child0, 0);
        var root_child0_child1 = new YogaNode(config);
        root_child0_child1.Width = YogaValue.Point(100f);
        root_child0_child1.Height = YogaValue.Point(50f);
        root_child0.InsertChild(root_child0_child1, 1);
        var root_child0_child2 = new YogaNode(config);
        root_child0_child2.Width = YogaValue.Point(25f);
        root_child0_child2.Height = YogaValue.Point(50f);
        root_child0.InsertChild(root_child0_child2, 2);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(500f, root.LayoutWidth);
        YogaAssert.Equal(50f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(500f, root_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child0.LayoutHeight);
        YogaAssert.Equal(50f, root_child0_child1.LayoutX);
        YogaAssert.Equal(0f, root_child0_child1.LayoutY);
        YogaAssert.Equal(100f, root_child0_child1.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child1.LayoutHeight);
        YogaAssert.Equal(150f, root_child0_child2.LayoutX);
        YogaAssert.Equal(0f, root_child0_child2.LayoutY);
        YogaAssert.Equal(25f, root_child0_child2.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child2.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(500f, root.LayoutWidth);
        YogaAssert.Equal(50f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(500f, root_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0.LayoutHeight);
        YogaAssert.Equal(450f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child0.LayoutHeight);
        YogaAssert.Equal(350f, root_child0_child1.LayoutX);
        YogaAssert.Equal(0f, root_child0_child1.LayoutY);
        YogaAssert.Equal(100f, root_child0_child1.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child1.LayoutHeight);
        YogaAssert.Equal(325f, root_child0_child2.LayoutX);
        YogaAssert.Equal(0f, root_child0_child2.LayoutY);
        YogaAssert.Equal(25f, root_child0_child2.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child2.LayoutHeight);
    }

    [TestMethod]
    public void Max_Content_Height()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Height = new YogaValue(0, YogaUnit.MaxContent);
        root.FlexWrap = FlexWrap.Wrap;
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Point(50f);
        root_child0.Height = YogaValue.Point(50f);
        root.InsertChild(root_child0, 0);
        var root_child1 = new YogaNode(config);
        root_child1.Width = YogaValue.Point(50f);
        root_child1.Height = YogaValue.Point(100f);
        root.InsertChild(root_child1, 1);
        var root_child2 = new YogaNode(config);
        root_child2.Width = YogaValue.Point(50f);
        root_child2.Height = YogaValue.Point(25f);
        root.InsertChild(root_child2, 2);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(50f, root.LayoutWidth);
        YogaAssert.Equal(175f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child1.LayoutX);
        YogaAssert.Equal(50f, root_child1.LayoutY);
        YogaAssert.Equal(50f, root_child1.LayoutWidth);
        YogaAssert.Equal(100f, root_child1.LayoutHeight);
        YogaAssert.Equal(0f, root_child2.LayoutX);
        YogaAssert.Equal(150f, root_child2.LayoutY);
        YogaAssert.Equal(50f, root_child2.LayoutWidth);
        YogaAssert.Equal(25f, root_child2.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(50f, root.LayoutWidth);
        YogaAssert.Equal(175f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child1.LayoutX);
        YogaAssert.Equal(50f, root_child1.LayoutY);
        YogaAssert.Equal(50f, root_child1.LayoutWidth);
        YogaAssert.Equal(100f, root_child1.LayoutHeight);
        YogaAssert.Equal(0f, root_child2.LayoutX);
        YogaAssert.Equal(150f, root_child2.LayoutY);
        YogaAssert.Equal(50f, root_child2.LayoutWidth);
        YogaAssert.Equal(25f, root_child2.LayoutHeight);
    }

    [TestMethod]
    [Ignore("Skipped in upstream Yoga (GTEST_SKIP)")]
    public void Fit_Content_Height()
    {
        // TODO: GTEST_SKIP();
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Height = YogaValue.Point(90f);
        var root_child0 = new YogaNode(config);
        root_child0.Height = new YogaValue(0, YogaUnit.FitContent);
        root_child0.FlexWrap = FlexWrap.Wrap;
        root.InsertChild(root_child0, 0);
        var root_child0_child0 = new YogaNode(config);
        root_child0_child0.Width = YogaValue.Point(50f);
        root_child0_child0.Height = YogaValue.Point(50f);
        root_child0.InsertChild(root_child0_child0, 0);
        var root_child0_child1 = new YogaNode(config);
        root_child0_child1.Width = YogaValue.Point(50f);
        root_child0_child1.Height = YogaValue.Point(100f);
        root_child0.InsertChild(root_child0_child1, 1);
        var root_child0_child2 = new YogaNode(config);
        root_child0_child2.Width = YogaValue.Point(50f);
        root_child0_child2.Height = YogaValue.Point(25f);
        root_child0.InsertChild(root_child0_child2, 2);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(50f, root.LayoutWidth);
        YogaAssert.Equal(90f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(175f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child1.LayoutX);
        YogaAssert.Equal(50f, root_child0_child1.LayoutY);
        YogaAssert.Equal(50f, root_child0_child1.LayoutWidth);
        YogaAssert.Equal(100f, root_child0_child1.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child2.LayoutX);
        YogaAssert.Equal(150f, root_child0_child2.LayoutY);
        YogaAssert.Equal(50f, root_child0_child2.LayoutWidth);
        YogaAssert.Equal(25f, root_child0_child2.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(50f, root.LayoutWidth);
        YogaAssert.Equal(90f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(175f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child1.LayoutX);
        YogaAssert.Equal(50f, root_child0_child1.LayoutY);
        YogaAssert.Equal(50f, root_child0_child1.LayoutWidth);
        YogaAssert.Equal(100f, root_child0_child1.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child2.LayoutX);
        YogaAssert.Equal(150f, root_child0_child2.LayoutY);
        YogaAssert.Equal(50f, root_child0_child2.LayoutWidth);
        YogaAssert.Equal(25f, root_child0_child2.LayoutHeight);
    }

    [TestMethod]
    [Ignore("Skipped in upstream Yoga (GTEST_SKIP)")]
    public void Stretch_Height()
    {
        // TODO: GTEST_SKIP();
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Height = YogaValue.Point(500f);
        var root_child0 = new YogaNode(config);
        root_child0.Height = new YogaValue(0, YogaUnit.Stretch);
        root_child0.FlexWrap = FlexWrap.Wrap;
        root.InsertChild(root_child0, 0);
        var root_child0_child0 = new YogaNode(config);
        root_child0_child0.Width = YogaValue.Point(50f);
        root_child0_child0.Height = YogaValue.Point(50f);
        root_child0.InsertChild(root_child0_child0, 0);
        var root_child0_child1 = new YogaNode(config);
        root_child0_child1.Width = YogaValue.Point(50f);
        root_child0_child1.Height = YogaValue.Point(100f);
        root_child0.InsertChild(root_child0_child1, 1);
        var root_child0_child2 = new YogaNode(config);
        root_child0_child2.Width = YogaValue.Point(50f);
        root_child0_child2.Height = YogaValue.Point(25f);
        root_child0.InsertChild(root_child0_child2, 2);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(50f, root.LayoutWidth);
        YogaAssert.Equal(500f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(500f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child1.LayoutX);
        YogaAssert.Equal(50f, root_child0_child1.LayoutY);
        YogaAssert.Equal(50f, root_child0_child1.LayoutWidth);
        YogaAssert.Equal(100f, root_child0_child1.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child2.LayoutX);
        YogaAssert.Equal(150f, root_child0_child2.LayoutY);
        YogaAssert.Equal(50f, root_child0_child2.LayoutWidth);
        YogaAssert.Equal(25f, root_child0_child2.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(50f, root.LayoutWidth);
        YogaAssert.Equal(500f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(500f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child1.LayoutX);
        YogaAssert.Equal(50f, root_child0_child1.LayoutY);
        YogaAssert.Equal(50f, root_child0_child1.LayoutWidth);
        YogaAssert.Equal(100f, root_child0_child1.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child2.LayoutX);
        YogaAssert.Equal(150f, root_child0_child2.LayoutY);
        YogaAssert.Equal(50f, root_child0_child2.LayoutWidth);
        YogaAssert.Equal(25f, root_child0_child2.LayoutHeight);
    }

    [TestMethod]
    public void Max_Content_Flex_Basis_Column()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.FlexBasis = new YogaValue(0, YogaUnit.MaxContent);
        root.FlexWrap = FlexWrap.Wrap;
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Point(50f);
        root_child0.Height = YogaValue.Point(50f);
        root.InsertChild(root_child0, 0);
        var root_child1 = new YogaNode(config);
        root_child1.Width = YogaValue.Point(50f);
        root_child1.Height = YogaValue.Point(100f);
        root.InsertChild(root_child1, 1);
        var root_child2 = new YogaNode(config);
        root_child2.Width = YogaValue.Point(50f);
        root_child2.Height = YogaValue.Point(25f);
        root.InsertChild(root_child2, 2);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(50f, root.LayoutWidth);
        YogaAssert.Equal(175f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child1.LayoutX);
        YogaAssert.Equal(50f, root_child1.LayoutY);
        YogaAssert.Equal(50f, root_child1.LayoutWidth);
        YogaAssert.Equal(100f, root_child1.LayoutHeight);
        YogaAssert.Equal(0f, root_child2.LayoutX);
        YogaAssert.Equal(150f, root_child2.LayoutY);
        YogaAssert.Equal(50f, root_child2.LayoutWidth);
        YogaAssert.Equal(25f, root_child2.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(50f, root.LayoutWidth);
        YogaAssert.Equal(175f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child1.LayoutX);
        YogaAssert.Equal(50f, root_child1.LayoutY);
        YogaAssert.Equal(50f, root_child1.LayoutWidth);
        YogaAssert.Equal(100f, root_child1.LayoutHeight);
        YogaAssert.Equal(0f, root_child2.LayoutX);
        YogaAssert.Equal(150f, root_child2.LayoutY);
        YogaAssert.Equal(50f, root_child2.LayoutWidth);
        YogaAssert.Equal(25f, root_child2.LayoutHeight);
    }

    [TestMethod]
    [Ignore("Skipped in upstream Yoga (GTEST_SKIP)")]
    public void Fit_Content_Flex_Basis_Column()
    {
        // TODO: GTEST_SKIP();
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Height = YogaValue.Point(90f);
        var root_child0 = new YogaNode(config);
        root_child0.FlexBasis = new YogaValue(0, YogaUnit.FitContent);
        root_child0.FlexWrap = FlexWrap.Wrap;
        root.InsertChild(root_child0, 0);
        var root_child0_child0 = new YogaNode(config);
        root_child0_child0.Width = YogaValue.Point(50f);
        root_child0_child0.Height = YogaValue.Point(50f);
        root_child0.InsertChild(root_child0_child0, 0);
        var root_child0_child1 = new YogaNode(config);
        root_child0_child1.Width = YogaValue.Point(50f);
        root_child0_child1.Height = YogaValue.Point(100f);
        root_child0.InsertChild(root_child0_child1, 1);
        var root_child0_child2 = new YogaNode(config);
        root_child0_child2.Width = YogaValue.Point(50f);
        root_child0_child2.Height = YogaValue.Point(25f);
        root_child0.InsertChild(root_child0_child2, 2);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(50f, root.LayoutWidth);
        YogaAssert.Equal(90f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(175f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child1.LayoutX);
        YogaAssert.Equal(50f, root_child0_child1.LayoutY);
        YogaAssert.Equal(50f, root_child0_child1.LayoutWidth);
        YogaAssert.Equal(100f, root_child0_child1.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child2.LayoutX);
        YogaAssert.Equal(150f, root_child0_child2.LayoutY);
        YogaAssert.Equal(50f, root_child0_child2.LayoutWidth);
        YogaAssert.Equal(25f, root_child0_child2.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(50f, root.LayoutWidth);
        YogaAssert.Equal(90f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(175f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child1.LayoutX);
        YogaAssert.Equal(50f, root_child0_child1.LayoutY);
        YogaAssert.Equal(50f, root_child0_child1.LayoutWidth);
        YogaAssert.Equal(100f, root_child0_child1.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child2.LayoutX);
        YogaAssert.Equal(150f, root_child0_child2.LayoutY);
        YogaAssert.Equal(50f, root_child0_child2.LayoutWidth);
        YogaAssert.Equal(25f, root_child0_child2.LayoutHeight);
    }

    [TestMethod]
    public void Stretch_Flex_Basis_Column()
    {
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Height = YogaValue.Point(500f);
        var root_child0 = new YogaNode(config);
        root_child0.FlexBasis = new YogaValue(0, YogaUnit.Stretch);
        root_child0.FlexWrap = FlexWrap.Wrap;
        root.InsertChild(root_child0, 0);
        var root_child0_child0 = new YogaNode(config);
        root_child0_child0.Width = YogaValue.Point(50f);
        root_child0_child0.Height = YogaValue.Point(50f);
        root_child0.InsertChild(root_child0_child0, 0);
        var root_child0_child1 = new YogaNode(config);
        root_child0_child1.Width = YogaValue.Point(50f);
        root_child0_child1.Height = YogaValue.Point(100f);
        root_child0.InsertChild(root_child0_child1, 1);
        var root_child0_child2 = new YogaNode(config);
        root_child0_child2.Width = YogaValue.Point(50f);
        root_child0_child2.Height = YogaValue.Point(25f);
        root_child0.InsertChild(root_child0_child2, 2);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(50f, root.LayoutWidth);
        YogaAssert.Equal(500f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(175f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child1.LayoutX);
        YogaAssert.Equal(50f, root_child0_child1.LayoutY);
        YogaAssert.Equal(50f, root_child0_child1.LayoutWidth);
        YogaAssert.Equal(100f, root_child0_child1.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child2.LayoutX);
        YogaAssert.Equal(150f, root_child0_child2.LayoutY);
        YogaAssert.Equal(50f, root_child0_child2.LayoutWidth);
        YogaAssert.Equal(25f, root_child0_child2.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(50f, root.LayoutWidth);
        YogaAssert.Equal(500f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(175f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child1.LayoutX);
        YogaAssert.Equal(50f, root_child0_child1.LayoutY);
        YogaAssert.Equal(50f, root_child0_child1.LayoutWidth);
        YogaAssert.Equal(100f, root_child0_child1.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child2.LayoutX);
        YogaAssert.Equal(150f, root_child0_child2.LayoutY);
        YogaAssert.Equal(50f, root_child0_child2.LayoutWidth);
        YogaAssert.Equal(25f, root_child0_child2.LayoutHeight);
    }

    [TestMethod]
    [Ignore("Skipped in upstream Yoga (GTEST_SKIP)")]
    public void Max_Content_Flex_Basis_Row()
    {
        // TODO: GTEST_SKIP();
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.FlexDirection = FlexDirection.Row;
        root.FlexBasis = new YogaValue(0, YogaUnit.MaxContent);
        root.FlexWrap = FlexWrap.Wrap;
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Point(50f);
        root_child0.Height = YogaValue.Point(50f);
        root.InsertChild(root_child0, 0);
        var root_child1 = new YogaNode(config);
        root_child1.Width = YogaValue.Point(100f);
        root_child1.Height = YogaValue.Point(500f);
        root.InsertChild(root_child1, 1);
        var root_child2 = new YogaNode(config);
        root_child2.Width = YogaValue.Point(25f);
        root_child2.Height = YogaValue.Point(50f);
        root.InsertChild(root_child2, 2);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(600f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child1.LayoutX);
        YogaAssert.Equal(50f, root_child1.LayoutY);
        YogaAssert.Equal(100f, root_child1.LayoutWidth);
        YogaAssert.Equal(500f, root_child1.LayoutHeight);
        YogaAssert.Equal(0f, root_child2.LayoutX);
        YogaAssert.Equal(550f, root_child2.LayoutY);
        YogaAssert.Equal(25f, root_child2.LayoutWidth);
        YogaAssert.Equal(50f, root_child2.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(100f, root.LayoutWidth);
        YogaAssert.Equal(600f, root.LayoutHeight);
        YogaAssert.Equal(50f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child1.LayoutX);
        YogaAssert.Equal(50f, root_child1.LayoutY);
        YogaAssert.Equal(100f, root_child1.LayoutWidth);
        YogaAssert.Equal(500f, root_child1.LayoutHeight);
        YogaAssert.Equal(75f, root_child2.LayoutX);
        YogaAssert.Equal(550f, root_child2.LayoutY);
        YogaAssert.Equal(25f, root_child2.LayoutWidth);
        YogaAssert.Equal(50f, root_child2.LayoutHeight);
    }

    [TestMethod]
    [Ignore("Skipped in upstream Yoga (GTEST_SKIP)")]
    public void Fit_Content_Flex_Basis_Row()
    {
        // TODO: GTEST_SKIP();
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(90f);
        var root_child0 = new YogaNode(config);
        root_child0.FlexDirection = FlexDirection.Row;
        root_child0.FlexBasis = new YogaValue(0, YogaUnit.FitContent);
        root_child0.FlexWrap = FlexWrap.Wrap;
        root.InsertChild(root_child0, 0);
        var root_child0_child0 = new YogaNode(config);
        root_child0_child0.Width = YogaValue.Point(50f);
        root_child0_child0.Height = YogaValue.Point(50f);
        root_child0.InsertChild(root_child0_child0, 0);
        var root_child0_child1 = new YogaNode(config);
        root_child0_child1.Width = YogaValue.Point(100f);
        root_child0_child1.Height = YogaValue.Point(50f);
        root_child0.InsertChild(root_child0_child1, 1);
        var root_child0_child2 = new YogaNode(config);
        root_child0_child2.Width = YogaValue.Point(25f);
        root_child0_child2.Height = YogaValue.Point(50f);
        root_child0.InsertChild(root_child0_child2, 2);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(90f, root.LayoutWidth);
        YogaAssert.Equal(150f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(90f, root_child0.LayoutWidth);
        YogaAssert.Equal(150f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child1.LayoutX);
        YogaAssert.Equal(50f, root_child0_child1.LayoutY);
        YogaAssert.Equal(100f, root_child0_child1.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child1.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child2.LayoutX);
        YogaAssert.Equal(100f, root_child0_child2.LayoutY);
        YogaAssert.Equal(25f, root_child0_child2.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child2.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(90f, root.LayoutWidth);
        YogaAssert.Equal(150f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(90f, root_child0.LayoutWidth);
        YogaAssert.Equal(150f, root_child0.LayoutHeight);
        YogaAssert.Equal(40f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child0.LayoutHeight);
        YogaAssert.Equal(-10f, root_child0_child1.LayoutX);
        YogaAssert.Equal(50f, root_child0_child1.LayoutY);
        YogaAssert.Equal(100f, root_child0_child1.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child1.LayoutHeight);
        YogaAssert.Equal(65f, root_child0_child2.LayoutX);
        YogaAssert.Equal(100f, root_child0_child2.LayoutY);
        YogaAssert.Equal(25f, root_child0_child2.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child2.LayoutHeight);
    }

    [TestMethod]
    [Ignore("Skipped in upstream Yoga (GTEST_SKIP)")]
    public void Stretch_Flex_Basis_Row()
    {
        // TODO: GTEST_SKIP();
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(500f);
        var root_child0 = new YogaNode(config);
        root_child0.FlexDirection = FlexDirection.Row;
        root_child0.FlexBasis = new YogaValue(0, YogaUnit.Stretch);
        root_child0.FlexWrap = FlexWrap.Wrap;
        root.InsertChild(root_child0, 0);
        var root_child0_child0 = new YogaNode(config);
        root_child0_child0.Width = YogaValue.Point(50f);
        root_child0_child0.Height = YogaValue.Point(50f);
        root_child0.InsertChild(root_child0_child0, 0);
        var root_child0_child1 = new YogaNode(config);
        root_child0_child1.Width = YogaValue.Point(100f);
        root_child0_child1.Height = YogaValue.Point(50f);
        root_child0.InsertChild(root_child0_child1, 1);
        var root_child0_child2 = new YogaNode(config);
        root_child0_child2.Width = YogaValue.Point(25f);
        root_child0_child2.Height = YogaValue.Point(50f);
        root_child0.InsertChild(root_child0_child2, 2);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(500f, root.LayoutWidth);
        YogaAssert.Equal(50f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(500f, root_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child0.LayoutHeight);
        YogaAssert.Equal(50f, root_child0_child1.LayoutX);
        YogaAssert.Equal(0f, root_child0_child1.LayoutY);
        YogaAssert.Equal(100f, root_child0_child1.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child1.LayoutHeight);
        YogaAssert.Equal(150f, root_child0_child2.LayoutX);
        YogaAssert.Equal(0f, root_child0_child2.LayoutY);
        YogaAssert.Equal(25f, root_child0_child2.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child2.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(500f, root.LayoutWidth);
        YogaAssert.Equal(50f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(500f, root_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0.LayoutHeight);
        YogaAssert.Equal(450f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child0.LayoutHeight);
        YogaAssert.Equal(350f, root_child0_child1.LayoutX);
        YogaAssert.Equal(0f, root_child0_child1.LayoutY);
        YogaAssert.Equal(100f, root_child0_child1.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child1.LayoutHeight);
        YogaAssert.Equal(325f, root_child0_child2.LayoutX);
        YogaAssert.Equal(0f, root_child0_child2.LayoutY);
        YogaAssert.Equal(25f, root_child0_child2.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child2.LayoutHeight);
    }

    [TestMethod]
    [Ignore("Skipped in upstream Yoga (GTEST_SKIP)")]
    public void Max_Content_Max_Width()
    {
        // TODO: GTEST_SKIP();
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.FlexDirection = FlexDirection.Row;
        root.MaxWidth = new YogaValue(0, YogaUnit.MaxContent);
        root.Width = YogaValue.Point(200f);
        root.FlexWrap = FlexWrap.Wrap;
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Point(50f);
        root_child0.Height = YogaValue.Point(50f);
        root.InsertChild(root_child0, 0);
        var root_child1 = new YogaNode(config);
        root_child1.Width = YogaValue.Point(100f);
        root_child1.Height = YogaValue.Point(50f);
        root.InsertChild(root_child1, 1);
        var root_child2 = new YogaNode(config);
        root_child2.Width = YogaValue.Point(25f);
        root_child2.Height = YogaValue.Point(50f);
        root.InsertChild(root_child2, 2);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(175f, root.LayoutWidth);
        YogaAssert.Equal(50f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0.LayoutHeight);
        YogaAssert.Equal(50f, root_child1.LayoutX);
        YogaAssert.Equal(0f, root_child1.LayoutY);
        YogaAssert.Equal(100f, root_child1.LayoutWidth);
        YogaAssert.Equal(50f, root_child1.LayoutHeight);
        YogaAssert.Equal(150f, root_child2.LayoutX);
        YogaAssert.Equal(0f, root_child2.LayoutY);
        YogaAssert.Equal(25f, root_child2.LayoutWidth);
        YogaAssert.Equal(50f, root_child2.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(175f, root.LayoutWidth);
        YogaAssert.Equal(50f, root.LayoutHeight);
        YogaAssert.Equal(125f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0.LayoutHeight);
        YogaAssert.Equal(25f, root_child1.LayoutX);
        YogaAssert.Equal(0f, root_child1.LayoutY);
        YogaAssert.Equal(100f, root_child1.LayoutWidth);
        YogaAssert.Equal(50f, root_child1.LayoutHeight);
        YogaAssert.Equal(0f, root_child2.LayoutX);
        YogaAssert.Equal(0f, root_child2.LayoutY);
        YogaAssert.Equal(25f, root_child2.LayoutWidth);
        YogaAssert.Equal(50f, root_child2.LayoutHeight);
    }

    [TestMethod]
    [Ignore("Skipped in upstream Yoga (GTEST_SKIP)")]
    public void Fit_Content_Max_Width()
    {
        // TODO: GTEST_SKIP();
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(90f);
        var root_child0 = new YogaNode(config);
        root_child0.FlexDirection = FlexDirection.Row;
        root_child0.MaxWidth = new YogaValue(0, YogaUnit.FitContent);
        root_child0.Width = YogaValue.Point(110f);
        root_child0.FlexWrap = FlexWrap.Wrap;
        root.InsertChild(root_child0, 0);
        var root_child0_child0 = new YogaNode(config);
        root_child0_child0.Width = YogaValue.Point(50f);
        root_child0_child0.Height = YogaValue.Point(50f);
        root_child0.InsertChild(root_child0_child0, 0);
        var root_child0_child1 = new YogaNode(config);
        root_child0_child1.Width = YogaValue.Point(100f);
        root_child0_child1.Height = YogaValue.Point(50f);
        root_child0.InsertChild(root_child0_child1, 1);
        var root_child0_child2 = new YogaNode(config);
        root_child0_child2.Width = YogaValue.Point(25f);
        root_child0_child2.Height = YogaValue.Point(50f);
        root_child0.InsertChild(root_child0_child2, 2);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(90f, root.LayoutWidth);
        YogaAssert.Equal(150f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(100f, root_child0.LayoutWidth);
        YogaAssert.Equal(150f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child1.LayoutX);
        YogaAssert.Equal(50f, root_child0_child1.LayoutY);
        YogaAssert.Equal(100f, root_child0_child1.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child1.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child2.LayoutX);
        YogaAssert.Equal(100f, root_child0_child2.LayoutY);
        YogaAssert.Equal(25f, root_child0_child2.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child2.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(90f, root.LayoutWidth);
        YogaAssert.Equal(150f, root.LayoutHeight);
        YogaAssert.Equal(-10f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(100f, root_child0.LayoutWidth);
        YogaAssert.Equal(150f, root_child0.LayoutHeight);
        YogaAssert.Equal(50f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child1.LayoutX);
        YogaAssert.Equal(50f, root_child0_child1.LayoutY);
        YogaAssert.Equal(100f, root_child0_child1.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child1.LayoutHeight);
        YogaAssert.Equal(75f, root_child0_child2.LayoutX);
        YogaAssert.Equal(100f, root_child0_child2.LayoutY);
        YogaAssert.Equal(25f, root_child0_child2.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child2.LayoutHeight);
    }

    [TestMethod]
    [Ignore("Skipped in upstream Yoga (GTEST_SKIP)")]
    public void Stretch_Max_Width()
    {
        // TODO: GTEST_SKIP();
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(500f);
        var root_child0 = new YogaNode(config);
        root_child0.FlexDirection = FlexDirection.Row;
        root_child0.MaxWidth = new YogaValue(0, YogaUnit.Stretch);
        root_child0.Width = YogaValue.Point(600f);
        root_child0.FlexWrap = FlexWrap.Wrap;
        root.InsertChild(root_child0, 0);
        var root_child0_child0 = new YogaNode(config);
        root_child0_child0.Width = YogaValue.Point(50f);
        root_child0_child0.Height = YogaValue.Point(50f);
        root_child0.InsertChild(root_child0_child0, 0);
        var root_child0_child1 = new YogaNode(config);
        root_child0_child1.Width = YogaValue.Point(100f);
        root_child0_child1.Height = YogaValue.Point(50f);
        root_child0.InsertChild(root_child0_child1, 1);
        var root_child0_child2 = new YogaNode(config);
        root_child0_child2.Width = YogaValue.Point(25f);
        root_child0_child2.Height = YogaValue.Point(50f);
        root_child0.InsertChild(root_child0_child2, 2);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(500f, root.LayoutWidth);
        YogaAssert.Equal(50f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(500f, root_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child0.LayoutHeight);
        YogaAssert.Equal(50f, root_child0_child1.LayoutX);
        YogaAssert.Equal(0f, root_child0_child1.LayoutY);
        YogaAssert.Equal(100f, root_child0_child1.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child1.LayoutHeight);
        YogaAssert.Equal(150f, root_child0_child2.LayoutX);
        YogaAssert.Equal(0f, root_child0_child2.LayoutY);
        YogaAssert.Equal(25f, root_child0_child2.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child2.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(500f, root.LayoutWidth);
        YogaAssert.Equal(50f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(500f, root_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0.LayoutHeight);
        YogaAssert.Equal(450f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child0.LayoutHeight);
        YogaAssert.Equal(350f, root_child0_child1.LayoutX);
        YogaAssert.Equal(0f, root_child0_child1.LayoutY);
        YogaAssert.Equal(100f, root_child0_child1.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child1.LayoutHeight);
        YogaAssert.Equal(325f, root_child0_child2.LayoutX);
        YogaAssert.Equal(0f, root_child0_child2.LayoutY);
        YogaAssert.Equal(25f, root_child0_child2.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child2.LayoutHeight);
    }

    [TestMethod]
    [Ignore("Skipped in upstream Yoga (GTEST_SKIP)")]
    public void Max_Content_Min_Width()
    {
        // TODO: GTEST_SKIP();
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.FlexDirection = FlexDirection.Row;
        root.MinWidth = new YogaValue(0, YogaUnit.MaxContent);
        root.Width = YogaValue.Point(100f);
        root.FlexWrap = FlexWrap.Wrap;
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Point(50f);
        root_child0.Height = YogaValue.Point(50f);
        root.InsertChild(root_child0, 0);
        var root_child1 = new YogaNode(config);
        root_child1.Width = YogaValue.Point(100f);
        root_child1.Height = YogaValue.Point(50f);
        root.InsertChild(root_child1, 1);
        var root_child2 = new YogaNode(config);
        root_child2.Width = YogaValue.Point(25f);
        root_child2.Height = YogaValue.Point(50f);
        root.InsertChild(root_child2, 2);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(175f, root.LayoutWidth);
        YogaAssert.Equal(50f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0.LayoutHeight);
        YogaAssert.Equal(50f, root_child1.LayoutX);
        YogaAssert.Equal(0f, root_child1.LayoutY);
        YogaAssert.Equal(100f, root_child1.LayoutWidth);
        YogaAssert.Equal(50f, root_child1.LayoutHeight);
        YogaAssert.Equal(150f, root_child2.LayoutX);
        YogaAssert.Equal(0f, root_child2.LayoutY);
        YogaAssert.Equal(25f, root_child2.LayoutWidth);
        YogaAssert.Equal(50f, root_child2.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(175f, root.LayoutWidth);
        YogaAssert.Equal(50f, root.LayoutHeight);
        YogaAssert.Equal(125f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0.LayoutHeight);
        YogaAssert.Equal(25f, root_child1.LayoutX);
        YogaAssert.Equal(0f, root_child1.LayoutY);
        YogaAssert.Equal(100f, root_child1.LayoutWidth);
        YogaAssert.Equal(50f, root_child1.LayoutHeight);
        YogaAssert.Equal(0f, root_child2.LayoutX);
        YogaAssert.Equal(0f, root_child2.LayoutY);
        YogaAssert.Equal(25f, root_child2.LayoutWidth);
        YogaAssert.Equal(50f, root_child2.LayoutHeight);
    }

    [TestMethod]
    [Ignore("Skipped in upstream Yoga (GTEST_SKIP)")]
    public void Fit_Content_Min_Width()
    {
        // TODO: GTEST_SKIP();
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(90f);
        var root_child0 = new YogaNode(config);
        root_child0.FlexDirection = FlexDirection.Row;
        root_child0.MinWidth = new YogaValue(0, YogaUnit.FitContent);
        root_child0.Width = YogaValue.Point(90f);
        root_child0.FlexWrap = FlexWrap.Wrap;
        root.InsertChild(root_child0, 0);
        var root_child0_child0 = new YogaNode(config);
        root_child0_child0.Width = YogaValue.Point(50f);
        root_child0_child0.Height = YogaValue.Point(50f);
        root_child0.InsertChild(root_child0_child0, 0);
        var root_child0_child1 = new YogaNode(config);
        root_child0_child1.Width = YogaValue.Point(100f);
        root_child0_child1.Height = YogaValue.Point(50f);
        root_child0.InsertChild(root_child0_child1, 1);
        var root_child0_child2 = new YogaNode(config);
        root_child0_child2.Width = YogaValue.Point(25f);
        root_child0_child2.Height = YogaValue.Point(50f);
        root_child0.InsertChild(root_child0_child2, 2);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(90f, root.LayoutWidth);
        YogaAssert.Equal(150f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(100f, root_child0.LayoutWidth);
        YogaAssert.Equal(150f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child1.LayoutX);
        YogaAssert.Equal(50f, root_child0_child1.LayoutY);
        YogaAssert.Equal(100f, root_child0_child1.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child1.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child2.LayoutX);
        YogaAssert.Equal(100f, root_child0_child2.LayoutY);
        YogaAssert.Equal(25f, root_child0_child2.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child2.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(90f, root.LayoutWidth);
        YogaAssert.Equal(150f, root.LayoutHeight);
        YogaAssert.Equal(-10f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(100f, root_child0.LayoutWidth);
        YogaAssert.Equal(150f, root_child0.LayoutHeight);
        YogaAssert.Equal(50f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child1.LayoutX);
        YogaAssert.Equal(50f, root_child0_child1.LayoutY);
        YogaAssert.Equal(100f, root_child0_child1.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child1.LayoutHeight);
        YogaAssert.Equal(75f, root_child0_child2.LayoutX);
        YogaAssert.Equal(100f, root_child0_child2.LayoutY);
        YogaAssert.Equal(25f, root_child0_child2.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child2.LayoutHeight);
    }

    [TestMethod]
    [Ignore("Skipped in upstream Yoga (GTEST_SKIP)")]
    public void Stretch_Min_Width()
    {
        // TODO: GTEST_SKIP();
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(500f);
        var root_child0 = new YogaNode(config);
        root_child0.FlexDirection = FlexDirection.Row;
        root_child0.MinWidth = new YogaValue(0, YogaUnit.Stretch);
        root_child0.Width = YogaValue.Point(400f);
        root_child0.FlexWrap = FlexWrap.Wrap;
        root.InsertChild(root_child0, 0);
        var root_child0_child0 = new YogaNode(config);
        root_child0_child0.Width = YogaValue.Point(50f);
        root_child0_child0.Height = YogaValue.Point(50f);
        root_child0.InsertChild(root_child0_child0, 0);
        var root_child0_child1 = new YogaNode(config);
        root_child0_child1.Width = YogaValue.Point(100f);
        root_child0_child1.Height = YogaValue.Point(50f);
        root_child0.InsertChild(root_child0_child1, 1);
        var root_child0_child2 = new YogaNode(config);
        root_child0_child2.Width = YogaValue.Point(25f);
        root_child0_child2.Height = YogaValue.Point(50f);
        root_child0.InsertChild(root_child0_child2, 2);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(500f, root.LayoutWidth);
        YogaAssert.Equal(50f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(500f, root_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child0.LayoutHeight);
        YogaAssert.Equal(50f, root_child0_child1.LayoutX);
        YogaAssert.Equal(0f, root_child0_child1.LayoutY);
        YogaAssert.Equal(100f, root_child0_child1.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child1.LayoutHeight);
        YogaAssert.Equal(150f, root_child0_child2.LayoutX);
        YogaAssert.Equal(0f, root_child0_child2.LayoutY);
        YogaAssert.Equal(25f, root_child0_child2.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child2.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(500f, root.LayoutWidth);
        YogaAssert.Equal(50f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(500f, root_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0.LayoutHeight);
        YogaAssert.Equal(450f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child0.LayoutHeight);
        YogaAssert.Equal(350f, root_child0_child1.LayoutX);
        YogaAssert.Equal(0f, root_child0_child1.LayoutY);
        YogaAssert.Equal(100f, root_child0_child1.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child1.LayoutHeight);
        YogaAssert.Equal(325f, root_child0_child2.LayoutX);
        YogaAssert.Equal(0f, root_child0_child2.LayoutY);
        YogaAssert.Equal(25f, root_child0_child2.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child2.LayoutHeight);
    }

    [TestMethod]
    [Ignore("Skipped in upstream Yoga (GTEST_SKIP)")]
    public void Max_Content_Max_Height()
    {
        // TODO: GTEST_SKIP();
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.MaxHeight = new YogaValue(0, YogaUnit.MaxContent);
        root.Height = YogaValue.Point(200f);
        root.FlexWrap = FlexWrap.Wrap;
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Point(50f);
        root_child0.Height = YogaValue.Point(50f);
        root.InsertChild(root_child0, 0);
        var root_child1 = new YogaNode(config);
        root_child1.Width = YogaValue.Point(50f);
        root_child1.Height = YogaValue.Point(100f);
        root.InsertChild(root_child1, 1);
        var root_child2 = new YogaNode(config);
        root_child2.Width = YogaValue.Point(50f);
        root_child2.Height = YogaValue.Point(25f);
        root.InsertChild(root_child2, 2);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(50f, root.LayoutWidth);
        YogaAssert.Equal(175f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child1.LayoutX);
        YogaAssert.Equal(50f, root_child1.LayoutY);
        YogaAssert.Equal(50f, root_child1.LayoutWidth);
        YogaAssert.Equal(100f, root_child1.LayoutHeight);
        YogaAssert.Equal(0f, root_child2.LayoutX);
        YogaAssert.Equal(150f, root_child2.LayoutY);
        YogaAssert.Equal(50f, root_child2.LayoutWidth);
        YogaAssert.Equal(25f, root_child2.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(50f, root.LayoutWidth);
        YogaAssert.Equal(175f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child1.LayoutX);
        YogaAssert.Equal(50f, root_child1.LayoutY);
        YogaAssert.Equal(50f, root_child1.LayoutWidth);
        YogaAssert.Equal(100f, root_child1.LayoutHeight);
        YogaAssert.Equal(0f, root_child2.LayoutX);
        YogaAssert.Equal(150f, root_child2.LayoutY);
        YogaAssert.Equal(50f, root_child2.LayoutWidth);
        YogaAssert.Equal(25f, root_child2.LayoutHeight);
    }

    [TestMethod]
    [Ignore("Skipped in upstream Yoga (GTEST_SKIP)")]
    public void Fit_Content_Max_Height()
    {
        // TODO: GTEST_SKIP();
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Height = YogaValue.Point(90f);
        var root_child0 = new YogaNode(config);
        root_child0.MaxHeight = new YogaValue(0, YogaUnit.FitContent);
        root_child0.Height = YogaValue.Point(110f);
        root_child0.FlexWrap = FlexWrap.Wrap;
        root.InsertChild(root_child0, 0);
        var root_child0_child0 = new YogaNode(config);
        root_child0_child0.Width = YogaValue.Point(50f);
        root_child0_child0.Height = YogaValue.Point(50f);
        root_child0.InsertChild(root_child0_child0, 0);
        var root_child0_child1 = new YogaNode(config);
        root_child0_child1.Width = YogaValue.Point(50f);
        root_child0_child1.Height = YogaValue.Point(100f);
        root_child0.InsertChild(root_child0_child1, 1);
        var root_child0_child2 = new YogaNode(config);
        root_child0_child2.Width = YogaValue.Point(50f);
        root_child0_child2.Height = YogaValue.Point(25f);
        root_child0.InsertChild(root_child0_child2, 2);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(50f, root.LayoutWidth);
        YogaAssert.Equal(90f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(100f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child0.LayoutHeight);
        YogaAssert.Equal(50f, root_child0_child1.LayoutX);
        YogaAssert.Equal(0f, root_child0_child1.LayoutY);
        YogaAssert.Equal(50f, root_child0_child1.LayoutWidth);
        YogaAssert.Equal(100f, root_child0_child1.LayoutHeight);
        YogaAssert.Equal(100f, root_child0_child2.LayoutX);
        YogaAssert.Equal(0f, root_child0_child2.LayoutY);
        YogaAssert.Equal(50f, root_child0_child2.LayoutWidth);
        YogaAssert.Equal(25f, root_child0_child2.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(50f, root.LayoutWidth);
        YogaAssert.Equal(90f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(100f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child0.LayoutHeight);
        YogaAssert.Equal(-50f, root_child0_child1.LayoutX);
        YogaAssert.Equal(0f, root_child0_child1.LayoutY);
        YogaAssert.Equal(50f, root_child0_child1.LayoutWidth);
        YogaAssert.Equal(100f, root_child0_child1.LayoutHeight);
        YogaAssert.Equal(-100f, root_child0_child2.LayoutX);
        YogaAssert.Equal(0f, root_child0_child2.LayoutY);
        YogaAssert.Equal(50f, root_child0_child2.LayoutWidth);
        YogaAssert.Equal(25f, root_child0_child2.LayoutHeight);
    }

    [TestMethod]
    [Ignore("Skipped in upstream Yoga (GTEST_SKIP)")]
    public void Stretch_Max_Height()
    {
        // TODO: GTEST_SKIP();
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Height = YogaValue.Point(500f);
        var root_child0 = new YogaNode(config);
        root_child0.MaxHeight = new YogaValue(0, YogaUnit.Stretch);
        root_child0.FlexWrap = FlexWrap.Wrap;
        root_child0.Height = YogaValue.Point(600f);
        root.InsertChild(root_child0, 0);
        var root_child0_child0 = new YogaNode(config);
        root_child0_child0.Width = YogaValue.Point(50f);
        root_child0_child0.Height = YogaValue.Point(50f);
        root_child0.InsertChild(root_child0_child0, 0);
        var root_child0_child1 = new YogaNode(config);
        root_child0_child1.Width = YogaValue.Point(50f);
        root_child0_child1.Height = YogaValue.Point(100f);
        root_child0.InsertChild(root_child0_child1, 1);
        var root_child0_child2 = new YogaNode(config);
        root_child0_child2.Width = YogaValue.Point(50f);
        root_child0_child2.Height = YogaValue.Point(25f);
        root_child0.InsertChild(root_child0_child2, 2);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(50f, root.LayoutWidth);
        YogaAssert.Equal(500f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(500f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child1.LayoutX);
        YogaAssert.Equal(50f, root_child0_child1.LayoutY);
        YogaAssert.Equal(50f, root_child0_child1.LayoutWidth);
        YogaAssert.Equal(100f, root_child0_child1.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child2.LayoutX);
        YogaAssert.Equal(150f, root_child0_child2.LayoutY);
        YogaAssert.Equal(50f, root_child0_child2.LayoutWidth);
        YogaAssert.Equal(25f, root_child0_child2.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(50f, root.LayoutWidth);
        YogaAssert.Equal(500f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(500f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child1.LayoutX);
        YogaAssert.Equal(50f, root_child0_child1.LayoutY);
        YogaAssert.Equal(50f, root_child0_child1.LayoutWidth);
        YogaAssert.Equal(100f, root_child0_child1.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child2.LayoutX);
        YogaAssert.Equal(150f, root_child0_child2.LayoutY);
        YogaAssert.Equal(50f, root_child0_child2.LayoutWidth);
        YogaAssert.Equal(25f, root_child0_child2.LayoutHeight);
    }

    [TestMethod]
    [Ignore("Skipped in upstream Yoga (GTEST_SKIP)")]
    public void Max_Content_Min_Height()
    {
        // TODO: GTEST_SKIP();
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.MinHeight = new YogaValue(0, YogaUnit.MaxContent);
        root.Height = YogaValue.Point(100f);
        root.FlexWrap = FlexWrap.Wrap;
        var root_child0 = new YogaNode(config);
        root_child0.Width = YogaValue.Point(50f);
        root_child0.Height = YogaValue.Point(50f);
        root.InsertChild(root_child0, 0);
        var root_child1 = new YogaNode(config);
        root_child1.Width = YogaValue.Point(50f);
        root_child1.Height = YogaValue.Point(100f);
        root.InsertChild(root_child1, 1);
        var root_child2 = new YogaNode(config);
        root_child2.Width = YogaValue.Point(50f);
        root_child2.Height = YogaValue.Point(25f);
        root.InsertChild(root_child2, 2);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(50f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0.LayoutHeight);
        YogaAssert.Equal(50f, root_child1.LayoutX);
        YogaAssert.Equal(0f, root_child1.LayoutY);
        YogaAssert.Equal(50f, root_child1.LayoutWidth);
        YogaAssert.Equal(100f, root_child1.LayoutHeight);
        YogaAssert.Equal(100f, root_child2.LayoutX);
        YogaAssert.Equal(0f, root_child2.LayoutY);
        YogaAssert.Equal(50f, root_child2.LayoutWidth);
        YogaAssert.Equal(25f, root_child2.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(50f, root.LayoutWidth);
        YogaAssert.Equal(100f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0.LayoutHeight);
        YogaAssert.Equal(-50f, root_child1.LayoutX);
        YogaAssert.Equal(0f, root_child1.LayoutY);
        YogaAssert.Equal(50f, root_child1.LayoutWidth);
        YogaAssert.Equal(100f, root_child1.LayoutHeight);
        YogaAssert.Equal(-100f, root_child2.LayoutX);
        YogaAssert.Equal(0f, root_child2.LayoutY);
        YogaAssert.Equal(50f, root_child2.LayoutWidth);
        YogaAssert.Equal(25f, root_child2.LayoutHeight);
    }

    [TestMethod]
    [Ignore("Skipped in upstream Yoga (GTEST_SKIP)")]
    public void Fit_Content_Min_Height()
    {
        // TODO: GTEST_SKIP();
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Height = YogaValue.Point(90f);
        var root_child0 = new YogaNode(config);
        root_child0.MinHeight = new YogaValue(0, YogaUnit.FitContent);
        root_child0.Height = YogaValue.Point(90f);
        root_child0.FlexWrap = FlexWrap.Wrap;
        root.InsertChild(root_child0, 0);
        var root_child0_child0 = new YogaNode(config);
        root_child0_child0.Width = YogaValue.Point(50f);
        root_child0_child0.Height = YogaValue.Point(50f);
        root_child0.InsertChild(root_child0_child0, 0);
        var root_child0_child1 = new YogaNode(config);
        root_child0_child1.Width = YogaValue.Point(50f);
        root_child0_child1.Height = YogaValue.Point(100f);
        root_child0.InsertChild(root_child0_child1, 1);
        var root_child0_child2 = new YogaNode(config);
        root_child0_child2.Width = YogaValue.Point(50f);
        root_child0_child2.Height = YogaValue.Point(25f);
        root_child0.InsertChild(root_child0_child2, 2);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(50f, root.LayoutWidth);
        YogaAssert.Equal(90f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(100f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child0.LayoutHeight);
        YogaAssert.Equal(50f, root_child0_child1.LayoutX);
        YogaAssert.Equal(0f, root_child0_child1.LayoutY);
        YogaAssert.Equal(50f, root_child0_child1.LayoutWidth);
        YogaAssert.Equal(100f, root_child0_child1.LayoutHeight);
        YogaAssert.Equal(100f, root_child0_child2.LayoutX);
        YogaAssert.Equal(0f, root_child0_child2.LayoutY);
        YogaAssert.Equal(50f, root_child0_child2.LayoutWidth);
        YogaAssert.Equal(25f, root_child0_child2.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(50f, root.LayoutWidth);
        YogaAssert.Equal(90f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(100f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child0.LayoutHeight);
        YogaAssert.Equal(-50f, root_child0_child1.LayoutX);
        YogaAssert.Equal(0f, root_child0_child1.LayoutY);
        YogaAssert.Equal(50f, root_child0_child1.LayoutWidth);
        YogaAssert.Equal(100f, root_child0_child1.LayoutHeight);
        YogaAssert.Equal(-100f, root_child0_child2.LayoutX);
        YogaAssert.Equal(0f, root_child0_child2.LayoutY);
        YogaAssert.Equal(50f, root_child0_child2.LayoutWidth);
        YogaAssert.Equal(25f, root_child0_child2.LayoutHeight);
    }

    [TestMethod]
    [Ignore("Skipped in upstream Yoga (GTEST_SKIP)")]
    public void Stretch_Min_Height()
    {
        // TODO: GTEST_SKIP();
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Height = YogaValue.Point(500f);
        var root_child0 = new YogaNode(config);
        root_child0.MinHeight = new YogaValue(0, YogaUnit.Stretch);
        root_child0.FlexWrap = FlexWrap.Wrap;
        root_child0.Height = YogaValue.Point(400f);
        root.InsertChild(root_child0, 0);
        var root_child0_child0 = new YogaNode(config);
        root_child0_child0.Width = YogaValue.Point(50f);
        root_child0_child0.Height = YogaValue.Point(50f);
        root_child0.InsertChild(root_child0_child0, 0);
        var root_child0_child1 = new YogaNode(config);
        root_child0_child1.Width = YogaValue.Point(50f);
        root_child0_child1.Height = YogaValue.Point(100f);
        root_child0.InsertChild(root_child0_child1, 1);
        var root_child0_child2 = new YogaNode(config);
        root_child0_child2.Width = YogaValue.Point(50f);
        root_child0_child2.Height = YogaValue.Point(25f);
        root_child0.InsertChild(root_child0_child2, 2);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(50f, root.LayoutWidth);
        YogaAssert.Equal(500f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(500f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child1.LayoutX);
        YogaAssert.Equal(50f, root_child0_child1.LayoutY);
        YogaAssert.Equal(50f, root_child0_child1.LayoutWidth);
        YogaAssert.Equal(100f, root_child0_child1.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child2.LayoutX);
        YogaAssert.Equal(150f, root_child0_child2.LayoutY);
        YogaAssert.Equal(50f, root_child0_child2.LayoutWidth);
        YogaAssert.Equal(25f, root_child0_child2.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(50f, root.LayoutWidth);
        YogaAssert.Equal(500f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0.LayoutWidth);
        YogaAssert.Equal(500f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(50f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(50f, root_child0_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child1.LayoutX);
        YogaAssert.Equal(50f, root_child0_child1.LayoutY);
        YogaAssert.Equal(50f, root_child0_child1.LayoutWidth);
        YogaAssert.Equal(100f, root_child0_child1.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child2.LayoutX);
        YogaAssert.Equal(150f, root_child0_child2.LayoutY);
        YogaAssert.Equal(50f, root_child0_child2.LayoutWidth);
        YogaAssert.Equal(25f, root_child0_child2.LayoutHeight);
    }

    [TestMethod]
    [Ignore("Skipped in upstream Yoga (GTEST_SKIP)")]
    public void Text_Max_Content_Width()
    {
        // TODO: GTEST_SKIP();
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(200f);
        var root_child0 = new YogaNode(config);
        root_child0.Width = new YogaValue(0, YogaUnit.MaxContent);
        root.InsertChild(root_child0, 0);
        var root_child0_child0 = new YogaNode(config);
        root_child0_child0.FlexDirection = FlexDirection.Row;
        root_child0.InsertChild(root_child0_child0, 0);
        root_child0_child0.Context = "Lorem ipsum sdafhasdfkjlasdhlkajsfhasldkfhasdlkahsdflkjasdhflaksdfasdlkjhasdlfjahsdfljkasdhalsdfhas dolor sit amet";
        root_child0_child0.MeasureFunction = IntrinsicSizeMeasureFunc;
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(200f, root.LayoutWidth);
        YogaAssert.Equal(10f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(1140f, root_child0.LayoutWidth);
        YogaAssert.Equal(10f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(1140f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(10f, root_child0_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(200f, root.LayoutWidth);
        YogaAssert.Equal(10f, root.LayoutHeight);
        YogaAssert.Equal(-940f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(1140f, root_child0.LayoutWidth);
        YogaAssert.Equal(10f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(1140f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(10f, root_child0_child0.LayoutHeight);
    }

    [TestMethod]
    [Ignore("Skipped in upstream Yoga (GTEST_SKIP)")]
    public void Text_Stretch_Width()
    {
        // TODO: GTEST_SKIP();
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(200f);
        var root_child0 = new YogaNode(config);
        root_child0.Width = new YogaValue(0, YogaUnit.Stretch);
        root.InsertChild(root_child0, 0);
        var root_child0_child0 = new YogaNode(config);
        root_child0_child0.FlexDirection = FlexDirection.Row;
        root_child0.InsertChild(root_child0_child0, 0);
        root_child0_child0.Context = "Lorem ipsum dolor sit amet";
        root_child0_child0.MeasureFunction = IntrinsicSizeMeasureFunc;
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(200f, root.LayoutWidth);
        YogaAssert.Equal(20f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(200f, root_child0.LayoutWidth);
        YogaAssert.Equal(20f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(200f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(20f, root_child0_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(200f, root.LayoutWidth);
        YogaAssert.Equal(20f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(200f, root_child0.LayoutWidth);
        YogaAssert.Equal(20f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(200f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(20f, root_child0_child0.LayoutHeight);
    }

    [TestMethod]
    [Ignore("Skipped in upstream Yoga (GTEST_SKIP)")]
    public void Text_Fit_Content_Width()
    {
        // TODO: GTEST_SKIP();
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(200f);
        var root_child0 = new YogaNode(config);
        root_child0.Width = new YogaValue(0, YogaUnit.FitContent);
        root.InsertChild(root_child0, 0);
        var root_child0_child0 = new YogaNode(config);
        root_child0_child0.FlexDirection = FlexDirection.Row;
        root_child0.InsertChild(root_child0_child0, 0);
        root_child0_child0.Context = "Lorem ipsum sdafhasdfkjlasdhlkajsfhasldkfhasdlkahsdflkjasdhflaksdfasdlkjhasdlfjahsdfljkasdhalsdfhas dolor sit amet";
        root_child0_child0.MeasureFunction = IntrinsicSizeMeasureFunc;
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(200f, root.LayoutWidth);
        YogaAssert.Equal(30f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(870f, root_child0.LayoutWidth);
        YogaAssert.Equal(30f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(870f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(30f, root_child0_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(200f, root.LayoutWidth);
        YogaAssert.Equal(30f, root.LayoutHeight);
        YogaAssert.Equal(-670f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(870f, root_child0.LayoutWidth);
        YogaAssert.Equal(30f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(870f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(30f, root_child0_child0.LayoutHeight);
    }

    [TestMethod]
    [Ignore("Skipped in upstream Yoga (GTEST_SKIP)")]
    public void Text_Max_Content_Min_Width()
    {
        // TODO: GTEST_SKIP();
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(200f);
        var root_child0 = new YogaNode(config);
        root_child0.MinWidth = new YogaValue(0, YogaUnit.MaxContent);
        root_child0.Width = YogaValue.Point(200f);
        root.InsertChild(root_child0, 0);
        var root_child0_child0 = new YogaNode(config);
        root_child0_child0.FlexDirection = FlexDirection.Row;
        root_child0.InsertChild(root_child0_child0, 0);
        root_child0_child0.Context = "Lorem ipsum sdafhasdfkjlasdhlkajsfhasldkfhasdlkahsdflkjasdhflaksdfasdlkjhasdlfjahsdfljkasdhalsdfhas dolor sit amet";
        root_child0_child0.MeasureFunction = IntrinsicSizeMeasureFunc;
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(200f, root.LayoutWidth);
        YogaAssert.Equal(10f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(1140f, root_child0.LayoutWidth);
        YogaAssert.Equal(10f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(1140f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(10f, root_child0_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(200f, root.LayoutWidth);
        YogaAssert.Equal(10f, root.LayoutHeight);
        YogaAssert.Equal(-940f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(1140f, root_child0.LayoutWidth);
        YogaAssert.Equal(10f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(1140f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(10f, root_child0_child0.LayoutHeight);
    }

    [TestMethod]
    [Ignore("Skipped in upstream Yoga (GTEST_SKIP)")]
    public void Text_Stretch_Min_Width()
    {
        // TODO: GTEST_SKIP();
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(200f);
        var root_child0 = new YogaNode(config);
        root_child0.MinWidth = new YogaValue(0, YogaUnit.Stretch);
        root_child0.Width = YogaValue.Point(100f);
        root.InsertChild(root_child0, 0);
        var root_child0_child0 = new YogaNode(config);
        root_child0_child0.FlexDirection = FlexDirection.Row;
        root_child0.InsertChild(root_child0_child0, 0);
        root_child0_child0.Context = "Lorem ipsum dolor sit amet";
        root_child0_child0.MeasureFunction = IntrinsicSizeMeasureFunc;
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(200f, root.LayoutWidth);
        YogaAssert.Equal(20f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(200f, root_child0.LayoutWidth);
        YogaAssert.Equal(20f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(200f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(20f, root_child0_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(200f, root.LayoutWidth);
        YogaAssert.Equal(20f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(200f, root_child0.LayoutWidth);
        YogaAssert.Equal(20f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(200f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(20f, root_child0_child0.LayoutHeight);
    }

    [TestMethod]
    [Ignore("Skipped in upstream Yoga (GTEST_SKIP)")]
    public void Text_Fit_Content_Min_Width()
    {
        // TODO: GTEST_SKIP();
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(200f);
        var root_child0 = new YogaNode(config);
        root_child0.MinWidth = new YogaValue(0, YogaUnit.FitContent);
        root_child0.Width = YogaValue.Point(300f);
        root.InsertChild(root_child0, 0);
        var root_child0_child0 = new YogaNode(config);
        root_child0_child0.FlexDirection = FlexDirection.Row;
        root_child0.InsertChild(root_child0_child0, 0);
        root_child0_child0.Context = "Lorem ipsum sdafhasdfkjlasdhlkajsfhasldkfhasdlkahsdflkjasdhflaksdfasdlkjhasdlfjahsdfljkasdhalsdfhas dolor sit amet";
        root_child0_child0.MeasureFunction = IntrinsicSizeMeasureFunc;
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(200f, root.LayoutWidth);
        YogaAssert.Equal(30f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(870f, root_child0.LayoutWidth);
        YogaAssert.Equal(30f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(870f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(30f, root_child0_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(200f, root.LayoutWidth);
        YogaAssert.Equal(30f, root.LayoutHeight);
        YogaAssert.Equal(-670f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(870f, root_child0.LayoutWidth);
        YogaAssert.Equal(30f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(870f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(30f, root_child0_child0.LayoutHeight);
    }

    [TestMethod]
    [Ignore("Skipped in upstream Yoga (GTEST_SKIP)")]
    public void Text_Max_Content_Max_Width()
    {
        // TODO: GTEST_SKIP();
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(200f);
        var root_child0 = new YogaNode(config);
        root_child0.MaxWidth = new YogaValue(0, YogaUnit.MaxContent);
        root_child0.Width = YogaValue.Point(2000f);
        root.InsertChild(root_child0, 0);
        var root_child0_child0 = new YogaNode(config);
        root_child0_child0.FlexDirection = FlexDirection.Row;
        root_child0.InsertChild(root_child0_child0, 0);
        root_child0_child0.Context = "Lorem ipsum sdafhasdfkjlasdhlkajsfhasldkfhasdlkahsdflkjasdhflaksdfasdlkjhasdlfjahsdfljkasdhalsdfhas dolor sit amet";
        root_child0_child0.MeasureFunction = IntrinsicSizeMeasureFunc;
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(200f, root.LayoutWidth);
        YogaAssert.Equal(10f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(1140f, root_child0.LayoutWidth);
        YogaAssert.Equal(10f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(1140f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(10f, root_child0_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(200f, root.LayoutWidth);
        YogaAssert.Equal(10f, root.LayoutHeight);
        YogaAssert.Equal(-940f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(1140f, root_child0.LayoutWidth);
        YogaAssert.Equal(10f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(1140f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(10f, root_child0_child0.LayoutHeight);
    }

    [TestMethod]
    [Ignore("Skipped in upstream Yoga (GTEST_SKIP)")]
    public void Text_Stretch_Max_Width()
    {
        // TODO: GTEST_SKIP();
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(200f);
        var root_child0 = new YogaNode(config);
        root_child0.MaxWidth = new YogaValue(0, YogaUnit.Stretch);
        root_child0.Width = YogaValue.Point(300f);
        root.InsertChild(root_child0, 0);
        var root_child0_child0 = new YogaNode(config);
        root_child0_child0.FlexDirection = FlexDirection.Row;
        root_child0.InsertChild(root_child0_child0, 0);
        root_child0_child0.Context = "Lorem ipsum dolor sit amet";
        root_child0_child0.MeasureFunction = IntrinsicSizeMeasureFunc;
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(200f, root.LayoutWidth);
        YogaAssert.Equal(20f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(200f, root_child0.LayoutWidth);
        YogaAssert.Equal(20f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(200f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(20f, root_child0_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(200f, root.LayoutWidth);
        YogaAssert.Equal(20f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(200f, root_child0.LayoutWidth);
        YogaAssert.Equal(20f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(200f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(20f, root_child0_child0.LayoutHeight);
    }

    [TestMethod]
    [Ignore("Skipped in upstream Yoga (GTEST_SKIP)")]
    public void Text_Fit_Content_Max_Width()
    {
        // TODO: GTEST_SKIP();
        var config = new YogaConfig();
        var root = new YogaNode(config);
        root.PositionType = FlexPositionType.Absolute;
        root.Width = YogaValue.Point(200f);
        var root_child0 = new YogaNode(config);
        root_child0.MaxWidth = new YogaValue(0, YogaUnit.FitContent);
        root_child0.Width = YogaValue.Point(1000f);
        root.InsertChild(root_child0, 0);
        var root_child0_child0 = new YogaNode(config);
        root_child0_child0.FlexDirection = FlexDirection.Row;
        root_child0.InsertChild(root_child0_child0, 0);
        root_child0_child0.Context = "Lorem ipsum sdafhasdfkjlasdhlkajsfhasldkfhasdlkahsdflkjasdhflaksdfasdlkjhasdlfjahsdfljkasdhalsdfhas dolor sit amet";
        root_child0_child0.MeasureFunction = IntrinsicSizeMeasureFunc;
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.LeftToRight);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(200f, root.LayoutWidth);
        YogaAssert.Equal(30f, root.LayoutHeight);
        YogaAssert.Equal(0f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(870f, root_child0.LayoutWidth);
        YogaAssert.Equal(30f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(870f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(30f, root_child0_child0.LayoutHeight);
        root.CalculateLayout(float.NaN, float.NaN, FlexLayoutDirection.RightToLeft);
        YogaAssert.Equal(0f, root.LayoutX);
        YogaAssert.Equal(0f, root.LayoutY);
        YogaAssert.Equal(200f, root.LayoutWidth);
        YogaAssert.Equal(30f, root.LayoutHeight);
        YogaAssert.Equal(-670f, root_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0.LayoutY);
        YogaAssert.Equal(870f, root_child0.LayoutWidth);
        YogaAssert.Equal(30f, root_child0.LayoutHeight);
        YogaAssert.Equal(0f, root_child0_child0.LayoutX);
        YogaAssert.Equal(0f, root_child0_child0.LayoutY);
        YogaAssert.Equal(870f, root_child0_child0.LayoutWidth);
        YogaAssert.Equal(30f, root_child0_child0.LayoutHeight);
    }


    private static YogaSize IntrinsicSizeMeasureFunc(
        YogaNode node, float width, YogaMeasureMode widthMode,
        float height, YogaMeasureMode heightMode)
    {
        string text = (string)node.Context!;
        float widthPerChar = 10f;
        float heightPerChar = 10f;

        float measuredWidth;
        if (widthMode == YogaMeasureMode.Exactly)
            measuredWidth = width;
        else if (widthMode == YogaMeasureMode.AtMost)
            measuredWidth = Math.Min(text.Length * widthPerChar, width);
        else
            measuredWidth = text.Length * widthPerChar;

        float measuredHeight;
        float effectiveWidth = node.FlexDirection == FlexDirection.Column
            ? measuredWidth
            : Math.Max(LongestWordWidth(text, widthPerChar), measuredWidth);

        if (heightMode == YogaMeasureMode.Exactly)
            measuredHeight = height;
        else
        {
            float calcHeight = CalculateTextHeight(text, effectiveWidth, widthPerChar, heightPerChar);
            measuredHeight = heightMode == YogaMeasureMode.AtMost
                ? Math.Min(calcHeight, height)
                : calcHeight;
        }

        return new YogaSize(measuredWidth, measuredHeight);
    }

    private static float LongestWordWidth(string text, float widthPerChar)
    {
        int maxLen = 0, curLen = 0;
        foreach (char c in text)
        {
            if (c == ' ') { maxLen = Math.Max(curLen, maxLen); curLen = 0; }
            else curLen++;
        }
        return Math.Max(curLen, maxLen) * widthPerChar;
    }

    private static float CalculateTextHeight(string text, float measuredWidth, float widthPerChar, float heightPerChar)
    {
        if (text.Length * widthPerChar <= measuredWidth) return heightPerChar;
        var words = text.Split(' ');
        float lines = 1, curLineLen = 0;
        foreach (var word in words)
        {
            float wordWidth = word.Length * widthPerChar;
            if (wordWidth > measuredWidth)
            {
                if (curLineLen > 0) lines++;
                lines++;
                curLineLen = 0;
            }
            else if (curLineLen + wordWidth <= measuredWidth)
            {
                curLineLen += wordWidth + widthPerChar;
            }
            else
            {
                lines++;
                curLineLen = wordWidth + widthPerChar;
            }
        }
        return (curLineLen == 0 ? lines - 1 : lines) * heightPerChar;
    }
}
