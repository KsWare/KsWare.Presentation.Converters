using System;
using System.Globalization;
using System.Linq;

namespace KsWare.Presentation.Converters {

	public class DisplayTimeSpanConverter:ValueConverterBase {

		private static readonly string[] customStrings = {"ddd", "hhh", "mmm", "sss"};

		public static readonly DisplayTimeSpanConverter HHHmmss = new DisplayTimeSpanConverter("hhh:mm:ss");

		public DisplayTimeSpanConverter() { }

		public DisplayTimeSpanConverter(string stringFormat) {
			StringFormat = stringFormat;
		}

		public string StringFormat { get; set; }

		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value == null) return null;
			var timespan = (TimeSpan) value;
			var stringFormat = !string.IsNullOrEmpty((string) parameter) ? (string)parameter : StringFormat;
			if (string.IsNullOrEmpty(stringFormat)) return timespan.ToString();

			if(!IsCustomStringFormat(stringFormat)) return timespan.ToString(StringFormat);

			var s = stringFormat;
			s = s.Replace("ddd", ((int)timespan.TotalDays   ).ToString(culture));
			s = s.Replace("dd" , ((int)timespan.Days        ).ToString("D2",culture));
			s = s.Replace("d"  , ((int)timespan.Days        ).ToString("D1",culture));
			s = s.Replace("hhh", ((int)timespan.Hours       ).ToString(     culture));
			s = s.Replace("hh" , ((int)timespan.TotalHours  ).ToString("D2",culture));
			s = s.Replace("h"  , ((int)timespan.Hours       ).ToString("D1",culture));
			s = s.Replace("mmm", ((int)timespan.TotalMinutes).ToString(     culture));
			s = s.Replace("mm" , ((int)timespan.Minutes     ).ToString("D2",culture));
			s = s.Replace("m"  , ((int)timespan.Minutes     ).ToString("D1",culture));
			s = s.Replace("sss", ((int)timespan.TotalSeconds).ToString(     culture));
			s = s.Replace("ss" , ((int)timespan.Seconds     ).ToString("D2",culture));
			s = s.Replace("s"  , ((int)timespan.Seconds     ).ToString("D1",culture));
			return s;
		}

		private bool IsCustomStringFormat(string stringFormat) {
			return customStrings.Any(stringFormat.Contains);
		}

	}
}