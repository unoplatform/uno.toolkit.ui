

using Microsoft.UI.Xaml.Data;


namespace Uno.Toolkit.Samples.Converters
{
	public class FromNullToValueConverter : IValueConverter
	{
		public object NullValue { get; set; }

		public object NotNullValue { get; set; }

		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value == null || value == DependencyProperty.UnsetValue)
			{
				return NullValue;
			}

			return NotNullValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotSupportedException();
		}
	}
}
