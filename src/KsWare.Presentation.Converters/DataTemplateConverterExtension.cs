using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Markup;

namespace KsWare.Presentation.Converters
{
	/// <summary>
	/// Class DataTemplateConverterExtension.
	/// Implements the <see cref="System.Windows.Markup.MarkupExtension" />
	/// </summary>
	/// <seealso cref="System.Windows.Markup.MarkupExtension" />
	[MarkupExtensionReturnType(typeof(IValueConverter))]
	public class DataTemplateConverterExtension : MarkupExtension
	{
		/// <summary>
		/// Prevents a default instance of the <see cref="DataTemplateConverterExtension"/> class from being created.
		/// </summary>
		private protected DataTemplateConverterExtension()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DataTemplateConverterExtension"/> class.
		/// </summary>
		/// <param name="resourcePath">The absolute or relative path to the resource. (relative to the root of assembly)</param>
		[SuppressMessage("ReSharper", "MemberCanBeProtected.Global", Justification = "public API")]
		public DataTemplateConverterExtension(string resourcePath)
		{
			ResourcePath = resourcePath ?? "";
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DataTemplateConverterExtension"/> class.
		/// </summary>
		/// <param name="assembly">The assembly name or one of the placeholders: "<c>EntryAssembly</c>", "<c>ExecutingAssembly</c>"</param>
		/// <param name="resourcePath">The relative path to the resource. (relative to the root of assembly)</param>
		public DataTemplateConverterExtension(string assembly, string resourcePath)
		{
			ResourcePath = CombinePath(assembly + ";component", resourcePath);
		}

		/// <summary>
		/// Gets the path to the resource.
		/// </summary>
		/// <value>The resource path.</value>
		/// <example>
		/// ResourcePath : "<c>Resources</c>" results in <br/>
		/// ConverterParameter "<c>pack://application:,,,/Name-Of-Executing-Assembly;component/Resources/{0}</c>"
		/// <br/>
		/// ResourcePath : "<c>/MyAssembly;component/Resources</c>" results in <br/>
		/// ConverterParameter "<c>pack://application:,,,/MyAssembly;component/Resources/{0}</c>"
		/// <br/>
		/// ResourcePath : "<c>/ExecutingAssembly;component/Resources</c>" results in <br/>
		/// ConverterParameter "<c>pack://application:,,,/Name-Of-Executing-Assembly;component/Resources/{0}</c>"
		/// <br/>
		/// ResourcePath : "<c>/EntryAssembly;component/Resources</c>" results in <br/>
		/// ConverterParameter "<c>pack://application:,,,/Name-Of-Entry-Assembly;component/Resources/{0}</c>"
		/// </example>
		public string ResourcePath { get; }


		/// <inheritdoc />
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			string p = null;
			if (ResourcePath.Contains("EntryAssembly"))
				p = EnhanceEntryAssemblyPath(ResourcePath);
			else if (ResourcePath.Contains("ExecutingAssembly"))
				p = EnhanceExecutingAssemblyPath(serviceProvider, ResourcePath);

			return new DataTemplateConverter {ConverterParameter = p};
		}

		private string CombinePath(string p0, string p1)
		{
			var sep = p0.EndsWith("/") || p1.StartsWith("/") ? "" : "/";
			return p0 + sep + p1;
		}

		private string EnhanceExecutingAssemblyPath(IServiceProvider serviceProvider, string resourcePath)
		{
			if (serviceProvider == null)  return resourcePath;
			var uriContext = serviceProvider.GetService(typeof(IUriContext)) as IUriContext;
			if (uriContext == null) return resourcePath; 
			var baseUri = uriContext.BaseUri;
			var assembly = baseUri.Segments[1].Split(';')[0];
			return resourcePath.Replace("ExecutingAssembly", assembly);
		}

		private string EnhanceEntryAssemblyPath(string resourcePath)
		{
			var assembly = Assembly.GetEntryAssembly()?.GetName(true).Name;
			if (assembly == null)
			{
				// TODO e.g. at test or design time
				assembly = Assembly.GetExecutingAssembly()?.GetName(true).Name;
			}

			return resourcePath.Replace("EntryAssembly", assembly);
		}
	}


	/// <summary>
	/// Class ExecutingAssemblyDataTemplateConverterExtension. This class cannot be inherited.
	/// Implements the <see cref="DataTemplateConverterExtension" />
	/// </summary>
	/// <seealso cref="DataTemplateConverterExtension" />
	public sealed class ExecutingAssemblyDataTemplateConverterExtension : DataTemplateConverterExtension
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ExecutingAssemblyDataTemplateConverterExtension"/> class.
		/// </summary>
		/// <param name="resourcePath">The relative path to the resource.</param>
		/// <example>
		/// ResourcePath : "<c>Resources</c>" results in <br/>
		/// ConverterParameter "<c>pack://application:,,,/Name-Of-Executing-Assembly;component/Resources/{0}</c>"
		/// </example>
		public ExecutingAssemblyDataTemplateConverterExtension(string resourcePath) : base(EnhancePath(resourcePath))
		{
		}

		private static string EnhancePath(string resourcePath)
		{
			return "ExecutingAssembly;component" + (resourcePath.StartsWith("/") ? "" : "/") + resourcePath;
		}
	}

	/// <summary>
	/// Class EntryAssemblyDataTemplateConverterExtension. This class cannot be inherited.
	/// Implements the <see cref="DataTemplateConverterExtension" />
	/// </summary>
	/// <seealso cref="DataTemplateConverterExtension" />
	public sealed class EntryAssemblyDataTemplateConverterExtension : DataTemplateConverterExtension
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="EntryAssemblyDataTemplateConverterExtension"/> class.
		/// </summary>
		/// <param name="resourcePath">The relative path to the resource.</param>
		/// <example>
		/// ResourcePath : "<c>Resources</c>" results in <br/>
		/// ConverterParameter "<c>pack://application:,,,/Name-Of-Entry-Assembly;component/Resources/{0}</c>"
		/// </example>
		public EntryAssemblyDataTemplateConverterExtension(string resourcePath) : base(EnhancePath(resourcePath))
		{
		}

		private static string EnhancePath(string resourcePath)
		{
			return "EntryAssembly;component" + (resourcePath.StartsWith("/") ? "" : "/") + resourcePath;
		}
	}
}