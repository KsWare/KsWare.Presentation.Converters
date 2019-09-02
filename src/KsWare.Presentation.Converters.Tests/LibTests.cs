using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace KsWare.Presentation.Converters.Tests {

	[TestFixture]
	public class LibTests {

		[Test]
		public void NamespaceMustMatchAssemblyName() {
			var t = typeof (KsWare.Presentation.Converters.AssemblyInfo);
			var assemblyName = KsWare.Presentation.Converters.AssemblyInfo.Assembly.GetName(false).Name;
			Assert.That(t.Namespace, Is.EqualTo(assemblyName));
		}
	}
}
