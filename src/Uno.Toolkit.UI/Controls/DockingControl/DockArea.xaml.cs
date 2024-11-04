using Microsoft.UI;
using Windows.UI;
using System;
using Windows.ApplicationModel.DataTransfer;

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

public sealed partial class DockArea : UserControl
{
    public DockArea()
    {
        this.InitializeComponent();

        Random random = new();
        byte r = (byte)random.Next(256);
        byte g = (byte)random.Next(256);
        byte b = (byte)random.Next(256);
        PanelContainer.BorderBrush = new SolidColorBrush(Color.FromArgb(255, r, g, b));

        HideDragIndicator();
    }

    public void AddPanel(TabViewItem panel)
    {
        PanelContainer.TabItems.Add(panel);
    }

    public bool ContainsPanel(TabViewItem panel)
    {
        return PanelContainer.TabItems.Contains(panel);
    }

    public void RemovePanel(TabViewItem panel)
    {
        PanelContainer.TabItems.Remove(panel);
    }

    public void ShowDragIndicator()
    {
        PanelContainer.Background = PanelContainer.BorderBrush;
        PanelContainer.Background.Opacity = 0.2;

        DockHelperStar.Visibility = Visibility.Visible;
    }

    public void HideDragIndicator()
    {
        PanelContainer.Background = new SolidColorBrush(Colors.Transparent);

        DockHelperStar.Visibility = Visibility.Collapsed;
    }

    public bool IsEmpty()
        => PanelContainer.TabItems.Count == 0;
}
