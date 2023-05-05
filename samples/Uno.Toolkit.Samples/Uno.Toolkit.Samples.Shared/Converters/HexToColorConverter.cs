using System;

using Windows.UI;

#if true
using Microsoft.UI.Xaml.Data;
using Microsoft.UI;
#else
using Windows.UI.Xaml.Data;
using Windows.UI;
#endif

namespace Uno.Toolkit.Samples.Converters
{
    public class HexToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is not String hex || !hex.StartsWith("#"))
            {
                return value;
            }

            hex = hex.Replace("#", string.Empty);
            byte a = (byte)(System.Convert.ToUInt32(hex.Substring(0, 2), 16));
            byte r = (byte)(System.Convert.ToUInt32(hex.Substring(2, 2), 16));
            byte g = (byte)(System.Convert.ToUInt32(hex.Substring(4, 2), 16));
            byte b = (byte)(System.Convert.ToUInt32(hex.Substring(6, 2), 16));

            return Color.FromArgb(a, r, g, b);
        }
    }
}
