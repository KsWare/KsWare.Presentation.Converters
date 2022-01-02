using System;
using System.Globalization;

namespace KsWare.Presentation.Converters {

	/// <summary>
	/// Class StringConverter. Converts anything to a string.
	/// </summary>
	public class StringConverter : ValueConverterBase {

		/// <summary>
		/// The default <see cref="StringConverter"/>. Uses string.Format("{0}")
		/// </summary>
		public static readonly StringConverter Default = new StringConverter();

		public StringConverter() { }

		public StringConverter(string stringFormat) {
			StringFormat = stringFormat;
		}

		/// <summary>
		/// Gets or sets the string format.
		/// </summary>
		/// <value>The string format to use in the converter.</value>
		public string StringFormat { get; set; }

		/// <summary>
		/// Gets or sets the culture.
		/// </summary>
		/// <value>The culture to use in the converter.</value>
		/// <remarks>If specified, this value overrides the 'culture' parameter from convert method.</remarks>
		public CultureInfo Culture { get; set; }

		/// <summary>
		/// Converts a value.
		/// </summary>
		/// <param name="value">The value produced by the binding source.</param>
		/// <param name="targetType">The type of the binding target property.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>A converted value. If the method returns <see langword="null" />, the valid null value is used.</returns>
		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			//return internalConverter.ConvertFrom(value); BUG??! cannot convert from System.Int32
			
			string format;
			if (parameter is string s && !string.IsNullOrEmpty(s)) format = s;
			else if (!string.IsNullOrEmpty(StringFormat)) format = StringFormat;
			else format = "";
			if (string.IsNullOrEmpty(format)) format = "{0}";
			else if (!format.Contains("{0")) format = "{0:"+format+"}";

			culture = Culture ?? culture;

			return string.Format(culture, format, value);
		}

		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}

	}

}
