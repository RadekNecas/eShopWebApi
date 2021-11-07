using eShopWebApi.Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Data.Common;

namespace eShopWebApi.SharedTests
{
    public class UsingSqliteInMemoryDbContextTestsBase : UsingDbContextTestsBase, IDisposable
    {
        private readonly DbConnection _connection;

        public UsingSqliteInMemoryDbContextTestsBase() 
            : base(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(CreateInMemoryDatabase())
                .Options)
        {
            _connection = RelationalOptionsExtension.Extract(ContextOptions).Connection;
        }

        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");

            connection.Open();

            return connection;
        }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}
