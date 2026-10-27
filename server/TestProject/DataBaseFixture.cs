using Microsoft.EntityFrameworkCore;
using Repositories;
using System;

namespace TestProject
{
    public class DatabaseFixture : IDisposable
    {
        public ApiShopContext Context { get; private set; }

        
        public DatabaseFixture()
        {
            // Each run gets its own throwaway database, on LocalDB unless
            // TEST_DB_SERVER points elsewhere
            string server = Environment.GetEnvironmentVariable("TEST_DB_SERVER") ?? @"(localdb)\MSSQLLocalDB";
            string database = $"ApiShopTest_{Guid.NewGuid():N}";
            var options = new DbContextOptionsBuilder<ApiShopContext>()

                .UseSqlServer($"Data Source={server};Initial Catalog={database};Integrated Security=True;Pooling=False;TrustServerCertificate=True")
                .Options;
            
            Context = new ApiShopContext(options);
            Context.Database.EnsureCreated();
            // The schema comes with the seeded catalog; tests start from empty tables
            Context.ProductMonthConfigs.ExecuteDelete();
            Context.Images.ExecuteDelete();
            Context.Products.ExecuteDelete();
            Context.Categories.ExecuteDelete();
        }

        public void Dispose()
        {
            Context.Database.EnsureDeleted();
            Context.Dispose();
        }
    }
}