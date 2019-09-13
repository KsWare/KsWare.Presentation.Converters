using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using KsWare.Presentation.Converters;
using WpfAnimatedGif;

namespace WpfApp1
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
//			DataTemplateConverter.GifFactory = new Func<Uri, DataTemplate>(locationUri =>
//			{
//				ImageBehavior.SetAnimatedSource(new Image(), new BitmapImage());
//
//					// REQUIRES: PM> Install-Package WpfAnimatedGif
//					var dataTemplateXaml = $@"<?xml version=""1.0"" encoding=""utf-8""?>
//<DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" xmlns:gif=""http://wpfanimatedgif.codeplex.com"">
//	<Image gif:ImageBehavior.AnimatedSource=""{locationUri}"" Stretch=""Uniform"" />
//</DataTemplate>";
//
//					var sr = new StringReader(dataTemplateXaml);
//					var xr = XmlReader.Create(sr);
//					return (DataTemplate)XamlReader.Load(xr);
//
//			});
			InitializeComponent();
		}
	}
}
