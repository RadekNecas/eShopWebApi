using eShopWebApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
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
            using (var context = CreateContext)
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                context.AddRange(dataToInsert);
                context.SaveChanges();
            }
        }

        protected virtual ApplicationDbContext CreateContext => new ApplicationDbContext(ContextOptions);
    }
}
