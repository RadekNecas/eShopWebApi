namespace eShopWebApi.Core.Tools
{
    public interface IDatabaseInitializer
    {
        void InitializeDatabase();
        public int GetExistingRecordsCount();
    }
}