using System;
using System.Collections.Generic;
using System.Text;
#if IS_WINUI
using Microsoft.UI.Xaml.Data;
#else
using Windows.UI.Xaml.Data;
#endif

namespace Uno.Toolkit.Samples.Converters
{
	public class FromStringToValueConverter : IValueConverter
	{
		public enum CheckMethod { IsNullOrEmpty, IsNullOrWhitespace }


		public CheckMethod Check { get; set; }

		public object TrueValue { get; set; }

		public object FalseValue { get; set; }

		public object Convert(object value, Type targetType, object parameter, string language) =>
			!(value is string text) || (Check == CheckMethod.IsNullOrEmpty ? string.IsNullOrEmpty(text) : string.IsNullOrWhiteSpace(text))
				? TrueValue
				: FalseValue;

		public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotSupportedException("Only one-way conversion is supported.");
	}
}
