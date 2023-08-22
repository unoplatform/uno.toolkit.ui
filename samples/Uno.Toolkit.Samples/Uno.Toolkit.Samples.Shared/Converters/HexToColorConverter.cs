using System;

using Windows.UI;

#if IS_WINUI
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Markup;
#else
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
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
			if (value is not string { Length: 9 } hex || !hex.StartsWith("#"))
			{
				return value;
			}

			return XamlBindingHelper.ConvertValue(typeof(Color), hex);
		}
	}
}
