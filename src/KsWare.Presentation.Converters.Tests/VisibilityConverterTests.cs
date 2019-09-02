using NUnit.Framework;
using System.Windows;

namespace KsWare.Presentation.Converters.Tests
{
	[TestFixture]
	public class VisibilityConverterTests
	{

		[TestCase(true, Visibility.Visible)]
		[TestCase(false, Visibility.Collapsed)]
		[TestCase(null, Visibility.Collapsed)]
		public void TrueVisibleElseCollapsed(object value, Visibility expectedResult)
		{
			Assert.That(VisibilityConverter.TrueVisibleElseCollapsed.Convert(value, typeof(Visibility), null, null),Is.EqualTo(expectedResult));
		}

		[TestCase(true, Visibility.Visible)]
		[TestCase(false, Visibility.Hidden)]
		[TestCase(null, Visibility.Hidden)]
		public void TrueVisibleElseHidden(object value, Visibility expectedResult)
		{
			Assert.That(VisibilityConverter.TrueVisibleElseHidden.Convert(value, typeof(Visibility), null, null), Is.EqualTo(expectedResult));
		}
	}
}