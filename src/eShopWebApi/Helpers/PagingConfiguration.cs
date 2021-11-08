namespace eShopWebApi.Helpers
{
    public class PagingConfiguration
    {
        public PagingConfiguration()
        {
        }

        public PagingConfiguration(int offset, int limit)
        {
            Offset = offset;
            Limit = limit;
        }

        public int Offset { get; set; }
        public int Limit { get; set; }
    }
}
