//disable nullable for the Prototype of the control
#nullable disable

using Microsoft.UI;
using System.Collections.Generic;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using System;


#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Navigation;
using XamlWindow = Microsoft.UI.Xaml.Window;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ViewManagement;
using XamlWindow = Windows.UI.Xaml.Window;

#endif

namespace Uno.Toolkit.UI;

public sealed partial class DockingControl : UserControl
{
    private Point _startPointerPosition;
    private TranslateTransform _transform;

    private bool _isDragging;
    private TabViewItem _currentDraggingPanel;
    private int _panelCount;

    private List<DockArea> _dockingAreas = new();
    private Window _secondaryWindow;

    public DockingControl()
    {
        this.InitializeComponent();

        CreateNewDockArea(Dock.Right);
    }

    private DockArea CreateNewDockArea(Dock position, DockArea triggeringDockArea = null, int recommendedWidthOrHeight = 0)
    {
        DockArea newDockArea = new()
        {
            Name = $"DockArea_{_dockingAreas.Count + 1}",
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };

        if (recommendedWidthOrHeight > 0)
        {
            if (position == Dock.Left || position == Dock.Right)
            {
                newDockArea.Width = recommendedWidthOrHeight;
            }
            else
            {
                newDockArea.Height = recommendedWidthOrHeight;
            }
        }

        int index = DCHolder.Children.IndexOf(triggeringDockArea);
        if (index != -1)
        {
            if (position == Dock.Left || position == Dock.Top)
            {
                DCHolder.Children.Insert(index, newDockArea);
            }
            else if (position == Dock.Right || position == Dock.Bottom)
            {
                DCHolder.Children.Insert(index + 1, newDockArea);
            }
        }
        else
        {
            DCHolder.Children.Add(newDockArea);
        }

        DockPanel.SetDock(newDockArea, position);
        _dockingAreas.Add(newDockArea);
        DCHolder.UpdateLayout();

        return newDockArea;
    }

    private void OnAddPanelClicked(object sender, RoutedEventArgs e)
    {
        var panelName = $"DraggablePanel_{_panelCount++}";
        TabViewItem newPanel = new()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Background = new SolidColorBrush(Colors.Blue),
            Name = panelName,
            CanDrag = true,
            IsHitTestVisible = true,
            Header = new Border()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Background = new SolidColorBrush(Colors.Red),
                Child = new TextBlock() { Text = panelName }
            },

