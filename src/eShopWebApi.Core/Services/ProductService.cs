using eShopWebApi.Core.Entities.Product;
using eShopWebApi.Core.QuerySpecifications;
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

        public async Task<Product> GetProductAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IReadOnlyList<Product>> GetProductsAsync(CancellationToken cancellationToken = default)
        {
            return await _repository.ListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Product>> GetProductsAsync(int offset, int limit, CancellationToken cancellationToken = default)
        {
            Guard.IsPositiveNumber(offset, nameof(offset));
            Guard.IsPositiveNumber(limit, nameof(limit));

            var specification = new PagedDataSpecification<Product>(offset, limit);
            return await _repository.ListAsync(specification, cancellationToken);
        }

        public async Task<int> GetProductsTotalCountAsync(CancellationToken cancellationToken = default)
        {
            return await _repository.CountAsync(cancellationToken);
        }

        public ProductService(IAsyncRepository<Product> repository)
        {
            _repository = Guard.ReturnIfChecked(repository, nameof(repository));
        }

        public async Task<Product> UpdateProductDescription(int id, string description, CancellationToken cancellationToken = default)
        {
            var product = await _repository.GetByIdAsync(id, cancellationToken);
            if(product != null)
            {
                product.SetDescription(description);
                await _repository.UpdateAsync(product, cancellationToken);
            }

            return product;
        }
    }
}
