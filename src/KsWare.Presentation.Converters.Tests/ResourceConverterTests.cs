using System.Threading;
using System.Windows;
using System.Windows.Controls;
using NUnit.Framework;

namespace KsWare.Presentation.Converters.Tests
{
	[TestFixture()]
	public class ResourceConverterTests
	{
		[SetUp]
		public void Setup()
		{
			var _ = System.IO.Packaging.PackUriHelper.UriSchemePack;
		}

		[TestCase("BitmapResource.bmp")]
		[TestCase("IconResource.ico")]
		[Apartment(ApartmentState.STA)]
		public void Test(string key)
		{
			var sut = new ResourceConverter()
			{
				ConverterParameter = @"pack://application:,,,/KsWare.Presentation.Converters.Tests;component/TestData/{0}"
			};
			var result = sut.Convert(key, typeof(object), null, null);
			Assert.That(result, Is.Not.Null.And.Not.TypeOf<TextBlock>());
		}

		[TestCase("IconResource")]
		[Apartment(ApartmentState.STA)]
		public void Image(string key) {
			var sut = new ResourceConverter() {
				ConverterParameter = @"pack://application:,,,/KsWare.Presentation.Converters.Tests;component/TestData/{0}.ico"
			};
			var result = sut.Convert(key, typeof(object), null, null);
			Assert.That(result, Is.Not.Null.And.Not.TypeOf<TextBlock>());
		}

		[TestCase("IconResource")]
		[Apartment(ApartmentState.STA)]
		public void DataTemplate(string key) {
			var sut = new ResourceConverter() {
				ConverterParameter = @"pack://application:,,,/KsWare.Presentation.Converters.Tests;component/TestData/{0}.ico"
			};
			var result = sut.Convert(key, typeof(DataTemplate), null, null);
			Assert.That(result, Is.TypeOf<DataTemplate>());
			//TODO test IsNotErrorTemplate
		}

		[TestCase("IconResource")]
		[Apartment(ApartmentState.STA)]
		public void ControlTemplate(string key) {
			var sut = new ResourceConverter() {
				ConverterParameter = @"pack://application:,,,/KsWare.Presentation.Converters.Tests;component/TestData/{0}.ico"
			};
			var result = sut.Convert(key, typeof(ControlTemplate), null, null);
			Assert.That(result, Is.TypeOf<ControlTemplate>());
			//TODO test IsNotErrorTemplate
		}

		[TestCase("Icon")] //TODO doesn't work with R# Testrunner. Icon not in directory
		[Apartment(ApartmentState.STA)]
		public void Test3(string key) {
			var sut = new ResourceConverter() {
				ConverterParameter = @"pack://application:,,,/KsWare.Presentation.Converters.Tests;component/TestData/{0}.ico"
			};
			var result = sut.Convert(key, typeof(object), null, null);
			Assert.That(result, Is.Not.Null.And.Not.TypeOf<TextBlock>());
		}

		[Test]
		[Apartment(ApartmentState.STA)]
		public void Test_Fail() {
			var sut = new ResourceConverter() {
				ConverterParameter = @"pack://application:,,,/KsWare.Presentation.Converters.Tests;component/TestData/{0}.ico"
			};
			var result = sut.Convert("_NA_", typeof(object), null, null);
			Assert.That(result, Is.TypeOf<TextBlock>());
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