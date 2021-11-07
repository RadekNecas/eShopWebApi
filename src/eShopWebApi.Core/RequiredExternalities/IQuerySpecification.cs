using eShopWebApi.Core.Entities;
using System;
using System.Linq.Expressions;

namespace eShopWebApi.Core.RequiredExternalities
{
    public interface IQuerySpecification<T> where T : BaseEntity, IAggregateRoot
    {
        Expression<Func<T, bool>> Query { get; }
        bool HasQuery { get; }
        bool IsPaginated { get; }
        int Skip { get; }
        int Take { get; }

        bool IsOrdered { get; }

        Expression<Func<T, int>> OrderBy { get; }
    }
}
