using System;
using System.ComponentModel;
using System.Globalization;

namespace KsWare.Presentation.Converters {

	public class BooleanComparingConverter:ValueConverterBase {

		private static readonly DoubleConverter DoubleConverter=new DoubleConverter();

		/// <summary>
		/// Gets the default BooleanComparingConverter.Expects operator in parameter. 
		/// [eq, ne, gt, lt, ge, le, is null, not null]
		/// </summary>
		public static readonly BooleanComparingConverter Default=new BooleanComparingConverter(CompareOperation.InParameter);

		public static readonly BooleanComparingConverter Equal=new BooleanComparingConverter(CompareOperation.Equal);
		public static readonly BooleanComparingConverter NotEqual=new BooleanComparingConverter(CompareOperation.Equal);

		public static readonly BooleanComparingConverter LessThen=new BooleanComparingConverter(CompareOperation.LessThen);
		public static readonly BooleanComparingConverter GreaterThan=new BooleanComparingConverter(CompareOperation.GreaterThan);
		public static readonly BooleanComparingConverter LessThenOrEqual=new BooleanComparingConverter(CompareOperation.LessThenOrEqual);
		public static readonly BooleanComparingConverter GreaterThenOrEqual=new BooleanComparingConverter(CompareOperation.GreaterThanOrEqual);
		public static readonly BooleanComparingConverter IsNull=new BooleanComparingConverter(CompareOperation.IsNull);
		public static readonly BooleanComparingConverter IsNotNull=new BooleanComparingConverter(CompareOperation.IsNotNull);

//		public static readonly BooleanComparingConverter StartsWith=new BooleanComparingConverter(CompareOperation.StartsWith);
//		public static readonly BooleanComparingConverter EndsWith=new BooleanComparingConverter(CompareOperation.EndsWith);
//		public static readonly BooleanComparingConverter Contains=new BooleanComparingConverter(CompareOperation.Contains);
//		public static readonly BooleanComparingConverter IsNullOrEmpty=new BooleanComparingConverter(CompareOperation.IsNullOrEmpty);
//		public static readonly BooleanComparingConverter IsNullOrWhitespace=new BooleanComparingConverter(CompareOperation.IsNullOrWhitespace);

		public enum CompareOperation {
			None,
			InParameter,

			Equal,		// eq
			NotEqual,	// ne

			LessThen,	//lt
			GreaterThan,	// gt
			LessThenOrEqual,	// le
			GreaterThanOrEqual, // ge
			IsNull,		// eq null
			IsNotNull,	// ne null

//			StartsWith,				// ssw 
//			EndsWith,				// sew
//			Contains,				// scs
//			IsNullOrEmpty,			// sne
//			IsNullOrWhitespace,		// snw
		}

		private CompareOperation Operation { get; set; }

		public BooleanComparingConverter() {
			Operation=CompareOperation.Equal;
		}

		public BooleanComparingConverter(CompareOperation operation) {
			Operation = operation;
		}

		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value==null     ) return CompareObject (value, targetType, parameter, culture);
			if (IsNumeric(value)) return CompareNumeric(value, targetType, parameter, culture);
//			if (value is string ) return CompareString (value, targetType, parameter, culture);
//			if (value is bool   ) return CompareBool   (value, targetType, parameter, culture);
			return CompareObject(value, targetType, parameter, culture);;
		}

		private bool IsNumeric(object value) {
			if (value == null) return false;
			var t = value.GetType();
			return (t == typeof (Int16) || t == typeof (Int32) || t == typeof (Int64) || t == typeof (UInt16) || t == typeof (UInt32) || t == typeof (UInt64)
					|| t == typeof (byte) || t == typeof (SByte) || t == typeof (Decimal) || t == typeof (Single) || t == typeof (Double));
		}

		public bool CompareObject(object value, Type targetType, object parameter, CultureInfo culture) {
			CompareOperation op;
			object cv;
			if (Operation == CompareOperation.InParameter) {
				var p=((string) parameter).Split(new char[] {' '}, 2);
				var ops = p[0];
				if (p[1].ToLowerInvariant() == "null" || p[1].ToLowerInvariant() == "{null}") ops = p[0]+" null"; //TODO nullable numeric?
				cv = p[1];
				switch (ops.ToLowerInvariant()) {
					case "eq"       : op=CompareOperation.Equal; break;
					case "ne"       : op=CompareOperation.NotEqual; break;
					case "not null" : op=CompareOperation.IsNotNull; break;
					case "ne null"  : op=CompareOperation.IsNotNull; break;
					case "is null"  : op=CompareOperation.IsNull; break;
					case "eq null"  : op=CompareOperation.IsNull; break;
					default         : op=CompareOperation.None; break;
				}
			}
			else {
				op = Operation;
				cv = parameter;
			}

			switch (op) {
				case CompareOperation.IsNotNull : return value!=null ;
				case CompareOperation.IsNull    : return value==null;
			}
			if (value != null) {
				switch (op) {
					case CompareOperation.Equal     : return value.Equals(System.Convert.ChangeType(cv,value.GetType()));
					case CompareOperation.NotEqual  : return !value.Equals(System.Convert.ChangeType(cv,value.GetType()));
				}				
			}

			return false;
		}

		public bool CompareNumeric(object value, Type targetType, object parameter, CultureInfo culture) {
			CompareOperation op;
			object cv;
			if (Operation == CompareOperation.InParameter) {
				var p=((string) parameter).Split(new char[] {' '}, 2);
				var ops = p[0];
				if (p[1].ToLowerInvariant() == "null" || p[1].ToLowerInvariant() == "{null}") ops = p[0]+" null"; //TODO nullable numeric?
				cv = p[1];
				switch (ops.ToLowerInvariant()) {
					case "eq": op=CompareOperation.Equal; break;
					case "gt": op=CompareOperation.GreaterThan; break;
					case "lt": op=CompareOperation.LessThen; break;
					case "ge": op=CompareOperation.GreaterThanOrEqual; break;
					case "le": op=CompareOperation.LessThenOrEqual; break;
					case "ne": op=CompareOperation.NotEqual; break;
					default  : op=CompareOperation.None; break;
				}
			}
			else {
				op = Operation;
				cv = parameter;
			}

			var v0 = System.Convert.ToDouble(value);
			var v1 = (double)DoubleConverter.ConvertFrom(cv);
			switch (op) {
				case CompareOperation.Equal              : return v0==v1;
				case CompareOperation.GreaterThan       : return v0>v1;
				case CompareOperation.LessThen           : return v0<v1;
				case CompareOperation.GreaterThanOrEqual: return v0>=v1;
				case CompareOperation.LessThenOrEqual    : return v0<=v1;
				case CompareOperation.NotEqual           : return v0!=v1;
			}
			return false;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}