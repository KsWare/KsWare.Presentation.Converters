using System;
using System.Globalization;
using System.Windows.Data;

namespace KsWare.Presentation.Converters {


	/// <summary> Provides a <see cref="IValueConverter"/> to convert a binding value into its type name.
	/// </summary>
	/// <example>Shows the current binding result:
	/// <code>
	/// &lt;TextBlock Text="{Binding ., Converter={x:Static viewFramework:TypeNameConverter.Default}}" /&gt;
	/// </code></example>
	public class TypeNameConverter:ValueConverterBase {

		public static readonly TypeNameConverter Default = new TypeNameConverter();

		public TypeNameConverter() { }

		public TypeNameConverter(bool fullName) {
			FullName = fullName;
		}

		public TypeNameConverter(bool fullName, bool encloseInCurlyBrackets) {
			FullName = fullName;
			EncloseInCurlyBrackets = encloseInCurlyBrackets;
		}

		public bool FullName { get; set; } = true;

		public bool EncloseInCurlyBrackets { get; set; } = false;

		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return FormatTypeName(value, FullName, EncloseInCurlyBrackets);
		}

		public static string FormatTypeName(object o, bool fullName, bool encloseInCurlyBrackets) {
			string n;
			if (o == null) {
				n = "Null";
			}
			else {
				Type t = o is Type ? (Type)o : o.GetType();
				if (!t.IsGenericType) {
					n = t.Name;
				}
				else {
					n = t.Name.Split('`')[0];
					n += "<";
					var a = t.GetGenericArguments();
					foreach (var at in a) {
						if (n[n.Length - 1] != '<') n += ",";
						n += FormatTypeName(at, false, false); //?? always short name in generic parameters?
					}
					n += ">";
				}
				if (fullName) {
					n = t.Namespace + "." + n; //TODO support also nested types
				}
			}
			if (encloseInCurlyBrackets) n = "{" + n + "}";
			return n;
		}
	}
}