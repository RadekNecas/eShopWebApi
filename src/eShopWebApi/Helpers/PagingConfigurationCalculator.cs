using eShopWebApi.Core.Tools;

namespace eShopWebApi.Helpers
{
    public class PagingConfigurationCalculator : IPagingConfigurationCalculator
    {
        public PagingConfiguration GetConfigurationForPreviousPage(int currentOffset, int currentLimit, int totalItemsCoun)
        {
            Guard.IsPositiveNumber(currentOffset, nameof(currentOffset));
            Guard.IsPositiveNumber(currentLimit, nameof(currentLimit));
            Guard.IsPositiveNumber(totalItemsCoun, nameof(totalItemsCoun));

            var prevPage = new PagingConfiguration();
            if (currentOffset > 0)
            {
                prevPage.Offset = currentOffset - currentLimit;
                if (prevPage.Offset < 0)
                {
                    prevPage.Offset = 0;
                    prevPage.Limit = currentOffset;
                }
                else
                {
                    prevPage.Limit = currentLimit;
                }

                return prevPage;
            }

            return null;
        }

        public PagingConfiguration GetConfigurationForNextPage(int currentOffset, int currentLimit, int totalItemsCount)
        {
            var nextPage = new PagingConfiguration();
            nextPage.Offset = currentOffset + currentLimit;
            nextPage.Limit = currentLimit;

            return nextPage.Offset < totalItemsCount ? nextPage : null;
        }
    }
}
