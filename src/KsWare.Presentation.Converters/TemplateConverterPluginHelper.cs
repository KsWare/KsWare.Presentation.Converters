using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using KsWare.Presentation.Interfaces.Plugins.TemplateConverter;

namespace KsWare.Presentation.Converters {

	internal static class TemplateConverterPluginHelper {

		private static readonly Lazy<Dictionary<string, Lazy<ITemplateConverterPlugin, TemplateConverterPluginExportMetadataView>>>
			LazyPlugins =
				new Lazy<Dictionary<string, Lazy<ITemplateConverterPlugin, TemplateConverterPluginExportMetadataView>>>(
					InitializePluginsFactory);

		public static ITemplateConverterPlugin GetPlugin(string type) =>
			LazyPlugins.Value.TryGetValue(type, out var lazyPlugin) ? lazyPlugin.Value : null;

		private static Dictionary<string, Lazy<ITemplateConverterPlugin, TemplateConverterPluginExportMetadataView>>
			InitializePluginsFactory() {
			var catalog = new AggregateCatalog();
			var container = new CompositionContainer(catalog);
			ComposeApplicationDirectory(container);
			var exports = container.GetExports<ITemplateConverterPlugin, TemplateConverterPluginExportMetadataView>();
			var dic = new Dictionary<string, Lazy<ITemplateConverterPlugin, TemplateConverterPluginExportMetadataView>>();
			foreach (var export in exports) {
				foreach (var metadata in export.Metadata.Array) {
					if (!dic.ContainsKey(metadata.MimeType)) dic.Add(metadata.MimeType, export);
				}
			}

			return dic;
		}

		private static void ComposeApplicationDirectory(CompositionContainer container) {
			var catalog = (AggregateCatalog) container.Catalog;
			var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
			var dir = new DirectoryInfo(Path.GetDirectoryName(assembly.Location));
			foreach (var file in dir.GetFiles("*.dll").Concat(dir.GetFiles("*.exe"))) {
				//TODO Filter
				if ((file.Name.StartsWith("KsWare.Presentation.Converters.") ||
				     file.Name.Contains("TemplateConverterPlugin")) &&
				    file.Name != "KsWare.Presentation.Converters.dll") {
					assembly = Assembly.LoadFile(file.FullName);
					byte[] assemblykey = assembly.GetName().GetPublicKey();
					Debug.WriteLine($"Compose: {assembly.GetName().FullName}");
					catalog.Catalogs.Add(new AssemblyCatalog(assembly));
				}

				// Die Datei oder Assembly "KsWare.Presentation.Interfaces, Version=0.1.2.0, Culture=neutral, PublicKeyToken=398cf6eb36307095" oder eine Abhängigkeit davon wurde nicht gefunden. Die gefundene Manifestdefinition der Assembly stimmt nicht mit dem Assemblyverweis überein. (Ausnahme von HRESULT: 0x80131040)
			}
		}

	}

}
