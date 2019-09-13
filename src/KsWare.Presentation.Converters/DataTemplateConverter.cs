using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
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

		private static object _gifPlugin;
        private static object _svgPlugin;

        static DataTemplateConverter()
		{
			_gifPlugin = Activator.CreateInstance("KsWare.Presentation.Converters.Gif", "KsWare.Presentation.Converters.Gif.DataTemplateConverterPlugin").Unwrap();
			GifFactory = locationUri =>
			{
				var dataTemplate = _gifPlugin.GetType().InvokeMember("CreateDataTemplate",
					BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod, null, _gifPlugin,
					new object[] {locationUri});
				return (DataTemplate) dataTemplate;
			};

            _svgPlugin = Activator.CreateInstance("KsWare.Presentation.Converters.Svg", "KsWare.Presentation.Converters.Svg.DataTemplateConverterPlugin").Unwrap();
            SvgFactory = locationUri =>
            {
                var dataTemplate = _gifPlugin.GetType().InvokeMember("CreateDataTemplate",
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod, null, _svgPlugin,
                    new object[] {locationUri});
                return (DataTemplate) dataTemplate;
            };
		}

		/// <summary>
		/// Gets or sets the converter parameter.
		/// </summary>
		/// <value>The converter parameter.</value>
		public string ConverterParameter { get; set; }

		public static Func<Uri, DataTemplate> GifFactory { get; set; }

		public static Func<Uri, DataTemplate> SvgFactory { get; set; }

		//		public string Suffix { get; set; }
		//
		//		public string Prefix { get; set; } = ".xaml";
		//
		//		public Uri LocationUri { get; set; }


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
					return GifFactory?.Invoke(locationUri);
				case "image/svg+xml":
					return SvgFactory?.Invoke(locationUri);
				default:
					return null;
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

	public class DataTemplateConverterMarkupExtension : MarkupExtension
	{
		public DataTemplateConverterMarkupExtension()
		{
		}

		public DataTemplateConverterMarkupExtension(string path)
		{
			Path = path ?? "";
		}

		public string Path { get; set; }

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return new DataTemplateConverter();
		}

		protected string GetNormalizedPath()
		{
			var path = Path;
			if (path.StartsWith("/")) path = path.Substring(1);
			if (!path.EndsWith("/")) path += "/";
			return path;
		}
	}


	public class ExecutingAssemblyDataTemplateConverterExtension : DataTemplateConverterMarkupExtension
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

	public class EntryAssemblyDataTemplateConverterExtension : DataTemplateConverterMarkupExtension
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
