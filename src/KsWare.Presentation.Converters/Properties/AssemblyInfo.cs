using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;

[assembly: ComVisible(false)]

[assembly: ThemeInfo(ResourceDictionaryLocation.None, ResourceDictionaryLocation.SourceAssembly)]

[assembly: XmlnsDefinition(KsWare.Presentation.Converters.AssemblyInfo.XmlNamespace, "KsWare.Presentation.Converters")]
[assembly: XmlnsPrefix(KsWare.Presentation.Converters.AssemblyInfo.XmlNamespace, "ksv")]

[assembly: InternalsVisibleTo("KsWare.Presentation.Converters.Tests, PublicKey=00240000048000009400000006020000002400005253413100040000010001001914D8B4713BE9E446538070A6EC797DA9A8707C756DB341D5A64E0FE0044D9376FE8A45BE183E81B8F183817A8A66EC1EA44FA82F2FD8DADABB36640C772F4F416C7409FDD8718FF72ACF47B12D06A440C269F6A1945DC5564D3FB767FD1DBA91B3524F4A92F74930CDD151C0A40B411683D82E49307C0BCCD74AD89EC332EA")]

// namespace must equal to assembly name
// ReSharper disable once CheckNamespace
namespace KsWare.Presentation.Converters {

	public static class AssemblyInfo {

		public static Assembly Assembly => Assembly.GetExecutingAssembly();

		public const string XmlNamespace = "http://ksware.de/Presentation/ViewFramework";

		public const string RootNamespace = "KsWare.Presentation.Converters";

	}
}
