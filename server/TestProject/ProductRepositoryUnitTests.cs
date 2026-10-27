using Entities;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using Repositories;

namespace TestProject
{
    public class ProductRepositoryUnitTests
    {
        [Fact]
        public async Task GetProducts_ShouldReturnCorrectFilters_WithMockData()
        {
            
            var options = new DbContextOptions<ApiShopContext>();
            var mockContext = new Mock<ApiShopContext>(options);

            
            var productsList = new List<Product>
    {
        
        new Product { ProductId = 1, ProductName = "Smartphone", Description = "Smart Phone 5G", Price = 1000, CategoryId = 1, IsActive = true },


        new Product { ProductId = 2, ProductName = "Laptop", Description = "Gaming Laptop", Price = 5000, CategoryId = 1, IsActive = true },

        new Product { ProductId = 3, ProductName = "Screen", Description = "LCD Screen", Price = 500, CategoryId = 1, IsActive = true }
    };

            mockContext.Setup(x => x.Products).ReturnsDbSet(productsList);

            var repository = new ProductsRepository(mockContext.Object);

            var result = await repository.GetProducts(
                position: 1,
                skip: 10,
                categoryIds: new int?[] { 1 },
                description: null,   // text search needs SQL Server; covered by the integration tests
                maxPrice: 2000,
                minPrice: null
            );

            // Laptop (5000) is above the max price; the rest come back cheapest first
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Items.Count);
            Assert.Equal("Screen", result.Items[0].ProductName);
            Assert.Equal("Smartphone", result.Items[1].ProductName);
        }

        [Fact]
        public async Task GetProducts_ShouldApplyPaginationCorrectly()
        {
            
            var options = new DbContextOptions<ApiShopContext>();
            var mockContext = new Mock<ApiShopContext>(options);

            
            var productsList = new List<Product>
    {
        new Product { ProductId = 1, Price = 10, IsActive = true },
        new Product { ProductId = 2, Price = 20, IsActive = true },
        new Product { ProductId = 3, Price = 30, IsActive = true },
        new Product { ProductId = 4, Price = 40, IsActive = true },
        new Product { ProductId = 5, Price = 50, IsActive = true }
    };

            mockContext.Setup(x => x.Products).ReturnsDbSet(productsList);
            var repository = new ProductsRepository(mockContext.Object);

            
            var result = await repository.GetProducts(
                position: 2,
                skip: 2,
                categoryIds: new int?[0],
                description: null,
                maxPrice: null,
                minPrice: null
            );

            
            Assert.Equal(5, result.TotalCount); 
            Assert.Equal(2, result.Items.Count); 
            Assert.Equal(30, result.Items[0].Price);
            Assert.Equal(40, result.Items[1].Price);
        }
        [Fact]
        public async Task GetProducts_ShouldReturnEmptyList_WhenNoProductsMatchFilters()
        {
            
            var options = new DbContextOptions<ApiShopContext>();
            var mockContext = new Mock<ApiShopContext>(options);

            
            var productsList = new List<Product>
    {
        new Product { ProductId = 1, ProductName = "Gold Watch", Price = 5000, CategoryId = 1 },
        new Product { ProductId = 2, ProductName = "Diamond Ring", Price = 10000, CategoryId = 1 }
    };

            mockContext.Setup(x => x.Products).ReturnsDbSet(productsList);

            var repository = new ProductsRepository(mockContext.Object);

            var result = await repository.GetProducts(
                position: 1,
                skip: 10,
                categoryIds: new int?[0],
                description: null,
                maxPrice: 100, 
                minPrice: null
            );

            
            Assert.NotNull(result.Items);
            Assert.Empty(result.Items); 
            Assert.Equal(0, result.TotalCount);
        }
    }
}