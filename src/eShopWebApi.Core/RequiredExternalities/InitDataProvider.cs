using System.Collections.Generic;

namespace eShopWebApi.Core.RequiredExternalities
{
    public interface InitDataProvider<T>
    {
        public IEnumerable<T> GetData();
    }
}