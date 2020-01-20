using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Resources;
using static KsWare.Presentation.Converters.TemplateConverterHelper;

namespace KsWare.Presentation.Converters {

	/// <summary>
	/// Class ResourceConverter.
	/// Implements the <see cref="System.Windows.Data.IValueConverter" />
	/// </summary>
	/// <seealso cref="System.Windows.Data.IValueConverter" />
	/// <remarks>
	/// <para>The <see cref="ResourceConverter"/> converts the specified value (a key string) together with parameter (a format string) into a resource URI.
	/// The resource will be loaded and converted into a <see cref="DataTemplate"/> or <see cref="ControlTemplate"/>.</para>
	/// <para>Examples:<br/>
	/// Value: "<c>OpenIcon.xaml</c>" Parameter: "<c>pack://application:,,,/MyAssembly;component/Resources/{0}</c>"<br/>
	/// Value: "<c>OpenIcon.ico</c>" Parameter: "<c>/MyAssembly;component/Resources</c>"<br/>
	/// </para>
	/// <para>See also available markup extensions: <see cref="ResourceConverterExtension"/>, <see cref="ExecutingAssemblyResourceConverterExtension"/>, <see cref="EntryAssemblyResourceConverterExtension"/>.</para>
	/// </remarks>
	/// <example>
	/// <code>
	/// &lt;ContentControl ContentTemplate="{Binding MyKey, Converter={x:Static ksv:ResourceConverter.Default}, ConverterParameter=/MyResourceAssembly;component/APath/{0}.xaml}"/&gt;
	/// </code>
	/// </example>
	public class ResourceConverter : IValueConverter {

		/// <summary>
		/// Gets the default <see cref="ResourceConverter"/>.
		/// </summary>
		public static readonly ResourceConverter Default = new ResourceConverter();

		/// <summary>
		/// Gets or sets the converter parameter.
		/// </summary>
		/// <value>The converter parameter.</value>
		/// <example>
		/// <c>pack://application:,,,/xxx;component/Resources/{0};</c>
		/// </example>
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
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			var locationUri = GetLocationUri(value, parameter ?? ConverterParameter);

			if (locationUri.OriginalString.Contains("ExecutingAssembly") ||
			    locationUri.OriginalString.Contains("EntryAssembly") ||
			    locationUri.OriginalString.StartsWith("/ERROR") ||
			    locationUri.OriginalString.StartsWith("pack://application:,,,/ERROR")
			) {
				//maybe design mode
				return CreateErrorTemplate($"{value}");
			}

			StreamResourceInfo streamResourceInfo;
			try { streamResourceInfo = Application.GetResourceStream(locationUri); }
			catch (IOException ex) {
				// Fallback to local directory
				var root=Path.GetDirectoryName((Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly()).Location);
				var path = string.Join("", locationUri.Segments.Skip(2)).Replace("/","\\");
				path = Path.Combine(root, path);
				if(!File.Exists(path)) CreateErrorTemplate($"{value}"); ;
				var contentType = GetContentType(path);
				var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
				streamResourceInfo = new StreamResourceInfo(stream, contentType);
			}

			try {

				switch (streamResourceInfo.ContentType) {
					case "application/baml+xml": // Page
					case "application/xaml+xml": // Resource
						object resourceObject = ReadResource(streamResourceInfo);
						switch (targetType.Name) {
							case nameof(DataTemplate):
								switch (resourceObject) {
									case DataTemplate dataTemplate: return dataTemplate;
									case UIElement uiElement: return CreateDataTemplateFromUIElement(uiElement);
									default: return null;
								}
							case nameof(ControlTemplate):
								switch (resourceObject) {
									case ControlTemplate controlTemplate: return controlTemplate;
									case UIElement uiElement: return CreateControlTemplateFromUIElement(uiElement);
									default: return null;
								}
							default:
								if (targetType.IsInstanceOfType(resourceObject)) return resourceObject;
								throw new NotSupportedException($"Conversion not supported. TargetType: {targetType?.Name ?? "Null"}");
						}

					case "image/bmp":
					case "image/tiff":
					case "image/jpeg":
					case "image/png":
					case "image/x-icon":
						switch (targetType.Name) {
							case nameof(DataTemplate): return CreateDataTemplateFromImage(locationUri);
							case nameof(ControlTemplate): return CreateControlTemplateFromImage(locationUri);
							default:
								if (targetType.IsAssignableFrom(typeof(System.Windows.Controls.Image)))
									return CreateImage(locationUri);
								throw new NotSupportedException($"Conversion not supported. TargetType: {targetType?.Name ?? "Null"}");
						}
					case "image/gif":
					case "image/svg+xml":
					default:
						switch (targetType.Name) {
							case nameof(DataTemplate):
								return TemplateConverterPluginHelper.GetPlugin(streamResourceInfo.ContentType)
									?.CreateDataTemplate(locationUri);
							case nameof(ControlTemplate):
								return TemplateConverterPluginHelper.GetPlugin(streamResourceInfo.ContentType)
									?.CreateControlTemplate(locationUri);
							default: throw new NotSupportedException($"Conversion not supported. TargetType: {targetType?.Name ?? "Null"}");
						}
				}
			}
			finally {
				streamResourceInfo?.Stream?.Dispose();
			}
		}

		private string GetContentType(string path) {
			var ext = Path.GetExtension(path)?.ToLower(CultureInfo.InstalledUICulture);
			switch (ext) {
				case ".xaml": return "application/xaml+xml";
				case ".bmp": return "image/bmp";
				case ".tiff": case ".tif": return "image/tiff";
				case ".jpeg": case ".jpg": return "image/jpeg";
				case ".png": return "image/png";
				case ".ico": return "image/x-icon";
				case ".gif": return "image/gif";
				case ".svg": return "image/svg+xml";
				default: throw new NotSupportedException($"Extension not supported. Extension: '{ext}'");
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
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException($"{nameof(ResourceConverter)}.ConvertBack is not supported!");
		}

	}

}
