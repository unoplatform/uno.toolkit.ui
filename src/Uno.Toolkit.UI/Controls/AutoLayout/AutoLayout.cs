#if IS_WINUI
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.UI;

public sealed partial class AutoLayout : RelativePanel
{
}
