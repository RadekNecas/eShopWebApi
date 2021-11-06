using eShopWebApi.Core.Entities;
using eShopWebApi.Core.RequiredExternalities;
using eShopWebApi.Core.Tools;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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
            return await _dbContext.Set<T>().ToListAsync();
        }
    }
}
