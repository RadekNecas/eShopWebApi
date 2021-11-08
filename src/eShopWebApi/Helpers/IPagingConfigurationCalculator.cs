namespace eShopWebApi.Helpers
{
    public interface IPagingConfigurationCalculator
    {
        PagingConfiguration GetConfigurationForNextPage(int currentOffset, int currentLimit, int totalItemsCount);
        PagingConfiguration GetConfigurationForPreviousPage(int currentOffset, int currentLimit, int totalItemsCoun);
    }
}