using eShopWebApi.Helpers;
using FluentAssertions;
using NUnit.Framework;

namespace eShopWebApiTests
{
    public class PagingConfigurationCalculatorTests
    {
        [Test]
        public void GetConfigurationForPreviousPage_PreviusPageDoesNotExist_NullReturned()
        {
            // Arrange
            int currentOffset = 0;
            int currentLimit = 10;
            int totalItemsCount = 13;

            var calculatorSut = new PagingConfigurationCalculator();

            // Act
            var pageConfig = calculatorSut.GetConfigurationForPreviousPage(currentOffset, currentLimit, totalItemsCount);

            // Assert
            pageConfig.Should().BeNull();
        }

        [Test]
        public void GetConfigurationForPreviousPage_CurrentPageIsRightAfterFirstPage_CorrectPreviousPage()
        {
            // Arrange
            int currentOffset = 13;
            int currentLimit = 10;
            int totalItemsCount = 13;

            var calculatorSut = new PagingConfigurationCalculator();

            // Act
            var pageConfig = calculatorSut.GetConfigurationForPreviousPage(currentOffset, currentLimit, totalItemsCount);

            // Assert
            pageConfig.Offset.Should().Be(3);
            pageConfig.Limit.Should().Be(10);
        }

        [Test]
        public void GetConfigurationForPreviousPage_PreviousPageHasLowerLimit_CorrectPreviousPageWithLowerLimit()
        {
            // Arrange
            int currentOffset = 3;
            int currentLimit = 10;
            int totalItemsCount = 13;

            var calculatorSut = new PagingConfigurationCalculator();

            // Act
            var pageConfig = calculatorSut.GetConfigurationForPreviousPage(currentOffset, currentLimit, totalItemsCount);

            // Assert
            pageConfig.Offset.Should().Be(0);
            pageConfig.Limit.Should().Be(currentOffset);
        }

        [Test]
        public void GetConfigurationForNextPage_NextPageDoesNotExist_NullReturned()
        {
            // Arrange
            int currentOffset = 10;
            int currentLimit = 10;
            int totalItemsCount = 13;

            var calculatorSut = new PagingConfigurationCalculator();

            // Act
            var pageConfig = calculatorSut.GetConfigurationForNextPage(currentOffset, currentLimit, totalItemsCount);

            // Assert
            pageConfig.Should().BeNull();
        }

        [Test]
        public void GetConfigurationForNextPage_NextPageIsSmallerThenLimit_CorrectNextPageLimitIsTheSameAsCurrent()
        {
            // Arrange
            int currentOffset = 2;
            int currentLimit = 10;
            int totalItemsCount = 13;

            var calculatorSut = new PagingConfigurationCalculator();

            // Act
            var pageConfig = calculatorSut.GetConfigurationForNextPage(currentOffset, currentLimit, totalItemsCount);

            // Assert
            pageConfig.Offset.Should().Be(12);
            pageConfig.Limit.Should().Be(currentLimit);
        }
    }
}