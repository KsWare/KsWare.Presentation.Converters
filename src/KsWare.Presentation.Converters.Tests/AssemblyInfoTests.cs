using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace KsWare.Presentation.Converters.Tests {

	[TestFixture]
	public class AssemblyInfoTests {

		[Test]
		public void NamespaceMustMatchAssemblyName() {
			var t = typeof (KsWare.Presentation.Converters.AssemblyInfo);
			var assemblyName = KsWare.Presentation.Converters.AssemblyInfo.Assembly.GetName(false).Name;
			Assert.That(t.Namespace, Is.EqualTo(assemblyName));
		}

		[Test]
		public void AssemblyMustHaveStrongName()
		{
			var n = Assembly.GetExecutingAssembly().FullName;
			Assert.That(n,Is.Not.Contains("PublicKeyToken=none"));
			Assert.That(typeof(KsWare.Presentation.Converters.AssemblyInfo).Assembly.FullName, Is.Not.Contains("PublicKeyToken=none"));
			var pkt1 = string.Join("", Assembly.GetExecutingAssembly().GetName(true).GetPublicKey().Select(b => $"{b:X2}"));
			var pkt2 = string.Join("", KsWare.Presentation.Converters.AssemblyInfo.Assembly.GetName(true).GetPublicKey().Select(b => $"{b:X2}"));
		}
	}
}
