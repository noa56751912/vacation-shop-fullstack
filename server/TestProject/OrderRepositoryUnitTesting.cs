using Entities;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace TestProject
{
    public class OrderRepositoryUnitTests
    {
        [Fact]
        
        public async Task GetOrderById_ShouldReturnOrder_WhenOrderExists()
        {
            var options = new DbContextOptions<ApiShopContext>();
            var mockContext = new Mock<ApiShopContext>(options);
            var orderId = 10;
            var order = new Order { OrderId = orderId, OrderSum = 500, UserId = 1 };

            mockContext.Setup(x => x.Orders).ReturnsDbSet(new List<Order> { order });

            var repository = new OrdersRepository(mockContext.Object);
            var result = await repository.GetOrderById(orderId);
            Assert.NotNull(result);
            Assert.Equal(500, result.OrderSum);
            Assert.Equal(orderId, result.OrderId);
        }

        [Fact]
        public async Task GetOrderById_ShouldReturnNull_WhenOrderDoesNotExist()
        {
            var options = new DbContextOptions<ApiShopContext>();
            var mockContext = new Mock<ApiShopContext>(options);
     
            mockContext.Setup(x => x.Orders).ReturnsDbSet(new List<Order>());
            var repository = new OrdersRepository(mockContext.Object);
            var result = await repository.GetOrderById(1);
            Assert.Null(result);
        }

        // AddOrder reloads the saved order with its items, which a mocked DbSet
        // cannot do; OrderRepositoryIntegrationTests covers it against a real database.
    }
}