            Content = new Border()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Background = new SolidColorBrush(Colors.Purple),
                Child = new TextBlock() { Text = panelName }
            }
        };

        newPanel.DragStarting += NewPanel_DragStarting;
        newPanel.PointerMoved += OnPointerMoved;
        newPanel.DropCompleted += NewPanel_DropCompleted;

        (newPanel.Header as Border)!.PointerPressed += (s, e) =>
        {
            _startPointerPosition = e.GetCurrentPoint(this).Position;
        };

        var dockArea = GetClosestDockArea(new Point(0, 0));
        dockArea.AddPanel(newPanel);
    }

    private void NewPanel_DragStarting(UIElement sender, DragStartingEventArgs args)
    {
        _currentDraggingPanel = sender as TabViewItem;

        if (_currentDraggingPanel == null || _isDragging)
        {
            return;
        }

        if (_currentDraggingPanel.RenderTransform == null || _currentDraggingPanel.RenderTransform is not TranslateTransform)
        {
            _transform = new TranslateTransform();
            _currentDraggingPanel.RenderTransform = _transform;
        }
        else
        {
            _transform = (TranslateTransform)_currentDraggingPanel.RenderTransform;
        }

        _currentDraggingPanel.Opacity = 0.4;
        _isDragging = true;

        var panelIdentifier = _currentDraggingPanel.Header.ToString();
        args.Data.SetData("UNODataFormat", panelIdentifier);

        args.AllowedOperations = DataPackageOperation.Move;
        args.Data.RequestedOperation = DataPackageOperation.Move;
    }

    private void OnPointerMoved(object sender, PointerRoutedEventArgs e)
    {
        if (_isDragging && _currentDraggingPanel != null)
        {
            Point currentPointerPosition = e.GetCurrentPoint(this).Position;
            double offsetX = currentPointerPosition.X - _startPointerPosition.X;
            double offsetY = currentPointerPosition.Y - _startPointerPosition.Y;

            _transform.X += offsetX;
            _transform.Y += offsetY;

            _startPointerPosition = currentPointerPosition;

            if (IsPointerOutsideAppWindow(_startPointerPosition))
            {
                if (_currentDraggingPanel.PointerCaptures?.Count > 0)
                {
                    _currentDraggingPanel.ReleasePointerCaptures();
                }

                OpenNewWindowWithPanel(_currentDraggingPanel);
                var currentDockArea = FindDockAreaContainingPanel(_currentDraggingPanel);
                RemovePanelFromArea(currentDockArea, _currentDraggingPanel);
                HideDropIndicators();

                _isDragging = false;
            }
            else
            {
                ShowDropIndicator(currentPointerPosition);
            }
        }
    }

    private void NewPanel_DropCompleted(UIElement sender, DropCompletedEventArgs args)
    {
        if (_isDragging && _currentDraggingPanel != null)
        {
            _isDragging = false;
            _currentDraggingPanel.Opacity = 1.0;

            if (_currentDraggingPanel.PointerCaptures?.Count > 0)
            {
                _currentDraggingPanel.ReleasePointerCaptures();
            }

            HandleDragInsideOrBackToMainWindow(_startPointerPosition);

            _currentDraggingPanel.RenderTransform = new TranslateTransform();
        }

        _currentDraggingPanel = null;
    }

    private void OpenNewWindowWithPanel(TabViewItem panel)
    {
        _secondaryWindow = new Window
        {
            Title = panel.Name.ToLower(),
            //ExtendsContentIntoTitleBar = true
        };

        var newContent = new Grid
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
        };

        newContent.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
        newContent.RowDefinitions.Add(new RowDefinition());

        //Find the tabbar and remove its child
        if (panel.Parent is Panel parentPanel)
        {
            parentPanel.Children.Remove(panel);
        }

        newContent.Children.Add(new TextBlock { Text = _secondaryWindow.Title });

        var exitButton = new Button
        {
            Content = "Close",
            Background = new SolidColorBrush(Colors.Bisque),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
        };

        exitButton.Click += (s, e) => { _secondaryWindow.Close(); };
        newContent.Children.Add(panel.Content as Border);
        newContent.Children.Add(new Button());
        Grid.SetRow(exitButton, 2);
        Grid.SetRow(panel.Content as Border, 2);

        _secondaryWindow.Content = newContent;
        _secondaryWindow.Activate();
    }

    private void HandleDragInsideOrBackToMainWindow(Point pointerPosition)
    {
        var currentDockArea = FindDockAreaContainingPanel(_currentDraggingPanel);
        bool isInSecondaryWindow = _currentDraggingPanel.Parent is Grid && _secondaryWindow != null;

        if (currentDockArea != null)
        {
            HandleDragInsideMainWindow(pointerPosition, currentDockArea);
        }
        else
        {
            DockArea closestDockArea = GetClosestDockArea(pointerPosition);
            if (closestDockArea != null)
            {
                HandleDragInsideMainWindow(pointerPosition, closestDockArea);

                if (isInSecondaryWindow)
                {
                    _secondaryWindow?.Close();
                    _secondaryWindow = null;
                }
            }
        }

        HideDropIndicators();
    }

    private void HandleDragInsideMainWindow(Point pointerPosition, DockArea currentDockArea)
    {
        var dockHelpers = FindDockHelpersInVisualTree(currentDockArea);
        bool newDockAreaCreated = false;

        foreach (var dockHelper in dockHelpers)
        {
            GeneralTransform transform = dockHelper.TransformToVisual(this);
            Point dockHelperPosition = transform.TransformPoint(new Point(0, 0));
            Rect dockHelperBounds = new Rect(dockHelperPosition.X, dockHelperPosition.Y,
                                              dockHelper.ActualWidth, dockHelper.ActualHeight);

            if (dockHelperBounds.Contains(pointerPosition))
            {
                if (dockHelper.DockPosition != null)
                {
                    RemovePanelFromArea(currentDockArea, _currentDraggingPanel);
                    var newDockArea = CreateNewDockArea((Dock)dockHelper.DockPosition, currentDockArea, 350);
                    SnapToGrid(_currentDraggingPanel, newDockArea);
                    newDockAreaCreated = true;
                }

                HideDropIndicators();
                return;
            }
        }

        if (!newDockAreaCreated)
        {
            var closestArea = GetClosestDockArea(pointerPosition);
            SnapToGrid(_currentDraggingPanel, closestArea);
        }

        HideDropIndicators();
    }

    private void SnapToGrid(TabViewItem panel, DockArea snapArea)
    {
        if (snapArea != null && panel.Parent != snapArea)
        {
            var currentDockArea = FindDockAreaContainingPanel(panel);
            RemovePanelFromArea(currentDockArea, panel);
            snapArea.AddPanel(panel);
        }
        panel.RenderTransform = new TranslateTransform();
    }

    private DockArea FindDockAreaContainingPanel(TabViewItem panel)
    {
        foreach (var area in _dockingAreas)
        {
            if (area.ContainsPanel(panel))
            {
                return area;
            }
        }
        return null;
    }

    private bool IsPointerOutsideAppWindow(Point pointerPosition)
        => (pointerPosition.X < 0 || pointerPosition.Y < 0 ||
            pointerPosition.X > ActualWidth || pointerPosition.Y > ActualHeight);

    private DockArea GetClosestDockArea(Point pointerPosition)
    {
        DockArea closestArea = null;
        double closestDistance = double.MaxValue;

        foreach (var area in _dockingAreas)
        {
            var areaTransform = area.TransformToVisual(this);
            Point areaPosition = areaTransform.TransformPoint(new Point(0, 0));
            var areaRect = new Rect(areaPosition.X, areaPosition.Y, area.ActualWidth, area.ActualHeight);

            if (areaRect.Contains(pointerPosition))
            {
                closestArea = area;
                break;
            }

            var areaCenter = new Point(areaRect.Left + areaRect.Width / 2, areaRect.Top + areaRect.Height / 2);
            double distance = Math.Sqrt(Math.Pow(areaCenter.X - pointerPosition.X, 2) + Math.Pow(areaCenter.Y - pointerPosition.Y, 2));
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestArea = area;
            }
        }

        return closestArea;
    }

    private void ShowDropIndicator(Point pointerPosition)
    {
        var closestArea = GetClosestDockArea(pointerPosition);
        HideDropIndicators();
        closestArea?.ShowDragIndicator();
    }

    private void HideDropIndicators()
    {
        foreach (var area in _dockingAreas)
        {
            area.HideDragIndicator();
        }
    }

    private static List<DockHelper> FindDockHelpersInVisualTree(DependencyObject parent)
    {
        var dockHelpers = new List<DockHelper>();
        int childCount = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < childCount; i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is DockHelper dockHelper)
            {
                dockHelpers.Add(dockHelper);
            }
            dockHelpers.AddRange(FindDockHelpersInVisualTree(child));
        }
        return dockHelpers;
    }

    private void RemovePanelFromArea(DockArea dockArea, TabViewItem panel)
    {
        if (dockArea is not { })
        {
            return;
        }

        dockArea.RemovePanel(panel);

        if (dockArea.IsEmpty() && _dockingAreas.Count > 1)
        {
            _dockingAreas.Remove(dockArea);
            DCHolder.Children.Remove(dockArea);
        }
    }
}
