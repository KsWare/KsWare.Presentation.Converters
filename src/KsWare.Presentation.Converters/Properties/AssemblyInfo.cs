using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("KsWare.Presentation.Converters")]
[assembly: AssemblyDescription("Converters for KsWare Presentation Framework")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("KsWare")]
[assembly: AssemblyProduct("Presentation Framework")]
[assembly: AssemblyCopyright("Copyright © 2002-2019 by KsWare. All rights reserved.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]

[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: AssemblyInformationalVersion("1.0.0.0")]

[assembly: ThemeInfo(ResourceDictionaryLocation.None, ResourceDictionaryLocation.SourceAssembly)]

[assembly: XmlnsDefinition(KsWare.Presentation.Converters.AssemblyInfo.XmlNamespace, "KsWare.Presentation.Converters")]
[assembly: XmlnsPrefix(KsWare.Presentation.Converters.AssemblyInfo.XmlNamespace, "ksv")]

// namespace must equal to assembly name
// ReSharper disable once CheckNamespace
namespace KsWare.Presentation.Converters
{
	public static class AssemblyInfo
	{

		public static Assembly Assembly => Assembly.GetExecutingAssembly();

		public const string XmlNamespace = "http://ksware.de/Presentation/ViewFramework";

		public const string RootNamespace = "KsWare.Presentation.Converters";

	}
}