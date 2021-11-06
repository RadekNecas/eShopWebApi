using eShopWebApi.Core.Entities.Product;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace eShopWebApi.Core.Services
{
    public interface IProductService
    {
        Task<IReadOnlyList<Product>> GetProducts(CancellationToken cancellationToken = default);
    }
}