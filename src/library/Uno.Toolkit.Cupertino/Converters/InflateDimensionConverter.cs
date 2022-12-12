using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if IS_WINUI
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;
#endif

namespace Uno.Toolkit.UI
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Converters")]
	internal class InflateDimensionConverter : IValueConverter
	{
		public double Inflation { get; set; }
		public double DefaultValue { get; set; }

		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (double.TryParse(value.ToString(), out var dimension))
			{
				var result = dimension + Inflation;
				if (result >= 0)
				{
					return result;
				}
			}

			return DefaultValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}
