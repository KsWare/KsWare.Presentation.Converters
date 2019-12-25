using System.Windows;
using NUnit.Framework;

namespace KsWare.Presentation.Converters.Tests
{
	[TestFixture()]
	public class DataTemplateConverterTests
	{
		[SetUp]
		public void Setup()
		{
			var s = System.IO.Packaging.PackUriHelper.UriSchemePack;
		}

		[TestCase("BitmapResource.bmp")]
		[TestCase("IconResource.ico")]
		public void Test(string key)
		{
			var sut = new TemplateConverter()
			{
				ConverterParameter = @"pack://application:,,,/KsWare.Presentation.Converters.Tests;component/TestData/{0}"
			};
			var result = sut.Convert(key, typeof(DataTemplate), null, null);
			Assert.That(result, Is.TypeOf<DataTemplate>());
		}

//		[Test, Apartment(ApartmentState.STA)]
//		public void Test2()
//		{
//			var element = new Image {Stretch = Stretch.Uniform};
//			ImageBehavior.SetAnimatedSource(element, new BitmapImage(new Uri("pack://application:,,,/KsWare.Presentation.Converters.Tests;component/TestData/GifResource.gif")));
//			var xaml = System.Windows.Markup.XamlWriter.Save(element);
//			var stream = new MemoryStream();
//			new StreamWriter(stream,Encoding.UTF8).Write(xaml);
//			stream.Position = 0;
//
//			var element2 = System.Windows.Markup.XamlReader.Load(stream);
//		}
	}
}