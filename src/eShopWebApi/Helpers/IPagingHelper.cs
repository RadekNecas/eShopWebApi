namespace eShopWebApi.Helpers
{
    public interface IPagingHelper
    {
        void UpdateHttpHeadersWithPagingInformations(int totalItemsCount);

        void UpdateHttpHeadersWithPagingInformations(int totalItemsCount, int offset, int currentPageLimit);
    }
}