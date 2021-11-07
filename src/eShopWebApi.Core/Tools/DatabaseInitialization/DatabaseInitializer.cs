using eShopWebApi.Core.Entities.Product;
using eShopWebApi.Core.RequiredExternalities;
using Microsoft.Extensions.Logging;

namespace eShopWebApi.Core.Tools.DatabaseInitialization
{
    /// <summary>
    /// Simple data initialization manager. Implementation is not async because is called from non-async context.
    /// For more complicated scenarios, with multiple entities that need to be initialized, I would use 
    /// Strategy pattern implementation. List of Strategy implementation for each entity would be injected into the constructor.
    /// This solution is good enough for purposes of this assignment.
    /// </summary>
    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly IAsyncRepository<Product> _productRepository;
        private readonly InitDataProvider<Product> _productDataProvider;

        public DatabaseInitializer(InitDataProvider<Product> productDataProvider, IAsyncRepository<Product> productRepository)
        {
            _productDataProvider = Guard.ReturnIfChecked(productDataProvider, nameof(productDataProvider));
            _productRepository = Guard.ReturnIfChecked(productRepository, nameof(productRepository));
        }

        public void InitializeDatabase()
        {
            var products = _productDataProvider.GetData();
            _productRepository.AddRangeAsync(products).Wait();
        }

        public int GetExistingRecordsCount() => _productRepository.CountAsync().Result;
    }
}
