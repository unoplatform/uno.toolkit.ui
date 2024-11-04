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

public sealed partial class DockHelper : UserControl
{
    public DockHelper()
    {
        this.InitializeComponent();
    }

    public Dock? DockPosition
    {
        get { return (Dock?)GetValue(DockPositionProperty); }
        set { SetValue(DockPositionProperty, value); }
    }

    public static readonly DependencyProperty DockPositionProperty =
        DependencyProperty.Register("DockPosition", typeof(Dock?), typeof(DockHelper), null);
}
