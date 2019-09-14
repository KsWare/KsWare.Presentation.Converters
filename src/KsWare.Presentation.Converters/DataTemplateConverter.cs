using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Baml2006;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Resources;
using System.Xaml;
using System.Xml;
using KsWare.Presentation.Interfaces.Plugins;
using KsWare.Presentation.Interfaces.Plugins.DataTemplateConverter;
using XamlReader = System.Windows.Markup.XamlReader;

namespace KsWare.Presentation.Converters
{
	/// <summary>
	/// Class DataTemplateConverter.
	/// Implements the <see cref="System.Windows.Data.IValueConverter" />
	/// </summary>
	/// <seealso cref="System.Windows.Data.IValueConverter" />
	public class DataTemplateConverter : IValueConverter
	{
		// @"pack://application:,,,/xxx;component/Resources/";

		/// <summary>
		/// Gets the default <see cref="DataTemplateConverter"/>.
		/// </summary>
		public static readonly DataTemplateConverter Default=new DataTemplateConverter();

        private static Lazy<Dictionary<string, Lazy<IDataTemplateConverterPlugin, DataTemplateConverterPluginExportMetadataView>>> _lazyPlugins = InitializePlugins();

        private static Lazy<Dictionary<string, Lazy<IDataTemplateConverterPlugin, DataTemplateConverterPluginExportMetadataView>>> InitializePlugins()
        {
	        return new Lazy<Dictionary<string, Lazy<IDataTemplateConverterPlugin, DataTemplateConverterPluginExportMetadataView>>>(() =>
	        {
				var catalog = new AggregateCatalog();
				var container = new CompositionContainer(catalog);
				ComposeApplicationDirectory(container);
				var exports = container.GetExports<IDataTemplateConverterPlugin, DataTemplateConverterPluginExportMetadataView>();
				var dic = new Dictionary<string, Lazy<IDataTemplateConverterPlugin, DataTemplateConverterPluginExportMetadataView>>();
				foreach (var export in exports)
				{
					foreach (var metadata in export.Metadata.Array)
					{
						if(!dic.ContainsKey(metadata.MimeType)) dic.Add(metadata.MimeType, export);
					}
				}

				return dic;
	        });
        }

        private static IDataTemplateConverterPlugin GetPlugin(string type) => _lazyPlugins.Value.TryGetValue(type, out var lazyPlugin) ? lazyPlugin.Value : null;

        private static void ComposeApplicationDirectory(CompositionContainer container)
        {
	        var catalog = (AggregateCatalog)container.Catalog;
	        var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
	        var dir = new DirectoryInfo(Path.GetDirectoryName(assembly.Location));
	        foreach (var file in dir.GetFiles("*.dll").Concat(dir.GetFiles("*.exe")))
	        {
		        if (file.Name.StartsWith("KsWare.Presentation.Converters.") ||
		            file.Name.Contains("DataTemplateConverterPlugin"))
		        {
			        assembly = Assembly.LoadFile(file.FullName);
			        byte[] assemblykey = assembly.GetName().GetPublicKey();
			        Debug.WriteLine($"Compose: {assembly.GetName().FullName}");
			        catalog.Catalogs.Add(new AssemblyCatalog(assembly));
		        }
	        }
        }

		/// <summary>
		/// Gets or sets the converter parameter.
		/// </summary>
		/// <value>The converter parameter.</value>
		public string ConverterParameter { get; set; }

		/// <summary>
		/// Converts a value.
		/// </summary>
		/// <param name="value">The value produced by the binding source.</param>
		/// <param name="targetType">The type of the binding target property.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>A converted value. If the method returns <see langword="null" />, the valid null value is used.</returns>
		[SuppressMessage("ReSharper", "TooManyArguments", Justification = "Interface implementation")]
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var locationUri = GetLocationUri(value, parameter ?? ConverterParameter);

			StreamResourceInfo streamResourceInfo;
			try { streamResourceInfo = Application.GetResourceStream(locationUri); }
			catch (IOException ex) { throw; }

			object resourceObject = ReadResource(streamResourceInfo);

			switch (streamResourceInfo.ContentType)
			{
				case "application/baml+xml":
				case "application/xaml+xml":
					switch (resourceObject)
					{
						case DataTemplate dataTemplate:
							return dataTemplate;
						case UIElement uiElement:
							return CreateDataTemplateFromUIElement(uiElement);
						default:
							return null;
					}
				case "image/bmp":
				case "image/tiff":
				case "image/jpeg":
				case "image/png":
				case "image/x-icon":
					return CreateDataTemplateFromImage(locationUri);
				case "image/gif":
				case "image/svg+xml":
				default:
					return GetPlugin(streamResourceInfo.ContentType)?.CreateDataTemplate(locationUri);
			}
		}


