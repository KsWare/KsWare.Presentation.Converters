
using System;
using System.Globalization;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using Moq;
using NUnit.Framework;

namespace KsWare.Presentation.Converters.Tests {

	[TestFixture]
	public class ResourceConverterExtensionTests {

		private Mock<IServiceProvider> ServiceProviderMock { get; set; }
		private IServiceProvider ServiceProvider => ServiceProviderMock.Object;
		private Mock<IUriContext> UriContextMock { get; set; }


		[SetUp]
		public void Setup() {
			var _ = System.IO.Packaging.PackUriHelper.UriSchemePack;

			// var uriContext = serviceProvider.GetService(typeof(IUriContext)) as IUriContext;
			UriContextMock = new Mock<IUriContext>();
			UriContextMock.SetupGet(x => x.BaseUri).Returns(new Uri("pack://application:,,,/KsWare.Presentation.Converters.Tests;component/TestData/dummy.xaml"));

			ServiceProviderMock = new Mock<IServiceProvider>();
			// var uriContext = serviceProvider.GetService(typeof(IUriContext)) as IUriContext;
			ServiceProviderMock.Setup(x => x.GetService(typeof(IUriContext))).Returns(() => UriContextMock.Object);

		}



		[TestCase("IconResource.ico", "pack://application:,,,/KsWare.Presentation.Converters.Tests;component/TestData")]
		[TestCase("IconResource.ico", "/KsWare.Presentation.Converters.Tests;component/TestData")]
		[TestCase("IconResource.ico", "")]
		[TestCase("IconResource.ico", ".")]
		[TestCase("IconResource.ico", "../TestData")]
		[TestCase("IconResource.ico", "/TestData")]
		[Apartment(ApartmentState.STA)]
		public void ConstructorResourcePath(string value, string resourcePath) {
			var sut = new ResourceConverterExtension(resourcePath);
			var converter = (IValueConverter)sut.ProvideValue(ServiceProvider);
			var result = converter.Convert(value, typeof(object), null, CultureInfo.CurrentCulture);
			Assert.That(result,Is.TypeOf<Image>());
		}

		[TestCase("IconResource.ico", "..")]
		[Apartment(ApartmentState.STA)]
		public void ConstructorResourcePath2(string value, string resourcePath) {
			UriContextMock.SetupGet(x => x.BaseUri).Returns(new Uri("pack://application:,,,/KsWare.Presentation.Converters.Tests;component/TestData/Sub/dummy.xaml"));
			var sut = new ResourceConverterExtension(resourcePath);
			var converter = (IValueConverter)sut.ProvideValue(ServiceProvider);
			var result = converter.Convert(value, typeof(object), null, CultureInfo.CurrentCulture);
			Assert.That(result, Is.TypeOf<Image>());
		}

		[TestCase("pack://application:,,,/KsWare.Presentation.Converters.Tests;component/TestData/IconResource.ico")]
		public void ConstructorResourcePath_Fail(string resourcePath) {
			var sut = new ResourceConverterExtension(resourcePath);
		}

		

	}

}


