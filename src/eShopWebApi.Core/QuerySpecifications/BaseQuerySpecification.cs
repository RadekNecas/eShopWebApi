using eShopWebApi.Core.Entities;
using eShopWebApi.Core.RequiredExternalities;
using System;
using System.Linq.Expressions;

namespace eShopWebApi.Core.QuerySpecifications
{
    internal abstract class BaseQuerySpecification<T> : IQuerySpecification<T> where T : BaseEntity, IAggregateRoot
    {
        public Expression<Func<T, bool>> Query { get; protected set; }

        public bool HasQuery { get; protected set; } = false;

        public bool IsPaginated => Skip > 0 || Take > 0;

        public int Skip { get; protected set; }

        public int Take { get; protected set; }

        public bool IsOrdered => OrderBy != null;

        public Expression<Func<T, int>> OrderBy { get; protected set; } = null;
    }
}
