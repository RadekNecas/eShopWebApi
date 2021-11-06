using eShopWebApi.Core.Entities.Product;
using eShopWebApi.Core.RequiredExternalities;
using eShopWebApi.Core.Tools;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace eShopWebApi.Core.Services
{
    public class ProductService : IProductService
    {
        private readonly IAsyncRepository<Product> _repository;


        public ProductService(IAsyncRepository<Product> repository)
        {
            _repository = Guard.ReturnIfChecked(repository, nameof(repository));
        }


        public async Task<IReadOnlyList<Product>> GetProducts(CancellationToken cancellationToken = default)
        {
            return await _repository.ListAsync(cancellationToken);
        }
    }
}
