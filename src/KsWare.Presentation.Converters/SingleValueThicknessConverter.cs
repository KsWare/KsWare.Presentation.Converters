using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;

namespace KsWare.Presentation.Converters {

	/// <summary> Provides a thickness converter to convert a single numeric value to a thickness.
	/// </summary>
	/// <example>
	/// Converts a double value to negative top thickness (Parameter: "-Top")
	/// equivalent to <c>new Thickness(0, -value, 0, 0)</c>
	/// <code>
	/// &gt;Control Margin="{Binding Distance, Converter={SingleValueThicknessConverter -Top}}"/>
	/// </code>
	/// </example>
	/// <remarks>
	/// </remarks>
	public class SingleValueThicknessConverter : ValueConverterBase {

		private static readonly IFormatProvider enus=CultureInfo.CreateSpecificCulture("en-US");
		private static readonly ThicknessConverter converter=new ThicknessConverter();

		public SingleValueThicknessConverter(string parameter) {
			Parameter=parameter;
		}

		public string Parameter { get; set; }

		/// <summary> Converts a numeric value value to <see cref="Thickness"/>.
		/// </summary>
		/// <param name="value">The value produced by the binding source.</param>
		/// <param name="targetType">The type of the binding target property.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>
		/// A converted value. If the method returns null, the valid null value is used.
		/// </returns>
		[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			double v;
			if     (value is string){v=double.Parse((string)value, enus);}
			else if(value is double){v=(double)value;}
			else if(value is float ){v=(float )value;}
			else if(value is long  ){v=(long  )value;}
			else if(value is int   ){v=(int   )value;}
			else if(value is short ){v=(short )value;}
			else if(value is byte  ){v=(byte  )value;}
			else if(value is sbyte ){v=(sbyte )value;}
			else if(value is ushort){v=(ushort)value;}
			else if(value is uint  ){v=(uint  )value;}
			else if(value is ulong ){v=(ulong )value;}
			else if(value is Decimal ){v=Decimal.ToDouble((Decimal)value);}
			else {v = 0; Debug.WriteLine("WARNING: Invalid type of value! "+GetType().FullName);}

			string p;
			if (!string.IsNullOrEmpty(Parameter)) p = Parameter;
			else if (parameter is string s && !string.IsNullOrEmpty(s)) p = s;
			else p = "* * * *";

			switch (p) {
				case "Left"   : return new Thickness(v, 0, 0, 0);
				case "Top"    : return new Thickness(0, v, 0, 0);
				case "Right"  : return new Thickness(0, 0, v, 0);
				case "Bottom" : return new Thickness(0, 0, 0, v);
				case "-Left"  : return new Thickness(-v, 0, 0, 0);
				case "-Top"   : return new Thickness(0, -v, 0, 0);
				case "-Right" : return new Thickness(0, 0, -v, 0);
				case "-Bottom": return new Thickness(0, 0, 0, -v);
				case string _ when p.Contains("*"): // ConverterParameter='0 * 0 0'
					return converter.ConvertFromString(p.Replace("*", v.ToString(enus)));
				// TODO not documented formats
				case string _ when p.Contains("{0}"): // ConverterParameter='0 -{0} 0 0'
					var match = Regex.Match(p, @"(\{)((?>\{(?<d>)|\}(?<-d>)|.?)*(?(d)(?!)))(\})", RegexOptions.Compiled);
					p = p.Replace(match.Value, v.ToString(enus));
					return converter.ConvertFromString(p);
				// case string _ when p.Contains("{0:}"): // ConverterParameter='0 -{0:+23} 0 0'  <-- Not implemented
				default: 
					throw new InvalidOperationException($"Conversion with the specified parameter is not supported. Parameter: '{p}'");

			}
		}

	}
 
}