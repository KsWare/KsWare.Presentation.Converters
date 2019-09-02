using System.Globalization;
using NUnit.Framework;

namespace KsWare.Presentation.Converters.Tests
{
	[TestFixture]
	public class StringConverterTests
	{
		private static readonly CultureInfo enUS=CultureInfo.GetCultureInfo("en-US");

		[TestCase(1, "1")]
		[TestCase(1.0, "1")]
		[TestCase(1.1, "1.1")]
		[TestCase(System.ConsoleColor.Black, "Black")]
		[TestCase(typeof(string), "System.String")]
		public void Test(object value, string expectedResult)
		{
			Assert.That(StringConverter.Default.Convert(value, typeof(string), null, enUS), Is.EqualTo(expectedResult));
		}
	}
}