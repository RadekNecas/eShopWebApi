using eShopWebApi.Core.Entities.Product;
using eShopWebApi.Infrastructure.Data;
using eShopWebApi.SharedTests;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace eShopWebApi.InfrastructureTests
{
    public class ProductEntityFrameworkRepositoryTests : UsingSqliteInMemoryDbContextTestsBase
    {
        public Product[] TestData = new[]
        {
            new Product(
                name: "Test Product 1",
                imgUri: "http://testproducts.com/1,",
                price: 100,
                description: "Test product 1 - description"),
            new Product(
                name: "Test Product 2",
                imgUri: "http://testproducts.com/2,",
                price: 200,
                description: "Test product 2 - description"),
            new Product(
                name: "Test Product 3",
                imgUri: "http://testproducts.com/3,",
                price: 300,
                description: "Test product 3 - description"),
            new Product(
                name: "Test Product 4",
                imgUri: "http://testproducts.com/4,",
                price: 400,
                description: "Test product 4 - description"),
            new Product(
                name: "Test Product 5",
                imgUri: "http://testproducts.com/5,",
                price: 500,
                description: "Test product 5 - description"),
        };

        [SetUp]
        public void Setup()
        {
            Seed(TestData);
        }

        [TearDown]
        public void TearDown()
        {
            using var dbContext = CreateContext();
            dbContext.Products.FromSqlRaw("DELETE FROM Products; SELECT * FROM Products;").ToList();
        }

        [Test]
        public async Task ListAsync_DataInserted_ReturnsAllData()
        {
            using(var dbContext = CreateContext())
            {
                // Arrange
                var repository = new EntityFrameworkRepository<Product>(dbContext);

                // Act
                var products = await repository.ListAsync();

                // Assert
                products.Should().HaveCount(TestData.Length, "Invalid number of returned records.");
                foreach(var product in products)
                {
                    var expectedProduct = TestData.FirstOrDefault(p => p.Name == product.Name);
                    expectedProduct.Should().NotBeNull($"Product with name {product.Name} should not be in the database.");
                    product.Should().BeEquivalentTo(expectedProduct);
                }
            }
        }

        [Test]
        public async Task ListAsync_EmptyTable_ReturnsEmptyCollection()
        {
            using (var dbContext = CreateContext())
            {
                // Arrange
                var repository = new EntityFrameworkRepository<Product>(dbContext);
                await dbContext.Products.FromSqlRaw("DELETE FROM Products; SELECT * FROM Products;").ToListAsync();

                // Act
                var products = await repository.ListAsync();

                // Assert
                products.Should().BeEmpty();
            }
        }
        
        [Test]
        public async Task CountAsync_DataInserted_CorrectCountReturned()
        {
            using (var dbContext = CreateContext())
            {
                // Arrange
                var repository = new EntityFrameworkRepository<Product>(dbContext);

                // Act
                var recordsCount = await repository.CountAsync();

                // Assert
                recordsCount.Should().Be(TestData.Length);
            }
        }

        [Test]
        public async Task CountAsync_EmptyTable_CorrectCountReturned()
        {
            using (var dbContext = CreateContext())
            {
                // Arrange
                var repository = new EntityFrameworkRepository<Product>(dbContext);
                await dbContext.Products.FromSqlRaw("DELETE FROM Products; SELECT * FROM Products;").ToListAsync();

                // Act
                var recordsCount = await repository.CountAsync();

                // Assert
                recordsCount.Should().Be(0);
            }
        }

        [Test]
        public async Task GetByIdAsync_RecordExists_CorrectRecordReturned()
        {
            using (var dbContext = CreateContext())
            {
                // Arrange
                var repository = new EntityFrameworkRepository<Product>(dbContext);
                var expectedProduct = await dbContext.Products.FirstOrDefaultAsync(p => p.Name == TestData.First().Name);

                // Act
                var testProduct = await repository.GetByIdAsync(expectedProduct.Id);

                // Assert
                testProduct.Should().BeEquivalentTo(expectedProduct);
            }
        }

        [Test]
        public async Task GetByIdAsync_RecordDoesNotExists_NullReturned()
        {
            using (var dbContext = CreateContext())
            {
                // Arrange
                var repository = new EntityFrameworkRepository<Product>(dbContext);
                var maxIdProduct = await dbContext.Products.OrderByDescending(p => p.Id).FirstAsync();

                // Act
                var testProduct = await repository.GetByIdAsync(maxIdProduct.Id+1);

                // Assert
                testProduct.Should().BeNull();
            }
        }
    }
}
