//CREATED: 2014-06-03

using System;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;

namespace KsWare.Presentation.Converters {

	public class StringJoinConverter : MarkupExtension, IMultiValueConverter {

		public static readonly StringJoinConverter Default = new StringJoinConverter();
		public string Separator { get; set; } = "";

		public StringJoinConverter() {
			
		}

		public StringJoinConverter(string separator) {
			Separator = separator;
		}

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
			if (targetType == null || targetType == typeof(object)) targetType = typeof(string);
			if (targetType != typeof(string)) throw new ArgumentOutOfRangeException(nameof(targetType), $"Conversion not supported. TargetType: {targetType.FullName}");
			if (values == null) return null;

			var separator = Separator ?? "";

			var sb = new StringBuilder();
			for (int i = 0; i < values.Length; i++) {
				if (i > 0) sb.Append(separator);
				sb.AppendFormat(culture, "{0}", values[i]);
			}

			return sb.ToString();
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
			throw new NotSupportedException($"ConvertBack ist not supported by {nameof(StringJoinConverter)}.");
		}

		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}

	}

}
