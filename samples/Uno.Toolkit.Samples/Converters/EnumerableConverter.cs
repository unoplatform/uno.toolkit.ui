using System.Collections;

using Microsoft.UI.Xaml.Data;


namespace Uno.Toolkit.Samples.Converters;

public class EnumerableConverter : IValueConverter
{
	public enum ConvertMode { StringJoin }

	public ConvertMode Mode { get; set; }

	public object Convert(object value, Type targetType, object parameter, string language)
	{
		return Mode switch
		{
			ConvertMode.StringJoin => string.Join(", ", GetSource()),

			_ => throw new NotImplementedException($"Unknown mode: {Mode}"),
		};

		IEnumerable<object> GetSource() => value is null
			? Enumerable.Empty<object>()
			: (value as IEnumerable)?.Cast<object>() ?? throw new ArgumentException($"The convert value is not an enumerable. Type={value.GetType().Name}");
	}

	public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotSupportedException("Only one-way conversion is supported.");
}
