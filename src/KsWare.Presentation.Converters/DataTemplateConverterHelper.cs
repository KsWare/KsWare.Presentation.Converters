using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Baml2006;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Resources;
using System.Xaml;
using System.Xml;
using XamlReader = System.Windows.Markup.XamlReader;

namespace KsWare.Presentation.Converters
{
	internal static class DataTemplateConverterHelper
	{
		public static object ReadResource(StreamResourceInfo streamResourceInfo)
		{
			switch (streamResourceInfo.ContentType)
			{
				case "application/baml+xml": return ReadPage(streamResourceInfo.Stream);
				case "application/xaml+xml": return ReadResource(streamResourceInfo.Stream);
				default: return null;
			}
		}

		public static object ReadResource(Stream stream)
		{
			// read the stream for build action "Resource"
			var xamlReader = new XamlReader();
			return xamlReader.LoadAsync(stream);
		}

		public static object ReadPage(Stream stream)
		{
			// read the stream for build action "Page"
			using (var bamlReader = new Baml2006Reader(stream))
			using (var writer = new XamlObjectWriter(bamlReader.SchemaContext))
			{
				while (bamlReader.Read()) writer.WriteNode(bamlReader);
				return writer.Result;
			}
		}

		public static Uri GetLocationUri(object value, object parameter)
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

		public static Uri GetLocationUriFromString(string value, object parameter)
		{
			var stringParameter = (parameter as string)??"";

			string url;
			if (string.IsNullOrEmpty(stringParameter))
				url = value;
			else if (stringParameter.Contains("{0}"))
				url = stringParameter.Replace("{0}", value);
			else if (stringParameter.Contains("{key}"))
				url = stringParameter.Replace("{key}", value);
			else if (stringParameter.Contains("{value}"))
				url = stringParameter.Replace("{value}", value);
			else 
				url=CombinePath(stringParameter, value);


			if (!url.StartsWith("pack://application:,,,"))
				url = CombinePath("pack://application:,,,", url);

			return new Uri(url, url.StartsWith("pack:") ? UriKind.Absolute : UriKind.Relative);
			// pack://application:,,,/{EntryAssembly};component/Resources/
		}

		public static DataTemplate CreateDataTemplateFromUIElement(UIElement content)
		{
			var contentXaml = SerializeToXaml(content);
			var dataTemplateXaml = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
{contentXaml}
</DataTemplate>";


			var sr = new StringReader(dataTemplateXaml);
			var xr = XmlReader.Create(sr);
			var dataTemplate = (DataTemplate)XamlReader.Load(xr);
			return dataTemplate;
		}

		public static DataTemplate CreateDataTemplateFromImage(Uri locationUri)
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

		public static DataTemplate CreateErrorTemplate(string message)
		{
			var dataTemplate = new DataTemplate();
			var textBlock = new FrameworkElementFactory(typeof(TextBlock));
			textBlock.SetValue(TextBlock.TextProperty, message);
			textBlock.SetValue(TextBlock.ForegroundProperty, Brushes.Red);
			dataTemplate.VisualTree = textBlock;
			return dataTemplate;
		}

		public static string SerializeToXaml(UIElement element)
		{
			var xaml = System.Windows.Markup.XamlWriter.Save(element);

			using (var stream = new MemoryStream())
			{
				using (var streamWriter = new StreamWriter(stream, Encoding.UTF8, 64 * 1024, true))
				{
					streamWriter.Write(xaml);
				}

				stream.Position = 0;
				return new StreamReader(stream).ReadToEnd();
			}
		}

		public static string GetLocationFormatString(string locationString)
		{
			return locationString + (locationString.EndsWith("/") ? "" : "/") + @"{0}.xaml";
		}

		public static string CombinePath(string p0, string p1)
		{
			var sep = p0.EndsWith("/") || p1.StartsWith("/") ? "" : "/";
			return p0 + sep + p1;
		}
	}
}
