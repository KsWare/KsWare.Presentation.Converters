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

		[Test]
		public void EqualVisibleElseCollapsed()
		{
			Assert.AreEqual(Visibility.Visible, (Visibility)VisibilityConverter.EqualVisibleElseCollapsed.Convert('A', typeof(Visibility), 'A', null));
			Assert.AreEqual(Visibility.Collapsed, (Visibility)VisibilityConverter.EqualVisibleElseCollapsed.Convert('A', typeof(Visibility), 'B', null));
		}

		[Test]
		public void NotEqualVisibleElseCollapsed()
		{
			Assert.AreEqual(Visibility.Visible, (Visibility)VisibilityConverter.NotEqualVisibleElseCollapsed.Convert('A', typeof(Visibility), 'B', null));
			Assert.AreEqual(Visibility.Collapsed, (Visibility)VisibilityConverter.NotEqualVisibleElseCollapsed.Convert('A', typeof(Visibility), 'A', null));
		}

		[Test]
		public void EqualVisibleElseCollapsed_Double()
		{
			double a = 1;
			double b1 = 0.99999999;
			double b2 = 0.9999999;
			Assert.AreEqual(Visibility.Visible, (Visibility)VisibilityConverter.EqualVisibleElseCollapsed.Convert(a, typeof(Visibility), a, null));
			Assert.AreEqual(Visibility.Visible, (Visibility)VisibilityConverter.EqualVisibleElseCollapsed.Convert(a, typeof(Visibility), b1, null));
			Assert.AreEqual(Visibility.Collapsed, (Visibility)VisibilityConverter.EqualVisibleElseCollapsed.Convert(a, typeof(Visibility), b2, null));
		}

		[Test]
		public void EqualVisibleElseCollapsed_Double_String()
		{
			double a = 1;
			var b1 = "0.99999999";
			var b2 = "0.9999999";
			Assert.AreEqual(Visibility.Visible, (Visibility)VisibilityConverter.EqualVisibleElseCollapsed.Convert(a, typeof(Visibility), a, null));
			Assert.AreEqual(Visibility.Visible, (Visibility)VisibilityConverter.EqualVisibleElseCollapsed.Convert(a, typeof(Visibility), b1, null));
			Assert.AreEqual(Visibility.Collapsed, (Visibility)VisibilityConverter.EqualVisibleElseCollapsed.Convert(a, typeof(Visibility), b2, null));
		}

		[Test]
		public void EqualVisibleElseCollapsed_Double_String_NaN()
		{
			double a = double.NaN;
			var b1 = "NaN";
			Assert.AreEqual(Visibility.Visible, (Visibility)VisibilityConverter.EqualVisibleElseCollapsed.Convert(a, typeof(Visibility), a, null));
			Assert.AreEqual(Visibility.Visible, (Visibility)VisibilityConverter.EqualVisibleElseCollapsed.Convert(a, typeof(Visibility), b1, null));
		}

		[Test]
		public void EqualVisibleElseCollapsed_Double_String_InvalidNumber()
		{
			double a = 1;
			var b = "invalid";
			Assert.AreEqual(Visibility.Collapsed, (Visibility)VisibilityConverter.EqualVisibleElseCollapsed.Convert(a, typeof(Visibility), b, null));
		}
	}
}