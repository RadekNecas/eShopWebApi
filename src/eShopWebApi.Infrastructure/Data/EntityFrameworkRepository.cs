using eShopWebApi.Core.Entities;
using eShopWebApi.Core.RequiredExternalities;
using eShopWebApi.Core.Tools;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace eShopWebApi.Infrastructure.Data
{
    public class EntityFrameworkRepository<T> : IAsyncRepository<T> where T  : BaseEntity, IAggregateRoot
    {
        private readonly ApplicationDbContext _dbContext;

        public EntityFrameworkRepository(ApplicationDbContext dbContext)
        {
            _dbContext = Guard.ReturnIfChecked(dbContext, nameof(dbContext));
        }

        public async Task<IReadOnlyList<T>> ListAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<T>().ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<T>> ListAsync(IQuerySpecification<T> specification, CancellationToken cancellationToken = default)
        {
            DbSet<T> data = EvaluateSpecification(specification);

            return await data.ToListAsync(cancellationToken);
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<T>().CountAsync(cancellationToken);
        }

        public async Task<int> CountAsync(IQuerySpecification<T> specification, CancellationToken cancellationToken = default)
        {
            DbSet<T> data = EvaluateSpecification(specification);

            return await data.CountAsync(cancellationToken);
        }

        public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            await _dbContext.AddRangeAsync(entities, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private DbSet<T> EvaluateSpecification(IQuerySpecification<T> specification)
        {
            var data = _dbContext.Set<T>();

            if (specification.HasQuery)
            {
                data.Where(specification.Query);
            }

            if (specification.IsOrdered)
            {
                data.OrderBy(specification.OrderBy);
            }

            if (specification.IsPaginated)
            {
                data.Skip(specification.Skip);
                data.Take(specification.Take);
            }

            return data;
        }
    }
}
