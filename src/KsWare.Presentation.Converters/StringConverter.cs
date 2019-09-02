using System;
using System.Globalization;
using System.Windows.Data;

namespace KsWare.Presentation.Converters {

	/// <summary>
	/// Class StringConverter.
	/// Implements the <see cref="System.Windows.Data.IValueConverter" />
	/// </summary>
	/// <seealso cref="System.Windows.Data.IValueConverter" />
	public class StringConverter:IValueConverter {

		/// <summary>
		/// The default StringConverter.
		/// </summary>
		public static readonly StringConverter Default = new StringConverter();

		/// <summary>
		/// The internal converter
		/// </summary>
		private static readonly System.ComponentModel.StringConverter internalConverter = new System.ComponentModel.StringConverter();


		/// <summary>
		/// Converts a value.
		/// </summary>
		/// <param name="value">The value produced by the binding source.</param>
		/// <param name="targetType">The type of the binding target property.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>A converted value. If the method returns <see langword="null" />, the valid null value is used.</returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			//return internalConverter.ConvertFrom(value); BUG??! cannot convert from System.Int32
			return string.Format(culture, "{0}",value);
		}

		/// <summary>
		/// Converts a value.
		/// </summary>
		/// <param name="value">The value that is produced by the binding target.</param>
		/// <param name="targetType">The type to convert to.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>A converted value. If the method returns <see langword="null" />, the valid null value is used.</returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
