using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace KsWare.Presentation.Converters {

	public abstract class ValueConverterBase : MarkupExtension, IValueConverter {

		public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

		public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException($"ConvertBack is not supported for {this.GetType().Name}.");
		}

		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}

	}

}
