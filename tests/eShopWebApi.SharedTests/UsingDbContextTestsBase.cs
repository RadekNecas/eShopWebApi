using eShopWebApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace eShopWebApi.SharedTests
{
    public abstract class UsingDbContextTestsBase
    {
        protected  DbContextOptions<ApplicationDbContext> ContextOptions { get; private set; }

        public UsingDbContextTestsBase(DbContextOptions<ApplicationDbContext> contextOptions)
        {
            ContextOptions = contextOptions;
        }

        protected virtual void Seed<T>(string pathToJsonFile)
        {
            var json = File.ReadAllText(pathToJsonFile);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var dataToInsert = JsonSerializer.Deserialize<List<T>>(json, options);
            Seed(dataToInsert);

        }

        protected virtual void Seed<T>(IEnumerable<T> dataToInsert)
        {
            using (var context = CreateContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                // TODO: EF Core uses wrong method overload here for AddRange(). It selects AddRange(params T) instead of
                // AddRange(IEnumerable<T>). This throws exception that type T[] is not attached to context via DBSet.
                foreach(var record in dataToInsert)
                {
                    context.Add(record);
                }
                context.SaveChanges();
            }
        }

        protected virtual ApplicationDbContext CreateContext() => new ApplicationDbContext(ContextOptions);
    }
}
