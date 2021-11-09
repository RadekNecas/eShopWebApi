using eShopWebApi.Core.Entities.Product;
using eShopWebApi.Core.QuerySpecifications;
using eShopWebApi.Core.RequiredExternalities;
using eShopWebApi.Core.Services;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace eShopWebApi.CoreTests
{
    /// <summary>
    /// Other methods of ProductService might be covered too. I decided to test only methods, that
    /// do more than call internal repository. I would choose better coverage for production code.
    /// </summary>
    public class ProductServiceTests
    {
        private Mock<IAsyncRepository<Product>> _productRepositoryMock;
        private ProductService _productServiceSut;

        [SetUp]
        public void Setup()
        {
            _productRepositoryMock = new Mock<IAsyncRepository<Product>>();
            _productServiceSut = new ProductService(_productRepositoryMock.Object);
        }

        [TestCase(10, -3)]
        [TestCase(-3, 10)]
        public void GetProductAsync_InvalidArgument_ExceptionThrown(int offset, int limit)
        {
            // Act
            Assert.ThrowsAsync<ArgumentException>(async() => await _productServiceSut.GetProductsAsync(offset, limit));
        }

        [TestCase(0, 5)]
        [TestCase(5, 0)]
        [TestCase(5, 5)]
        public async Task GetProductAsync_ValidArguments_RepositoryResultReturned(int offset, int limit)
        {
            // Arrange
            var expectedResult = Array.Empty<Product>();
            _productRepositoryMock.Setup(m => m.ListAsync(It.IsAny<IQuerySpecification<Product>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _productServiceSut.GetProductsAsync(offset, limit);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public async Task UpdateProductDescription_ProductWithIdDoesNotExist_NullReturnedUpdateNotCalled()
        {
            // Arrange
            Product nullProduct = null;
            var notExistingId = 1;
            var newDescritpion = "testing description that will never be set";
            _productRepositoryMock.Setup(m => m.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(nullProduct);

            // Act
            var result = await _productServiceSut.UpdateProductDescriptionAsync(notExistingId, newDescritpion);

            // Assert
            result.Should().BeNull();
            _productRepositoryMock.Verify(m => m.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}