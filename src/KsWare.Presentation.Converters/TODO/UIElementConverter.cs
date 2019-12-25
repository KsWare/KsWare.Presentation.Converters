﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows.Baml2006;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace KsWare.Presentation.Converters {
	//TODO UIElementConverter join with TemplateConvter?/EnumImageConverter?/ResourceDictionaryLockupConverter?

	// Loads UIElements (xaml/baml) from resources
	// supports additional also image resources (converted to Image)

	public class UIElementConverter : IValueConverter {

		static readonly ImageSourceConverter ImageSourceConverter = new ImageSourceConverter();
		static readonly UIElementConverter DefaultInstance = new UIElementConverter();

		public static UIElementConverter Default => DefaultInstance;

		private static readonly Dictionary<string, Assembly2> CachedAssemblies = new Dictionary<string, Assembly2>();

//		private readonly Assembly        _CachedAssembly;
//		private readonly ResourceManager _CachedResourceManager;
//		private          ResourceSet     _CachedResourceSet;
//		private readonly CultureInfo     _CachedCulture;

		public UIElementConverter() {
//			this._CachedAssembly        = Assembly.GetExecutingAssembly();
//			this._CachedResourceManager = new ResourceManager(this._CachedAssembly.GetName().Name + ".g", this._CachedAssembly);
//			this._CachedCulture         = Thread.CurrentThread.CurrentUICulture;
//			this._CachedResourceSet     = this._CachedResourceManager.GetResourceSet(this._CachedCulture, true, true);
		}

		/* Usage A

			<ContentPresenter Content="{Binding Source, RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type Controls:ResourcePresenter}}, Converter={x:Static Converters:UIElementConverter.Default}}"/>
			- or more easy width intellisense support -
			<Controls:ResourcePresenter Source="pack://application:,,,/Folder/FileName.Ext"/>
		*/

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (culture == null) culture = Thread.CurrentThread.CurrentUICulture;
			Uri ressourceUri;
			if (parameter != null) {
				if (parameter is string) { ressourceUri = new Uri((string) parameter); }
				else {
					Debug.WriteLine("=>WARNING: Parameter not supported! " +
					                "\r\n\t" + "Parameter:" + (value == null ? "{NULL}" : value.ToString()) +
					                "\r\n\t" + "UniqueID: {22C20F20-E9CA-4586-8928-0A080BAAEA83}");
					return null;
				}
			}
			else {
				if (value is Uri) {
					//->  pack://application:,,,/ReferencedAssembly;component/Subfolder/ResourceFile.xaml
					//->  /ReferencedAssembly;component/Subfolder/ResourceFile.xaml
					ressourceUri = (Uri) value;
				}
				else {
					Debug.WriteLine("=>WARNING: Value not supported! " +
					                "\r\n\t" + "Value:" + (value == null ? "{NULL}" : value.ToString()) +
					                "\r\n\t" + "UniqueID: {5ED3F3D1-8AA8-4BF2-885B-5834DA371F12}");
					return null;
				}
			}

			// make pack uri (ImageSourceConverter need this)
			if (!ressourceUri.IsAbsoluteUri) {
				ressourceUri = new Uri("pack://application:,,," + ressourceUri.ToString()); //TODO add some checks
			}

			// extract the extension
			var ext = Path.GetExtension(ressourceUri.ToString().ToLower(CultureInfo.InvariantCulture));

			var uiElement = (object) null;
			switch (ext) {
				case ".ico":
				case ".jpg":
				case ".png":
				case ".gif":
				case ".bmp":
					//TODO possible NullReferenceException if resource not exists
					var imageSource = (ImageSource) ImageSourceConverter.ConvertFrom(ressourceUri);
					uiElement = new Image {Source = imageSource};
					break;
				case ".baml":
				case ".xaml":
					var stream = GetResourceStream(ressourceUri, culture, ref ext);
					if (ext == ".baml") {
						using (var bamlReader = new Baml2006Reader(stream)) uiElement = XamlReader.Load(bamlReader);
					}
					else if (ext == ".xaml") { uiElement = XamlReader.Load(stream); }

					break;
				default:
					Debug.WriteLine("=>ERROR: Type of resource not supported!" +
					                "\r\n\t" + "Extension: " + ext +
					                "\r\n\t" + "Resource: " + ressourceUri.ToString() +
					                "\r\n\t" + "UniqueID: {66CC87C5-2A06-4424-9797-0F931ED533C0}");
					break;
			}

			return uiElement;
		}

		private Stream GetResourceStream(Uri uri, CultureInfo culture, ref string ext) {
			Stream stream;

			//* from file 
			//			stream = new FileStream((string) parameter, FileMode.Open);

			//* from manifest resource file (Resource)
			//			stream = cachedAssembly.GetManifestResourceStream(resourceName);//"KsWare.TestApp.View.Resources.Graphics.a_EmbeddedResource_Viewbox.xaml"

			//* from (WPF) manifest resource file (EmbeddedResource, Page)

			string resourceName;
			string assemblyName;
			var puq = uri.PathAndQuery;
			var p = puq.IndexOf(";component/");
			if (p > -1) {
				resourceName = puq.Substring(p + (";component/".Length));
				assemblyName = puq.Substring(1, p - 1);
			}
			else {
				resourceName = puq.TrimStart(new[] {'/'});
				assemblyName = Assembly.GetEntryAssembly().GetName(false).Name; //REVISE which assembly should be used if no one specified?
			}

			resourceName = resourceName.ToLower(CultureInfo.InvariantCulture).Replace(" ", "%20").Replace('\\', '/');
			ResourceSet resourceSet = GetCachedResourceSet(assemblyName, culture);
			stream = (Stream) resourceSet.GetObject(resourceName);

			#region xaml/baml fallback

			if (stream == null) {
				if (ext == ".xaml") {
					var name0 = resourceName.Substring(0, resourceName.Length - 5) + ".baml";
					stream = (Stream) resourceSet.GetObject(name0);
					if (stream != null) {
						resourceName = name0;
						ext = ".baml";
					}
				}
				else if (ext == ".baml") {
					var name0 = resourceName.Substring(0, resourceName.Length - 5) + ".xaml";
					stream = (Stream) resourceSet.GetObject(name0);
					if (stream != null) {
						resourceName = name0;
						ext = ".xaml";
					}
				}
			}

			#endregion

			if (stream == null) {
				Debug.WriteLine("=>WARNING: Resource not found! " +
				                "\r\n\t" + "Resource: " + resourceName +
//				"\r\n\t"+"Assembly: "+ _CachedAssembly.FullName+
				                "\r\n\t" + "CallingAssembly: " + Assembly.GetCallingAssembly().FullName +
				                "\r\n\t" + "UniqueID: {536714F7-D844-4093-A595-041E83AFAF51}");
				return null;
			}

			return stream;
		}


		private Assembly2 GetAssembly(string assemblyName) {
			if (assemblyName == null) assemblyName = Assembly.GetExecutingAssembly().GetName(false).Name;
			if (!CachedAssemblies.ContainsKey(assemblyName)) {
				Assembly assembly = null;
				if (assemblyName == Assembly.GetEntryAssembly().GetName(false).Name) { assembly = Assembly.GetEntryAssembly(); }
				else if (assemblyName == Assembly.GetExecutingAssembly().GetName(false).Name) {
					assembly = Assembly.GetExecutingAssembly();
				}
				else if (assemblyName == Assembly.GetCallingAssembly().GetName(false).Name) { assembly = Assembly.GetCallingAssembly(); }

				if (assembly == null) {
					foreach (var assembly1 in AppDomain.CurrentDomain.GetAssemblies()) {
						if (assemblyName == assembly1.GetName(false).Name) {
							assembly = assembly1;
							break;
						}
					}
				}

//				if (assembly == null) {
//					assembly = Assembly.LoadWithPartialName(assemblyName);
//				}

				if (assembly == null) {
					Debug.WriteLine("Assembly not found! Name: " + assemblyName + " ErrorID: {80103DCD-085A-4C63-800E-39FA1BB40035}");
					return null;
				}

				var assembly2 = new Assembly2(assembly);
				CachedAssemblies.Add(assemblyName, assembly2);
				return assembly2;
			}
			else { return CachedAssemblies[assemblyName]; }
		}

		private ResourceSet GetCachedResourceSet(string assemblyName, CultureInfo culture) {
			var assembly = GetAssembly(assemblyName);
			if (assembly == null) return null;
			var resourceSet = assembly.GetResourceSet(culture);
			return resourceSet;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}

//RESERVED
//		static object GetResource(string name, Assembly asm, System.Globalization.CultureInfo culture) {
//			// "resources/graphics/a_page_viewbox.baml"
//			name = name.ToLower().Replace(" ", "%20").Replace('\\','/');
//			if(asm==null) asm = Assembly.GetExecutingAssembly();
//			if(culture==null) culture = Thread.CurrentThread.CurrentUICulture;
//			string resourceName = asm.GetName().Name + ".g";
//			ResourceManager rm = new ResourceManager(resourceName, asm);
//			ResourceSet resourceSet = rm.GetResourceSet(culture, true, true);
//			var o = resourceSet.GetObject(name);
//			return o;
//		}

//RESERVED
//		static List<string> GetResourceNames(Assembly asm, System.Globalization.CultureInfo culture) {
//			if(asm==null) asm = Assembly.GetExecutingAssembly();
//			if(culture==null) culture = Thread.CurrentThread.CurrentUICulture;
//			string resourceName = asm.GetName().Name + ".g";
//			ResourceManager rm = new ResourceManager(resourceName, asm);
//			ResourceSet resourceSet = rm.GetResourceSet(culture, true, true);
//			List<string> resources = new List<string>();
//			foreach (DictionaryEntry resource in resourceSet) {
//
//				resources.Add((string) resource.Key);
//			}
//			rm.ReleaseAllResources();
//			return resources;
//		}
		// resources/graphics/a_page_viewbox.baml
		// resources/graphics/a_resource_viewbox.baml

		private class Assembly2 {

			private ResourceManager _resourceManager;
			private readonly Dictionary<CultureInfo, ResourceSet> _resourceSets = new Dictionary<CultureInfo, ResourceSet>();

			public Assembly2(Assembly assembly) {
				Assembly = assembly;
			}

			public Assembly Assembly { get; private set; }

			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
			public ResourceSet ResourceSet => GetResourceSet(Thread.CurrentThread.CurrentUICulture);

			public ResourceSet GetResourceSet(CultureInfo culture) {
				if (culture == null) culture = Thread.CurrentThread.CurrentUICulture;

				if (_resourceManager == null) { _resourceManager = new ResourceManager(Assembly.GetName().Name + ".g", Assembly); }

				if (!_resourceSets.ContainsKey(culture)) {
					var resourceSet = _resourceManager.GetResourceSet(culture, true, true);
					_resourceSets.Add(culture, resourceSet);
					return resourceSet;
				}
				else { return _resourceSets[culture]; }
			}

		}

	}

}
