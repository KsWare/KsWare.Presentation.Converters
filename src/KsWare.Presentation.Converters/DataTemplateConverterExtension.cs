using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Markup;
using static KsWare.Presentation.Converters.DataTemplateConverterHelper;

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
		public DataTemplateConverterExtension()
		{
			ResourcePath = ".";
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
			var resourcePath = (ResourcePath ?? "").Trim();
			string p = null;
			if (resourcePath.Length==0 || resourcePath.StartsWith("."))
				p = EnhanceCurrentPath(serviceProvider, ResourcePath);
			else if (resourcePath.Contains("EntryAssembly"))
				p = EnhanceEntryAssemblyPath(ResourcePath);
			else if (resourcePath.Contains("ExecutingAssembly"))
				p = EnhanceExecutingAssemblyPath(serviceProvider, resourcePath);

			return new DataTemplateConverter {ConverterParameter = p};
		}

		private string EnhanceCurrentPath(IServiceProvider serviceProvider, string resourcePath)
		{
			if (serviceProvider == null) return CombinePath("ERROR-ExecutingAssembly-NotAvailable;component", resourcePath);
			var uriContext = serviceProvider.GetService(typeof(IUriContext)) as IUriContext;
			if (uriContext == null) return CombinePath("ERROR-ExecutingAssembly-NotAvailable;component", resourcePath);
			var baseUri = uriContext.BaseUri; 

			if (resourcePath.StartsWith(".."))
			{
				var rp = resourcePath;
				var sc = baseUri.Segments.Length - 1;
				var segments = resourcePath.Split('/');
				var backCount = segments.TakeWhile(s => s == "..").Count();
				sc -= backCount;
				if (sc < 2)
				{
					return CombinePath("/"+baseUri.Segments[1], resourcePath); // ERROR: return invalid url!
				}
				rp = string.Join("/", segments.Skip(backCount));
				var folder = string.Join("", baseUri.Segments.Take(sc));
				return CombinePath(folder, rp);
			}
			else if(resourcePath.StartsWith("."))
			{
				var folder = string.Join("", baseUri.Segments.Take(baseUri.Segments.Length - 1));
				return CombinePath(folder, resourcePath.Substring(1));
			}
			else
			{
				var folder = string.Join("", baseUri.Segments.Take(baseUri.Segments.Length - 1));
				return CombinePath(folder, resourcePath);
			}
		}

		private string EnhanceExecutingAssemblyPath(IServiceProvider serviceProvider, string resourcePath)
		{
			if (serviceProvider == null) return resourcePath.Replace("ExecutingAssembly", "ERROR-ExecutingAssembly-NotAvailable");
			var uriContext = serviceProvider.GetService(typeof(IUriContext)) as IUriContext;
			if (uriContext == null) return resourcePath.Replace("ExecutingAssembly", "ERROR-ExecutingAssembly-NotAvailable");
			var baseUri = uriContext.BaseUri;
			var assembly = baseUri.Segments[1].Split(';')[0];
			return resourcePath.Replace("ExecutingAssembly", assembly);
		}

		private string EnhanceEntryAssemblyPath(string resourcePath)
		{
			var assembly = Assembly.GetEntryAssembly()?.GetName(true).Name;
			if (string.IsNullOrEmpty(assembly) || assembly == "XDesProc")
			{
				// TODO e.g. at test or design time
				assembly = "ERROR-EntryAssembly-NotAvailable";
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
			return CombinePath("ExecutingAssembly;component", resourcePath);
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
			return CombinePath("EntryAssembly;component", resourcePath);
		}
	}
}