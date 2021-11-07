using eShopWebApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace eShopWebApi.SharedTests
{
    public class UsingInMemoryDbContextTestsBase : UsingDbContextTestsBase
    {
        public UsingInMemoryDbContextTestsBase()
            : base(new DbContextOptionsBuilder<ApplicationDbContext>()
                            .UseInMemoryDatabase("eshopWebApiTestsDatabase")
                            .Options)
        {
        }
    }
}
