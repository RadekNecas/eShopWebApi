using eShopWebApi.Core.Entities;

namespace eShopWebApi.Core.QuerySpecifications
{
    internal class PagedDataSpecification<T> : BaseQuerySpecification<T> where T : BaseEntity, IAggregateRoot
    {
        public PagedDataSpecification(int offset, int size)
        {
            Skip = offset;
            Take = size;
            OrderBy = d => d.Id;
        }
    }
}