		/// <summary>
		/// [Not supported]
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="culture"></param>
		/// <returns></returns>
		/// <exception cref="NotSupportedException">ConvertBack is not supported!</exception>
		[SuppressMessage("ReSharper", "TooManyArguments", Justification = "Interface implementation")]
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException($"{nameof(DataTemplateConverter)}.ConvertBack is not supported!");
		}

		private object ReadResource(Stream stream)
		{
			// read the stream for build action "Resource"
			var xamlReader = new XamlReader();
			return xamlReader.LoadAsync(stream);
		}

		private object ReadPage(Stream stream)
		{
			// read the stream for build action "Page"
			using (var bamlReader = new Baml2006Reader(stream))
			using (var writer = new XamlObjectWriter(bamlReader.SchemaContext))
			{
				while (bamlReader.Read()) writer.WriteNode(bamlReader);
				return writer.Result;
			}
		}

        private object ReadResource(StreamResourceInfo streamResourceInfo)
        {
	        switch (streamResourceInfo.ContentType)
	        {
		        case "application/baml+xml":
		        {
			        // read stream for build action "Page"
			        using (var bamlReader = new Baml2006Reader(streamResourceInfo.Stream))
			        using (var writer = new XamlObjectWriter(bamlReader.SchemaContext))
			        {
				        while (bamlReader.Read()) writer.WriteNode(bamlReader);
				        return writer.Result;
			        }
		        }
		        case "application/xaml+xml":
		        {
			        // read stream for build action "Resource"
			        var xamlReader = new XamlReader();
			        return xamlReader.LoadAsync(streamResourceInfo.Stream);
		        }
		        default: return null;
	        }
        }

		private Uri GetLocationUri(object value, object parameter)
		{
			switch (value)
			{
				case string s: return GetLocationUriFromString(s, parameter);
				default:
				{
					var valueAsString = value as string;
					if (string.IsNullOrEmpty(valueAsString))
						throw new InvalidOperationException("Resource key not specified!");
					return GetLocationUriFromString(valueAsString, parameter);
				}
			}
			
		}

		private Uri GetLocationUriFromString(string value, object parameter)
		{
			var stringParameter = parameter as string;
			if (string.IsNullOrEmpty(stringParameter))
			{

			}
			else if (stringParameter.Contains("{0}"))
			{
				value = string.Format(stringParameter, value);
			}
			else if (stringParameter.Contains("{value}"))
			{
				value = value.Replace("{value}",value);
			}

			if (value.Contains("{EntryAssembly}"))
			{
				value = value.Replace("{EntryAssembly}", Assembly.GetEntryAssembly().GetName(false).Name);
			}

			if (!value.StartsWith("pack:")) value = "pack://application:,,," + value;

			return new Uri(value, value.StartsWith("pack:") ? UriKind.Absolute : UriKind.Relative);
			// pack://application:,,,/{EntryAssembly};component/Resources/
		}

		private DataTemplate CreateDataTemplateFromUIElement(UIElement content)
		{
			var contentXaml = SerializeToXaml(content);
			var dataTemplateXaml = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
{contentXaml}
</DataTemplate>";


			var sr = new StringReader(dataTemplateXaml);
			var xr = XmlReader.Create(sr);
			var dataTemplate = (DataTemplate) XamlReader.Load(xr);
			return dataTemplate;
		}

		private DataTemplate CreateDataTemplateFromImage(Uri locationUri)
		{
			var dataTemplateXaml = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
	<Image Source=""{locationUri.OriginalString}"" Stretch=""Uniform"" />
</DataTemplate>";


			var sr = new StringReader(dataTemplateXaml);
			var xr = XmlReader.Create(sr);
			var dataTemplate = (DataTemplate)XamlReader.Load(xr);
			return dataTemplate;
		}
        
		private DataTemplate CreateErrorTemplate(string message)
		{
			var dataTemplate = new DataTemplate();
			var textBlock = new FrameworkElementFactory(typeof(TextBlock));
			textBlock.SetValue(TextBlock.TextProperty, message);
			textBlock.SetValue(TextBlock.ForegroundProperty, Brushes.Red);
			dataTemplate.VisualTree = textBlock;
			return dataTemplate;
		}

		private static string SerializeToXaml(UIElement element)
		{
			var xaml = System.Windows.Markup.XamlWriter.Save(element);

			using (var stream = new MemoryStream())
			{
				using (var streamWriter = new StreamWriter(stream, Encoding.UTF8, 64*1024, true))
				{
					streamWriter.Write(xaml);
				}

				stream.Position = 0;
				return new StreamReader(stream).ReadToEnd();
			}
		}

		private static string GetLocationFormatString(string locationString)
		{
			return locationString + (locationString.EndsWith("/") ? "" : "/") + @"{0}.xaml";
		}
	}

	[MarkupExtensionReturnType(typeof(IValueConverter))]
	public class DataTemplateConverterMarkupExtension : MarkupExtension
	{
		private protected DataTemplateConverterMarkupExtension()
		{
		}

		private protected DataTemplateConverterMarkupExtension(string path)
		{
			Path = path ?? "";
		}

		public string Path { get; }

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return new DataTemplateConverter();
		}

		private protected string GetNormalizedPath()
		{
			var path = Path;
			if (path.StartsWith("/")) path = path.Substring(1);
			if (!path.EndsWith("/")) path += "/";
			return path;
		}
	}


	public sealed class ExecutingAssemblyDataTemplateConverterExtension : DataTemplateConverterMarkupExtension
	{
		public ExecutingAssemblyDataTemplateConverterExtension(string path) : base(path)
		{
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{

            var uriContext = serviceProvider.GetService(typeof(IUriContext)) as IUriContext;
            var baseUri = uriContext.BaseUri;
            var assembly = baseUri.Segments[1].Split(';')[0];

			return new DataTemplateConverter()
			{
				ConverterParameter= $"pack://application:,,,/{assembly};component/{GetNormalizedPath()}" + "{0}.xaml"
			};
		}
	}

	public sealed class EntryAssemblyDataTemplateConverterExtension : DataTemplateConverterMarkupExtension
	{
		public EntryAssemblyDataTemplateConverterExtension(string path) : base(path)
		{
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			var assembly = Assembly.GetEntryAssembly();
			return new DataTemplateConverter()
			{
				ConverterParameter= $"pack://application:,,,/{assembly};component/{GetNormalizedPath()}" + "{0}.xaml"
			};
		}
	}

}
