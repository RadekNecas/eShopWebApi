using eShopWebApi.Core.Entities.Product;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace eShopWebApi.Core.Services
{
    public interface IProductService
    {
        Task<IReadOnlyList<Product>> GetProductsAsync(CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Product>> GetProductsAsync(int offset, int limit, CancellationToken cancellationToken = default);

        Task<int> GetProductsTotalCountAsync(CancellationToken cancellationToken = default);

        Task<Product> GetProductAsync(int id, CancellationToken cancellationToken = default);

        Task<Product> UpdateProductDescriptionAsync(int id, string description, CancellationToken cancellationToken = default);
    }
}