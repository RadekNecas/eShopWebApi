using eShopWebApi.Core.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace eShopWebApi.Core.RequiredExternalities
{
    public interface IAsyncRepository<T> where T : BaseEntity, IAggregateRoot
    {
        Task<IReadOnlyList<T>> ListAsync(CancellationToken cancellationToken = default);

    }
}
