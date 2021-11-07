using eShopWebApi.Core.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace eShopWebApi.Core.RequiredExternalities
{
    public interface IAsyncRepository<T> where T : BaseEntity, IAggregateRoot
    {
        Task<T> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<T>> ListAsync(CancellationToken cancellationToken = default);

        Task<IReadOnlyList<T>> ListAsync(IQuerySpecification<T> specification, CancellationToken cancellationToken = default);

        Task<int> CountAsync(CancellationToken cancellationToken = default);

        Task<int> CountAsync(IQuerySpecification<T> specification, CancellationToken cancellationToken = default);

        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

        Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    }
}
