using eShopWebApi.Infrastructure.Data.Initialization;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Linq;

namespace eShopWebApi.InfrastructureTests
{
    public class ProductsDataJsonProviderTests
    {
        private Mock<IFileReader> _fileReaderMock;
        private Mock<IConfiguration> _configMock;
        private Mock<ILogger<ProductsDataJsonProvider>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            _fileReaderMock = new Mock<IFileReader>();
            _fileReaderMock.Setup(m => m.ReadAllFile(It.IsAny<string>())).Returns(string.Empty);

            _configMock = new Mock<IConfiguration>();
            _configMock.Setup(m => m[It.IsAny<string>()]).Returns(string.Empty);

            _loggerMock = new Mock<ILogger<ProductsDataJsonProvider>>();
        }

        private ProductsDataJsonProvider CreateSutWithDefaultMocks() 
            => new ProductsDataJsonProvider(_configMock.Object, _fileReaderMock.Object, _loggerMock.Object);


        [Test]
        public void GetData_EmptyJsonFile_ReturnsEmptyCollection()
        {
            // Arrange
            _configMock.Setup(m => m[It.IsAny<string>()]).Returns(string.Empty);

            var productsProviderSut = CreateSutWithDefaultMocks();

            // Act
            var products = productsProviderSut.GetData();

            // Assert
            products.Should().BeEmpty();
        }

        [Test]
        public void GetData_CorrectSetup_CorrectConfigurationReadAndUsed()
        {
            // Arrange
            const string configKey = "eShopWebApi:PathToInitProductsJsonFile";
            const string pathToFile = "myTestFilePath.json";

            _configMock.Setup(m => m[It.Is<string>(p => p == configKey)]).Returns(pathToFile);
            var productsProviderSut = CreateSutWithDefaultMocks();

            // Act
            var products = productsProviderSut.GetData();

            // Assert
            _configMock.Verify(m => m[It.Is<string>(p => p == "eShopWebApi:PathToInitProductsJsonFile")], Times.AtLeastOnce);
            _fileReaderMock.Verify(m => m.ReadAllFile(It.Is<string>(p => p == pathToFile)), Times.Once);
        }

        [Test]
        public void GetData_CaseInsensitiveJson_CorrectDataReturned()
        {
            // Arrange
            var testJson = @"[
{
    ""Name"" : ""test1"",
    ""imguri"" : ""http://test1.com"",
    ""PRICE"" : 1200.50,
    ""dEsCRipTION"" : ""test description""
}
]";
            _fileReaderMock.Setup(m => m.ReadAllFile(It.IsAny<string>())).Returns(testJson);

            var productsProviderSut = CreateSutWithDefaultMocks();

            // Act
            var products = productsProviderSut.GetData();

            // Assert
            products.Should().HaveCount(1);
            var testedProduct = products.First();
            testedProduct.Name.Should().Be("test1");
            testedProduct.ImgUri.Should().Be("http://test1.com");
            testedProduct.Price.Should().Be(new decimal(1200.50));
            testedProduct.Description.Should().Be("test description");
        }
    }
}