using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if IS_WINUI
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#endif

namespace Uno.UI.ToolkitLib.Extensions
{
	internal static class VisualTreeHelperEx
	{
        public static T FindChild<T>(this DependencyObject depObj)
            where T : DependencyObject
        {
            if (depObj == null) return default(T);

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                
                var result = (child is T t) ? t : FindChild<T>(child);
                if (result != null) return result;
            }
            return default(T);
        }
    }
}
